namespace ApiGympass.Data.Dtos
{
    public class CreateCheckInDto
    {
        public Guid UserId { get; set; }
        public Guid GymId { get; set; }
    }
}