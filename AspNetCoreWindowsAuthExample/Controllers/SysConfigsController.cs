using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNetCoreWindowsAuthExample.Models;
using AspNetCoreWindowsAuthExample.Repositories;
using System.Threading.Tasks;
using AspNetCoreWindowsAuthExample.ViewModels;

namespace AspNetCoreWindowsAuthExample.Controllers
{
    public class SysConfigsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SysConfigsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: SysConfigs

        // GET: SysConfigs/Edit/5
        public async Task<IActionResult> Index()
        {
            var sysConfig = await _unitOfWork.SysConfigs.GetSysConfig();
            if (sysConfig == null)
            {
                return NotFound();
            }

            var pageView = new SysConfigViewModel();
            pageView.SysConfig = sysConfig;
            pageView.Title = "Edit System Configuration";
            return View(pageView);
        }

        // POST: SysConfigs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("SysConfigId,AppName,AppFolder,DeveloperName,DeveloperEmail,BusinessOwnerName,BusinessOwnerEmail,AppFromName,AppFromEmail,SmtpServer,SmtpPort,UserAdministratorName,UserAdministratorEmail,Rebuild")] SysConfig sysConfig)
        {
            if (sysConfig.SysConfigId != 1)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.SysConfigs.UpdateSysConfig(sysConfig);
                    await _unitOfWork.SysConfigs.Save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(Index), "Admin");
            }

            var pageView = new SysConfigViewModel();
            pageView.SysConfig = sysConfig;
            pageView.Title = "Edit System Configuration";
            return View(pageView);
        }
    }
}