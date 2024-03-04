using Jwt.Token.Generator.Models;
using Microsoft.Extensions.Options;

namespace Jwt.Token.Generator.Options
{
    public class JwtOptionsSetup : IConfigureOptions<JwtOptions>
    {
        private readonly IConfiguration configuration;
        private string JwtSectionName = "Jwt";

        public JwtOptionsSetup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public void Configure(JwtOptions options)
        {
            configuration.GetSection(JwtSectionName).Bind(options);
        }
    }
}
