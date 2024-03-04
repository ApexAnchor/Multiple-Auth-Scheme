using System.ComponentModel.DataAnnotations;

namespace Jwt.Token.Generator.Models
{
    public class RegisterRequest
    {
        [EmailAddress]
        public string Email { get; set; }

        [Length(8,16)]
        public string Password { get; set; }
    }
}
