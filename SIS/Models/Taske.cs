using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIS.Models
{

    public class Taske
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [Display(Name ="العنوان")]
        public string Title { get; set; }
        [Required]
        [Display(Name = "الوصف")]
        public string Description { get; set; }
        [Display(Name ="الحالة")]
        public bool State { get; set; }
        public string? UserId { get; set; }
    }
}
