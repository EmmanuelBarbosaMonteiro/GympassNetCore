namespace ApiGympass.Data.Dtos
{
    public class CreateCheckInDto
    {
        public Guid Id { get; set;}
        public DateTime? ValidateAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid UserId { get; set; }
        public Guid GymId { get; set; }
    }
}