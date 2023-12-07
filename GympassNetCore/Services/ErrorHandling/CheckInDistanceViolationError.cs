namespace ApiGympass.Services.ErrorHandling
{
    public class CheckInDistanceViolationError : Exception
    {
        public CheckInDistanceViolationError() : base("Check-in distance violation")
        {
        }
    }
}