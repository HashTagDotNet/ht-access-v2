namespace HT.Common.ApiMessaging
{
    /// <summary>
    /// Used on paging and virtual scrolling data calls.  Consider using ODATA endpoint instead, if possible
    /// </summary>
    public class ApiPageRequest
    {
        public int PageSize { get; set; }
        public bool RequestTotalRows { get; set; }
        public int PageNumber { get; set; }
        public string[] SortColumns { get; set; }
    }
}
