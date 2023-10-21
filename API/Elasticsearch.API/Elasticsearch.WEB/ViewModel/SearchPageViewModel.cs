namespace Elasticsearch.WEB.ViewModel
{
    public class SearchPageViewModel
    {
        public long TotalCount { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public long PageLinkCount { get; set; }
        public List<ECommerceViewModel> List { get; set; }
        public ECommerceSearchViewModel SearchViewModel { get; set; }

        public int StartPage()
        {
            return Page - 12 <=0 ? 1 : Page-12;
        }

        public long EndPage()
        {
            return Page + 12 >= PageLinkCount ? PageLinkCount : Page + 12;
        }
        public string CreatePageUrl(HttpRequest request, int page, int pageSize)
        {
            string currentUrl = new Uri($"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}").AbsoluteUri;

            if (currentUrl.Contains("page", StringComparison.OrdinalIgnoreCase))
            {
                currentUrl = currentUrl.Replace($"Page={Page}", $"Page={page}", StringComparison.OrdinalIgnoreCase);
                currentUrl = currentUrl.Replace($"PageSize={PageSize}", $"PageSize={PageSize}", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                currentUrl = $"{currentUrl}?Page={page}";
                currentUrl = $"{currentUrl}&PageSize={PageSize}";
            }

            return currentUrl;
        }
    }
}
