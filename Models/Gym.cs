using System.ComponentModel.DataAnnotations;

namespace ApiGympass.Models
{
    public class Gym
    {
        [Key]
        [Required]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        public decimal? Latitude { get; set; }

        [Required]
        public decimal? Longitude { get; set; }

        #nullable enable
        public string? Description {get; set; }        
        public string? Phone { get; set; }
        #nullable disable

        public virtual ICollection<CheckIn> CheckIns { get; set; }
    }
}