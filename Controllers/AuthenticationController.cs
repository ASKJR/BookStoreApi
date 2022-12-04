using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using BookStoreApi.Models;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Authorization;
using BC = BCrypt.Net.BCrypt;

namespace BookStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController, AllowAnonymous]
    public class AuthenticationController : ControllerBase
    {
        private readonly UsersService _usersService;

        public AuthenticationController(UsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] Login user)
        {
            if (user.Email is null || user.Password is null)
            {
                return BadRequest("Invalid request body.");
            }

            var foundUser = await _usersService.GetAsync(user.Email);

            if (foundUser is null)
            {
                return Unauthorized();
            }

            if (BC.Verify(user.Password, foundUser.Password))
            {
                var secretKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes("ASKJR-secret-secret-secret")
                    );
                var signinCredentials = new SigningCredentials(
                        secretKey, 
                        SecurityAlgorithms.HmacSha256
                    );
                var tokeOptions = new JwtSecurityToken(
                        issuer: "ASKJR-issuer", 
                        audience: "ASKJR-audience", 
                        claims: new List<Claim>(), 
                        expires: DateTime.Now.AddMinutes(6), 
                        signingCredentials: signinCredentials
                    );
                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return Ok(tokenString);
            }
            return Unauthorized();
        }
    }
}
