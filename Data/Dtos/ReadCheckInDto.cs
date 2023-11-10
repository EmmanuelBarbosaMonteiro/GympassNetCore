namespace ApiGympass.Data.Dtos
{
    public class ReadCheckInDto
    {
        public Guid Id { get;} 
        public DateTime CreatedAt { get; }
        public DateTime ValidateAt { get; }
        public Guid UserId { get; }
        public Guid GymId { get; }
    }
}