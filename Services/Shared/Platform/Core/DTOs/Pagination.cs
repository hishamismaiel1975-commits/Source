namespace Platform.Lib.Core.DTOs
{
    public sealed class Pagination<T> where T : class
    {
        public Pagination() { }
        public Pagination(int pageIndex, int pageSize, int totalPages, int count, IReadOnlyCollection<T> data)
        {
            this.pageIndex = pageIndex;
            this.pageSize = pageSize;
            this.totalPages = totalPages;
            this.count = count;
            this.data = data;
        }

        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public int count { get; set; }
        public int totalPages { get; set; }

        public IReadOnlyCollection<T> data { get; set; }

    }
}
