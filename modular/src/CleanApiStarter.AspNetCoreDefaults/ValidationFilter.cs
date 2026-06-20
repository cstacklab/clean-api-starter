namespace CleanApiStarter.AspNetCoreDefaults;

public sealed class ValidationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        List<FluentValidation.Results.ValidationFailure> validationFailures = [];

        foreach (object? argument in context.Arguments)
        {
            if (argument == null)
            {
                continue;
            }

            Type validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            object? validator = context.HttpContext.RequestServices.GetService(validatorType);

            if (validator is not IValidator fluentValidator)
            {
                continue;
            }

            ValidationContext<object> validationContext = new(argument);
            ValidationResult validationResult = await fluentValidator.ValidateAsync(
                validationContext,
                context.HttpContext.RequestAborted);

            validationFailures.AddRange(validationResult.Errors);
        }

        if (validationFailures.Count == 0)
        {
            return await next(context);
        }

        Dictionary<string, string[]> errors = validationFailures
            .GroupBy(failure => failure.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(failure => failure.ErrorMessage).ToArray());

        return Results.ValidationProblem(errors, statusCode: StatusCodes.Status422UnprocessableEntity);
    }
}
