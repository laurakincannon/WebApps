using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WDLMassage.Models;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace WDLMassage.Controllers
{
    public class AppointmentsController : Controller
    {
       
        private readonly Team102DBContext _context;

        public AppointmentsController(Team102DBContext context)
        {
            _context = context;
        }

        // GET: Appointments/Index
        public async Task<IActionResult> Index()
        {
            //var team102DBContext = _context.Appointment.Include(a => a.FkclientNavigation).Include(a => a.FkmassageNavigation).Include(a => a.FkstaffNavigation);

            int userPk = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);

            var appointment = _context.Appointment
                .Include(a => a.FkclientNavigation)
                .Include(a => a.FkmassageNavigation)
                .Include(a => a.FkstaffNavigation)
                .Where(m => m.Fkclient == userPk);

            return View(await appointment.OrderByDescending(d => d.Date).ThenBy(t => t.Time).ToListAsync());
        }
        
        // GET: Appointments/Details
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

        // GET: Appointments/Create
        public IActionResult Create()
        {
            ViewData["Fkclient"] = new SelectList(_context.User, "UserPk", "Email");
            ViewData["Fkmassage"] = new SelectList(_context.Massage, "MassagePk", "Description");
            ViewData["Fkstaff"] = new SelectList(_context.User, "UserPk", "Email");
            return View();
        }

        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointmentPk,Duration,Time,Date,Price,Fkmassage,Fkstaff,Fkclient")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Fkclient"] = new SelectList(_context.User, "UserPk", "Email", appointment.Fkclient);
            ViewData["Fkmassage"] = new SelectList(_context.Massage, "MassagePk", "Description", appointment.Fkmassage);
            ViewData["Fkstaff"] = new SelectList(_context.User, "UserPk", "Email", appointment.Fkstaff);
            return View(appointment);
        }

        [Authorize]
        // GET: Appointments/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointment.FindAsync(id);
            if (appointment == null)
            {
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    int userPk = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);
                    appointment.Fkclient = 1;
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    TempData["message"] = $"Unable to Cancel Appointment.";
                    return RedirectToAction(nameof(Index));
                }
                TempData["message"] = "Appointment Cancelled.";
                return RedirectToAction(nameof(Index));
            }


            //var appointment = await _context.Appointment.FindAsync(id);
            //if (appointment == null)
            //{
            //    return NotFound();
            //}
            //ViewData["Fkclient"] = new SelectList(_context.User, "UserPk", "Email", appointment.Fkclient);
            //ViewData["Fkmassage"] = new SelectList(_context.Massage, "MassagePk", "Description", appointment.Fkmassage);
            //ViewData["Fkstaff"] = new SelectList(_context.User, "UserPk", "Email", appointment.Fkstaff);
            return View(appointment);
        }

        // POST: Appointments/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointmentPk,Duration,Time,Date,Price,Fkmassage,Fkstaff,Fkclient")] Appointment appointment)
        {
            if (id != appointment.AppointmentPk)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.AppointmentPk))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Fkclient"] = new SelectList(_context.User, "UserPk", "Email", appointment.Fkclient);
            ViewData["Fkmassage"] = new SelectList(_context.Massage, "MassagePk", "Description", appointment.Fkmassage);
            ViewData["Fkstaff"] = new SelectList(_context.User, "UserPk", "Email", appointment.Fkstaff);
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointment.FindAsync(id);
            _context.Appointment.Remove(appointment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointment.Any(e => e.AppointmentPk == id);
        }
    }
}
