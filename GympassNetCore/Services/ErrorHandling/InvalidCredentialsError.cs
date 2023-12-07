namespace Project.Services.ErrorHandling
{
    public class InvalidCredentialsError : Exception
    {
        public InvalidCredentialsError() : base("Invalid credentials.")
        {
        }
    }
}