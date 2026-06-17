namespace CleanApiStarter.Api.IntegrationTests.Features.Projects;

public sealed class ProjectsTests : IClassFixture<ApiApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProjectsTests(ApiApplicationFactory<Program> applicationFactory)
    {
        _client = applicationFactory.CreateClient();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            applicationFactory.CreateAccessToken("integration-test-user"));
    }

    [Fact]
    public async Task Projects_PostAuthenticatedRequest_CreatesProject()
    {
        // Arrange
        object request = new
        {
            Name = "Integration test project",
            Description = "Created through the API integration test host"
        };

        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/projects", request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        Guid projectId = await response.Content.ReadFromJsonAsync<Guid>(
            TestContext.Current.CancellationToken);
        projectId.ShouldNotBe(Guid.Empty);

        response.Headers.Location.ShouldNotBeNull();
        response.Headers.Location!.ToString().ShouldContain($"/api/projects/{projectId}");
    }
}
