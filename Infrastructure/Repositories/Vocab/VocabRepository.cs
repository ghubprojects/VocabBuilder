using Microsoft.EntityFrameworkCore;
using VocabBuilder.Infrastructure.DataContext;
using VocabBuilder.Infrastructure.Entities.Vocab;
using VocabBuilder.Models;
using VocabBuilder.Models.Vocab;

namespace VocabBuilder.Infrastructure.Repositories.Vocab;

public class VocabRepository(IDbContextFactory<AppDbContext> factory) : IVocabRepository
{
    public async Task<List<VocabEntity>> GetAllAsync(VocabSearchCriteria criteria)
    {
        return await GetQuery(criteria)
            .OrderByDescending(x => x.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<PaginationResult<VocabEntity>> GetPagedAsync(VocabSearchCriteria criteria)
    {
        var query = GetQuery(criteria);
        var pagedQuery = query
            .OrderByDescending(x => x.CreatedAt)
            .Skip(criteria.StartIndex);

        if (criteria.Count.HasValue)
            pagedQuery = pagedQuery.Take(criteria.Count.Value);

        var totalCount = await query.CountAsync();
        var items = await pagedQuery
            .AsNoTracking()
            .ToListAsync();

        return new PaginationResult<VocabEntity>(items, totalCount);
    }

    private IQueryable<VocabEntity> GetQuery(VocabSearchCriteria criteria)
    {
        var context = factory.CreateDbContext();
        var query = context.Vocabs.AsQueryable();
        if (!string.IsNullOrEmpty(criteria.Word))
            query = query.Where(x => EF.Functions.ILike(x.Word, $"%{criteria.Word}%"));
        return query;
    }

    public async Task<VocabEntity?> GetByIdAsync(int id)
    {
        var context = factory.CreateDbContext();
        return await context.Vocabs.FindAsync(id);
    }

    public async Task AddAsync(VocabEntity entity)
    {
        var context = factory.CreateDbContext();
        context.Vocabs.Add(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(VocabEntity entity)
    {
        var context = factory.CreateDbContext();
        context.Vocabs.Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(VocabEntity entity)
    {
        var context = factory.CreateDbContext();
        context.Vocabs.Remove(entity);
        await context.SaveChangesAsync();
    }
}