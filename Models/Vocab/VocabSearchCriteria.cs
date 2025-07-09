namespace VocabBuilder.Models.Vocab;

public class VocabSearchCriteria
{
    public string? Word { get; set; }
    public int StartIndex { get; set; }
    public int? Count { get; set; }
}
