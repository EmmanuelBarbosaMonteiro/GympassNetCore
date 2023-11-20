namespace ApiGympass.Services.ErrorHandling
{
    public class CheckInNotFoundError : Exception
    {
        public CheckInNotFoundError() : base("CheckIn not found.")
        {
        }
    }
}