namespace VollMed.Web.Dtos
{
    public interface IPaginatedList
    {
        int TotalItemCount { get; set; }
        int PageNumber { get; set; }
        int PageSize { get; set; }
        int TotalPages { get; set; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
    }
}


