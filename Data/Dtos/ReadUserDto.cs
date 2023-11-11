using ApiGympass.Models;

namespace ApiGympass.Data.Dtos
{
    public class ReadUserDto
    {
        public Guid Id { get; }
        public string Email { get; }
        public string Name { get; }
        public State State { get; set; }
        

        public ReadUserDto(Guid id, string email, string name)
        {
            Id = id;
            Email = email;
            Name = name;
        }
    }
}