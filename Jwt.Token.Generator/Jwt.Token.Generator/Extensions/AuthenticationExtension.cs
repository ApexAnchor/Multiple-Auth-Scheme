using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Jwt.Token.Generator.Extensions
{
    public static class AuthenticationExtension
    {
        public const string AuthenticationHeader = "X-API-KEY";

        public const string JwtScheme = JwtBearerDefaults.AuthenticationScheme;

        public const string ApiScheme = "ApiScheme";

        public const string PolicySelector = "PolicySelector";

        public static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = PolicySelector;
                options.DefaultChallengeScheme = PolicySelector;
            })
            .AddPolicyScheme(PolicySelector, PolicySelector, options =>
                {
                    options.ForwardDefaultSelector = context =>
                    {
                        if (context.Request.Headers.TryGetValue(AuthenticationHeader, out var apiKey))
                        {
                            return ApiScheme;
                        }
                        else
                            return JwtScheme;
                    };
                })
            .AddJwtBearer(JwtScheme, o =>
            {
                o.RequireHttpsMetadata = false;
                var key = Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"].ToString());
                var audience = configuration["Jwt:Audience"].ToString();
                var issuer = configuration["Jwt:Issuer"].ToString();
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = audience,
                    ValidIssuer = issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            })
            .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ApiScheme, options =>
            {

            });


        }
    }
}
