namespace ApiGympass.Utils
{
    public class ApiResponse<T>
    {
        public T Codigo { get; set; }
        public string Msg { get; set; }
        public bool HasError { get; set; }

        public ApiResponse(T codigo, string msg, bool hasError)
        {
            Codigo = codigo;
            Msg = msg;
            HasError = hasError;
        }
    }
}