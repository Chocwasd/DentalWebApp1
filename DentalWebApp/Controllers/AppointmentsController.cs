using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DentalWebApp.Data;
using DentalWebApp.Models;

namespace DentalWebApp.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
              return _context.Appointments != null ? 
                          View(await _context.Appointments.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Appointments'  is null.");
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Appointments == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }
        public IActionResult Create()
        {
            // Get a list of doctors, concatenating their FirstName and LastName
            var doctors = _context.Doctors
                .Select(d => new
                {
                    d.DoctorId,
                    FullName = d.FirstName + " " + d.LastName  // Combine FirstName and LastName
                }).ToList();

            // Pass the list of doctors to ViewBag as a SelectList
            ViewBag.DoctorId = new SelectList(doctors, "DoctorId", "FullName");

            // Get a list of services
            var services = _context.Services
                .Select(s => new
                {
                    s.ServiceId,
                    s.ServiceName  // Display ServiceName in the dropdown
                }).ToList();

            // Pass the list of services to ViewBag as a SelectList
            ViewBag.ServiceId = new SelectList(services, "ServiceId", "ServiceName");

            return View();
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointmentId,DoctorId,ServiceId,AppointmentDate,PatientName,PatientContact,Notes")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                // Check if the selected doctor is already booked at the chosen appointment time
                var existingAppointment = await _context.Appointments
                    .FirstOrDefaultAsync(a => a.DoctorId == appointment.DoctorId && a.AppointmentDate == appointment.AppointmentDate);

                if (existingAppointment != null)
                {
                    // If an appointment already exists, add a custom error message
                    ModelState.AddModelError("AppointmentDate", "The selected doctor is already booked for this time.");

                    // Repopulate ViewBag for dropdowns before returning the view
                    ViewBag.DoctorId = new SelectList(_context.Doctors, "DoctorId", "FirstName", appointment.DoctorId);
                    ViewBag.ServiceId = new SelectList(_context.Services, "ServiceId", "ServiceName", appointment.ServiceId);

                    return View(appointment);
                }

                // If no conflicts, save the new appointment
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Repopulate ViewBag if the model state is invalid
            ViewBag.DoctorId = new SelectList(_context.Doctors, "DoctorId", "FirstName", appointment.DoctorId);
            ViewBag.ServiceId = new SelectList(_context.Services, "ServiceId", "ServiceName", appointment.ServiceId);

            return View(appointment);
        }


        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Appointments == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            // Populate ViewBag for Doctor and Service dropdowns
            ViewBag.DoctorId = new SelectList(_context.Doctors, "DoctorId", "FirstName", appointment.DoctorId);
            ViewBag.ServiceId = new SelectList(_context.Services, "ServiceId", "ServiceName", appointment.ServiceId);

            return View(appointment);
        }

        // POST: Appointments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointmentId,DoctorId,ServiceId,AppointmentDate,PatientName,PatientContact,Notes")] Appointment appointment)
        {
            if (id != appointment.AppointmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Check if the selected doctor is already booked for the new appointment date and time
                var existingAppointment = await _context.Appointments
                    .FirstOrDefaultAsync(a => a.DoctorId == appointment.DoctorId && a.AppointmentDate == appointment.AppointmentDate && a.AppointmentId != id);

                if (existingAppointment != null)
                {
                    // If a conflict exists, add a model error
                    ModelState.AddModelError("AppointmentDate", "The selected doctor is already booked for this time.");

                    // Repopulate the dropdowns to retain the selected values
                    ViewBag.DoctorId = new SelectList(_context.Doctors, "DoctorId", "FirstName", appointment.DoctorId);
                    ViewBag.ServiceId = new SelectList(_context.Services, "ServiceId", "ServiceName", appointment.ServiceId);

                    return View(appointment);
                }

                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.AppointmentId))
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

            // Repopulate ViewBag for dropdowns in case of invalid model state
            ViewBag.DoctorId = new SelectList(_context.Doctors, "DoctorId", "FirstName", appointment.DoctorId);
            ViewBag.ServiceId = new SelectList(_context.Services, "ServiceId", "ServiceName", appointment.ServiceId);

            return View(appointment);
        }

        private bool AppointmentExists(int id)
        {
            return (_context.Appointments?.Any(e => e.AppointmentId == id)).GetValueOrDefault();
        }


        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Appointments == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
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
            if (_context.Appointments == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Appointments'  is null.");
            }
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
