namespace CleanApiStarter.Api.IntegrationTests.Features.Projects;

[TestClass]
public sealed class ProjectsTests
{
    private ApiApplicationFactory<Program> _applicationFactory = null!;

    private HttpClient _client = null!;

    [TestInitialize]
    public async Task TestInitialize()
    {
        _applicationFactory = new ApiApplicationFactory<Program>();
        await _applicationFactory.InitializeAsync();

        _client = _applicationFactory.CreateClient();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            _applicationFactory.CreateAccessToken("integration-test-user"));
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        _client.Dispose();
        await _applicationFactory.DisposeAsync();
    }

    [TestMethod]
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
            cancellationToken: TestContext.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        Guid projectId = await response.Content.ReadFromJsonAsync<Guid>();
        projectId.ShouldNotBe(Guid.Empty);

        response.Headers.Location.ShouldNotBeNull();
        response.Headers.Location!.ToString().ShouldContain($"/api/projects/{projectId}");
    }

    public TestContext TestContext { get; set; }
}