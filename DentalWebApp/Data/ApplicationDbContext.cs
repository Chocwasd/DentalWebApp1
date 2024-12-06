using DentalWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DentalWebApp.Data
{

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Doctor - Appointment relationship with Cascade Delete
            modelBuilder.Entity<Appointment>()
                .HasOne<Doctor>()
                .WithMany()  // No navigation property in Doctor to appointments
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);  // Automatically delete Appointments when Doctor is deleted

            // Configure Service - Appointment relationship with Cascade Delete
            modelBuilder.Entity<Appointment>()
                .HasOne<Service>()
                .WithMany()  // No navigation property in Service to appointments
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);  // Automatically delete Appointments when Service is deleted
        }

    }
}