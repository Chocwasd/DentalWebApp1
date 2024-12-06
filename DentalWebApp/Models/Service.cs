using System.ComponentModel.DataAnnotations;

namespace DentalWebApp.Models
{
    public class Service
    {
        public int ServiceId { get; set; } // Primary Key

        [Required]
        [StringLength(100)]
        public string ServiceName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public decimal Price { get; set; }
    }
}
