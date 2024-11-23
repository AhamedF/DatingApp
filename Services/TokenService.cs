using API.Entities;  // Added to access AppUser class
using API.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration config;

        public TokenService(IConfiguration configuration)
        {
            config = configuration;  // Assign injected IConfiguration to the class variable
        }

        public string CreateToken(AppUser user)
        {
            // Logic for creating a token (e.g., JWT token generation)
            var tokenKey = config["TokenKey"] ?? throw new Exception("Cannot access token key from appsettings");

            if (tokenKey.Length < 56)
                throw new Exception("Your token key needs to be longer");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserName) // Changed Username to UserName
            };

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
