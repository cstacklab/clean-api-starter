namespace CleanApiStarter.Application.Common.Models;

public sealed class PaginatedQueryValidator : AbstractValidator<PaginatedQuery>
{
    public PaginatedQueryValidator()
    {
        RuleFor(query => query.Limit)
            .InclusiveBetween(1, 100);

        RuleFor(query => query.Offset)
            .GreaterThanOrEqualTo(0);
    }
}
