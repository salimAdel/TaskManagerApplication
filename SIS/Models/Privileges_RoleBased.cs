using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SIS.Models
{
    public class Privileges_RoleBased
    {
        public Privileges_RoleBased()
        {

        }
        public int id { get; set; }
        public bool is_displayed { get; set; }
        public bool is_insert { get; set; }
        public bool is_update { get; set; }
        public bool is_delete { get; set; }
        public bool is_print { get; set; }
        public int PrivilegesId { get; set; }
        //public string RoleId { get; set; }
        public virtual Privileges Privileges { get; set; }
        public virtual IdentityRole role { get; set; }
    }
}
