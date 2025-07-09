using Microsoft.FluentUI.AspNetCore.Components;
using static VocabBuilder.Shared.Enums;

namespace VocabBuilder.ViewModels;

public class InputFileViewModel
{
    public FileType FileType { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string FileSource { get; set; } = string.Empty;
    public string Accept { get; set; } = string.Empty;
    public string AnchorId { get; set; } = string.Empty;
    public int MaxFileSizeInKB { get; set; }
    public Icon Icon { get; set; } = default!;

    public string GetPlaceholder(bool isEditMode = true)
    {
        if (!isEditMode)
            return "No file selected";
        var typeLabel = FileType.ToString().ToLower(); // e.g. audio, image
        return $"Select an {typeLabel} file (≤{MaxFileSizeInKB} KB)";
    }
}