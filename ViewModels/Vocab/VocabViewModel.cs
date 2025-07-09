using Microsoft.FluentUI.AspNetCore.Components;
using VocabBuilder.Services.Vocab;
using static VocabBuilder.Shared.Enums;

namespace VocabBuilder.ViewModels.Vocab;

public class VocabViewModel
{
    public ScreenType ScreenType { get; set; }
    public VocabSearchViewModel Search { get; set; } = default!;
    public VocabSearchResultViewModel Result { get; set; } = default!;
    public VocabDetailViewModel Detail { get; set; } = default!;
    public VocabDetailViewModel CloneDetail { get; set; } = default!;

    public async Task InitializeAsync(ScreenType screenType, IVocabService service, int? id = null)
    {
        ScreenType = screenType;
        switch (screenType)
        {
            case ScreenType.List:
                Search = new();
                Result = new();
                break;

            case ScreenType.Detail:
                if (id is null)
                    throw new ArgumentException("Id is required for Detail screen");
                Detail = await service.GetVocabByIdAsync(id.Value);
                break;

            case ScreenType.Add:
                Detail = new();
                break;

            case ScreenType.Edit:
                if (id is null)
                    throw new ArgumentException("Id is required for Edit screen");
                Detail = await service.GetVocabByIdAsync(id.Value);
                CloneDetail = Detail.Clone();
                break;
        }
    }

    public async Task RefreshDataGridAsync(GridItemsProviderRequest<VocabDetailViewModel> req, IVocabService service, PaginationState pagination)
    {
        Search.StartIndex = req.StartIndex;
        Search.Count = req.Count;
        Result = await service.GetVocabsAsync(Search.ToSearchCriteria());
        await pagination.SetTotalItemCountAsync(Result.TotalCount);
    }
}
