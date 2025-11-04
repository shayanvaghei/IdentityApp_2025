using API.Utility;

namespace API.DTOs.Pagination
{
    public class BaseParams
    {
        private const int MaxPageSize = SD.MaxPageSize;
        private const int MinPageSize = SD.MinPageSize;
        private int _pageSize;
        private string _sortBy;
        private string _searchBy;

        public BaseParams()
        {
            PageSize = SD.MinPageSize;
            PageNumber = 1;
        }

        public int PageNumber { get; set; }
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value < MinPageSize ? MinPageSize : value;
        }
        public string SortBy
        {
            get => _sortBy;
            set => _sortBy = value.TrimTrailing();
        }
        public string SearchBy
        {
            get => _searchBy;
            set => _searchBy = value.TrimTrailing();
        }
    }
}
