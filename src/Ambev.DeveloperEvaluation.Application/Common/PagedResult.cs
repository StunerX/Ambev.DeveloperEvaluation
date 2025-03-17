namespace Ambev.DeveloperEvaluation.Application.Common
{
    /// <summary>
    /// Represents a paginated result of any query.
    /// </summary>
    /// <typeparam name="T">The type of items returned.</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// The current page number (1-based index).
        /// </summary>
        public int CurrentPage { get; private set; }

        /// <summary>
        /// The number of items per page.
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// The total number of items in the data source.
        /// </summary>
        public int TotalItems { get; private set; }

        /// <summary>
        /// The total number of pages.
        /// </summary>
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

        /// <summary>
        /// Indicates whether there is a previous page.
        /// </summary>
        public bool HasPreviousPage => CurrentPage > 1;

        /// <summary>
        /// Indicates whether there is a next page.
        /// </summary>
        public bool HasNextPage => CurrentPage < TotalPages;

        /// <summary>
        /// The list of items in the current page.
        /// </summary>
        public IReadOnlyList<T> Items { get; private set; }

        private PagedResult(List<T> items, int currentPage, int pageSize, int totalItems)
        {
            Items = items;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalItems = totalItems;
        }

        /// <summary>
        /// Creates a paginated result.
        /// </summary>
        /// <param name="items">The items returned for the current page.</param>
        /// <param name="page">Current page.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="totalItems">Total items count.</param>
        /// <returns></returns>
        public static PagedResult<T> Create(List<T> items, int page, int pageSize, int totalItems)
        {
            return new PagedResult<T>(items, page, pageSize, totalItems);
        }
    }
}