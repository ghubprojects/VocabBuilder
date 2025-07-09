namespace VocabBuilder.ViewModels.Vocab;

public class VocabSearchResultViewModel
{
    public List<VocabDetailViewModel> Details { get; set; } = [];
    public int TotalCount { get; set; }
}
