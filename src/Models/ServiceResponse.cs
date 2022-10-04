namespace shopping_bag.Models
{
    public class ServiceResponse<T>
    {
        public ServiceResponse(T data)
        {
            Data = data;
            IsSuccess = true;
        }

        public ServiceResponse(string error)
        {
            Error = error;
            IsSuccess = false;
        }

        public bool IsSuccess { get; private set; }
        public string? Error { get; private set; }
        public T? Data { get; private set; }
    }
}
