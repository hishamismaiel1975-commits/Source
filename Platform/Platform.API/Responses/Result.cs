namespace Platform.API.Responses
{
    public sealed class Result<T>
    {
        public bool IsSuccess { get; init; }
        public string? ErrorMessage { get; init; }
        public T? Value { get; init; }

        public static Result<T> Success(T value)
        {
            return new Result<T>
            {
                IsSuccess = true,
                Value = value
            };
        }

        public static Result<T> Failure(string error)
        {
            return new Result<T>
            {
                IsSuccess = false,
                ErrorMessage = error
            };
        }
    }
}
