using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreWindowsAuthExample.Models;
using AspNetCoreWindowsAuthExample.Dtos;

namespace AspNetCoreWindowsAuthExample.ViewModels
{
    public class UserInfoAdminEditViewModel : BaseViewModel
    {
        public UserInformation UserInformation { get; set; }
        public List<AssignedRoleData> AssignedRoleDatas { get; set; }
    }
}