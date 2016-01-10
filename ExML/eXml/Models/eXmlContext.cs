using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using eXml.Entities;

namespace eXml.Models
{
    public class eXmlContext : DbContext
    {
        public DbSet<PostedTransaction> PostedTransaction { get; set; }
        public DbSet<PurchaseRegister> PurchaseRegister { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public eXmlContext()
            : base("eXmlConn")
        {
        } 
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasMany(u => u.Roles).WithMany(p => p.Users)
               .Map(u => u.ToTable("UserRoles")
                   .MapLeftKey("UserId")
                   .MapRightKey("RoleId"));
            modelBuilder.Entity<Role>().ToTable("Roles");

            modelBuilder.Entity<Role>().HasMany(r => r.Permissions).WithMany(r => r.Roles)
                .Map(r => r.ToTable("RolePermissions")
                    .MapLeftKey("RoleId")
                    .MapRightKey("PermissionId"));
            modelBuilder.Entity<Permission>().ToTable("Permissions");

            modelBuilder.Entity<PostedTransaction>().ToTable("PostedTransactions");

            modelBuilder.Entity<PurchaseRegister>().ToTable("PurchaseRegister");

            modelBuilder.Entity<ListInvoiceReportModel>().ToTable("ListInvoiceReportsDAO");

            modelBuilder.Entity<ListPaymentReportModel>().ToTable("ListPaymentReportsDAO");

            modelBuilder.Entity<ListInventoryReportModel>().ToTable("ListInventoryReportsDAO");

            modelBuilder.Entity<ListInvoiceTransModel>().ToTable("ListInvoiceTransDAO");
            base.OnModelCreating(modelBuilder);
        }
    }
}