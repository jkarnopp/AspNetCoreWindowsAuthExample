using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreWindowsAuthExample.Models;
using AspNetCoreWindowsAuthExample.Repositories;
using AspNetCoreWindowsAuthExample.Common.Email;
using AspNetCoreWindowsAuthExample.PagedListActions;
using AspNetCoreWindowsAuthExample.ViewModels;
using AspNetCoreWindowsAuthExample.Dtos;

namespace AspNetCoreWindowsAuthExample.Services
{
    public interface IUserInformationAdminService
    {
        UserInfoAdminIndexViewModel GetIndexView(string sortOrder, string currentFilter, string searchString,
            int? page, int? id, int pageSize);

        UserInfoAdminEditViewModel GetCreateView();

        UserInfoAdminEditViewModel GetEditView(string lanId, string title);

        UserInfoAdminEditViewModel GetEditView(int id, string title);

        UserInfoAdminEditViewModel GetEditView(UserInformation userInformation, string[] selectedUserRoles, string title);

        bool CheckUser(string lanId);

        void CreateUser(UserInformation userInformation, string[] selectedUserRoles);

        void UpdateUser(UserInformation userInformation, string[] selectedUserRoles);

        void DeleteUser(int id);

        void NotifyForNewUser(UserInformation userInformation, string hostingEnvironmentWebRootPath);
    }

    public class UserInformationAdminService : IUserInformationAdminService
    {
        private IUnitOfWork _unitOfWork;
        private IEmailService _emailService;

        public UserInformationAdminService(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public UserInfoAdminIndexViewModel GetIndexView(string sortOrder, string currentFilter, string searchString,
            int? page, int? id, int pageSize = 10)
        {
            var pageListAction = new UserInfoAdminIndexPageListAction(pageSize: pageSize, sortOrder: sortOrder, currentFilter: currentFilter, searchString: searchString, page: page, id: id);

            var userInformation = pageListAction.GetUserInfos(_unitOfWork);
            var userInfoAdminIndexViewModel = new UserInfoAdminIndexViewModel(pageListAction, userInformation.Result);

            if (pageListAction.UserInformationId != null)
            {
                userInfoAdminIndexViewModel.UserRoles = userInformation
                    .Result.Single(i => id != null && i.UserInformationId == id.Value).UserInformationUserRoles.Select(u => u.UserRole);
                pageListAction.LanId = userInformation.Result.Single(i => id != null && i.UserInformationId == id.Value).LanId;
            }

            userInfoAdminIndexViewModel.Title = "User Administration";

            return userInfoAdminIndexViewModel;
        }

        public UserInfoAdminEditViewModel GetCreateView()
        {
            var userInformation = new UserInformation();
            string[] selectedUserRoles = new string[0];

            return GetEditView(userInformation, selectedUserRoles, "Create User");
        }

        public UserInfoAdminEditViewModel GetEditView(string lanId, string title)
        {
            var userInformationId = _unitOfWork.UserInformations.GetAsync(u => u.LanId == lanId).Result.Select(i => i.UserInformationId).SingleOrDefault();
            if (userInformationId == 0) throw new ApplicationException("User ID not found in Database");
            return GetEditView(userInformationId, title);
        }

        public UserInfoAdminEditViewModel GetEditView(int id, string title)
        {
            var userInformation = _unitOfWork.UserInformations.FindAsync(u => u.UserInformationId == id).Result.SingleOrDefault();

            if (userInformation == null) throw new ApplicationException("User ID not found in Database");

            string[] selectedUserRoles = _unitOfWork.UserInformationUserRole.FindAsync(u => u.UserInformationId == id).Result
                .Select(r => r.UserRoleId.ToString()).ToArray();

            return GetEditView(userInformation, selectedUserRoles, title);
        }

        public UserInfoAdminEditViewModel GetEditView(UserInformation userInformation, string[] selectedUserRoles, string title)
        {
            var userInfoAdminEditViewModel = new UserInfoAdminEditViewModel();
            userInfoAdminEditViewModel.Title = title;
            userInfoAdminEditViewModel.UserInformation = userInformation;
            userInfoAdminEditViewModel.AssignedRoleDatas = GetAssignedUserRoles(selectedUserRoles);

            return userInfoAdminEditViewModel;
        }

        public bool CheckUser(string lanId)
        {
            var user = _unitOfWork.UserInformations.GetAsync(u => u.LanId == lanId);
            if (!user.Result.Any()) return false;
            return true;
        }

        public void CreateUser(UserInformation userInformation, string[] selectedUserRoles)
        {
            _unitOfWork.UserInformations.Add(userInformation);
            _unitOfWork.SaveChanges();

            AddRolesForUser(userInformation.UserInformationId, selectedUserRoles);
        }

        public void UpdateUser(UserInformation userInformation, string[] selectedUserRoles)
        {
            _unitOfWork.UserInformations.Update(userInformation);
            _unitOfWork.SaveChanges();

            AddRolesForUser(userInformation.UserInformationId, selectedUserRoles);
        }

        public void DeleteUser(int id)
        {
            var userToDelete = _unitOfWork.UserInformations.Find(u => u.UserInformationId == id).SingleOrDefault();
            _unitOfWork.UserInformations.Remove(userToDelete);
            _unitOfWork.SaveChanges();
        }

        /// <summary>
        /// This method uses MailLink service to sent email to access approver
        /// with a link to the edit page where the approval can be set for the user.
        /// </summary>
        /// <param name="userInformation"></param>
        /// <param name="hostingEnvironmentWebRootPath"></param>
        public void NotifyForNewUser(UserInformation userInformation, string hostingEnvironmentWebRootPath)
        {
            var sysConfig = _unitOfWork.SysConfigs.GetSysConfig();
            var fromAddress = new EmailAddress
            {
                Address = sysConfig.Result.AppFromEmail,
                Name = sysConfig.Result.AppFromName
            };
            var toAddress = new EmailAddress
            {
                Address = sysConfig.Result.UserAdministratorEmail,
                Name = sysConfig.Result.UserAdministratorName
            };
            var linkUrl = hostingEnvironmentWebRootPath + @"/UserInformationAdmin/edit/" +
                          userInformation.UserInformationId;
            var emailMessage = new EmailMessage();
            emailMessage.FromAddresses.Add(fromAddress);
            emailMessage.ToAddresses.Add(toAddress);
            emailMessage.Subject = "New User Request for Warranty IQ";
            emailMessage.Content =
                $"A new user request has been made for {userInformation.FirstName} {userInformation.LastName} " +
                $"Please use the following link to enable the user in the admin interface. " +
                $"{linkUrl}";

            _emailService.Send(emailMessage);
        }

        /// <summary>
        /// This method builds the list that can be used to have populated
        /// checkboxes on the view.
        /// </summary>
        /// <param name="selectedUserRoles"></param>
        /// <returns></returns>
        private List<AssignedRoleData> GetAssignedUserRoles(string[] selectedUserRoles)
        {
            var allUserRoles = _unitOfWork.UserRoles.GetAll();
            var assignedRoleDataDto = new List<AssignedRoleData>();
            foreach (var role in allUserRoles)
            {
                assignedRoleDataDto.Add(new AssignedRoleData
                {
                    UserRoleId = role.UserRoleId,
                    Name = role.Name,
                    Assigned = selectedUserRoles.Contains(role.UserRoleId.ToString())
                });
            }

            return assignedRoleDataDto;
        }

        /// <summary>
        /// This method first deletes all the existing roles for a user then
        /// adds the selected roles back in the database. This has to be done
        /// with the intermediary table because .Net core doesn't support
        /// Many to Many relationships yet.
        /// </summary>
        /// <param name="userInformationId"></param>
        /// <param name="selectedUserRoles"></param>
        private void AddRolesForUser(int userInformationId, string[] selectedUserRoles)
        {
            var rolesToDelete = _unitOfWork.UserInformationUserRole
                .Find(u => u.UserInformationId == userInformationId);

            _unitOfWork.UserInformationUserRole.RemoveRange(rolesToDelete);

            foreach (var selectedUserRole in selectedUserRoles)
            {
                _unitOfWork.UserInformationUserRole.Add(new UserInformationUserRole { UserInformationId = userInformationId, UserRoleId = int.Parse(selectedUserRole) });
            }

            _unitOfWork.SaveChanges();
        }

        //******* This Comment Saved in case Dot Net Core supports Many to Many ******
        //private List<AssignedRoleData> PopulateAssignedRoleData(UserInformation userInformation)
        //{
        //    var allUserRoles = _unitOfWork.UserRoles.GetAll();
        //    var userInformationUserRoles = new HashSet<int>(userInformation.UserInformationUserRoles.Select(r => r.UserRoleId));
        //    var assignedRoleDataDto = new List<AssignedRoleData>();
        //    foreach (var role in allUserRoles)
        //    {
        //        assignedRoleDataDto.Add(new AssignedRoleData
        //        {
        //            UserRoleId = role.UserRoleId,
        //            Name = role.Name,
        //            Assigned = userInformationUserRoles.Contains(role.UserRoleId)
        //        });
        //    }
        //   return assignedRoleDataDto;
        //}
    }
}