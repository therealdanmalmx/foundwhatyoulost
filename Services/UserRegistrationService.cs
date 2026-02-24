using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using foundWhatYouLost.DTOs;
using Microsoft.AspNetCore.Identity;

namespace foundWhatYouLost.Services
{
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly UserManager<User> _userManager;

        public UserRegistrationService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserRegistrationResponse> RegisterUser(UserRegistrationRequest request)
        {
            var newUser = new User {Name = request.Name, UserName = request.UserName, PhoneNumber = request.PhoneNumber, Email = request.Email};

            var result = await _userManager.CreateAsync(newUser, request.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return new UserRegistrationResponse(false, errors);
            }

            return new UserRegistrationResponse(true);
        }
    }
}