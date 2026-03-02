using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace foundWhatYouLost.DTOs
{
    public class UserLoginResponse
    {
        public bool IsSuccessful { get; set; }
        public string? Errors { get; set; } = null;
        public string? Token { get; set; } = null;

        public UserLoginResponse() { }
        public UserLoginResponse(bool isSuccessful, string? errors)
        {
            IsSuccessful = isSuccessful;
            Errors = errors;
        }
        public UserLoginResponse(bool isSuccessful, string? errors, string? token)
        {
            IsSuccessful = isSuccessful;
            Errors = errors;
            Token = token;
        }
    }
}