namespace Luger.Api.Endpoints.Models
{
    public class ApiResponse<T> where T : class
    {
        public ApiResponse(T? data) : this(data, null) { }

        public ApiResponse(LugerError err) : this(null, err) { }

        public ApiResponse(T? data, LugerError error)
        {
            Data = data;
            Error = error;
        }

        public T? Data { get; private set; }
        public LugerError? Error { get; private set; }
    }

    public static class ApiResponse
    {
        public static ApiResponse<T> FromData<T>(T data) where T : class
        {
            return new(data);
        }

        public static ApiResponse<object> FromError(LugerError err)
        {
            return new(err);
        }

        //public static ApiResponse<object> FromMemzException(MemzException ex)
        //{
        //    return new(null, LugerError.FromMemzException(ex));
        //}
    }
}
