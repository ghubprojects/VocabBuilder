using VocabBuilder.Models;
using VocabBuilder.Models.Vocab;
using VocabBuilder.ViewModels.Vocab;

namespace VocabBuilder.Services.Vocab;

public interface IVocabService
{
    Task<VocabSearchResultViewModel> GetVocabsAsync(VocabSearchCriteria criteria);
    Task<VocabDetailViewModel> GetVocabByIdAsync(int id);
    Task AddVocabAsync(VocabDetailViewModel model);
    Task UpdateVocabAsync(VocabDetailViewModel model);
    Task DeleteVocabAsync(VocabDetailViewModel model);
    Task<byte[]> ExportVocabsToCsvAsync(VocabSearchCriteria criteria);
    Task<LookupResult<VocabLookupResult>> LookupWordAsync(string word);
    Task<LookupResult<VocabMediaLookupResult>> LookupMediaAsync(string audioUrl, string imageUrl, string word);
}