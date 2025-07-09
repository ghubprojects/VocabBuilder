using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using VocabBuilder.Services.Navigation;

namespace VocabBuilder.Components.Pages;

public abstract class BasePage<T> : ComponentBase where T : class, new()
{
    [Inject] protected IDialogService DialogService { get; set; } = default!;
    [Inject] protected IToastService ToastService { get; set; } = default!;
    [Inject] protected INavigationService Navigation { get; set; } = default!;

    protected T ViewModel { get; set; } = new T();
    protected bool IsLoading { get; set; } = false;

    protected async Task ExecuteWithLoadingAsync(Func<Task> action)
    {
        try
        {
            IsLoading = true;
            StateHasChanged();

            await action();
        }
        catch (Exception e)
        {
            ToastService.ShowError(e.Message);
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }
}
