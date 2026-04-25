WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddAspNetCoreDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAspNetCoreDefaults();
app.UseHttpsRedirection();
app.MapWordEndpoints();
app.MapDefaultEndpoints();

app.Run();
