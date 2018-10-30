using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreWindowsAuthExample.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreWindowsAuthExample.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            var page = new BaseViewModel();
            page.Title = "Admin Menu";
            return View(page);
        }
    }
}