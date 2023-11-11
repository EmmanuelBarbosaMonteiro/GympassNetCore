using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;


namespace ApiGympass.Models
{
    public class User : IdentityUser<Guid>
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<CheckIn> CheckIns { get; set; }

        public State State { get; set; } = State.Active;

        public User() : base() {}
    }
}