using System.ComponentModel.DataAnnotations;

namespace DentalWebApp.Models
{
    public class Doctor
    {
        public int DoctorId { get; set; } // Primary Key

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        public string Specialization { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }
    }
}
