namespace ApiGympass.Utils
{
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; }
        public T Entity { get; }
        public string ErrorMessage { get; }

        public ServiceResult(bool isSuccess, T entity, string errorMessage)
        {
            IsSuccess = isSuccess;
            Entity = entity;
            ErrorMessage = errorMessage;
        }
    }
}