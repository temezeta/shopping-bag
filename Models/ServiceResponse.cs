namespace shopping_bag.Models
{
    public class ServiceResponse<T>
    {
        public ServiceResponse(T data)
        {
            Data = data;
        }

        public ServiceResponse(string error)
        {
            Error = error;
        }

        public bool IsSuccess => Data != null;
        public string? Error { get; private set; }
        public T? Data { get; private set; }
    }
}
