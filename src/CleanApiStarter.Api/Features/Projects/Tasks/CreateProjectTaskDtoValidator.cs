namespace CleanApiStarter.Api.Features.Projects.Tasks;

public sealed class CreateProjectTaskDtoValidator : AbstractValidator<CreateProjectTaskDto>
{
    public CreateProjectTaskDtoValidator()
    {
        RuleFor(task => task.Title)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(task => task.Description)
            .MaximumLength(4_000);

        RuleFor(task => task.DueDate)
            .Must(dueDate => !dueDate.HasValue || dueDate.Value > DateTime.UtcNow)
            .When(task => task.DueDate.HasValue)
            .WithMessage("Due date must be in the future.");
    }
}
