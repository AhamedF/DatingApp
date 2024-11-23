using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context, ITokenService tokenService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService)); // Corrected tokenService
        }

        [HttpPost("register")] // account/register
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (string.IsNullOrEmpty(registerDto.Username) || string.IsNullOrEmpty(registerDto.Password))
            {
                return BadRequest("Username and password cannot be empty.");
            }

            if (_context.Users == null)
            {
                return BadRequest("Users collection is not initialized in the database context.");
            }

            if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");

            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Username = user.UserName, // Corrected: user.UserName instead of user.Username
                Token = _tokenService.CreateToken(user) // Use _tokenService instead of tokenService
            };
        }

        [HttpPost("login")] // account/login
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _context.Users?.FirstOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower()); // Corrected UserName

            if (user == null) return Unauthorized("Invalid username");

            if (user.PasswordSalt == null || user.PasswordHash == null)
                return Unauthorized("Invalid password configuration.");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                    return Unauthorized("Invalid password");
            }

            return new UserDto
            {
                Username = user.UserName, // Corrected: user.UserName instead of User.UserName
                Token = _tokenService.CreateToken(user)
            };
        }

        private async Task<bool> UserExists(string userName)
        {
            return await _context.Users.AnyAsync(x => x.UserName.ToLower() == userName.ToLower());
        }
    }
}
