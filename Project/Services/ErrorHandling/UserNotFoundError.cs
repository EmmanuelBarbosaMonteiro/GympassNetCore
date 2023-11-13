namespace ApiGympass.Services.ErrorHandling
{
    public class UserNotFoundError : Exception
    {
        public UserNotFoundError() : base("User not found.")
        {
        }
    }
}