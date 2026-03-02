using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace foundWhatYouLost.DTOs
{
    public class UserLoginRequest
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }
}