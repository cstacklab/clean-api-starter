namespace CleanApiStarter.AspNetCore;

public static class WebApplicationExtensions
{
    public static WebApplication MapEndpoints(this WebApplication app, Assembly assembly)
    {
        IEnumerable<Type> endpointGroupTypes = assembly.GetExportedTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false }
                && type.IsAssignableTo(typeof(IEndpointGroup)));

        foreach (Type type in endpointGroupTypes)
        {
            ApiVersion apiVersion = GetApiVersion(type);
            string groupName = type.Name;
            string routePrefix = type.GetProperty(nameof(IEndpointGroup.RoutePrefix))?.GetValue(null) as string
                ?? $"/api/{groupName}";
            ApiVersionSet versionSet = app.NewApiVersionSet()
                .HasApiVersion(apiVersion)
                .ReportApiVersions()
                .Build();

            RouteGroupBuilder group = app.MapGroup(routePrefix)
                .WithTags(groupName)
                .WithApiVersionSet(versionSet)
                .MapToApiVersion(apiVersion);

            type.GetMethod(nameof(IEndpointGroup.Map))!.Invoke(null, [group]);
        }

        return app;
    }

    private static ApiVersion GetApiVersion(Type endpointGroupType)
    {
        int majorVersion = endpointGroupType.GetProperty(nameof(IEndpointGroup.MajorVersion))?.GetValue(null) as int? ?? 1;

        return new ApiVersion(majorVersion);
    }
}
