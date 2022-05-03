using System.ComponentModel.DataAnnotations;

namespace InsuranceAgency.Models.Security
{
    public class ChangePassword
    {
        [Required(ErrorMessage = "Введите Старый пароль")]
        [Display(Name = "Старый пароль")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Введите Новый пароль")]
        [Display(Name = "Новый пароль")]
        [StringLength(40, MinimumLength = 6, ErrorMessage = "Пароль должен быть от 6 до 40 символов")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Введите Подтверждение пароля")]
        [Display(Name = "Подтвердите пароль")]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmNewPassword { get; set; }
    }
}