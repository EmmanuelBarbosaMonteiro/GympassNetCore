namespace ApiGympass.Services.ErrorHandling
{
    public class CheckInLimitExceededError : Exception
    {
        public CheckInLimitExceededError() : base("CheckIn limit exceeded.")
        {
        }
    }
}