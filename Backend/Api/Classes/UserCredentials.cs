using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Classes {
    public class UserCredentials {
        public string Login { get; set; }
        public string Password { get; set; }

        public UserCredentials(string login, string password) {
            Login = login;
            Password = password;
        }
    }
}
