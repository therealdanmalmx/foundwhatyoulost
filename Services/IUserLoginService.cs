using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using foundWhatYouLost.DTOs;

namespace foundWhatYouLost.Services
{
    public interface IUserLoginService
    {
        Task<UserLoginResponse> LoginUser(UserLoginRequest request);
    }
}