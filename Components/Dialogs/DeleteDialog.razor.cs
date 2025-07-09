using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace VocabBuilder.Components.Dialogs;

public partial class DeleteDialog
{
    [Parameter] public string Content { get; set; } = default!;

    [CascadingParameter] public FluentDialog Dialog { get; set; } = default!;

    private async Task DeleteAsync() => await Dialog.CloseAsync(Content);

    private async Task CancelAsync() => await Dialog.CancelAsync();
}
