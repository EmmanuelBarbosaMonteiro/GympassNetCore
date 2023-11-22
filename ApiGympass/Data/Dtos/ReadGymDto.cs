namespace ApiGympass.Data.Dtos
{
    public class ReadGymDto
    {
        public Guid Id { get; }
        public string? Title { get; }
        public decimal? Latitude { get; }
        public decimal? Longitude { get; }
        public string? Description { get; }
        public string? Phone { get; }
    }
}