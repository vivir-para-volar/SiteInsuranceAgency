using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InsuranceAgency.Models
{
    public class PersonAllowedToDrive
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "ФИО")]
        [Required(ErrorMessage = "Введите ФИО")]
        [MaxLength(64, ErrorMessage = "ФИО не должено превышать 64 символа")]
        public string FullName { get; set; }

        [Display(Name = "Водительское удостоверение")]
        [Required(ErrorMessage = "Введите Водительское удостоверение")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Водительское удостоверение должно содержать 10 цифр")]
        public string DrivingLicence { get; set; }

        public ICollection<Policy> Policies { get; set; }

        public PersonAllowedToDrive()
        {
            Policies = new List<Policy>();
        }
    }
}
