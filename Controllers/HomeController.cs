using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BankAccount.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BankAccount.Controllers {
    public class HomeController : Controller {

        public IActionResult Index () {
            return View ();
        }

    }
}