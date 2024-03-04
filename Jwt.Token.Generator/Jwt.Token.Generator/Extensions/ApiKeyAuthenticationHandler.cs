using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Jwt.Token.Generator.Extensions
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        public const string ApiKeySectionName = "Authentication:ApiKey";

        public const string AuthenticationHeader = "X-API-KEY";
        
        private readonly IConfiguration configuration;

        [Obsolete]
        public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationOptions> options,
                                           ILoggerFactory logger, 
                                           UrlEncoder encoder, 
                                           ISystemClock clock,
                                           IConfiguration configuration) : base(options, logger, encoder, clock)
        {
            this.configuration = configuration;
        }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(AuthenticationHeader,out var apiKey))
            {
                return AuthenticateResult.Fail("Api Key is missing!!");
            }

            var key = configuration.GetValue<string>(ApiKeySectionName);

            if (key != apiKey)
            {
              return AuthenticateResult.Fail("Invalid Api Key!!");
            }

            var claims = new ClaimsIdentity(nameof(ApiKeyAuthenticationHandler));
            claims.AddClaim(new Claim(ClaimTypes.Role, "User"));
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(claims), Scheme.Name);

            return await Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {

    }
}
