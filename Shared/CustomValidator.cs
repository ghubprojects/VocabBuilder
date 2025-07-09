using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace VocabBuilder.Shared;

public class CustomValidator : ComponentBase
{
    [CascadingParameter] private EditContext? CurrentEditContext { get; set; }

    private ValidationMessageStore messageStore = default!;

    protected override void OnInitialized()
    {
        if (CurrentEditContext is null)
            throw new InvalidOperationException($"{nameof(CustomValidator)} requires a cascading parameter of type {nameof(EditContext)}.");

        messageStore = new(CurrentEditContext);
        CurrentEditContext.OnValidationRequested += (_, _) => messageStore.Clear();
        CurrentEditContext.OnFieldChanged += (_, e) => messageStore.Clear(e.FieldIdentifier);
    }

    public void AddError(string fieldName, string errorMessage)
    {
        if (CurrentEditContext is null)
            return;

        var fieldIdentifier = CurrentEditContext.Field(fieldName);
        messageStore.Clear(fieldIdentifier);
        messageStore.Add(fieldIdentifier, errorMessage);
        NotifyValidationStateChanged();
    }

    public void ClearError(string fieldName)
    {
        if (CurrentEditContext is null)
            return;

        messageStore.Clear(CurrentEditContext.Field(fieldName));
        NotifyValidationStateChanged();
    }

    public void ClearErrors()
    {
        messageStore.Clear();
        NotifyValidationStateChanged();
    }

    public void DisplayErrors()
    {
        NotifyValidationStateChanged();
    }

    private void NotifyValidationStateChanged()
    {
        CurrentEditContext?.NotifyValidationStateChanged();
    }
}
