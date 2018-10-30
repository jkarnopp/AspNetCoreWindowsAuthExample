using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreWindowsAuthExample.Models
{
    public class UserInformation
    {
        [Key]
        public int UserInformationId { get; set; }

        [MaxLength(25), Display(Name = "LAN ID"), Required]
        public String LanId { get; set; }

        [MaxLength(50), Display(Name = "First Name"), Required]
        public string FirstName { get; set; }

        [MaxLength(50), Display(Name = "Last Name"), Required]
        public string LastName { get; set; }

        [MaxLength(100), Required]
        public string Email { get; set; }

        public bool Enabled { get; set; }

        public virtual ICollection<UserInformationUserRole> UserInformationUserRoles { get; set; }
    }
}