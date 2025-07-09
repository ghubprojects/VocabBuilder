using VocabBuilder.Models;
using VocabBuilder.Models.Vocab;

namespace VocabBuilder.Infrastructure.Providers.CambridgeDictionary;

public interface ICambridgeDictionaryProvider
{
    Task<LookupResult<VocabLookupResult>> LookupAsync(string word);
    Task<LookupResult<VocabMediaLookupResult>> LookupMediaAsync(string audioUrl, string imageUrl, string word);
}
