namespace ApiGympass.Data.Dtos
{
    public class ReadGymDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? Description { get; set; }
        public string? Phone { get; set; }
    }
}