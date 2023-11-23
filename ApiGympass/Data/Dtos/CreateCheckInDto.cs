using System.ComponentModel.DataAnnotations;

namespace ApiGympass.Data.Dtos
{
    public class CreateCheckInDto
    {
        public DateTime? ValidateAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public decimal? UserLatitude { get; set; }

        [Required]
        public decimal? UserLongitude { get; set; }

        [Required]
        public Guid GymId { get; set; }
    }
}