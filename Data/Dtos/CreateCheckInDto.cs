namespace ApiGympass.Data.Dtos
{
    public class CreateCheckInDto
    {
        public DateTime? ValidateAt { get; set; }
        public Guid UserId { get; set; }
        public Guid GymId { get; set; }

    }
}