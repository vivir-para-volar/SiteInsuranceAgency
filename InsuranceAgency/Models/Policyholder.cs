using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InsuranceAgency.Models
{
    public class Policyholder
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
        [DataType(DataType.PhoneNumber)]
        [MaxLength(15, ErrorMessage = "Телефон не должен превышать 15 цифр")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Данное поле может включать только цифры")]
        public string Telephone { get; set; }

        [Display(Name = "Паспорт")]
        [Required(ErrorMessage = "Введите Паспорт")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Пасспорт должен содержать 10 цифр")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Данное поле может включать только цифры")]
        public string Passport { get; set; }

        public ICollection<Policy> Policies { get; set; }

        public Policyholder()
        {
            Policies = new List<Policy>();
        }
    }
}
