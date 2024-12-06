using System.ComponentModel.DataAnnotations;

namespace DentalWebApp.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; } // Primary Key

        [Required]
        public int DoctorId { get; set; } // Foreign Key to Doctor

        [Required]
        public int ServiceId { get; set; } // Foreign Key to Service

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        [StringLength(100)]
        public string PatientName { get; set; }

        public string PatientContact { get; set; }

        public string Notes { get; set; }
    }
}
