using System;
using System.ComponentModel.DataAnnotations;

namespace InsuranceAgency.Models.ViewModels
{
    public class CreateEmployee
    {
        [Display(Name = "ФИО")]
        [Required(ErrorMessage = "Введите ФИО")]
        [MaxLength(64, ErrorMessage = "ФИО не должено превышать 64 символа")]
        public string FullName { get; set; }

        [Display(Name = "Дата рождения")]
        [Required(ErrorMessage = "Введите Дату рождения")]
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }

        [Display(Name = "Телефон")]
        [Required(ErrorMessage = "Введите Телефон")]
        [MaxLength(15, ErrorMessage = "Телефон не должен превышать 15 цифр")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Данное поле может включать только цифры")]
        public string Telephone { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Введите Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Паспорт")]
        [Required(ErrorMessage = "Введите Паспорт")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Пасспорт должен содержать 10 цифр")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Данное поле может включать только цифры")]
        public string Passport { get; set; }

        [Display(Name = "Логин")]
        [Required(ErrorMessage = "Введите Логин")]
        [MinLength(6, ErrorMessage = "Логин должен содержать минимум 6 символов")]
        [MaxLength(40, ErrorMessage = "Логин должен содержать максимум 40 символов")]
        public string Login { get; set; }

        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Введите Пароль")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
        [MaxLength(40, ErrorMessage = "Пароль должен содержать максимум 40 символа")]
        public string Password { get; set; }
    }
}