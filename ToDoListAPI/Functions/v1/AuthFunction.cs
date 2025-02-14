using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ToDoListAPI.Model;

namespace ToDoListAPI.Functions.v1
{
    [ApiController]
    [Route("api/auth")]
    public class AuthFunction : ControllerBase
    {
        private static List<User> users = new List<User>();
        private readonly string secretKey = "your_secret_key";

        [HttpPost("register")]
        public IActionResult Register([FromBody] User request)
        {
            if (users.Any(u => u.Email == request.Email))
            {
                return BadRequest("User sudah terdaftar");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            users.Add(new User { Email = request.Email, Password = hashedPassword });
            return Ok("Registrasi berhasil");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User request)
        {
            var user = users.FirstOrDefault(u => u.Email == request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return BadRequest("Email atau password salah");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, user.Email) }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new { Token = tokenHandler.WriteToken(token) });
        }
    }
}
