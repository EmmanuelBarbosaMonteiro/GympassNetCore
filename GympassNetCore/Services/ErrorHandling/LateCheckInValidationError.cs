namespace ApiGympass.Services.ErrorHandling
{
    public class LateCheckInValidationError : Exception
    {
        public LateCheckInValidationError() : base("The check-in can only be validated until 20 minutes of its creation.")
        {
        }
    }
}