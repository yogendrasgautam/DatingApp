namespace DatingApp.Api.Helpers
{
    public class PaginationHeader
    {
         public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalCount { get; set; }
        public PaginationHeader(int currentPage, int totalPages, int itemsPerPage, int totalCount)
        {
            this.CurrentPage = currentPage;
            this.TotalPages = totalPages;
            this.ItemsPerPage = itemsPerPage;
            this.TotalCount = totalCount;
        }
    }
}