using System;
using System.ComponentModel.DataAnnotations;

namespace InsuranceAgency.Models.Security
{
    public class Register
    {
        [Required(ErrorMessage = "Введите Логин")]
        [Display(Name = "Логин")]
        [MinLength(6, ErrorMessage = "Логин должен содержать минимум 6 символов")]
        [MaxLength(40, ErrorMessage = "Логин должен содержать максимум 40 символов")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Введите Полное имя")]
        [Display(Name = "Полное имя")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Введите Дату рождения")]
        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Введите Номер телефона")]
        [Display(Name = "Номер телефона")]
        [DataType(DataType.PhoneNumber)]
        [MaxLength(15, ErrorMessage = "Телефон не должен превышать 15 цифр")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Данное поле может включать только цифры")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Введите Пароль")]
        [Display(Name = "Пароль")]
        [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
        [MaxLength(40, ErrorMessage = "Пароль должен содержать максимум 40 символа")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Введите Подтверждение пароля")]
        [Display(Name = "Подтвердите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }

}