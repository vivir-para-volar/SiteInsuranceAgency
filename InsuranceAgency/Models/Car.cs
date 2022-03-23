using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InsuranceAgency.Models
{
    public class Car
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Марка, модель")]
        [Required(ErrorMessage = "Введите Марку, модель")]
        [MaxLength(50, ErrorMessage = "Марка, модель не должна превышать 50 символов")]
        public string Model { get; set; }

        [Display(Name = "Идентификационный номер")]
        [Required(ErrorMessage = "Введите Идентификационный номер")]
        [StringLength(17, MinimumLength = 17, ErrorMessage = "Идентификационный номер должен содержать 17 символов")]
        public string VIN { get; set; }

        [Display(Name = "Регистрационный знак")]
        [Required(ErrorMessage = "Введите Регистрационный знак")]
        [MaxLength(25, ErrorMessage = "Регистрационный знак не должен превышать 25 символов")]
        public string RegistrationPlate { get; set; }

        [Display(Name = "Паспорт ТС")]
        [Required(ErrorMessage = "Введите Паспорт ТС")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Паспорт ТС должен содержать 10 символов")]
        public string VehiclePassport { get; set; }

        public ICollection<Policy> Policies { get; set; }
        public ICollection<Photo> Photos { get; set; }

        public Car()
        {
            Policies = new List<Policy>();
            Photos = new List<Photo>();
        }
    }
}
