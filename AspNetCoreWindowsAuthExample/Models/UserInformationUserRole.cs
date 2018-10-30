using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreWindowsAuthExample.Models
{
    public class UserInformationUserRole
    {
        public int UserInformationId { get; set; }
        public UserInformation UserInformation { get; set; }
        public int UserRoleId { get; set; }
        public UserRole UserRole { get; set; }
    }
}