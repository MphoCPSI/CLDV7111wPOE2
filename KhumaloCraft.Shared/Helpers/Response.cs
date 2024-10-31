public class Response<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }

    public static Response<T> SuccessResponse(T data, string message = "Successful")
    {
        return new Response<T> { Success = true, Message = message, Data = data };
    }

    public static Response<T> ErrorResponse(string message)
    {
        return new Response<T> { Success = false, Message = message };
    }
}
