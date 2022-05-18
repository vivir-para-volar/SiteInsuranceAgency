using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceAgency.Models
{
    public class Policy
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Вид страхования")]
        [Required(ErrorMessage = "Введите Вид страхования")]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "Вид страхования должен содержать 5 символов")]
        public string InsuranceType { get; set; }

        [Display(Name = "Страховая премия")]
        [Required(ErrorMessage = "Введите Страховую сумму")]
        public int InsurancePremium { get; set; }

        [Display(Name = "Страховая сумма")]
        [Required(ErrorMessage = "Введите Страховую премию")]
        public int InsuranceAmount { get; set; }

        [Display(Name = "Дата заключения")]
        [Required(ErrorMessage = "Введите Дату заключения")]
        [DataType(DataType.Date)]
        public DateTime DateOfConclusion { get; set; }

        [Display(Name = "Дата окончания действия")]
        [Required(ErrorMessage = "Введите Дату окончания действия")]
        [DataType(DataType.Date)]
        public DateTime ExpirationDate { get; set; }


        [ForeignKey("Policyholder")]
        public int PolicyholderID { get; set; }

        [ForeignKey("PolicyholderID")]
        public Policyholder Policyholder { get; set; }

        [ForeignKey("Car")]
        public int CarID { get; set; }

        [ForeignKey("CarID")]
        public Car Car { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeID { get; set; }

        [ForeignKey("EmployeeID")]
        public Employee Employee { get; set; }

        public ICollection<PersonAllowedToDrive> PersonsAllowedToDrive { get; set; }
        public ICollection<InsuranceEvent> InsuranceEvents { get; set;}

        public Policy()
        {
            PersonsAllowedToDrive = new List<PersonAllowedToDrive>();
            InsuranceEvents = new List<InsuranceEvent>();
        }
    }
}