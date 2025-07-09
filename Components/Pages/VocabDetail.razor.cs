using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using VocabBuilder.Components.Dialogs;
using VocabBuilder.Services.Vocab;
using static VocabBuilder.Shared.Enums;

namespace VocabBuilder.Components.Pages;

public partial class VocabDetail
{
    [Parameter] public int Id { get; set; }

    [Inject] protected IVocabService VocabService { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await ExecuteWithLoadingAsync(() => ViewModel.InitializeAsync(ScreenType.Detail, VocabService, Id));
    }

    private async Task OpenDeleteDialogAsync()
    {
        var dialog = await DialogService.ShowDialogAsync<DeleteDialog>(ViewModel.Detail.Word, new DialogParameters
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
                await VocabService.DeleteVocabAsync(ViewModel.Detail);
                ToastService.ShowSuccess("Vocab deleted successfully.");
                Navigation.NavigateToVocabList();
            }
            catch (Exception e)
            {
                ToastService.ShowError(e.Message);
            }
        }
    }
}
