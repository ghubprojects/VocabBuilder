using VocabBuilder.Infrastructure.Entities.Vocab;
using VocabBuilder.Models;
using VocabBuilder.Models.Vocab;

namespace VocabBuilder.Infrastructure.Repositories.Vocab;

public interface IVocabRepository
{
    Task<PaginationResult<VocabEntity>> GetPagedAsync(VocabSearchCriteria criteria);
    Task<VocabEntity?> GetByIdAsync(int id);
    Task AddAsync(VocabEntity entity);
    Task UpdateAsync(VocabEntity entity);
    Task DeleteAsync(VocabEntity entity);
}