namespace PermissionsApp.Application.DTOs
{
    public class ResultDto<T>
    {
        public T? Data { get; set; }
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public static ResultDto<T> Success(T data)
        {
            return new ResultDto<T>
            {
                Data = data,
                IsError = false
            };
        }

        public static ResultDto<T> Failure(string errorMessage)
        {
            return new ResultDto<T>
            {
                IsError = true,
                ErrorMessage = errorMessage
            };
        }
    }
}