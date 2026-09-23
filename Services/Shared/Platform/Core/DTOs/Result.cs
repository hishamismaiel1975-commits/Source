namespace Platform.Lib.Core.DTOs
{
    public sealed class Result<T>
    {
        public bool isSuccess { get; init; }
        public IList<string>? errorMessages { get; init; }
        public T? data { get; init; }

        public static Result<T> Success(T value)
        {
            return new Result<T>
            {
                isSuccess = true,
                data = value
            };
        }

        public static Result<T> Success()
        {
            return new Result<T>
            {
                isSuccess = true,
            };
        }

        public static Result<T> Failure(string error)
        {
            return new Result<T>
            {
                isSuccess = false,
                errorMessages = new List<string> { error }
            };
        }

        public static Result<T> Failure(IList<string> errors)
        {
            return new Result<T>
            {
                isSuccess = false,
                errorMessages = errors
            };
        }



    }

}
