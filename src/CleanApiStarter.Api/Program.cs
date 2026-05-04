WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddAspNetCoreDefaults();

builder.Services.AddAppSettings(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddScoped<IUser, CurrentUser>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi()
        .WithDocumentPerVersion();

    app.MapScalarApiReference(options =>
    {
        IReadOnlyList<ApiVersionDescription> descriptions = app.DescribeApiVersions();

        for (int index = 0; index < descriptions.Count; index++)
        {
            ApiVersionDescription description = descriptions[index];
            bool isDefault = index == descriptions.Count - 1;

            options.AddDocument(description.GroupName, description.GroupName, isDefault: isDefault);
        }
    });
}

app.UseAspNetCoreDefaults();
app.UseHttpsRedirection();
app.MapGoogleLoginPage();
app.MapEndpoints(Assembly.GetExecutingAssembly());
app.MapDefaultEndpoints();

app.Run();

public partial class Program;
