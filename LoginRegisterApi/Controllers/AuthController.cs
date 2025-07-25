using Microsoft.AspNetCore.Mvc;
using LoginRegisterApi.Models;
using LoginRegisterApi.Services;
using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;

namespace AuthApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;

         public AuthController(UserService userService, IConfiguration configuration)
         {
            _userService = userService;
            _configuration = configuration;
         }

        public class RegisterRequest
        {
            [Required]
            public string Username { get; set; }
            [Required]
            public string Email { get; set; }
            [Required]
            public string Password { get; set; }
        }

        public class LoginRequest
        {
            [Required]
            public string Username { get; set; }
            [Required]
            public string Password { get; set; }
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var added = _userService.AddUser(request.Username, request.Email, request.Password);

            if (!added)
                return BadRequest(new { message = "Username or Email already exists." });

            return Ok(new { message = "User registered successfully." });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = _userService.GetUserByUsername(request.Username);
            if (user == null)
                return Unauthorized(new { message = "Invalid username or password." });

            var isPasswordValid = _userService.VerifyPassword(request.Username, request.Password);
            if (!isPasswordValid)
                return Unauthorized(new { message = "Invalid username or password." });

            var token = GenerateJwtToken(user);

            return Ok(new { token });
        }

        private string GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtIssuer,
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
