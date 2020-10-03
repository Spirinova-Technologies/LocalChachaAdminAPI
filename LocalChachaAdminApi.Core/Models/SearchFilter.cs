namespace LocalChachaAdminApi.Core.Models
{
    public class SearchFilter
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public string OrderBy { get; set; }
    }
}