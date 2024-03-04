using Jwt.Token.Generator.Data;
using Jwt.Token.Generator.Models;
using Jwt.Token.Generator.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace Jwt.Token.Generator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext applicationDbContext;
        private readonly IJwtTokenProvider tokenProvider;
        private readonly JwtOptions jwtOptions;

        public AuthController(ApplicationDbContext applicationDbContext, IJwtTokenProvider tokenProvider, IOptionsMonitor<JwtOptions> jwtOptions)
        {
            this.applicationDbContext = applicationDbContext;
            this.tokenProvider = tokenProvider;
            this.jwtOptions = jwtOptions.CurrentValue;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (ModelState.IsValid)
            {
                User user = new() { Email = request.Email, Password = HashPassword(request.Password) };
                await applicationDbContext.Users.AddAsync(user);
                await applicationDbContext.SaveChangesAsync();
                return Ok("Registration successfull");
            }
            return BadRequest("Invalid data supplied");
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginRequest request)
        {
            if (ModelState.IsValid)
            {
                var user = await applicationDbContext.Users.FindAsync(request.Email);

                if (user is null)
                {
                    return BadRequest("User not registered");
                }

                else if (user.Password != HashPassword(request.Password))
                {
                    return BadRequest("Incorrect Password");
                }

                string token = await tokenProvider.GenerateToken(user);
                return Ok(token);
            }
            return BadRequest("Invalid data supplied");
        }

        
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                // Convert byte array to a string   
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}
