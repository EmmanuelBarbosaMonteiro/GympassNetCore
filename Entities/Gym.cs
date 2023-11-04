using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiGympass.Entities
{
    public class Gym
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        #nullable enable
        public string? Description {get; set; }        
        public string? Phone { get; set; }
        #nullable disable

        [Required]
        public decimal Latitude { get; set; }

        [Required]
        public decimal Longitude { get; set; }

        public List<CheckIn> CheckIns { get; set; } = new List<CheckIn>();
    }
}