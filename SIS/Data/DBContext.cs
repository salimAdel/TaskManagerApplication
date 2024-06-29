using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SIS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
namespace SIS.Data
{
    public class DBContext : IdentityDbContext<User>
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {

        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSnakeCaseNamingConvention();
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSnakeCaseNamingConvention();
            optionsBuilder.UseLazyLoadingProxies();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.HasDefaultSchema("public");
            base.OnModelCreating(modelBuilder);
            
        }
        /// <summary>Folder
        /// ////////////////////////

        public DbSet<Privileges> Privileges { get; set; }
        public DbSet<Privileges_RoleBased> Privileges_RoleBased { get; set; }
        public DbSet<Privileges_UserBased> Privileges_UserBased { get; set; }
        public DbSet<Taske> Taskes { get; set; }



    }

}
