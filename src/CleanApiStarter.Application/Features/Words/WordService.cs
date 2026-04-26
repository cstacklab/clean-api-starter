namespace CleanApiStarter.Application.Features.Words;

public class WordService(IWordRepository wordRepository, ILogger<WordService> logger) : IWordService
{
    public async Task<Guid> AddWordAsync(CreateWordDto wordDto, CancellationToken cancellationToken)
    {
        Word word = new()
        {
            Id = Guid.NewGuid(),
            Text = wordDto.Text,
            Meaning = wordDto.Meaning,
            Synonyms = wordDto.Synonyms,
            UsageExample = wordDto.UsageExample,
            CreatedAt = DateTime.UtcNow
        };

        logger.LogInformation(
            "Creating word {WordId} with {SynonymCount} synonyms",
            word.Id,
            word.Synonyms.Count);

        Guid wordId = await wordRepository.AddWordAsync(word, cancellationToken);

        logger.LogInformation("Created word {WordId}", wordId);

        return wordId;
    }

    public async Task<WordDto?> GetWordByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        Word? word = await wordRepository.GetWordByIdAsync(id, cancellationToken);

        if (word == null)
        {
            logger.LogWarning("Word {WordId} was not found", id);
            return null;
        }

        logger.LogInformation("Retrieved word {WordId}", id);

        return MapToDto(word);
    }

    public async Task<IEnumerable<WordDto>> GetAllWordsAsync(CancellationToken cancellationToken)
    {
        IEnumerable<Word> words = await wordRepository.GetAllWordsAsync(cancellationToken);
        Word[] wordList = words.ToArray();

        logger.LogInformation("Retrieved {WordCount} words", wordList.Length);

        return wordList.Select(MapToDto);
    }

    public async Task<bool> UpdateWordAsync(Guid id, CreateWordDto wordDto, CancellationToken cancellationToken)
    {
        Word? existingWord = await wordRepository.GetWordByIdAsync(id, cancellationToken);
        if (existingWord == null)
        {
            logger.LogWarning("Word {WordId} could not be updated because it was not found", id);
            return false;
        }

        existingWord.Text = wordDto.Text;
        existingWord.Meaning = wordDto.Meaning;
        existingWord.Synonyms = wordDto.Synonyms;
        existingWord.UsageExample = wordDto.UsageExample;
        existingWord.UpdatedAt = DateTime.UtcNow;

        bool updated = await wordRepository.UpdateWordAsync(existingWord, cancellationToken);

        logger.LogInformation(
            "Updated word {WordId}; success: {UpdateSucceeded}",
            id,
            updated);

        return updated;
    }

    public async Task<bool> DeleteWordAsync(Guid id, CancellationToken cancellationToken)
    {
        bool deleted = await wordRepository.DeleteWordAsync(id, cancellationToken);

        if (!deleted)
        {
            logger.LogWarning("Word {WordId} could not be deleted because it was not found", id);
            return false;
        }

        logger.LogInformation("Deleted word {WordId}", id);

        return true;
    }

    private static WordDto MapToDto(Word word)
    {
        return new WordDto
        {
            Id = word.Id,
            Text = word.Text,
            Meaning = word.Meaning,
            Synonyms = word.Synonyms,
            UsageExample = word.UsageExample
        };
    }
}