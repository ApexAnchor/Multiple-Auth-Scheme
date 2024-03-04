using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Jwt.Token.Generator.Models
{
    public class User
    {
        [Key]
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
