using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.FluentUI.AspNetCore.Components;
using VocabBuilder.Components.Pages;
using VocabBuilder.Models.Vocab;
using VocabBuilder.Services.Vocab;
using VocabBuilder.Shared;
using VocabBuilder.ViewModels;
using VocabBuilder.ViewModels.Vocab;
using static VocabBuilder.Shared.Constants;
using static VocabBuilder.Shared.Enums;

namespace VocabBuilder.Components.Forms;

public partial class VocabFormBase : BasePage<VocabViewModel>
{
    [Inject] protected IWebHostEnvironment Environment { get; set; } = default!;
    [Inject] protected HttpClient HttpClient { get; set; } = default!;
    [Inject] protected IVocabService VocabService { get; set; } = default!;

    protected EditContext _editContext = default!;
    protected CustomValidator _customValidator = default!;

    protected bool _isSubmitting;
    protected bool _isWordSearching;
    protected bool _isMediaSearching;

    protected VocabLookupResult _lookupResult = new();
    protected VocabMediaLookupResult _mediaLookupResult = new();

    protected readonly List<string> _availableWordTypes = [];
    protected readonly List<string> _availableDefinitions = [];
    protected readonly List<string> _availableExamples = [];
    protected readonly List<string> _availableMeanings = [];
    protected readonly List<string> _availableImages = [];

    protected int _imageIndex = 0;

    protected bool IsValidFile(FluentInputFileEventArgs file, InputFileViewModel inputFile)
    {
        if (!string.IsNullOrEmpty(file.ErrorMessage))
        {
            _customValidator.AddError(nameof(ViewModel.Detail.InputFiles),
                $"Error uploading file '{file.Name}': {file.ErrorMessage}");
            return false;
        }

        if (file.Size > inputFile.MaxFileSizeInKB * BytesPerKB)
        {
            _customValidator.AddError(nameof(ViewModel.Detail.InputFiles),
                $"File '{file.Name}' exceeds the size limit of {inputFile.MaxFileSizeInKB} KB.");
            return false;
        }

        if (!string.IsNullOrWhiteSpace(inputFile.Accept))
        {
            var isAccepted = FileHelper.IsAcceptedFileType(file.Name, inputFile.Accept);
            if (!isAccepted)
            {
                _customValidator.AddError(nameof(ViewModel.Detail.InputFiles),
                    $"File '{file.Name}' is not an accepted type. Allowed types: {inputFile.Accept}.");
                return false;
            }
        }

        if (file.Stream is null || file.Stream.Length == 0)
        {
            _customValidator.AddError(nameof(ViewModel.Detail.InputFiles),
                $"File '{file.Name}' is empty.");
            return false;
        }

        _customValidator.ClearError(nameof(ViewModel.Detail.InputFiles));
        return true;
    }

    protected async Task HandleFileUploadCompletedAsync(IEnumerable<FluentInputFileEventArgs> files, InputFileViewModel inputFile)
    {
        var file = files.First();
        if (!IsValidFile(file, inputFile))
            return;

        var fileName = FileHelper.NormalizeFileName(ViewModel.Detail.Word, Path.GetExtension(file.Name));
        var tempFilePath = FileHelper.GetFullTempUploadPath(fileName, Environment.WebRootPath);

        await using var fs = new FileStream(tempFilePath, FileMode.Create);
        await file.Stream!.CopyToAsync(fs);
        await file.Stream.DisposeAsync();

        inputFile.FilePath = FileHelper.GetRelativePath(tempFilePath, Environment.WebRootPath);
        inputFile.FileSource = FileSource.Local;
        ToastService.ShowSuccess($"File '{file.Name}' uploaded successfully.");
    }

    protected void HandleRemoveFile(FileType fileType)
    {
        ViewModel.Detail.RemoveFile(fileType);
        if (fileType == FileType.Image)
            _availableImages.Clear();
    }

    protected async Task HandleChangeImageAsync()
    {
        var imageInputFile = ViewModel.Detail.InputFiles.Find(x => x.FileType == FileType.Image);
        if (imageInputFile is null)
        {
            ToastService.ShowWarning("No image input file configured.");
            return;
        }

        if (_availableImages.Count == 0)
        {
            _availableImages.Add(imageInputFile.FilePath);

            var downloadTasks = _mediaLookupResult.AlternativeImageUrls
                .Where(url => !string.IsNullOrWhiteSpace(url))
                .Select(url => DownloadHelper.DownloadUrlToTempFileAsync(HttpClient, url, Environment.WebRootPath, ViewModel.Detail.Word));

            var downloaded = await Task.WhenAll(downloadTasks);
            foreach (var filePath in downloaded.Where(path => !string.IsNullOrWhiteSpace(path)))
                _availableImages.Add(filePath);
        }

        imageInputFile.FilePath = _availableImages[_imageIndex];
        _imageIndex = (_imageIndex + 1) % _availableImages.Count;
    }
}
