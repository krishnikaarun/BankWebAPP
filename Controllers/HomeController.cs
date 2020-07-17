using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BankAPPWeb.Model;
using BankAPPWeb.Banks;
using Microsoft.AspNetCore.Http;

namespace BankAPPWeb.Controllers
{
    public class HomeController : Controller
    {
        Bank bank;
        public HomeController()
        {
            this.bank = new Bank();
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Main([FromForm] int UserID, int PIN)
        {
            User item = this.bank.LoginUser(UserID, PIN);
            this.HttpContext.Session.SetInt32("UserID",UserID);
            var UserIDOne = this.HttpContext.Session.GetInt32("UserID");
            ViewData["UserID1"] = UserIDOne;
            return View("Main");
        }
        [HttpGet]
        public IActionResult Logouts()
        {
            return View();
        }
        [HttpPost]
        public IActionResult GetBalance(int UserID)
        {
            User item = this.bank.BalanceCheckUser(UserID);
            return View("Main");
        }
    }
}
