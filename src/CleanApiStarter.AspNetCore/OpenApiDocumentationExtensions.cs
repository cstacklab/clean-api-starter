namespace CleanApiStarter.AspNetCore;

public static class OpenApiDocumentationExtensions
{
    public static WebApplication MapOpenApiDocumentation(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            return app;
        }

        app.MapOpenApi()
            .WithDocumentPerVersion()
            .AllowAnonymous();

        app.MapScalarApiReference(options =>
            {
                IReadOnlyList<ApiVersionDescription> descriptions = app.DescribeApiVersions();

                for (int index = 0; index < descriptions.Count; index++)
                {
                    ApiVersionDescription description = descriptions[index];
                    bool isDefault = index == descriptions.Count - 1;

                    options.AddDocument(description.GroupName, description.GroupName, isDefault: isDefault);
                }
            })
            .AllowAnonymous();

        return app;
    }
}
