using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.Auth.DTO
{

    public class LogOnDto
    {
        public string UserName { get; set; }
        public string Key { get; set; }
        public string Password { get; set; }
        public string PasswordHex { get; set; }
        public bool RememberMe { get; set; }
        public string ErrMessage { get; set; }
    }
}
