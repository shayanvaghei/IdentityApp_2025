using System.Collections.Generic;

namespace API.DTOs.Pagination
{
    public class PaginatedResult<T> where T : class
    {
        public PaginatedResult(IReadOnlyList<T> items, int totalItemsCount, int pageNumber, int pageSize, int totalPages)
        {
            Items = items;
            TotalItemsCount = totalItemsCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = totalPages;
        }

        public IReadOnlyList<T> Items { get; set; }
        public int TotalItemsCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
