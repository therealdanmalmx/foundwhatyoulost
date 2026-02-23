using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace foundWhatYouLost.DTOs
{
    public record struct UserRegistrationResponse(
        bool isSuccessful,
        IEnumerable<string>? Errors = null
    );
}