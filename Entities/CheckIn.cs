using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiGympass.Entities
{
    public class CheckIn
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ValidateAt { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid GymId { get; set; }
        public Gym Gym { get; set; }
    }
}