using ApiGympass.Models;

namespace ApiGympass.Data.Dtos
{
    public class ReadUserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public State State { get; set; }
        

        public ReadUserDto(Guid id, string email, string name)
        {
            Id = id;
            Email = email;
            UserName = name;
        }
    }
}