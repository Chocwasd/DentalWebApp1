using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DentalWebApp.Data;
using DentalWebApp.Models;

namespace DentalWebApp.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoctorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Doctors
        public async Task<IActionResult> Index()
        {
            return _context.Doctors != null ?
                View(await _context.Doctors.ToListAsync()) :
                Problem("Entity set 'ApplicationDbContext.Doctors'  is null.");
        }

        // GET: Doctors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Doctors == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(m => m.DoctorId == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // GET: Doctors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Doctors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Specialization,PhoneNumber,ImageFile")] DoctorDTO doctorDto)
        {
            // Validate the ImageFile
            if (doctorDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The Image file is required.");
            }

            // If model is invalid, return the view with the current model
            if (!ModelState.IsValid)
            {
                return View(doctorDto);
            }

            // Save image as byte array
            byte[] imageBytes = null;
            if (doctorDto.ImageFile != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await doctorDto.ImageFile.CopyToAsync(memoryStream);
                    imageBytes = memoryStream.ToArray(); // Convert image to byte array
                }
            }

            // Create a new Doctor entity
            Doctor doctor = new Doctor()
            {
                FirstName = doctorDto.FirstName,
                LastName = doctorDto.LastName,
                Specialization = doctorDto.Specialization,
                PhoneNumber = doctorDto.PhoneNumber,
                ImageData = imageBytes, // Store the image as a byte array
            };

            // Add the doctor entity to the database
            _context.Add(doctor);
            await _context.SaveChangesAsync();

            // Redirect to the Index page after successfully creating a doctor
            return RedirectToAction(nameof(Index));
        }

        // GET: Doctors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Doctors == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            return View(doctor);
        }

        // POST: Doctors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DoctorId,FirstName,LastName,Specialization,PhoneNumber,ImageFile")] DoctorDTO doctorDto)
        {
            if (id != doctorDto.DoctorId)
            {
                return NotFound();
            }

            // Validate the ImageFile
            if (doctorDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The Image file is required.");
            }

            if (!ModelState.IsValid)
            {
                return View(doctorDto);
            }

            try
            {
                // Save image as byte array
                byte[] imageBytes = null;
                if (doctorDto.ImageFile != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await doctorDto.ImageFile.CopyToAsync(memoryStream);
                        imageBytes = memoryStream.ToArray(); // Convert image to byte array
                    }
                }

                // Find the doctor to edit and update their details
                var doctor = await _context.Doctors.FindAsync(id);
                if (doctor != null)
                {
                    doctor.FirstName = doctorDto.FirstName;
                    doctor.LastName = doctorDto.LastName;
                    doctor.Specialization = doctorDto.Specialization;
                    doctor.PhoneNumber = doctorDto.PhoneNumber;
                    doctor.ImageData = imageBytes; // Update the image data if provided

                    _context.Update(doctor);
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoctorExists(doctorDto.DoctorId))
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

        // GET: Doctors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Doctors == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(m => m.DoctorId == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // POST: Doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor != null)
            {
                _context.Doctors.Remove(doctor);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool DoctorExists(int id)
        {
            return (_context.Doctors?.Any(e => e.DoctorId == id)).GetValueOrDefault();
        }
    }
}
