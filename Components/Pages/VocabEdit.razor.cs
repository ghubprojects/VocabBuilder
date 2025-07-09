using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using static VocabBuilder.Shared.Enums;

namespace VocabBuilder.Components.Pages;

public partial class VocabEdit
{
    [Parameter] public int Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await ExecuteWithLoadingAsync(() => ViewModel.InitializeAsync(ScreenType.Edit, VocabService, Id));
        _editContext = new EditContext(ViewModel.Detail);
    }

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate() || _isSubmitting)
            return;

        _isSubmitting = true;
        StateHasChanged();

        try
        {
            await VocabService.UpdateVocabAsync(ViewModel.Detail);
            ToastService.ShowSuccess("Vocab updated successfully.");
            Navigation.NavigateToVocabList();
        }
        catch (Exception ex)
        {
            ToastService.ShowError($"Error updating vocab: {ex.Message}");
        }
        finally
        {
            _isSubmitting = false;
            StateHasChanged();
        }
    }

    private void RevertForm()
    {
        ViewModel.Detail = ViewModel.CloneDetail;
        _editContext = new EditContext(ViewModel.Detail);
    }

    private async Task LookupWordAsync()
    {
        if (string.IsNullOrWhiteSpace(ViewModel.Detail.Word))
        {
            ToastService.ShowWarning("Please enter a word first.");
            return;
        }

        if (_isWordSearching)
            return;

        _isWordSearching = true;
        StateHasChanged();

        try
        {
            var result = await VocabService.LookupWordAsync(ViewModel.Detail.Word);
            if (!result.Success)
                ToastService.ShowWarning(result.Error);

            _lookupResult = result.Data!;
            if (_lookupResult is null)
                return;

            _availableWordTypes.Clear();
            _availableWordTypes.AddRange(_lookupResult.Entries.Select(e => e.WordType).Distinct());

            _availableDefinitions.Clear();
            _availableDefinitions.AddRange(
                _lookupResult.Entries.SelectMany(
                    e => e.Definitions.Select(d => d.Definition).Distinct()));

            _availableExamples.Clear();
            _availableExamples.AddRange(
                _lookupResult.Entries.SelectMany(e => e.Definitions.SelectMany(d => d.Examples)));

            _availableMeanings.Clear();
            _availableMeanings.AddRange(_lookupResult.Meanings);

            // Initialize the detail view model with the first entry
            var firstEntry = _lookupResult.Entries.First();
            ViewModel.Detail.WordType = firstEntry.WordType;
            ViewModel.Detail.Phonetic = firstEntry.Phonetic;
            ViewModel.Detail.Definition = firstEntry.Definitions.First().Definition;
            ViewModel.Detail.Example = firstEntry.Definitions.First().Examples.First();
        }
        catch (Exception e)
        {
            ToastService.ShowError($"Error during searching word: {e.Message}");
        }
        finally
        {
            _isWordSearching = false;
            StateHasChanged();
        }
    }

    private void HandleChangeWordType()
    {
        try
        {
            //ViewModel.Detail.WordType = wordType;

            if (_lookupResult is null)
                return;

            var entry = _lookupResult.Entries.Find(e => e.WordType == ViewModel.Detail.WordType);
            if (entry is null)
                return;

            ViewModel.Detail.Phonetic = entry.Phonetic;

            _availableDefinitions.Clear();
            _availableDefinitions.AddRange(entry.Definitions.Select(d => d.Definition));
            ViewModel.Detail.Definition = _availableDefinitions.First();

            UpdateExamples();
        }
        catch (Exception e)
        {
            ToastService.ShowError($"Error changing word type: {e.Message}");
        }
    }

    private void HandleChangeDefinition()
    {
        try
        {
            if (_lookupResult is null)
                return;

            var entry = _lookupResult.Entries.Find(
                e => e.Definitions.Any(d => d.Definition == ViewModel.Detail.Definition));
            if (entry is null)
                return;

            ViewModel.Detail.WordType = entry.WordType;
            ViewModel.Detail.Phonetic = entry.Phonetic;

            UpdateExamples();
        }
        catch (Exception e)
        {
            ToastService.ShowError($"Error changing definition: {e.Message}");
        }
    }

    private void HandleChangeExample()
    {
        try
        {
            if (_lookupResult is null)
                return;

            var entry = _lookupResult.Entries.Find(
                e => e.Definitions.Any(d => d.Examples.Contains(ViewModel.Detail.Example)));
            if (entry is null)
                return;

            ViewModel.Detail.WordType = entry.WordType;
            ViewModel.Detail.Phonetic = entry.Phonetic;

            var definition = entry.Definitions.Find(d => d.Examples.Contains(ViewModel.Detail.Example));
            if (definition is not null)
            {
                ViewModel.Detail.Definition = definition.Definition;
            }
        }
        catch (Exception e)
        {
            ToastService.ShowError($"Error changing example: {e.Message}");
        }
    }

    private void UpdateExamples()
    {
        try
        {
            if (_lookupResult is null)
                return;

            var entry = _lookupResult.Entries.Find(
                e => e.Definitions.Any(d => d.Definition == ViewModel.Detail.Definition));
            if (entry is null)
                return;

            _availableExamples.Clear();
            _availableExamples.AddRange(
                entry.Definitions
                .Where(d => d.Definition == ViewModel.Detail.Definition)
                .SelectMany(d => d.Examples));
            ViewModel.Detail.Example = _availableExamples.First();
        }
        catch (Exception e)
        {
            ToastService.ShowError($"Error updating examples: {e.Message}");
        }
    }

    private async Task LookupMediaAsync()
    {
        if (_lookupResult is null)
        {
            ToastService.ShowWarning("Please search for a word first.");
            return;
        }

        if (string.IsNullOrEmpty(ViewModel.Detail.Word)
            || string.IsNullOrEmpty(ViewModel.Detail.WordType)
            || string.IsNullOrEmpty(ViewModel.Detail.Definition))
        {
            ToastService.ShowWarning("Please complete the word, word type, and definition first.");
            return;
        }

        if (_isMediaSearching)
            return;

        _isMediaSearching = true;
        StateHasChanged();

        try
        {
            var entry = _lookupResult.Entries.First(e => e.WordType == ViewModel.Detail.WordType);
            var definition = entry.Definitions.First(d => d.Definition == ViewModel.Detail.Definition);

            var result = await VocabService.LookupMediaAsync(entry.AudioUrl, definition.ImageUrl, ViewModel.Detail.Word);
            if (!result.Success)
                ToastService.ShowWarning(result.Error);

            _mediaLookupResult = result.Data!;
            if (_mediaLookupResult is null)
                return;

            foreach (var inputFile in ViewModel.Detail.InputFiles)
            {
                switch (inputFile.FileType)
                {
                    case FileType.Audio:
                        inputFile.FilePath = _mediaLookupResult.AudioFilePath;
                        inputFile.FileSource = _mediaLookupResult.AudioFileSource;
                        break;
                    case FileType.Image:
                        inputFile.FilePath = _mediaLookupResult.ImageFilePath;
                        inputFile.FileSource = _mediaLookupResult.ImageFileSource;
                        break;

                }
            }
        }
        catch (Exception e)
        {
            ToastService.ShowError($"Error during searching media: {e.Message}");
        }
        finally
        {
            _isMediaSearching = false;
            StateHasChanged();
        }
    }
}