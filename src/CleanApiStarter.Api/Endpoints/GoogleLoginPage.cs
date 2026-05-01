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
                        .token-header { align-items: center; display: flex; gap: 12px; margin-top: 32px; }
                        .token-header h2 { margin: 0; }
                        button { cursor: pointer; padding: 6px 10px; }
                        #copyStatus { color: #4b5563; font-size: 14px; }
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
                      <div class="token-header">
                        <h2>API JWT</h2>
                        <button id="copyToken" type="button" onclick="copyAccessToken()" disabled>Copy token</button>
                        <span id="copyStatus"></span>
                      </div>
                      <pre id="token">Sign in with Google to generate a token.</pre>

                      <script>
                        let accessToken = '';

                        async function handleCredentialResponse(response) {
                          const result = await fetch('/api/auth/google', {
                            method: 'POST',
                            headers: {
                              'Content-Type': 'application/json'
                            },
                            body: JSON.stringify({ idToken: response.credential })
                          });

                          const json = await result.json();
                          accessToken = json.accessToken || '';
                          document.getElementById('token').textContent = JSON.stringify(json, null, 2);
                          document.getElementById('copyToken').disabled = !accessToken;
                          document.getElementById('copyStatus').textContent = accessToken ? 'Ready to copy.' : '';
                        }

                        async function copyAccessToken() {
                          if (!accessToken) {
                            return;
                          }

                          if (navigator.clipboard && window.isSecureContext) {
                            await navigator.clipboard.writeText(accessToken);
                          } else {
                            const textArea = document.createElement('textarea');
                            textArea.value = accessToken;
                            textArea.style.position = 'fixed';
                            textArea.style.opacity = '0';
                            document.body.appendChild(textArea);
                            textArea.focus();
                            textArea.select();
                            document.execCommand('copy');
                            textArea.remove();
                          }

                          document.getElementById('copyStatus').textContent = 'Copied.';
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