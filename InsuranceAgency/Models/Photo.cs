using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceAgency.Models
{
    public class Photo
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Путь")]
        [Required(ErrorMessage = "Введите Путь")]
        [MaxLength(100, ErrorMessage = "Путь не должен превышать 100 символов")]
        public string Path { get; set; }

        [ForeignKey("Car")]
        public int CarID { get; set; }

        [ForeignKey("CarID")]
        public Car Car { get; set; }
    }
}
