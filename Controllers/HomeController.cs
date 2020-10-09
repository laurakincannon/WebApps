using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Microsoft.Extensions.Logging;

namespace WDLMassage.Controllers
{
    public class HomeController : Controller
    {
        private readonly Team102DBContext _context;

        public HomeController(Team102DBContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index(DateTime? appointmentDate, int massageType = 0, int provider = 0)
        {
            List<SelectListItem> typeList = new SelectList(_context.Massage.OrderBy(m => m.Name), "MassagePk", "Name", massageType).ToList();

            typeList.Insert(0, (new SelectListItem { Text = "Pick a massage type", Value = "0" }));

            ViewData["TypeFilter"] = typeList;

            List<SelectListItem> providerList = new SelectList(_context.User.Where(u => u.IsActive == true).Where(u => u.IsAdmin == true).OrderBy(u => u.NameLast), "UserPk", "NameFull", provider).ToList();

            providerList.Insert(0, (new SelectListItem { Text = "Pick a provider", Value = "0" }));

            ViewData["ProviderFilter"] = providerList;

            var appointmentContext = _context.Appointment
                .Include(a => a.FkclientNavigation)
                .Include(a => a.FkmassageNavigation)
                .Include(a => a.FkstaffNavigation)
                .Where(u => u.Fkclient == 1)
                .Where(d => d.Date > DateTime.Now);

            if (appointmentDate != null)
            {
                appointmentContext = appointmentContext.Where(a => a.Date == appointmentDate);
            }
            if (massageType > 0)
            {
                appointmentContext = appointmentContext.Where(a => a.Fkmassage == massageType);
            }
            if (provider > 0)
            {
                appointmentContext = appointmentContext.Where(a => a.Fkstaff == provider);

            }

            return View(await appointmentContext.OrderByDescending(d => d.Date).ThenBy(t => t.Time).ToListAsync());
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointment
                .Include(a => a.FkclientNavigation)
                .Include(a => a.FkmassageNavigation)
                .Include(a => a.FkstaffNavigation)
                .FirstOrDefaultAsync(m => m.AppointmentPk == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }


        // GET: Appointments/Edit/5

        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var appointment = _context.Appointment.FirstOrDefault(f => f.AppointmentPk == id);

            if (appointment == null)
            {
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    int userPk = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);
                    appointment.Fkclient = userPk;
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    TempData["failure"] = $"Unable to Book Appointment.";
                    return RedirectToAction(nameof(Index));
                }
                TempData["success"] = "Appointment successfully booked.";
                return RedirectToAction(nameof(Index));
            }

            return View(appointment);
        }

        

        [Authorize]
        public async Task<IActionResult> StaffInfo()
        {
            var userContext = _context.User
                .Where(u => u.IsAdmin == true)
                .OrderByDescending(u => u.IsActive)
                .ThenByDescending(u => u.NameLast);

            return View(await userContext.ToListAsync());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
