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
using Org.BouncyCastle.Asn1.Cmp;

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
            //session-UserID
            this.HttpContext.Session.SetInt32("UserID",UserID);
            var UserIDOne = this.HttpContext.Session.GetInt32("UserID");
            ViewData["UserID1"] = UserIDOne;
            //Cookie
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddMinutes(30);
            option.SameSite = SameSiteMode.Strict;
            string UserID1 = Convert.ToString(UserID);
            Response.Cookies.Append("Cookie1",UserID1,option);
            return View("Main");
        }
        [HttpGet]
        public IActionResult Display()
        {
            var IsCookieAvail = Request.Cookies.ContainsKey("Cookie1");
            string value;
            Request.Cookies.TryGetValue("Cookie1", out value);
            int item1 = this.bank.BalanceCheckUser(value);
            //
            this.HttpContext.Session.SetInt32("Balance", item1);
            var Bal = this.HttpContext.Session.GetInt32("Balance");
            ViewData["Balance"] = Bal;
            //
            return View("Display");
        }
        /*
        [HttpPost]
        public IActionResult Display([FromForm] int UserID)
        {
        var IsCookieAvail = Request.Cookies.ContainsKey("Cookie1");
        string value;
        Request.Cookies.TryGetValue("Cookie1",out value);
        int item1 = this.bank.BalanceCheckUser(value);
            Console.WriteLine("HC:"+item1);
            //
            this.HttpContext.Session.SetInt32("Balance",item1);
            var Bal = this.HttpContext.Session.GetInt32("Balance");
            ViewData["Balance"] = Bal;
            //
            return View("Display");
        }

        */

        [HttpGet]
        public IActionResult DepositView()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Deposit([FromForm] int Amt)
        {
            var IsCookieAvail = Request.Cookies.ContainsKey("Cookie1");
            string value;
            Request.Cookies.TryGetValue("Cookie1", out value);
            int item1 = this.bank.DepositUser(Amt, value);
            return View("DepositSuccess");
        }


        [HttpGet]
        public IActionResult WithdrawView()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Withdraw([FromForm] int Amt)
        {
            var IsCookieAvail = Request.Cookies.ContainsKey("Cookie1");
            string value;
            Request.Cookies.TryGetValue("Cookie1", out value);
            int UID = Convert.ToInt32(value);
            int item1 = this.bank.WithdrawUser(Amt, UID);
            return View("WithdrawSuccess");
        }
        [HttpGet]
        public IActionResult TransferView()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Transfer([FromForm] int Amt, int id2)
        {
            var IsCookieAvail = Request.Cookies.ContainsKey("Cookie1");
            string value;
            Request.Cookies.TryGetValue("Cookie1", out value);
            int UID = Convert.ToInt32(value);
            int item = this.bank.TransferUser(Amt,UID,id2);
            return View("TransferSuccess");
        }
        [HttpGet]
        public IActionResult Logouts()
        {
            return View();
        }
        [HttpPost]
        public IActionResult GetBalance(String UserID)
        {
            int item = this.bank.BalanceCheckUser(UserID);
            return View("Main");
        }
    }
}
