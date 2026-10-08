namespace Platform.Lib.Services.File
{
    public record FileDownloadResult(
        Stream Content,
        string ContentType,
        string FileName);
}
