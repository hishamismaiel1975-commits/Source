namespace Platform.Lib.Core.DTOs
{
    public sealed class Pagination<T> where T : class
    {
        public Pagination() { }
        public Pagination(int pageIndex, int pageSize, int totalPages, int count, IReadOnlyCollection<T> data)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.TotalPages = totalPages;
            this.Count = count;
            this.Data = data;
        }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
        public int TotalPages { get; set; }

        public IReadOnlyCollection<T> Data { get; set; }

    }
}
