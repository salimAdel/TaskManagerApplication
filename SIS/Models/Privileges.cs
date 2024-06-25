using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIS.Models
{
    public class Privileges
    {
        public Privileges()
        {
            
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "هذا الحقل مطلوب")]
        [Display(Name = "الإسم")]
        [DataType(DataType.Text)]
        public string priv_name { get; set; }


        public Guid priv_key { get; set; }
        [Required(ErrorMessage = "هذا الحقل مطلوب")]
        [Display(Name = "المجموعة")]
        [DataType(DataType.Text)]
        public string priv_cat { get; set; }
        public virtual ICollection<Privileges_UserBased> Privileges_UserBased { get; set; }
        public virtual ICollection<Privileges_RoleBased> Privileges_RoleBased { get; set; }
    }
}
