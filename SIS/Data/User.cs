using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SIS.Models;
namespace SIS.Data
{
    // Add profile data for application users by adding properties to the User class
    public class User : IdentityUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override string Id { get; set; }
        [Required(ErrorMessage = "هذا الحقل مطلوب")]
        [Display(Name = "الاسم الكامل")]
        public string FullName { get; set; }
        //[Required(ErrorMessage = "هذا الحقل مطلوب")]
        [Display(Name = "تاريخ التسجيل")]
        public virtual DateTime? JoinedDate { get; set; } = DateTime.Now;
        public virtual DateTime? LastLogin { get; set; }

        [Display(Name = " العنوان")]
        public string Address { get; set; }
        [Display(Name = "الدولة")]
        public string? Country { get; set; }
        [Display(Name = "المدينة")]
        public string? City { get; set; }
        [Display(Name = "المنطقة")]
        public string? HomeTown { get; set; }
        [Display(Name = "الصورة")]
        public byte[]? UserLogo { get; set; }

        public int? collegeId { get; set; }
        public int? DeptId { get; set; }
        public int? SpecialId { get; set; }

        public virtual ICollection<Privileges_UserBased> Privileges_UserBased { get; set; }
        public virtual ICollection<Privileges_RoleBased> Privileges_RoleBased { get; set; }
    }
}
