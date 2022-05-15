using System;
using System.ComponentModel.DataAnnotations;

namespace InsuranceAgency.Models.ViewModels
{
    public class Report
    {
        [Display(Name = "Вид страхования")]
        [Required(ErrorMessage = "Введите Вид страхования")]
        public string InsuranceType { get; set; }

        [Display(Name = "Дата начала")]
        [Required(ErrorMessage = "Введите Дата начала")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "Дата окончания")]
        [Required(ErrorMessage = "Введите Дату окончания")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
    }
}