using System;
using System.Collections.Generic;
using System.Linq;
using BankAccount.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // For Password Hashing
using Microsoft.AspNetCore.Http; // For Session

namespace BankAccount.Controllers {
    public class HomeController : Controller {

        private MyContext _context;

        public HomeController (MyContext context) {
            _context = context;
        }

        //*********************** GET REQUEST ****************************//

        [HttpGet ("")]
        public IActionResult Index () {
            return View ();
        }

        [HttpGet ("loginpage")]
        public ViewResult LoginPage () {
            return View ();
        }

        [HttpGet ("account/{Id}")]
        public IActionResult Account (int Id) {
            if (HttpContext.Session.GetInt32 ("UserId") != Id) {
                return RedirectToAction ("loginpage");
            } else {
                User user = _context.Users
                    .Include (u => u.OwnerTransactions)
                    .FirstOrDefault (u => u.UserId == Id);
                ViewBag.UT = _context.Transactions
                    .Where (t => t.Owner.UserId == Id)
                    .ToList ();
                int? num = HttpContext.Session.GetInt32 ("UserId");
                Console.WriteLine ($"I AM logged in. My Id => {num}");
                return View (user);
            }
        }

        [HttpGet ("logout")]
        public IActionResult Logout () {
            Console.WriteLine ($"I WAS login. My Id => {HttpContext.Session.GetInt32 ("UserId")}");
            HttpContext.Session.Clear();
            Console.WriteLine ($"NOW IM out. Id => {HttpContext.Session.GetInt32 ("UserId")}");
            return View ("Index");
        }

        //*********************** POST REQUEST ****************************//

        [HttpPost ("login")]
        public IActionResult Login (LoginUser log) {
            if (ModelState.IsValid) {
                User userInDb = _context.Users.FirstOrDefault (u => u.Email == log.LoginEmail);
                Console.WriteLine (userInDb.FirstName);
                if (userInDb == null) {
                    ModelState.AddModelError ("LoginEmail", "Invalid Email/Password");
                    return View ("loginpage");
                } else {
                    var hasher = new PasswordHasher<LoginUser> ();
                    var result = hasher.VerifyHashedPassword (log, userInDb.Password, log.LoginPassword);
                    if (result == 0) {
                        ModelState.AddModelError ("LoginPassword", "Invalid Email/Password");
                        return View ("loginpage");
                    } else {
                        HttpContext.Session.SetInt32 ("UserId", userInDb.UserId);
                        return RedirectToAction ("account", new { Id = userInDb.UserId });
                    }
                }
            } else {
                Console.WriteLine (log.LoginEmail);
                return View ("loginpage");
            }
        }

        [HttpPost ("register")]
        public IActionResult Register (User user) {
            if (ModelState.IsValid) {
                if (_context.Users.Any (u => u.Email == user.Email)) {
                    ModelState.AddModelError ("Email", "Email already in use!");
                    return View ("Index");
                } else {
                    PasswordHasher<User> Hasher = new PasswordHasher<User> ();
                    user.Password = Hasher.HashPassword (user, user.Password);
                    _context.Users.Add (user);
                    _context.SaveChanges ();
                    HttpContext.Session.SetInt32 ("UserId", user.UserId);
                    Console.WriteLine ($"User id: {user.UserId}\nFirst Name: {user.FirstName}\nLastName: {user.LastName}\nEmail: {user.Email}\nSessionId: {HttpContext.Session.GetInt32("UserId")}");
                    return RedirectToAction ("account", new { Id = user.UserId });
                }
            } else {
                return View ("Index");
            }
        }

        [HttpPost ("transaction")]
        public IActionResult Transaction (Transaction transaction) {
            int? TransId = HttpContext.Session.GetInt32 ("UserId");
            User CurrentUser = _context.Users
                .FirstOrDefault (u => u.UserId == TransId);
            if ((CurrentUser.Balance + transaction.Amount) < 0) {
                return RedirectToAction ("account", new { Id = TransId });
            } else {
                _context.Transactions.Add (transaction);
                _context.SaveChanges ();
                CurrentUser.Balance += transaction.Amount;
                _context.SaveChanges ();
                Console.WriteLine ($"Transaction: ${transaction.Amount}");
            }
            return RedirectToAction ("account", new { Id = TransId });
        }
    }
}