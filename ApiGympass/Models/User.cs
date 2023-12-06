using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;


namespace ApiGympass.Models
{
    public class User : IdentityUser<Guid>
    {
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual ICollection<CheckIn>? CheckIns { get; set; }
        public State State { get; set; } = State.Active;
        public Role Role { get; set; } = Role.Member;
        public User() : base() {}
    }
}