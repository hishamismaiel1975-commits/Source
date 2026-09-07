namespace Platform.API.Responses
{
    public sealed class Result<T>
    {
        public bool IsSuccess { get; init; }
        public IList<string>? ErrorMessages { get; init; }
        public T? data { get; init; }

        public static Result<T> Success(T value)
        {
            return new Result<T>
            {
                IsSuccess = true,
                data = value
            };
        }

        public static Result<T> Success()
        {
            return new Result<T>
            {
                IsSuccess = true,
            };
        }

        public static Result<T> Failure(string error)
        {
            return new Result<T>
            {
                IsSuccess = false,
                ErrorMessages = new List<string> { error }
            };
        }

        public static Result<T> Failure(IList<string> errors)
        {
            return new Result<T>
            {
                IsSuccess = false,
                ErrorMessages = errors
            };
        }



    }

}
