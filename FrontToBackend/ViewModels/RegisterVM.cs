using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontToBackend.ViewModels
{
    public class RegisterVM
    {
        [Required,StringLength(100)]
        public  string  FullName { get; set; }
        [Required, StringLength(100)]
        public string  UserName { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required,DataType(DataType.Password)]
        public string Password { get; set; }
        [Required, DataType(DataType.Password),Compare("Password")]

        public string RepeatPassword { get; set; }
    }
}
