using System;
using System.Collections.Generic;
using System.Linq;
using BankAccount.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity; // For Password Hashing
using Microsoft.AspNetCore.Http; // For Session

namespace BankAccount.Controllers {
    public class HomeController : Controller {

        private MyContext _context;

        public HomeController (MyContext context) {
            _context = context;
        }

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
            if (HttpContext.Session.GetInt32 ("UserId") == null) {
                return RedirectToAction ("loginpage");
            } else {
                User user = _context.Users
                        .Include(u => u.OwnerTransactions)
                        .FirstOrDefault (u => u.UserId == Id);
                
                ViewBag.UserTransactions = _context.Transactions
                        .Include(t => t.Owner)
                        .Where(t => t.Owner.UserId == Id)
                        .ToList();
                        
                int? num = HttpContext.Session.GetInt32 ("UserId");
                Console.WriteLine ($"I AM logged in. My Id => {num}");
                return View (user);
            }
        }

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

        [HttpPost("transaction/{UserId}")]
        public IActionResult Transaction (int UserId)
        {
            
            return RedirectToAction("account/{UserId}");
        }

    }
}