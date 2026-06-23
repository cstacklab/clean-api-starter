WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddAspNetCoreDefaults();

builder.Services.AddAppSettings(builder.Configuration);
builder.Services.AddJwtBearerOptions();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddInfrastructure();
builder.Services.AddScoped<IUser, CurrentUser>();

WebApplication app = builder.Build();

app.UseAspNetCoreDefaults();
app.MapOpenApiDocumentation();
app.MapGoogleLoginPage();
app.MapEndpoints(Assembly.GetExecutingAssembly());
app.MapDefaultEndpoints();

app.Run();

public partial class Program;
