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
    public class SurveyController : Controller
    {
        private readonly Team102DBContext _context;

        public SurveyController(Team102DBContext context)
        {
            _context = context;
        }

        // GET: Survey
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var team102DBContext = _context.Outtake.Include(o => o.FkappointmentNavigation).Include(o => o.FkclientNavigation).Include(o => o.FkstaffNavigation);
            return View(await team102DBContext.ToListAsync());
        }

        // GET: Survey/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var outtake = await _context.Outtake
                .Include(o => o.FkappointmentNavigation)
                .Include(o => o.FkclientNavigation)
                .Include(o => o.FkstaffNavigation)
                .FirstOrDefaultAsync(m => m.SurveyPk == id);
            if (outtake == null)
            {
                return NotFound();
            }

            return View(outtake);
        }

        // GET: Survey/Create
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

            var aSurvey = _context.Intake
                          .FirstOrDefault(i => i.Fkappointment == id && i.Fkclient == userPK);

            // If user has an intake, redirect to My Appointments

            if (aSurvey != null)
            {
                return RedirectToAction("Index", "Appointments");
            }

            Outtake survey = new Outtake { Fkappointment = appointment.AppointmentPk };
            //TODO need to update appointment record w/ value for intakeSurvey

            return View(survey);

        }

        // POST: Survey/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SurveyPk,RatingProfessional,RatingAddressedNeeds,RatingMassageQuality,TotalScore,Comments,IsComplete,Fkappointment,Fkstaff,Fkclient")] Outtake survey)
        {

            //if appointment Pk is not valid
            var appointment = _context.Appointment.FirstOrDefault(f => f.AppointmentPk == survey.Fkappointment);


            if (appointment == null)
            {
                return RedirectToAction("Index", "Appointments");
            }

            // retrieve user's PK from the Claims collection
            int userPK = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);
            survey.Fkclient = userPK;
            survey.Fkstaff = appointment.Fkstaff;


            try
            {
                _context.Add(survey);
                await _context.SaveChangesAsync();

                //update outake survey foreign key value in appointment table
                var enteredSurvey = await _context.Outtake.FirstOrDefaultAsync(surv => surv.Fkappointment == appointment.AppointmentPk);
                appointment.Fkouttake = enteredSurvey.SurveyPk;
                _context.Update(appointment);
                await _context.SaveChangesAsync();
            }
            catch
            {
                TempData["failure"] = $"Your feedback was not added";
                return RedirectToAction("Index", "Appointments");
            }

            //set IsComplete to true      
            survey.IsComplete = true;

            TempData["success"] = $"Thank you for submitting your feedback!";
            return RedirectToAction("Index", "Appointments");

        }

        // GET: Survey/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var outtake = await _context.Outtake.FindAsync(id);
            if (outtake == null)
            {
                return NotFound();
            }
            ViewData["Fkappointment"] = new SelectList(_context.Appointment, "AppointmentPk", "Duration", outtake.Fkappointment);
            ViewData["Fkclient"] = new SelectList(_context.User, "UserPk", "Email", outtake.Fkclient);
            ViewData["Fkstaff"] = new SelectList(_context.User, "UserPk", "Email", outtake.Fkstaff);
            return View(outtake);
        }

        // POST: Survey/Edit
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SurveyPk,RatingProfessional,RatingAddressedNeeds,RatingMassageQuality,TotalScore,Comments,IsComplete,Fkappointment,Fkstaff,Fkclient")] Outtake outtake)
        {
            if (id != outtake.SurveyPk)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(outtake);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OuttakeExists(outtake.SurveyPk))
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
            ViewData["Fkappointment"] = new SelectList(_context.Appointment, "AppointmentPk", "Duration", outtake.Fkappointment);
            ViewData["Fkclient"] = new SelectList(_context.User, "UserPk", "Email", outtake.Fkclient);
            ViewData["Fkstaff"] = new SelectList(_context.User, "UserPk", "Email", outtake.Fkstaff);
            return View(outtake);
        }


        //outtake delete
        [Authorize]
        [ActionName("Delete")]

        public async Task<IActionResult> DeleteConfirmed(int id)

        {
            // retrieve user's PK from the Claims collection
            int userPK = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);

            //retrieve user's survey
            var survey = await _context.Outtake.
            FirstOrDefaultAsync(i => i.SurveyPk == id && i.Fkclient == userPK);

            //if survey PK is not valid

            if (survey == null)
            {
                return RedirectToAction("Index", "Appointments");
            }

            try

            {
                //update outtake  foreign key value in appointment table
                var appointment = _context.Appointment.FirstOrDefault(a => a.AppointmentPk == survey.Fkappointment);
               appointment.Fkintake = null;
               _context.Update(appointment);

                var outtake = await _context.Outtake.FindAsync(id);
                _context.Outtake.Remove(outtake);
                await _context.SaveChangesAsync();
                
            }

            catch
            {
                TempData["failure"] = $"Your survey was not deleted";
                return RedirectToAction("Index", "Appointments");
            }

            TempData["success"] = $"Your survey was sucessfully deleted";

            return RedirectToAction("Index", "Appointments");
        }
        private bool OuttakeExists(int id)
        {
            return _context.Outtake.Any(e => e.SurveyPk == id);
        }
    }
}
