using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eXml.Entities
{
    public enum enRoleType
    {
        Admin =1 ,
        NormalUser
    }
    public class User
    {
        public User()
        {
            Roles = new HashSet<Role>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsLicensed { get; set; }
        public DateTime ExpiryDate { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
    }
    public class Role
    {
        public Role()
        {
            Users = new HashSet<User>();
            Permissions = new HashSet<Permission>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public enRoleType RoleType { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Permission> Permissions { get; set; }
    }
    public class UserRole
    {
        [Key]
        [Column(Order = 1)]
        public int UserId { get; set; }
        [Key]
        [Column(Order = 2)]
        public int RoleId { get; set; }
    }
    public class Permission
    {
        public Permission()
        {
            Roles = new HashSet<Role>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PermissionId { get; set; }
        public string PermissionName { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
    }
    public class RolePermission
    {
        [Key]
        [Column(Order = 1)]
        public int RoleId { get; set; }

        [Key]
        [Column(Order = 2)]
        public int PermissionId { get; set; }
    }
}