namespace ApiGympass.Services.ErrorHandling
{
    public class CheckInLimitExceeded : Exception
    {
        public CheckInLimitExceeded() : base("CheckIn limit exceeded.")
        {
        }
    }
}