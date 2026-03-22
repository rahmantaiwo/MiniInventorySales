namespace MiniInventorySales.Application.Common
{
    public class BaseResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();

        public static BaseResponse<T> IsSuccessful(T data, string message = "")
        {
            return new BaseResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }


        public static BaseResponse<T> Failure(string message, params string[] errors)
        {
            return new BaseResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors.ToList()
            };
        }
    }
}
