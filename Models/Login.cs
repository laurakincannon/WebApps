using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace WDLMassage.Models
{
    public class Login
    {
        [Required(ErrorMessage = "Username required")]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password required")]
        [MaxLength(50)]
        [UIHint("password")]
        public string Password { get; set; }

        public string ReturnURL { get; set; } //to store the url that user should be directed to if the login is sucessfull
    }
}
