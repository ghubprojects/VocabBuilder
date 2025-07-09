using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using VocabBuilder.Components.Dialogs;
using VocabBuilder.Services.Vocab;
using VocabBuilder.ViewModels.Vocab;
using static VocabBuilder.Shared.Enums;

namespace VocabBuilder.Components.Pages;

public partial class VocabList
{
    [Inject] protected IJSRuntime JS { get; set; } = default!;
    [Inject] protected IVocabService VocabService { get; set; } = default!;

    private FluentDataGrid<VocabDetailViewModel> _dataGrid = default!;
    private readonly PaginationState _pagination = new() { ItemsPerPage = 20 };

    protected override async Task OnInitializedAsync()
    {
        await ViewModel.InitializeAsync(ScreenType.List, VocabService);
    }

    private async Task RefreshItemsAsync(GridItemsProviderRequest<VocabDetailViewModel> req)
    {
        await ExecuteWithLoadingAsync(() => ViewModel.RefreshDataGridAsync(req, VocabService, _pagination));
    }

    private async Task RefreshDataAsync()
    {
        await _dataGrid.RefreshDataAsync(true);
        StateHasChanged();
    }

    private async Task ExportAsync()
    {
        try
        {
            var searchViewModel = ViewModel.Search.Clone();
            searchViewModel.StartIndex = 0;
            searchViewModel.Count = null;
            var bytes = await VocabService.ExportVocabsToCsvAsync(searchViewModel.ToSearchCriteria());
            var fileName = $"vocabs_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
            await DownloadFile(fileName, bytes, "text/csv");
        }
        catch (Exception e)
        {
            ToastService.ShowError(e.Message);
            return;
        }
    }

    private async Task DownloadFile(string fileName, byte[] content, string contentType)
    {
        await JS.InvokeVoidAsync("downloadFile", fileName, content, contentType);
    }

    private async Task OpenDeleteDialogAsync(VocabDetailViewModel model)
    {
        var dialog = await DialogService.ShowDialogAsync<DeleteDialog>(model.Word, new DialogParameters
        {
            Title = $"Confirm Delete",
            Width = "480px",
            Height = "240px",
        });

        var result = await dialog.Result;
        if (!result.Cancelled && result.Data is not null)
        {
            try
            {
                await VocabService.DeleteVocabAsync(model);
                ToastService.ShowSuccess("Vocab deleted successfully.");
                await RefreshDataAsync();
            }
            catch (Exception e)
            {
                ToastService.ShowError(e.Message);
            }
        }
    }
}
