using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using foundWhatYouLost.DTOs;

namespace foundWhatYouLost.Services
{
    public interface IUserRegistrationService
    {
        Task<UserRegistrationResponse> RegisterUser(UserRegistrationRequest request);
    }
}