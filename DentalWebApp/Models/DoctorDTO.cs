using System.ComponentModel.DataAnnotations;

namespace DentalWebApp.Models
{
    public class DoctorDTO
    {
        public int DoctorId { get; set; } // Primary Key (include if needed on the client side)

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        public string Specialization { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        // You might decide not to include ImageData in the DTO if it's not needed.
        // If you want to include it, you can keep it as a base64 string for convenience.
        public IFormFile? ImageFile { get; set; }
    }
}
