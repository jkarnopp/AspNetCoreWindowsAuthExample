using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNetCoreWindowsAuthExample.Models;
using System.Threading.Tasks;
using AspNetCoreWindowsAuthExample.Services;

namespace AspNetCoreWindowsAuthExample.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UserInformationAdminController : Controller
    {
        private readonly IUserInformationAdminService _userInformationAdminService;

        public UserInformationAdminController(IUserInformationAdminService userInformationAdminService)
        {
            _userInformationAdminService = userInformationAdminService;
        }

        // GET: UserInformationAdmin
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page, int? id)
        {
            var pageSize = 10;
            //Todo: make the service methods asynchronous
            var userView = _userInformationAdminService.GetIndexView(sortOrder, currentFilter, searchString, page, id, pageSize);
            return View(userView);
        }

        // GET: UserInformationAdmin/Create
        public IActionResult Create()
        {
            var createView = _userInformationAdminService.GetCreateView();
            return View(createView);
        }

        // POST: UserInformationAdmin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserInformationId,LanId,FirstName,LastName,Email,Enabled")] UserInformation userInformation, string[] selectedUserRoles)
        {
            if (userInformation.LanId != null && _userInformationAdminService.CheckUser(userInformation.LanId))
            {
                ModelState.AddModelError("LanId", "LanId Already Exists");
            }

            if (ModelState.IsValid)
            {
                _userInformationAdminService.CreateUser(userInformation, selectedUserRoles);

                return RedirectToAction(nameof(Index));
            }
            var createView = _userInformationAdminService.GetEditView(userInformation, selectedUserRoles, "Create User");
            return View(createView);
        }

        // GET: UserInformationAdmin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var editView = _userInformationAdminService.GetEditView((int)id, "Edit User");

            return View(editView);
        }

        // POST: UserInformationAdmin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserInformationId,LanId,FirstName,LastName,Email,Enabled")] UserInformation userInformation, string[] selectedUserRoles)
        {
            if (id != userInformation.UserInformationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(userInformation);
                    //await _context.SaveChangesAsync();
                    _userInformationAdminService.UpdateUser(userInformation, selectedUserRoles);
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            var editView = _userInformationAdminService.GetEditView(userInformation, selectedUserRoles, "Edit User");
            return View(editView);
        }

        // GET: UserInformationAdmin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var editView = _userInformationAdminService.GetEditView((int)id, "Delete User");

            return View(editView);
        }

        // POST: UserInformationAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _userInformationAdminService.DeleteUser(id);
            return RedirectToAction(nameof(Index));
        }
    }
}