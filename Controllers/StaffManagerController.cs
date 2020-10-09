using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Microsoft.EntityFrameworkCore;
using WDLMassage.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace WDLMassage.Controllers
{
    public class StaffManagerController : Controller
    {
        private readonly Team102DBContext _context;

        public StaffManagerController(Team102DBContext context)
        {
            _context = context;
        }

        // GET: StaffManager
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var staffContext = _context.User.Where(staff => staff.IsAdmin == true).OrderByDescending(staff => staff.IsActive).ThenBy(staff => staff.NameLast);

            return View(await staffContext.ToListAsync());
        }

        // GET: StaffManager/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
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

        // GET: StaffManager/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: StaffManager/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NameFirst, NameLast, Email, Phone, Username, Password")] User user) /*UserPk,IsAdmin,IsActive*/
        {
            if (ModelState.IsValid)
            {
                var aUser = await _context.User.FirstOrDefaultAsync(u => u.Username == user.Username);

                if(aUser is null)
                {
                    user.IsAdmin = true;
                    user.IsActive = true;

                    _context.Add(user);
                    await _context.SaveChangesAsync();//commits data

                    TempData["success"] = $"New staff member {user.NameFull} successfully added.";

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewData["message"] = $"Username {user.Username} already taken";
                }
            }            
            
            return View(user);
        }

        // GET: StaffManager/Deactivate
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Deactivate(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }
            
            var user = await _context.User.FirstOrDefaultAsync(m => m.UserPk == id); 
            
            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }
          
           return View(user); 
        }

        // POST: StaffManager/Deactivate
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id, [Bind("UserPk, NameFirst, NameLast, Email, Phone, Username, Password, IsAdmin")] User user)
        {
            if (id != user.UserPk)
            {
                return RedirectToAction(nameof(Index));
            }            

            if (ModelState.IsValid)
            {
                //find count of future appointments scheduled to user
                int countAppointments = _context.Appointment.Where(appt => appt.Fkstaff == user.UserPk).Where(appt => appt.Date > DateTime.Now).Count();

                if (countAppointments == 0)
                {
                    try
                    {
                        user.IsActive = false;
                        _context.Update(user);
                        await _context.SaveChangesAsync();
                    }
                    catch
                    {
                        TempData["message"] = $"Cannot deactivate {user.NameFull}.";
                        return RedirectToAction(nameof(Index));
                    }
                    TempData["message"] = $"{user.NameFull} deactivated.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["message"] = $"{user.NameFull} has scheduled appointments and cannot be deactivated.";
                    return RedirectToAction(nameof(Index));
                }                
            }

            return View(user);
        }
        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Activate(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var user = await _context.User.FirstOrDefaultAsync(m => m.UserPk == id); 


            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        // POST: StaffManager/Activate  "UserPk,NameFirst,NameLast,Email,Phone,Username,Password,IsAdmin,IsActive"
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id, [Bind("UserPk, NameFirst, NameLast, Email, Phone, Username, Password, IsAdmin")] User user)
        {
            if (id != user.UserPk)
            {
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.IsActive = true;
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    TempData["message"] = $"Unable to Activate {user.NameFull}.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["message"] = $"{user.NameFull} Activated.";
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

    }
}
