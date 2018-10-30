using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreWindowsAuthExample.Common.Ldap;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using AspNetCoreWindowsAuthExample.Models;
using AspNetCoreWindowsAuthExample.Security;
using AspNetCoreWindowsAuthExample.Services;

namespace WarrantyIq2018.Site.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly IUserInformationAdminService _userInformationAdminService;
        private readonly ILdapConfiguration _ldapConfiguration;

        public RegistrationController(IUserInformationAdminService userInformationAdminService,
            ILdapConfiguration ldapConfiguration)
        {
            _userInformationAdminService = userInformationAdminService;
            _ldapConfiguration = ldapConfiguration;
        }

        public IActionResult Index()
        {
            if (!_userInformationAdminService.CheckUser(User.Identity.Name))
            {
                return RedirectToAction(nameof(Register));
            }
            var createView = _userInformationAdminService.GetEditView(User.Identity.Name, "Registration");
            if (createView.UserInformation.Enabled)
            {
                createView.Message =
                    "You currently have an ID in the system, but the area you do not have permission to access the area requested.";
            }
            else
            {
                createView.Message =
                    "A request for access has already been registered in the system for you ID, but it has not been approved yet. Please check back later to see if your ID has been approved.";
            }
            return View(createView);
        }

        public IActionResult Register()
        {
            var createView = _userInformationAdminService.GetCreateView();
            createView.UserInformation.LanId = User.Identity.Name;
            createView.Title = "Registration";

            var id = new AdInfo(User.Identity.Name, new ActiveDirectoryConnectionInfo(_ldapConfiguration.GlobalCatalog, _ldapConfiguration.BasePath, _ldapConfiguration.LdapUsername, _ldapConfiguration.LdapPassword));

            createView.UserInformation.LanId = User.Identity.Name;
            createView.UserInformation.FirstName = id.FirstName;
            createView.UserInformation.LastName = id.LastName;
            createView.UserInformation.Email = id.EmailAddress;
            createView.Message =
                "You currently do not have access to this application.Click the Request Access button below to submit your request.";

            return View(createView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("UserInformationId,LanId,FirstName,LastName,Email")] UserInformation userInformation, string[] selectedUserRoles = null)
        {
            if (userInformation.LanId != null && _userInformationAdminService.CheckUser(userInformation.LanId))
            {
                ModelState.AddModelError("LanId", "LanId Already Exists");
            }

            if (ModelState.IsValid)
            {
                _userInformationAdminService.CreateUser(userInformation, selectedUserRoles);
                var url = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
                _userInformationAdminService.NotifyForNewUser(userInformation, url);

                return RedirectToAction(nameof(Index));
            }
            var createView = _userInformationAdminService.GetEditView(userInformation, selectedUserRoles, "Create User");
            return View(createView);
        }
    }
}