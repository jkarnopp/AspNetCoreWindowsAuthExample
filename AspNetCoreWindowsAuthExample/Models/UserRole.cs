using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreWindowsAuthExample.Models
{
    public class UserRole
    {
        [Key]
        public int UserRoleId { get; set; }

        [MaxLength(25)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }

        public virtual ICollection<UserInformationUserRole> UserInformationUserRoles { get; set; }
    }
}