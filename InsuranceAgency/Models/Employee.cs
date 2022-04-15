using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InsuranceAgency.Models
{
    public class Employee
    {
        [Key]
        public int ID { get; set; }

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

        [Display(Name = "Паспорт")]
        [Required(ErrorMessage = "Введите Паспорт")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Пасспорт должен содержать 10 цифр")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Данное поле может включать только цифры")]
        public string Passport { get; set; }

        [Display(Name = "Логин")]
        [Required(ErrorMessage = "Введите Логин")]
        [MinLength(4, ErrorMessage = "Логин должен содержать минимум 4 символа")]
        [MaxLength(50, ErrorMessage = "Логин должен содержать максимум 50 символов")]
        public string Login { get; set; }

        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Введите Пароль")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "Пароль должен содержать минимум 4 символа")]
        [MaxLength(32, ErrorMessage = "Пароль должен содержать максимум 32 символа")]
        public string Password { get; set; }

        [Display(Name = "Админ")]
        [UIHint("Boolean")]
        public bool Admin { get; set; }

        [Display(Name = "Работает")]
        [UIHint("Boolean")]
        public bool Works { get; set; }

        public ICollection<Policy> Policies { get; set; }

        public Employee()
        {
            Policies = new List<Policy>();
        }
    }
}
