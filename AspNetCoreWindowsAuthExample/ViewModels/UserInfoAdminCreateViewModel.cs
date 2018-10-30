using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreWindowsAuthExample.Models;
using AspNetCoreWindowsAuthExample.Dtos;

namespace AspNetCoreWindowsAuthExample.ViewModels
{
    public class UserInfoAdminCreateViewModel : BaseViewModel
    {
        public UserInfoAdminCreateViewModel()
        {
            this.UserInformation = new UserInformation();
        }

        public UserInformation UserInformation { get; set; }
        public List<AssignedRoleData> AssignedRoleDatas { get; set; }
    }
}