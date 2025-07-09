using Microsoft.AspNetCore.Components;
using static VocabBuilder.Shared.Enums;

namespace VocabBuilder.Services.Navigation;

public class NavigationService(NavigationManager navigation) : INavigationService
{
    private static string GetRoute(ScreenModule module, ScreenType type)
    {
        return type switch
        {
            ScreenType.List => $"/{module}",
            ScreenType.Add => $"/{module}/Add",
            ScreenType.Detail => $"/{module}/{{id}}",
            ScreenType.Edit => $"/{module}/{{id}}/Edit",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    private void NavigateTo(ScreenModule module, ScreenType type, int? id = null)
    {
        var route = GetRoute(module, type);
        if (route.Contains("{id}"))
        {
            if (id is null)
                throw new ArgumentException("Id is required for this screen type.", nameof(id));
            route = route.Replace("{id}", id.ToString());
        }
        navigation.NavigateTo(route);
    }

    public void NavigateToVocabList() => NavigateTo(ScreenModule.Vocab, ScreenType.List);
    public void NavigateToVocabAdd() => NavigateTo(ScreenModule.Vocab, ScreenType.Add);
    public void NavigateToVocabDetail(int id) => NavigateTo(ScreenModule.Vocab, ScreenType.Detail, id);
    public void NavigateToVocabEdit(int id) => NavigateTo(ScreenModule.Vocab, ScreenType.Edit, id);
}
