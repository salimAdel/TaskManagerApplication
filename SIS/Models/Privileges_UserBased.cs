﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SIS.Data;
namespace SIS.Models
{
    public class Privileges_UserBased
    {
        public Privileges_UserBased()
        {

        }
        public int id { get; set; }
        public bool is_displayed { get; set; }
        public bool is_insert { get; set; }
        public bool is_update { get; set; }
        public bool is_delete { get; set; }
        public bool is_print { get; set; }
        public int PrivilegesId { get; set; }
        public string UserId { get; set; }
        public virtual Privileges Privileges { get; set; }
        public virtual User User { get; set; }
    }
}
