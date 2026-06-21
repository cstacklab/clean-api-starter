namespace CleanApiStarter.Api.UnitTests.Features.Projects.Tasks;

public sealed class CompleteTaskTests
{
    [Theory]
    [AutoNSubstituteData]
    public async Task Handle_TaskIsAlreadyCompleted_ReturnsConflictAndDoesNotSave(
        Guid projectId,
        Guid taskId,
        string userId,
        [Frozen] IUser currentUser,
        [Frozen] IProjectRepository projectRepository)
    {
        // Arrange
        CancellationToken cancellationToken = CancellationToken.None;
        ProjectTask task = new()
        {
            Id = taskId,
            ProjectId = projectId,
            Title = "Write first unit test",
            Description = "Cover already completed tasks",
            Status = ProjectTaskStatus.Done,
            CreatedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow
        };

        currentUser.Id.Returns(userId);
        projectRepository
            .GetTaskForUpdateAsync(projectId, taskId, userId, cancellationToken)
            .Returns(Task.FromResult<ProjectTask?>(task));

        // Act
        IResult result = await CompleteTask.Handle(projectId, taskId, projectRepository, currentUser, cancellationToken);

        // Assert
        result.ShouldBeOfType<Conflict<string>>();
        _ = projectRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
