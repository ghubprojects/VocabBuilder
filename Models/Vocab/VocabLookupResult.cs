namespace VocabBuilder.Models.Vocab;

public class VocabLookupResult
{
    public string Word { get; set; } = string.Empty;
    public List<VocabEntry> Entries { get; set; } = [];
    public List<string> Meanings { get; set; } = [];
}

public class VocabEntry
{
    public string WordType { get; set; } = string.Empty;
    public string Phonetic { get; set; } = string.Empty;
    public string AudioUrl { get; set; } = string.Empty;
    public List<VocabDefinition> Definitions { get; set; } = [];
}

public class VocabDefinition
{
    public string Definition { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public List<string> Examples { get; set; } = [];
}

public class VocabMediaLookupResult
{
    public string AudioFilePath { get; set; } = string.Empty;
    public string ImageFilePath { get; set; } = string.Empty;
    public string AudioFileSource { get; set; } = string.Empty;
    public string ImageFileSource { get; set; } = string.Empty;
    public List<string> AlternativeImageUrls { get; set; } = [];
}