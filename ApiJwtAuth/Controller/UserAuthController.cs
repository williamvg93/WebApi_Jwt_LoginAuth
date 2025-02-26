using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ApiJwtAuth.Data;
using ApiJwtAuth.Dtos;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ApiJwtAuth.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserAuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration configuration) : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly SignInManager<AppUser> _signInManager = signInManager;
        private readonly string? _jwtKey = configuration["Jwt:Key"];
        private readonly string? _jwtIssuer = configuration["Jwt:Issuer"];
        private readonly string? _jwtAudience = configuration["Jwt:YourAudience"];
        private readonly int? _jwtExpiryMinutes = int.Parse(configuration["Jwt:ExpiryMinutes"]!);

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] SignUpDto reqData)
        {
            if (reqData is null 
                || string.IsNullOrEmpty(reqData.Name)
                || string.IsNullOrEmpty(reqData. Email)
                || string.IsNullOrEmpty(reqData.Name))
            {
                return BadRequest("Invalid Data to Register !!");
            }

            var userExist = await _userManager.FindByEmailAsync(reqData.Email);

            if (userExist != null) return Conflict("Email Already Exist !!!");

            var user = new AppUser {
               UserName = reqData.UserName,
               Name = reqData.Name,
               Email = reqData.Email 
            };

            var createUser = await _userManager.CreateAsync(user, reqData.Password!);

            if (!createUser.Succeeded) return BadRequest(createUser.Errors);

            return Ok("User Created Successfully");
        }

        [HttpPost("LogIn")]
        public async Task<IActionResult> LogIn([FromBody] LogInDto logInDto)
        {
            var user = await _userManager.FindByEmailAsync(logInDto.Email);

            if (user is null)
                return Unauthorized(new { success = false, message = "Invalid Username !!!"});

            var checkPass = await _signInManager.CheckPasswordSignInAsync(user, logInDto.Password, false);

            if (!checkPass.Succeeded) 
                return Unauthorized(new {success = false, message = "Invalid Password !!!"});
            
            var token = GenerateJWT(user);

            return Ok(new {success = true, token} );
        }

        private string GenerateJWT(AppUser appUser)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub , appUser.Id),
                new Claim(JwtRegisteredClaimNames.Email, appUser.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Name", appUser.Name)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims : claims,
                expires : DateTime.Now.AddMinutes((double)_jwtExpiryMinutes!),
                signingCredentials : creds
            );
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}