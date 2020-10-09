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
    public class ApptsManagerController : Controller
    {
        private readonly Team102DBContext _context;

        public ApptsManagerController(Team102DBContext context)
        {
            _context = context;
        }

        // GET: Appointments
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var appointmentContext = _context.Appointment
                .OrderByDescending(appt => appt.Date)
                .ThenBy(appt => appt.Time)
                .Include(client => client.FkclientNavigation)
                .Include(massage => massage.FkmassageNavigation)
                .Include(staff => staff.FkstaffNavigation);
            return View(await appointmentContext.ToListAsync());
        }

        // GET: Appointments/Details/5
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public IActionResult Create(DateTime? appointmentDate)
        {
            ViewData["DateFilter"] = (appointmentDate != null) ? appointmentDate.Value.ToString("yyyy-MM-dd") : appointmentDate.ToString();

            List<SelectListItem> massageList = new SelectList(_context.Massage.OrderBy(massage => massage.Name), "MassagePk", "Name").ToList();

            List<SelectListItem> clientList = new SelectList(_context.User.Where(staff => staff.IsAdmin != true).OrderBy(client => client.NameLast), "UserPk", "NameFull").ToList();
                       
            List<SelectListItem> staffList= new SelectList(_context.User.Where(staff => staff.IsActive == true).Where(staff => staff.IsAdmin == true).OrderBy(staff => staff.NameLast), "UserPk", "NameFull").ToList();
            
            ViewData["Fkmassage"] = massageList;
            ViewData["Fkclient"] = clientList;
            ViewData["Fkstaff"] = staffList;

            return View();
        }

        // POST: Appointments/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointmentPk,Duration,Time,Date,Price,Fkmassage,Fkstaff,Fkclient")] Appointment appointment)
        {
            DateTime apptDate = appointment.Date;//find date of entered item

            List<SelectListItem> massageList = new SelectList(_context.Massage.OrderBy(massage => massage.Name), "MassagePk", "Name", appointment.Fkmassage).ToList();

            List<SelectListItem> clientList = new SelectList(_context.User.Where(staff => staff.IsAdmin != true).OrderBy(client => client.NameLast), "UserPk", "NameFull", appointment.Fkclient).ToList();

            List<SelectListItem> staffList = new SelectList(_context.User.Where(staff => staff.IsActive == true).Where(staff => staff.IsAdmin == true).OrderBy(staff => staff.NameLast), "UserPk", "NameFull", appointment.Fkstaff).ToList();

            ViewData["Fkmassage"] = massageList;
            ViewData["Fkclient"] = clientList;
            ViewData["Fkstaff"] = staffList;

            if (ModelState.IsValid)
            {
                if (apptDate >= DateTime.Today)//can only add appointments for a date in future
                {
                    _context.Add(appointment);
                    await _context.SaveChangesAsync();
                    TempData["message"] = $"Appointment added";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["message"] = $"Cannot create appointment for a past date.";
                    return View(appointment);
                }                
            }
            return View(appointment);
        }

        // GET: Appointments/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var appointment = await _context.Appointment.FindAsync(id);
            if (appointment == null)
            {
                return RedirectToAction(nameof(Index));
            }

            List<SelectListItem> massageList = new SelectList(_context.Massage.OrderBy(massage => massage.Name), "MassagePk", "Name", appointment.Fkmassage).ToList();

            List<SelectListItem> clientList = new SelectList(_context.User.Where(staff => staff.IsAdmin != true).OrderBy(client => client.NameLast), "UserPk", "NameFull", appointment.Fkclient).ToList();

            List<SelectListItem> staffList = new SelectList(_context.User.Where(staff => staff.IsActive == true).Where(staff => staff.IsAdmin == true).OrderBy(staff => staff.NameLast), "UserPk", "NameFull", appointment.Fkstaff).ToList();

            ViewData["Fkmassage"] = massageList;
            ViewData["Fkclient"] = clientList;
            ViewData["Fkstaff"] = staffList;

            return View(appointment);
        }

        // POST: Appointments/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointmentPk,Duration,Time,Date,Price,Fkmassage,Fkstaff,Fkclient")] Appointment appointment)
        {
            if (id != appointment.AppointmentPk)
            {
                return RedirectToAction(nameof(Index));
            }

            List<SelectListItem> massageList = new SelectList(_context.Massage.OrderBy(massage => massage.Name), "MassagePk", "Name", appointment.Fkmassage).ToList();

            List<SelectListItem> clientList = new SelectList(_context.User.Where(staff => staff.IsAdmin != true).OrderBy(client => client.NameLast), "UserPk", "NameFull", appointment.Fkclient).ToList();

            List<SelectListItem> staffList = new SelectList(_context.User.Where(staff => staff.IsActive == true).Where(staff => staff.IsAdmin == true).OrderBy(staff => staff.NameLast), "UserPk", "NameFull", appointment.Fkstaff).ToList();

            ViewData["Fkmassage"] = massageList;
            ViewData["Fkclient"] = clientList;
            ViewData["Fkstaff"] = staffList;

            DateTime apptDate = appointment.Date;
            
            if (ModelState.IsValid)
            {
                if(apptDate >= DateTime.Today)
                {
                    try
                    {
                        _context.Update(appointment);
                        await _context.SaveChangesAsync();
                    }
                    catch
                    {
                        TempData["message"] = $"Unable to Modify Appointment.";
                        return RedirectToAction(nameof(Index));
                    }
                    TempData["message"] = "Appointment successfully updated.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["message"] = $"Cannot reschedule appointment for a past date.";
                    return View(appointment);
                }                
            }            

            return View(appointment);
        }

        // GET: Appointments/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var appointment = await _context.Appointment
                .Include(a => a.FkclientNavigation)
                .Include(a => a.FkmassageNavigation)
                .Include(a => a.FkstaffNavigation)
                .FirstOrDefaultAsync(m => m.AppointmentPk == id);
            if (appointment == null)
            { 
                return RedirectToAction(nameof(Index));
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointment.FindAsync(id);

            if(appointment == null)
            {
                return RedirectToAction(nameof(Index));
            }
            try
            {
                _context.Appointment.Remove(appointment);
                await _context.SaveChangesAsync();
            }
            catch
            {
                TempData["message"] = "Unable to delete appointment";
                return RedirectToAction(nameof(Index));
            }

            TempData["message"] = "Appointment deleted successfully";
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointment.Any(e => e.AppointmentPk == id);
        }
    }
}
