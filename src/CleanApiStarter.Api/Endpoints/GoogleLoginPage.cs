namespace CleanApiStarter.Api.Endpoints;

public static class GoogleLoginPage
{
    public static WebApplication MapGoogleLoginPage(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            return app;
        }

        app.MapGet("/auth/google-login", (AppSettings appSettings) =>
            {
                string clientId = HtmlEncoder.Default.Encode(appSettings.Authentication.Google.ClientId);

                string html =
                    $$"""
                    <!doctype html>
                    <html lang="en">
                    <head>
                      <meta charset="utf-8">
                      <meta name="viewport" content="width=device-width, initial-scale=1">
                      <title>Google Login</title>
                      <script src="https://accounts.google.com/gsi/client" async defer></script>
                      <style>
                        body { font-family: system-ui, sans-serif; max-width: 760px; margin: 48px auto; padding: 0 24px; }
                        pre { background: #111827; color: #e5e7eb; padding: 16px; overflow: auto; }
                      </style>
                    </head>
                    <body>
                      <h1>Google Login</h1>
                      <div id="g_id_onload"
                           data-client_id="{{clientId}}"
                           data-callback="handleCredentialResponse">
                      </div>
                      <div class="g_id_signin" data-type="standard"></div>
                      <h2>API JWT</h2>
                      <pre id="token">Sign in with Google to generate a token.</pre>

                      <script>
                        async function handleCredentialResponse(response) {
                          const result = await fetch('/api/auth/google', {
                            method: 'POST',
                            headers: {
                              'Content-Type': 'application/json'
                            },
                            body: JSON.stringify({ idToken: response.credential })
                          });

                          const json = await result.json();
                          document.getElementById('token').textContent = JSON.stringify(json, null, 2);
                        }
                      </script>
                    </body>
                    </html>
                    """;

                return Results.Content(html, "text/html");
            })
            .AllowAnonymous()
            .WithName("GoogleLoginPage");

        return app;
    }
}
