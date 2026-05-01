namespace CleanApiStarter.Infrastructure.Repositories;

public class WordRepository(ApplicationDbContext dbContext) : IWordRepository
{
    public async Task<Guid> AddWordAsync(Word word, CancellationToken cancellationToken)
    {
        dbContext.Words.Add(word);
        await dbContext.SaveChangesAsync(cancellationToken);

        return word.Id;
    }

    public async Task<Word?> GetWordByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Words
            .AsNoTracking()
            .SingleOrDefaultAsync(word => word.Id == id, cancellationToken);
    }

    public async Task<PaginatedResult<Word>> GetWordsAsync(PaginatedQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Word> wordsQuery = dbContext.Words
            .AsNoTracking()
            .OrderBy(word => word.Text);

        int totalCount = await wordsQuery.CountAsync(cancellationToken);
        List<Word> words = await wordsQuery
            .Skip(query.Offset)
            .Take(query.Limit)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<Word>
        {
            Items = words,
            Limit = query.Limit,
            Offset = query.Offset,
            TotalCount = totalCount
        };
    }

    public async Task<bool> UpdateWordAsync(Word word, CancellationToken cancellationToken)
    {
        dbContext.Words.Update(word);
        int rowsAffected = await dbContext.SaveChangesAsync(cancellationToken);

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteWordAsync(Guid id, CancellationToken cancellationToken)
    {
        int rowsAffected = await dbContext.Words
            .Where(word => word.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return rowsAffected > 0;
    }
}