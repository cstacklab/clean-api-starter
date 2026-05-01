namespace CleanApiStarter.Application.Features.Words;

public sealed class CreateWordDtoValidator : AbstractValidator<CreateWordDto>
{
    public CreateWordDtoValidator()
    {
        RuleFor(word => word.Text)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(word => word.Meaning)
            .NotEmpty();

        RuleFor(word => word.Synonyms)
            .NotNull();

        RuleForEach(word => word.Synonyms)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(word => word.UsageExample)
            .NotEmpty();
    }
}