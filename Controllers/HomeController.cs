using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LoginAndRegistration.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace LoginAndRegistration.Controllers
{
    public class HomeController : Controller
    {
        private LoginAndRegistrationContext db;

        public HomeController(LoginAndRegistrationContext context)
        {
            db = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("/register")]
        public IActionResult Register(User newUser)
        {
            if (ModelState.IsValid)
            {
                // If any user already exists with email.
                if (db.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "is taken.");
                }
            }

            // If we added an error above.
            if (ModelState.IsValid == false)
            {
                // Show form again to display errors.
                return View("Index");
            }

            PasswordHasher<User> hasher = new PasswordHasher<User>();
            newUser.Password = hasher.HashPassword(newUser, newUser.Password);

            db.Users.Add(newUser);
            db.SaveChanges();

            HttpContext.Session.SetInt32("UserId", newUser.UserId);
            HttpContext.Session.SetString("FullName", newUser.FullName());

            return RedirectToAction("Success");
        }

        [HttpPost("/login")]
        public IActionResult LoginUser(LoginUser loginUser)
        {

            if (ModelState.IsValid == false)
            {
                // Show form again to display errors.
                return View("Index");
            }


            User dbUser = db.Users.FirstOrDefault(user => user.Email == loginUser.EmailLogin);

            if (dbUser == null)
            {
                /* 
                Normally we want to be ambiguous with these error messages to
                not reveal too much info. E.g., if we say password is incorrect
                a hacker may now know the email existed.
                */
                ModelState.AddModelError("EmailLogin", "email not found.");
                return View("Index"); // Display errors.
            }

            PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();

            // Right click PasswordVerificationResult and go to definition for more info.
            PasswordVerificationResult pwCompareResult = hasher.VerifyHashedPassword(loginUser, dbUser.Password, loginUser.PasswordLogin);

            if (pwCompareResult == 0)
            {
                ModelState.AddModelError("LoginEmail", "incorrect credentials.");
                return View("Index"); // Display Errors.
            }

            HttpContext.Session.SetInt32("UserId", dbUser.UserId);
            HttpContext.Session.SetString("FullName", dbUser.FullName());

            return RedirectToAction("Success");
        }

        [HttpGet("/success")]
        public IActionResult Success()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }

            return View("Success");
        }
        
        [HttpGet("/logout")]
        public IActionResult Logout()
        {

            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
