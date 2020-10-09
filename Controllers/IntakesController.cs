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
    public class IntakesController : Controller
    {
        private readonly Team102DBContext _context;

        public IntakesController(Team102DBContext context)
        {
            _context = context;
        }

        [Authorize]
        // GET: Intakes
        public async Task<IActionResult> Index()
        {
            var team102DBContext = _context.Intake.Include(i => i.FkappointmentNavigation).Include(i => i.FkclientNavigation).Include(i => i.FkstaffNavigation);
            return View(await team102DBContext.ToListAsync());
        }

        [Authorize]
        // GET: Intakes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var intake = await _context.Intake
                .Include(i => i.FkappointmentNavigation)
                .Include(i => i.FkclientNavigation)
                .Include(i => i.FkstaffNavigation)
                .FirstOrDefaultAsync(m => m.IntakePk == id);
            if (intake == null)
            {
                return NotFound();
            }

            return View(intake);
        }


        [Authorize]
        public IActionResult Create(int? id)
        {
            //if id is null

            if (id == null)
            {
                return RedirectToAction("Index", "Appointments");
            }

            //if appointmentpk is not valid

            var appointment = _context.Appointment.FirstOrDefault(f => f.AppointmentPk == id);

            if (appointment == null)
            {
                return RedirectToAction("Index", "Appointments");
            }

            // retrieve user's PK from the Claims collection

            int userPK = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);

            // Check if user already has an intake for the appointment

            var anIntake = _context.Intake
                          .FirstOrDefault(i => i.Fkappointment == id && i.Fkclient == userPK);

            // If user has an intake, redirect to My Appointments

            if (anIntake != null)
            {
                return RedirectToAction("Index", "Appointments");
            }

            Intake intake = new Intake { Fkappointment = appointment.AppointmentPk };

            return View(intake);
        }


        // POST: Intakes/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FeelingWell,Surgeries,Medications,Sensitives,FocusAreas,IsComplete, Fkappointment,Fkclient,Fkstaff")] Intake intake)
        {

            //if appointment Pk is not valid
            var appointment = _context.Appointment.FirstOrDefault(f => f.AppointmentPk == intake.Fkappointment);
            
            if (appointment == null)
            {
                return RedirectToAction("Index", "Appointments");
            }

            // retrieve user's PK from the Claims collection
            int userPK = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);
            intake.Fkclient = userPK;
            intake.Fkstaff = appointment.Fkstaff;

            try
            {
                _context.Add(intake);
                await _context.SaveChangesAsync();

                //update intake survey foreign key value in appointment table
                var enteredSurvey = await _context.Intake.FirstOrDefaultAsync(surv => surv.Fkappointment == appointment.AppointmentPk);
                appointment.Fkintake = enteredSurvey.IntakePk;
                _context.Update(appointment);
                await _context.SaveChangesAsync();
            }
            catch
            {
                TempData["failure"] = $"Your intake form was not added";
                return RedirectToAction("Index", "Appointments");
            }

            //set IsComplete to true      
            intake.IsComplete = true;

            TempData["success"] = $"Your intake form was sucessfully added";
            return RedirectToAction("Index", "Appointments");

            //return View(intake);
        }


        // GET: Intakes/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var intake = await _context.Intake.FindAsync(id);
            if (intake == null)
            {
                return NotFound();
            }
            ViewData["Fkappointment"] = new SelectList(_context.Appointment, "AppointmentPk", "Duration", intake.Fkappointment);
            ViewData["Fkclient"] = new SelectList(_context.User, "UserPk", "Email", intake.Fkclient);
            ViewData["Fkstaff"] = new SelectList(_context.User, "UserPk", "Email", intake.Fkstaff);
            return View(intake);
        }

        // POST: Intakes/Edit
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IntakePk,FeelingWell,Surgeries,Medications,Sensitives,FocusAreas,IsComplete,Fkappointment,Fkclient,Fkstaff")] Intake intake)
        {
            if (id != intake.IntakePk)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(intake);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IntakeExists(intake.IntakePk))
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
            ViewData["Fkappointment"] = new SelectList(_context.Appointment, "AppointmentPk", "Duration", intake.Fkappointment);
            ViewData["Fkclient"] = new SelectList(_context.User, "UserPk", "Email", intake.Fkclient);
            ViewData["Fkstaff"] = new SelectList(_context.User, "UserPk", "Email", intake.Fkstaff);
            return View(intake);
        }

        // Intakes/Delete
        [Authorize]
        [ActionName("Delete")]
      
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            // retrieve user's PK from the Claims collection
            int userPK = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);

            //retrieve user's intake
            var intake = await _context.Intake.
               FirstOrDefaultAsync(i => i.IntakePk == id && i.Fkclient == userPK);

            //if intakepk is not valid

            if (intake == null)
            {
                return RedirectToAction("Index", "Appointments");
            }

            try
            {
                //update intake  foreign key value in appointment table

                var appointment = _context.Appointment.FirstOrDefault(a => a.AppointmentPk == intake.Fkappointment);
                appointment.Fkintake = null;
                _context.Update(appointment);
                _context.Intake.Remove(intake);
                await _context.SaveChangesAsync();

            }
            catch
            {
                {
                    TempData["failure"] = $"Your intake was not deleted";
                    return RedirectToAction("Index", "Appointments");
                }
            }

            TempData["success"] = $"Your intake was sucessfully deleted";

            return RedirectToAction("Index", "Appointments");
        }

        private bool IntakeExists(int id)
        {
            return _context.Intake.Any(e => e.IntakePk == id);
        }

    }
}





