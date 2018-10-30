using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using X.PagedList;
using AspNetCoreWindowsAuthExample.Models;
using AspNetCoreWindowsAuthExample.PagedListActions;

namespace AspNetCoreWindowsAuthExample.ViewModels
{
    public class UserInfoAdminIndexViewModel : BaseViewModel
    {
        public UserInfoAdminIndexViewModel(UserInfoAdminIndexPageListAction pageListAction, IPagedList<UserInformation> userInfos)
        {
            this.UserInformations = userInfos;
            this.PageListAction = pageListAction;
        }

        public UserInfoAdminIndexPageListAction PageListAction { get; set; }
        public IPagedList<UserInformation> UserInformations { get; set; }

        public IEnumerable<UserRole> UserRoles { get; set; }
    }
}