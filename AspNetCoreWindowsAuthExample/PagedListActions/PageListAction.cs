namespace AspNetCoreWindowsAuthExample.PagedListActions
{
    public abstract class PageListAction
    {
        public PageListAction(int pageSize, string sortOrder, string currentFilter, string searchString, int? page)
        {
            this.PageSize = pageSize;
            this.PageNumber = 1;
            this.SortOrder = sortOrder;
            this.CurrentSort = sortOrder;
            this.CurrentFilter = currentFilter;
            this.SearchString = searchString;
            this.Page = page;
        }

        public string SortOrder { get; set; }
        public string CurrentSort { get; set; }
        public string CurrentFilter { get; set; }
        public string SearchString { get; set; }
        public string IncludedProperties { get; set; }
        public int? Page { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}