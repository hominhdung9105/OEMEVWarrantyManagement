using System.Linq.Expressions;

namespace OEMEVWarrantyManagement.Share.Models.Pagination
{
    public class PaginationRequest
    {
        private int _page = 0;
        private int _size = 20;

        // Page index starts at 0
        public int Page
        {
            get => _page;
            set => _page = value < 0 ? 0 : value;
        }

        // Page size default 20, clamp to [1, 100]
        public int Size
        {
            get => _size;
            set
            {
                if (value <= 0) _size = 20;
                else if (value > 100) _size = 100;
                else _size = value;
            }
        }
    }

    public class PagedResult<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public long TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    }
}
