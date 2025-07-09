using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.Options;
using System.Text.Json;
using VocabBuilder.Infrastructure.Providers.GoogleTts;
using VocabBuilder.Infrastructure.Repositories.Phonetic;
using VocabBuilder.Models;
using VocabBuilder.Models.Vocab;
using VocabBuilder.Shared;
using VocabBuilder.Shared.Configurations;
using static VocabBuilder.Shared.Constants;
using static VocabBuilder.Shared.Enums;

namespace VocabBuilder.Infrastructure.Providers.CambridgeDictionary;

public class CambridgeDictionaryProvider(
    HttpClient httpClient,
    IWebHostEnvironment environment,
    IOptions<PixabayOptions> pixabayOptions,
    IPhoneticRepository phoneticRepository,
    IGoogleTtsProvider googleTtsProvider) : ICambridgeDictionaryProvider
{
    #region Lookup methods
    /// <summary>
    /// Looks up the given word in the Cambridge Dictionary and returns its definitions and meanings.
    /// </summary>
    /// <param name="word"></param>
    /// <returns></returns>
    public async Task<LookupResult<VocabLookupResult>> LookupAsync(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
            return LookupResult<VocabLookupResult>.Fail("Word cannot be empty.");

        try
        {
            word = word.Trim().ToLowerInvariant();
            var isPhrase = word.Contains(' ');

            var result = await LookupDictionaryAsync(word, isPhrase);
            var meanings = await LookupMeaningsAsync(word);
            result.Meanings.AddRange(meanings);

            return result.Entries.Count > 0
                ? LookupResult<VocabLookupResult>.Ok(result)
                : LookupResult<VocabLookupResult>.Fail("No definitions found.");
        }
        catch (Exception ex)
        {
            return LookupResult<VocabLookupResult>.Fail($"Lookup failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Looks up the given word in the Cambridge Dictionary and returns its entries.
    /// </summary>
    /// <param name="word"></param>
    /// <param name="isPhrase"></param>
    /// <returns></returns>
    private async Task<VocabLookupResult> LookupDictionaryAsync(string word, bool isPhrase)
    {
        var doc = await ScrapeCambridgeDictionaryAsync(word, "english");
        var entries = doc.QuerySelectorAll(Selectors.Cambridge.Entry);

        if (entries.Length > 0)
        {
            var resultEntries = entries.Select(entry => new VocabEntry
            {
                WordType = ExtractText(entry, Selectors.Cambridge.WordType),
                Phonetic = ExtractText(entry, Selectors.Cambridge.Phonetic),
                AudioUrl = ExtractAudio(entry, Selectors.Cambridge.Audio),
                Definitions = entry.QuerySelectorAll(Selectors.Cambridge.DefinitionWrapper).Select(def => new VocabDefinition
                {
                    Definition = ExtractDefinition(def, Selectors.Cambridge.Definition),
                    ImageUrl = ExtractImage(def, Selectors.Cambridge.Image),
                    Examples = ExtractTexts(def, Selectors.Cambridge.Example)
                }).ToList()
            }).ToList();

            return new VocabLookupResult { Word = word, Entries = resultEntries };
        }

        if (isPhrase)
        {
            var fallbackEntry = await CreateFallbackPhraseEntryAsync(word);
            return new VocabLookupResult { Word = word, Entries = [fallbackEntry] };
        }

        return new VocabLookupResult { Word = word, Entries = [] };
    }

    /// <summary>
    /// Looks up the meanings of the given word in the Cambridge Dictionary.
    /// </summary>
    /// <param name="word"></param>
    /// <returns></returns>
    private async Task<List<string>> LookupMeaningsAsync(string word)
    {
        var doc = await ScrapeCambridgeDictionaryAsync(word, "english-vietnamese");
        var result = ExtractTexts(doc, Selectors.Cambridge.Meaning);
        return result.Count > 0 ? result : await FallbackLookupMeaningsAsync(word);
    }

    /// <summary>
    /// Fallback method to look up meanings of the word using Coviet Dictionary if Cambridge Dictionary fails.
    /// This is used when the word is not found in the Cambridge Dictionary or when the meanings are empty.
    /// </summary>
    /// <param name="word"></param>
    /// <returns></returns>
    private async Task<List<string>> FallbackLookupMeaningsAsync(string word)
    {
        var doc = await ScrapeCovietDictionaryAsync(word);
        return ExtractTexts(doc, Selectors.Coviet.FallbackMeaning);
    }

    /// <summary>
    /// Creates a fallback entry for phrases that are not found in the Cambridge Dictionary.
    /// </summary>
    /// <param name="phrase"></param>
    /// <returns></returns>
    private async Task<VocabEntry> CreateFallbackPhraseEntryAsync(string phrase)
    {
        var words = phrase.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var phoneticParts = new List<string>();

        foreach (var w in words)
        {
            var word = w.ToLowerInvariant();
            var phonetic = phoneticRepository.GetPhonetic(word);

            if (!string.IsNullOrWhiteSpace(phonetic))
                phoneticParts.Add(phonetic);
            else
            {
                var doc = await ScrapeCambridgeDictionaryAsync(word, "english");
                var entry = doc.QuerySelector(Selectors.Cambridge.Entry);
                if (entry is null)
                    continue;

                var extracted = ExtractText(entry, Selectors.Cambridge.Phonetic);
                phoneticParts.Add(extracted);
            }
        }

        var cleaned = phoneticParts.Select(p => p.Replace("/", "").Trim());
        var combinedPhonetic = cleaned.Any() ? $"/{string.Join(" ", cleaned)}/" : string.Empty;

        return new VocabEntry
        {
            WordType = WordType.Phrase.ToString().ToLower(),
            Phonetic = combinedPhonetic,
        };
    }
    #endregion

    #region Media lookup methods
    /// <summary>
    /// Looks up media files (audio and image) for the given word using Cambridge Dictionary, Google TTS and Pixabay.
    /// </summary>
    /// <param name="audioUrl"></param>
    /// <param name="imageUrl"></param>
    /// <param name="word"></param>
    /// <returns></returns>
    public async Task<LookupResult<VocabMediaLookupResult>> LookupMediaAsync(string audioUrl, string imageUrl, string word)
    {
        try
        {
            word = word.Trim().ToLowerInvariant();
            var result = new VocabMediaLookupResult();

            if (!string.IsNullOrWhiteSpace(audioUrl))
            {
                result.AudioFilePath = await DownloadToTempFileAsync(audioUrl, word);
                result.AudioFileSource = FileSource.Cambridge;
            }
            else
            {
                result.AudioFilePath = await googleTtsProvider.GenerateAudioAsync(word);
                result.AudioFileSource = FileSource.GoogleTts;
            }

            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                result.ImageFilePath = await DownloadToTempFileAsync(imageUrl, word);
                result.ImageFileSource = FileSource.Cambridge;
            }
            else
            {
                var urls = await FallbackGetImageUrlsAsync(word, 5);
                if (urls.Count == 0)
                    return LookupResult<VocabMediaLookupResult>.Fail("No image URLs found for the word.");

                result.ImageFilePath = await DownloadToTempFileAsync(urls[0], word);
                result.ImageFileSource = FileSource.Pixabay;
                result.AlternativeImageUrls = urls.Skip(1).ToList();
            }

            return string.IsNullOrWhiteSpace(result.AudioFilePath) && string.IsNullOrWhiteSpace(result.ImageFilePath)
                ? LookupResult<VocabMediaLookupResult>.Fail("Failed to download media files.")
                : LookupResult<VocabMediaLookupResult>.Ok(result);
        }
        catch (Exception ex)
        {
            return LookupResult<VocabMediaLookupResult>.Fail($"Media lookup failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Downloads a file from the given URL to a temporary file in the web root path.
    /// </summary>
    /// <param name="url"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    private async Task<string> DownloadToTempFileAsync(string url, string fileName)
        => await DownloadHelper.DownloadUrlToTempFileAsync(httpClient, url, environment.WebRootPath, fileName);

    /// <summary>
    /// Fallback method to get image URLs from Pixabay API if Cambridge Dictionary fails to provide images.
    /// This is used when the image URL is not found in the Cambridge Dictionary or when the image URL is empty.
    /// </summary>
    /// <param name="word"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    private async Task<List<string>> FallbackGetImageUrlsAsync(string word, int limit = 3)
    {
        var apiKey = pixabayOptions.Value.ApiKey;
        var url = $"https://pixabay.com/api/?key={apiKey}&q={Uri.EscapeDataString(word)}&per_page={limit}&image_type=photo&orientation=horizontal";

        var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        if (!response.IsSuccessStatusCode)
            return [];

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        if (!doc.RootElement.TryGetProperty("hits", out var hits))
            return [];

        return hits.EnumerateArray()
            .Select(hit => hit.GetProperty("webformatURL").GetString())
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .Cast<string>()
            .ToList();
    }
    #endregion

    #region Scraping methods
    private readonly IBrowsingContext _browsingContext = BrowsingContext.New(Configuration.Default);

    private async Task<IDocument> ScrapeCambridgeDictionaryAsync(string word, string langPath)
    {
        var url = $"https://dictionary.cambridge.org/dictionary/{langPath}/{Uri.EscapeDataString(word)}";
        var html = await httpClient.GetStringAsync(url);
        return await _browsingContext.OpenAsync(req => req.Content(html));
    }

    private async Task<IDocument> ScrapeCovietDictionaryAsync(string word)
    {
        var url = $"https://tratu.coviet.vn/hoc-tieng-anh/tu-dien/lac-viet/A-V/{Uri.EscapeDataString(word)}.html";
        var html = await httpClient.GetStringAsync(url);
        return await _browsingContext.OpenAsync(req => req.Content(html));
    }
    #endregion

    #region Extraction methods
    private static string ExtractText(IElement element, string selector) =>
        element.QuerySelector(selector)?.TextContent.Trim() ?? string.Empty;

    private static List<string> ExtractTexts(IParentNode node, string selector) =>
        node.QuerySelectorAll(selector)
        .Select(e => e.TextContent.Trim())
        .Where(text => !string.IsNullOrWhiteSpace(text))
        .Distinct()
        .ToList();

    private static string ExtractDefinition(IElement element, string selector)
    {
        var definition = ExtractText(element, selector);
        return string.IsNullOrEmpty(definition) ? string.Empty : definition.TrimEnd(':');
    }

    private static string ExtractAudio(IElement entry, string selector)
    {
        var src = entry.QuerySelector(selector)?.GetAttribute("src");
        return string.IsNullOrWhiteSpace(src) ? string.Empty : $"https://dictionary.cambridge.org{src}";
    }

    private static string ExtractImage(IElement entry, string selector)
    {
        var src = entry.QuerySelector(selector)?.GetAttribute("src");
        return string.IsNullOrWhiteSpace(src) ? string.Empty : $"https://dictionary.cambridge.org{src}";
    }
    #endregion
}

public static class Selectors
{
    public static class Cambridge
    {
        public const string Entry = ".pr.dictionary[data-id='cald4'] .pr.entry-body__el";
        public const string WordType = ".pos.dpos";
        public const string Phonetic = ".us > .pron.dpron";
        public const string DefinitionWrapper = ".def-block.ddef_block";
        public const string Definition = ".def.ddef_d.db";
        public const string Example = ".examp.dexamp > .eg.deg";
        public const string Audio = ".us.dpron-i source";
        public const string Image = ".dimg .dimg_i";
        public const string Meaning = ".def-block.ddef_block .trans.dtrans";
    }

    public static class Coviet
    {
        public const string FallbackMeaning = "#divContent .m";
    }
}