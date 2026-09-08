namespace Platform.Core.Exceptions
{
    public static class AppException
    {
        public static ApplicationException Throw(string exceptionMessage, string innerException)
        {
            throw new ApplicationException(exceptionMessage, new Exception(innerException));
        }
    }
}
