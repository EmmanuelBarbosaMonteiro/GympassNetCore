namespace ApiGympass.Services.ErrorHandling
{
    public class GymNotFoundError : Exception
    {
        public GymNotFoundError() : base("Gym not found.")
        {
        }
    }
}