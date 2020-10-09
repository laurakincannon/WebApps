using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using WDLMassage.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace WDLMassage.Controllers
{
    public class AccountController : Controller
    {
        private readonly Team102DBContext _context;

        public AccountController(Team102DBContext context) //dependency injection
        {
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            //finds the id of user logged in from their claim
            int? id = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);

            //updates context w/ user info
            var userContext = _context.User.Where(u => u.UserPk == id);

            if (id == null)
            {
                return NotFound();
            }

            var staff = await _context.User
                .FirstOrDefaultAsync(m => m.UserPk == id);

            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        public IActionResult Login(string returnURL)
        {
            returnURL = string.IsNullOrEmpty(returnURL) ? "~/" : returnURL;

            return View(new Login { ReturnURL = returnURL });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Username, Password, ReturnURL")] Login login) //bind is to specify the properties that we want to associate with the model
        {
            if(ModelState.IsValid)
            {
                var aUser = await _context.User.FirstOrDefaultAsync(user => user.Username == login.Username && user.Password == login.Password);

                if (aUser != null)
                {
                    var claims = new List<Claim>();

                    claims.Add(new Claim(ClaimTypes.Name, aUser.NameFull));
                    claims.Add(new Claim(ClaimTypes.Sid, aUser.UserPk.ToString()));
                    claims.Add(new Claim(ClaimTypes.Role, aUser.Role));

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return Redirect(login?.ReturnURL ?? "~/"); //two question marks - assign a value directly if is null
                }
                else
                {
                    ViewData["message"] = "Invalid credentials";
                }
            }
            return View(login);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("NameFirst, NameLast, Email, Phone, Username, Password")] User user)
        {
            if (ModelState.IsValid)
            {
                var aUser = await _context.User.FirstOrDefaultAsync(newUser => newUser.Username == user.Username);

                if(aUser is null)
                {
                    user.IsAdmin = false;
                    user.IsActive = false;

                    _context.Add(user);
                    await _context.SaveChangesAsync(); //added 

                    TempData["success"] = $"Welcome {user.NameFull}. Please login to book a massage and review your account.";

                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    ViewData["message"] = $"Username {user.Username} already taken";
                }                
            }
            return View(user);
        }

        //GET: Account/Edit
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }
            var user = await _context.User.FindAsync(id);
            
            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        // POST: Account/Edit
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserPk, NameFirst, NameLast, Email, Phone, Username, Password")] User user)
        {
            if (id != user.UserPk)
            {
                return RedirectToAction(nameof(Index));
            }

            //finds the id of user logged in from their claim
            int? userID = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);

            var aUser = await _context.User.FirstOrDefaultAsync(u => u.UserPk == userID);

            if (aUser is null || aUser.UserPk != id)
            {
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    aUser.NameFirst = user.NameFirst;
                    aUser.NameLast = user.NameLast;
                    aUser.Phone = user.Phone;
                    aUser.Email = user.Email;
                    aUser.Password = user.Password;

                    _context.Update(aUser);
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    TempData["message"] = $"Unable to Update Account Info.";
                    return RedirectToAction(nameof(Index));
                }
                TempData["message"] = "Account Info succesfully updated.";
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        public async Task<RedirectToActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }


    }
}