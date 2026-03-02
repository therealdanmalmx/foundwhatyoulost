using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using foundWhatYouLost.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace foundWhatYouLost.Services
{
    public class UserLoginService : IUserLoginService
    {
                private readonly SignInManager<User> _registerSignInManager;
        private readonly IConfiguration _config;

        public UserLoginService(SignInManager<User> registeredParticipantSignInManager, IConfiguration config)
        {
            _registerSignInManager = registeredParticipantSignInManager;
            _config = config;
        }

        public async Task<UserLoginResponse> LoginUser(UserLoginRequest request)
        {
            var result = await _registerSignInManager.PasswordSignInAsync(
                request.UserName,
                request.Password,
                false,
                false
            );

            if (!result.Succeeded)
            {
                return new UserLoginResponse(false, "Email eller lösenord är fel");
            }

            var claims = new []
            {
                new Claim(ClaimTypes.Name, request.UserName)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["JwtsecurityKey"]!)
            );

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiryDate = DateTime.UtcNow.AddDays(Convert.ToInt16(_config["JwtExpiryDate"]));
            var token = new JwtSecurityToken(
                issuer: _config["JwtIssuer"],
                audience: _config["JwtAudience"],
                claims: claims,
                expires: expiryDate,
                signingCredentials: credentials
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return new UserLoginResponse(true, null, jwt);
        }
    }
}