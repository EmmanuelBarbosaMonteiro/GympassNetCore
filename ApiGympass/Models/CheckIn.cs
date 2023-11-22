using System.ComponentModel.DataAnnotations;

namespace ApiGympass.Models
{
    public class CheckIn
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ValidateAt { get; set; }

        public Guid? UserId { get; set; }
        public virtual User? User { get; set; }

        public Guid? GymId { get; set; }
        public virtual Gym? Gym { get; set; }
    }
}