using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Packaging;

namespace VollMed.Web.Dtos
{
    [Serializable]
    public class PaginatedList<T> : IPaginatedList where T : class
    {
        [JsonProperty]
        public IList<T> Items { get; set; } = [];
        [JsonProperty]
        public int TotalItemCount { get; set; }
        [JsonProperty]
        public int PageNumber { get; set; }
        [JsonProperty]
        public int PageSize { get; set; }
        [JsonProperty]
        public int TotalPages { get; set; }
        [JsonProperty]
        public bool HasPreviousPage => PageNumber > 1;
        [JsonProperty]
        public bool HasNextPage => PageNumber < TotalPages;

        public PaginatedList()
        {
        }

        public PaginatedList(List<T> items, int totalItemCount, int pageNumber, int pageSize)
        {
            TotalItemCount = totalItemCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(totalItemCount / (double)pageSize);

            Items.AddRange(items);
        }

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageNumber)
        {
            var totalItemCount = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageNumber).Take(pageNumber).ToListAsync();
            return new PaginatedList<T>(items, totalItemCount, pageIndex, pageNumber);
        }
    }
}


