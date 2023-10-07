using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Search;
using Elasticsearch.API.Model.ECommerceModel;

namespace Elasticsearch.API.Extensions
{
    public static class Document
    {
        public static SearchResponse<ECommerce> MoveDocumentId(SearchResponse<ECommerce> ecommerceDataList)
        {
            foreach (var item in ecommerceDataList.Hits) item.Source.Id = item.Id;

            return ecommerceDataList;
        }
    }
}
