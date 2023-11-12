namespace ApiGympass.Services.ErrorHandling
{
    public class UserAlreadyExistsError : Exception
    {
        public UserAlreadyExistsError() : base("User already exists.")
        {
        }
    }
}