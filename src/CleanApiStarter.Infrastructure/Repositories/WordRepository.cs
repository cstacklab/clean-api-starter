namespace CleanApiStarter.Infrastructure.Repositories;

public class WordRepository(IDatabaseConnectionFactory connectionFactory) : IWordRepository
{
    public async Task<Guid> AddWordAsync(Word word, CancellationToken cancellationToken)
    {
        const string sql =
            """
               INSERT INTO words (id, text, meaning, synonyms, usage_example, created_at, updated_at)
               VALUES (@Id, @Text, @Meaning, @Synonyms::jsonb, @UsageExample, @CreatedAt, @UpdatedAt)
               RETURNING id
            """;

        using IDbConnection connection = connectionFactory.CreateConnection();

        var parameters = new
        {
            word.Id,
            word.Text,
            word.Meaning,
            Synonyms = JsonSerializer.Serialize(word.Synonyms),
            word.UsageExample,
            word.CreatedAt,
            word.UpdatedAt
        };

        Guid id = await connection.ExecuteScalarAsync<Guid>(sql, parameters);
        return id;
    }

    public async Task<Word?> GetWordByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        const string sql =
            """
            SELECT id, text, meaning, synonyms::text as SynonymsJson, usage_example as UsageExample, created_at as CreatedAt, updated_at as UpdatedAt 
                                       FROM words 
                                       WHERE id = @Id
            """;

        using IDbConnection connection = connectionFactory.CreateConnection();
        WordRow? word = await connection.QuerySingleOrDefaultAsync<WordRow>(sql, new { Id = id });

        return word?.ToDomain();
    }

    public async Task<IEnumerable<Word>> GetAllWordsAsync(CancellationToken cancellationToken)
    {
        const string sql =
            """
            SELECT id, text, meaning, synonyms::text as SynonymsJson, usage_example as UsageExample, created_at as CreatedAt, updated_at as UpdatedAt 
                                       FROM words
            """;

        using IDbConnection connection = connectionFactory.CreateConnection();
        IEnumerable<WordRow> words = await connection.QueryAsync<WordRow>(sql);

        return words.Select(word => word.ToDomain());
    }

    public async Task<bool> UpdateWordAsync(Word word, CancellationToken cancellationToken)
    {
        const string sql =
            """
               UPDATE words 
               SET text = @Text, 
                   meaning = @Meaning, 
                   synonyms = @Synonyms, 
                   usage_example = @UsageExample, 
                   updated_at = @UpdatedAt
               WHERE id = @Id
            """;

        using IDbConnection connection = connectionFactory.CreateConnection();
        var parameters = new
        {
            word.Id,
            word.Text,
            word.Meaning,
            Synonyms = JsonSerializer.Serialize(word.Synonyms),
            word.UsageExample,
            word.UpdatedAt
        };

        int rowsAffected = await connection.ExecuteAsync(sql, parameters);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteWordAsync(Guid id, CancellationToken cancellationToken)
    {
        const string sql = "DELETE FROM words WHERE id = @Id";

        using IDbConnection connection = connectionFactory.CreateConnection();
        int rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }

    private sealed class WordRow
    {
        public Guid Id { get; init; }
        public string Text { get; init; } = string.Empty;
        public string Meaning { get; init; } = string.Empty;
        public string SynonymsJson { get; init; } = string.Empty;
        public string UsageExample { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }

        public Word ToDomain()
        {
            return new Word
            {
                Id = Id,
                Text = Text,
                Meaning = Meaning,
                Synonyms = JsonSerializer.Deserialize<List<string>>(SynonymsJson) ?? [],
                UsageExample = UsageExample,
                CreatedAt = CreatedAt,
                UpdatedAt = UpdatedAt
            };
        }
    }
}