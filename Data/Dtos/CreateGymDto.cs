using System.ComponentModel.DataAnnotations;

namespace ApiGympass.Data.Dtos
{
    public class CreateGymDto
    {
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
    }
}