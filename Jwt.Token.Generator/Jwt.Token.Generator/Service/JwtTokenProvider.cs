using Jwt.Token.Generator.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Jwt.Token.Generator.Service
{
    public interface IJwtTokenProvider
    {
        Task<string> GenerateToken(User user);
    }
    public sealed class JwtTokenProvider : IJwtTokenProvider
    {
        private readonly JwtOptions options;
        public JwtTokenProvider(IOptionsMonitor<JwtOptions> options)
        {
            this.options = options.CurrentValue;
            
        }
        public async Task<string> GenerateToken(User user)
        {
            var claims = new Claim[]
            {   
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim(ClaimTypes.Role, "User") // hardcoded for demo purpose
            };

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey)), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(options.Issuer, options.Audience, claims, null, DateTime.Now.AddHours(6), signingCredentials);

            string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            return await Task.FromResult(tokenValue);
        }
    }
}
