namespace CleanApiStarter.Application.Features.Projects;

public sealed class UpdateProjectTaskDtoValidator : AbstractValidator<UpdateProjectTaskDto>
{
    public UpdateProjectTaskDtoValidator()
    {
        RuleFor(task => task.Title)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(task => task.Description)
            .MaximumLength(4_000);

        RuleFor(task => task.Status)
            .IsInEnum();

        RuleFor(task => task.DueDate)
            .Must((task, dueDate) => !dueDate.HasValue
                || task.Status == ProjectTaskStatus.Done
                || dueDate.Value > DateTime.UtcNow)
            .When(task => task.DueDate.HasValue && task.Status != ProjectTaskStatus.Done)
            .WithMessage("Due date must be in the future.");
    }
}