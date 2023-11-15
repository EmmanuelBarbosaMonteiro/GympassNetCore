using System.ComponentModel.DataAnnotations;

namespace ApiGympass.Data.Dtos
{
    public class CreateUserDto
    {

        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}