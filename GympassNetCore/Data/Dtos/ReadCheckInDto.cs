namespace ApiGympass.Data.Dtos
{
    public class ReadCheckInDto
    {
        public Guid Id { get; set;} 
        public DateTime CreatedAt { get; set; }
        public DateTime? ValidateAt { get; set; }
        public Guid UserId { get; set; }
        public Guid GymId { get; set; }

        public ReadCheckInDto(Guid id, Guid userId, Guid gymId, DateTime? validateAt, DateTime createdAt)
        {
            Id = id;
            UserId = userId;
            GymId = gymId;
            ValidateAt = validateAt;
            CreatedAt = createdAt;
        }
    }
}