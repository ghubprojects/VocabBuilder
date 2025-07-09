using VocabBuilder.Models.Vocab;

namespace VocabBuilder.ViewModels.Vocab;

public class VocabSearchViewModel
{
    public string Word { get; set; } = string.Empty;
    public int StartIndex { get; set; }
    public int? Count { get; set; }

    public VocabSearchCriteria ToSearchCriteria() => new()
    {
        Word = Word,
        StartIndex = StartIndex,
        Count = Count
    };

    public VocabSearchViewModel Clone() => new()
    {
        Word = Word,
        StartIndex = StartIndex,
        Count = Count
    };
}
