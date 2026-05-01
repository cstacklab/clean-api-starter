namespace CleanApiStarter.Infrastructure.Identity;

public sealed class GoogleAuthService(
    AppSettings appSettings,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager) : IAuthService
{
    private const string GoogleLoginProvider = "Google";
    private const string DefaultRole = "User";

    public async Task<AuthTokenDto> SignInWithGoogleAsync(GoogleSignInDto signInDto, CancellationToken cancellationToken)
    {
        GoogleJsonWebSignature.Payload payload = await ValidateGoogleTokenAsync(signInDto.IdToken);
        ApplicationUser user = await FindOrCreateUserAsync(payload, cancellationToken);
        IList<string> roles = await userManager.GetRolesAsync(user);
        DateTimeOffset expiresAt = DateTimeOffset.UtcNow.AddMinutes(appSettings.Authentication.Jwt.ExpirationMinutes);

        return new AuthTokenDto
        {
            AccessToken = CreateAccessToken(user, payload.Name, roles, expiresAt),
            ExpiresAt = expiresAt
        };
    }

    public Task<CurrentUserDto> GetCurrentUserAsync(ClaimsPrincipal principal, CancellationToken cancellationToken)
    {
        string userId = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        string email = principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        string name = principal.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
        string[] roles = principal.FindAll(ClaimTypes.Role)
            .Select(claim => claim.Value)
            .ToArray();

        return Task.FromResult(new CurrentUserDto
        {
            UserId = userId,
            Email = email,
            Name = name,
            Roles = roles
        });
    }

    private async Task<GoogleJsonWebSignature.Payload> ValidateGoogleTokenAsync(string idToken)
    {
        GoogleJsonWebSignature.ValidationSettings validationSettings = new()
        {
            Audience = [appSettings.Authentication.Google.ClientId]
        };

        return await GoogleJsonWebSignature.ValidateAsync(idToken, validationSettings);
    }

    private async Task<ApplicationUser> FindOrCreateUserAsync(
        GoogleJsonWebSignature.Payload payload,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(payload.Email) || !payload.EmailVerified)
        {
            throw new UnauthorizedAccessException("Google account email must be verified.");
        }

        ApplicationUser? user = await userManager.FindByLoginAsync(GoogleLoginProvider, payload.Subject);

        if (user == null)
        {
            user = await userManager.FindByEmailAsync(payload.Email);
        }

        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = payload.Email,
                Email = payload.Email,
                EmailConfirmed = payload.EmailVerified
            };

            IdentityResult createResult = await userManager.CreateAsync(user);
            ThrowIfFailed(createResult);
        }

        UserLoginInfo googleLogin = new(GoogleLoginProvider, payload.Subject, GoogleLoginProvider);
        IList<UserLoginInfo> logins = await userManager.GetLoginsAsync(user);

        if (!logins.Any(login => login.LoginProvider == GoogleLoginProvider && login.ProviderKey == payload.Subject))
        {
            IdentityResult loginResult = await userManager.AddLoginAsync(user, googleLogin);
            ThrowIfFailed(loginResult);
        }

        await EnsureDefaultRoleAsync(user);

        return user;
    }

    private async Task EnsureDefaultRoleAsync(ApplicationUser user)
    {
        if (!await roleManager.RoleExistsAsync(DefaultRole))
        {
            IdentityResult roleResult = await roleManager.CreateAsync(new IdentityRole(DefaultRole));
            ThrowIfFailed(roleResult);
        }

        if (!await userManager.IsInRoleAsync(user, DefaultRole))
        {
            IdentityResult userRoleResult = await userManager.AddToRoleAsync(user, DefaultRole);
            ThrowIfFailed(userRoleResult);
        }
    }

    private string CreateAccessToken(
        ApplicationUser user,
        string? googleName,
        IEnumerable<string> roles,
        DateTimeOffset expiresAt)
    {
        JwtAuthenticationSettings jwtSettings = appSettings.Authentication.Jwt;
        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(jwtSettings.SigningKey));
        SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);
        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, googleName ?? user.UserName ?? string.Empty)
        ];

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        JwtSecurityToken token = new(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static void ThrowIfFailed(IdentityResult result)
    {
        if (result.Succeeded)
        {
            return;
        }

        string errors = string.Join("; ", result.Errors.Select(error => error.Description));
        throw new InvalidOperationException(errors);
    }
}