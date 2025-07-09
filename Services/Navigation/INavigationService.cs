namespace VocabBuilder.Services.Navigation;

public interface INavigationService
{
    void NavigateToVocabList();
    void NavigateToVocabAdd();
    void NavigateToVocabDetail(int id);
    void NavigateToVocabEdit(int id);
}