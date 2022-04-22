using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InsuranceAgency.Models.Security
{
    public class ChangePassword
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(40, MinimumLength = 6, ErrorMessage = "Пароль должен быть от 6 до 40 символов")]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmNewPassword { get; set; }
    }
}