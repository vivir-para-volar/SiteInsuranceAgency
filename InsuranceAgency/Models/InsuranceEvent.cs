using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceAgency.Models
{
    public class InsuranceEvent
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Дата")]
        [Required(ErrorMessage = "Введите Дату")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "Страховая выплата")]
        [Required(ErrorMessage = "Введите Страховую выплату")]
        public int InsurancePayment { get; set; }

        [Display(Name = "Описание")]
        [Required(ErrorMessage = "Введите Описание")]
        [MaxLength(1000, ErrorMessage = "Описание не должно превышать 1000 символов")]
        public string Description { get; set; }

        [ForeignKey("Policy")]
        public int PolicyID { get; set; }

        [ForeignKey("PolicyID")]
        public Policy Policy { get; set; }
    }
}
