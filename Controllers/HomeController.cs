using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Project.Controllers
{
    public class HomeController : Controller
    {
        private ProjectContext _context;
 
        public HomeController(ProjectContext context)
        {
            _context = context;
        }
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {

            return View("Login");
        }

        [HttpPost]
        [Route("Register")]
        public IActionResult Register(CreateUser User)
        {
            if(ModelState.IsValid)
            {
                User LogUser = _context.user.Where(b => b.Email == User.Email).SingleOrDefault();
                if(LogUser == null)
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();

                    User NewUser = new User{First_name = User.First_name, Last_name = User.Last_name, Email = User.Email};
                    NewUser.Password = Hasher.HashPassword(NewUser, User.Password);
                    _context.Add(NewUser);
                    _context.SaveChanges();
                    return Redirect("/");
                }
                else
                {
                    ViewBag.InvalidEmail = true;
                }
            }
            return View("Login");
        }
        
        [HttpPost]
        [Route("Login")]
        public IActionResult Login(LoginUser CurUser)
        {
            if(ModelState.IsValid)
            {
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                User LogUser = _context.user.Where(b => b.Email == CurUser.Email).SingleOrDefault();
                if(LogUser != null)
                {
                    if(Hasher.VerifyHashedPassword(LogUser, LogUser.Password, CurUser.Password) != 0)
                    {
                        HttpContext.Session.SetInt32("LogUserId", LogUser.UserId);
                        return RedirectToAction("Home","Project");
                    }
                }
                else{
                    ViewBag.Invalid = false;
                    return View("Login");
                }
            }
            ViewBag.Invalid = false;
            return View("Login");
        }
        [Route("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }     
    }
}