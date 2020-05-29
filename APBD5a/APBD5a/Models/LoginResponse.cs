using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APBD5a.Models
{
    public class LoginResponse
    {

        public string accessToken { get; set; }
        public Guid refreshToken { get; set; }
    }
}
