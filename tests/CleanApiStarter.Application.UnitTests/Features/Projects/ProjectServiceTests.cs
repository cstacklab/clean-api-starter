namespace CleanApiStarter.Application.UnitTests.Features.Projects;

public sealed class ProjectServiceTests
{
    [Theory]
    [AutoNSubstituteData]
    public async Task CompleteTaskAsync_TaskIsAlreadyCompleted_ReturnsAlreadyCompleted(
        Guid projectId,
        Guid taskId,
        string userId,
        [Frozen] IUser currentUser,
        [Frozen] IProjectRepository projectRepository,
        ProjectService sut)
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
        ProjectTaskMutationResult result = await sut.CompleteTaskAsync(projectId, taskId, cancellationToken);

        // Assert
        result.ShouldBe(ProjectTaskMutationResult.AlreadyCompleted);
        _ = projectRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
