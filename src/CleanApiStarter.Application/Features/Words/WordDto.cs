namespace CleanApiStarter.Application.Features.Words;

public class WordDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Meaning { get; set; } = string.Empty;
    public List<string> Synonyms { get; set; } = new();
    public string UsageExample { get; set; } = string.Empty;
}

public class CreateWordDto
{
    public string Text { get; set; } = string.Empty;
    public string Meaning { get; set; } = string.Empty;
    public List<string> Synonyms { get; set; } = new();
    public string UsageExample { get; set; } = string.Empty;
}