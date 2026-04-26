WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddAspNetCoreDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddAppSettings(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddScoped<IUser, CurrentUser>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAspNetCoreDefaults();
app.UseHttpsRedirection();
app.MapGoogleLoginPage();
app.MapEndpoints(Assembly.GetExecutingAssembly());
app.MapDefaultEndpoints();

app.Run();