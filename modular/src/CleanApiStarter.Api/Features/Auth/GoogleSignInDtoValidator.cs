namespace CleanApiStarter.Api.Features.Auth;

public sealed class GoogleSignInDtoValidator : AbstractValidator<GoogleSignInDto>
{
    public GoogleSignInDtoValidator()
    {
        RuleFor(signIn => signIn.IdToken)
            .NotEmpty();
    }
}
