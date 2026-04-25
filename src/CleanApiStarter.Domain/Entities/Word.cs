namespace CleanApiStarter.Domain.Entities;

public class Word
{
    public Guid Id { get; init; }
    public string Text { get; set; } = string.Empty;
    public string Meaning { get; set; } = string.Empty;
    public List<string> Synonyms { get; set; } = [];
    public string UsageExample { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}