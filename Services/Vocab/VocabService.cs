using VocabBuilder.Infrastructure.Providers.CambridgeDictionary;
using VocabBuilder.Infrastructure.Repositories.Vocab;
using VocabBuilder.Models;
using VocabBuilder.Models.Vocab;
using VocabBuilder.Services.Export;
using VocabBuilder.Shared;
using VocabBuilder.ViewModels;
using VocabBuilder.ViewModels.Vocab;

namespace VocabBuilder.Services.Vocab;

public class VocabService(
    IVocabRepository vocabRepository,
    ICsvExporter csvExporter,
    ICambridgeDictionaryProvider cambridgeDictionaryProvider,
    IWebHostEnvironment environment) : IVocabService
{
    public async Task<VocabSearchResultViewModel> GetVocabsAsync(VocabSearchCriteria criteria)
    {
        var result = await vocabRepository.GetPagedAsync(criteria);
        return new VocabSearchResultViewModel
        {
            Details = result.Items.Select(VocabDetailViewModel.FromEntity).ToList(),
            TotalCount = result.TotalCount
        };
    }

    public async Task<VocabDetailViewModel> GetVocabByIdAsync(int id)
    {
        var item = await vocabRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Vocab with ID {id} not found");
        return VocabDetailViewModel.FromEntity(item);
    }

    public async Task AddVocabAsync(VocabDetailViewModel model)
    {
        await vocabRepository.AddAsync(model.ToEntity());
        SaveFiles(model.InputFiles);
    }

    public async Task UpdateVocabAsync(VocabDetailViewModel model)
    {
        await vocabRepository.UpdateAsync(model.ToEntity());
        SaveFiles(model.InputFiles);
    }

    public async Task DeleteVocabAsync(VocabDetailViewModel model)
    {
        await vocabRepository.DeleteAsync(model.ToEntity());
    }

    public async Task<byte[]> ExportVocabsToCsvAsync(VocabSearchCriteria criteria)
    {
        var result = await vocabRepository.GetPagedAsync(criteria);
        var exportModels = result.Items.Select(VocabCsvExport.FromEntity).ToList();
        return csvExporter.Export(exportModels);
    }

    public async Task<LookupResult<VocabLookupResult>> LookupWordAsync(string word)
    {
        return await cambridgeDictionaryProvider.LookupAsync(word);
    }

    public async Task<LookupResult<VocabMediaLookupResult>> LookupMediaAsync(string audioUrl, string imageUrl, string word)
    {
        return await cambridgeDictionaryProvider.LookupMediaAsync(audioUrl, imageUrl, word);
    }

    private void SaveFiles(List<InputFileViewModel> inputFiles)
    {
        foreach (var inputFile in inputFiles)
        {
            var filePath = inputFile.FilePath;
            if (string.IsNullOrEmpty(filePath))
                continue;

            var sourcePath = FileHelper.GetFullPath(filePath, environment.WebRootPath);
            var targetPath = FileHelper.GetFullUploadPath(filePath, environment.WebRootPath);
            File.Copy(sourcePath, targetPath, overwrite: true);
        }
    }
}
