using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elasticsearch.WEB.Models;
using Elasticsearch.WEB.ViewModel;

namespace Elasticsearch.WEB.Repositories
{
    public class ECommerceRepository
    {
        private readonly ElasticsearchClient _elasticsearchClient;
        private const string indexName = "kibana_sample_data_ecommerce";

        public ECommerceRepository(ElasticsearchClient client)
        {
            _elasticsearchClient = client;
        }


        public async Task<(List<ECommerce> list, long count)> SearchAsync(ECommerceSearchViewModel searchViewModel, int page, int pageSize)
        {

            // totalCount 100
            //page 1 pagesize=10 1-10,
            //page 2 pagesize=10 11-20,


            List<Action<QueryDescriptor<ECommerce>>> listQuery = new();

            if(searchViewModel is null)
            {
                listQuery.Add(g => g.MatchAll());

                return await CalculateResultSet(page, pageSize, listQuery);
            }

            if (!string.IsNullOrWhiteSpace(searchViewModel.Category))
            {
                Action<QueryDescriptor<ECommerce>> query = (q) => q.Match(m => m
                                                .Field(f => f.Category)
                                                .Query(searchViewModel.Category));
                listQuery.Add(query);
            }


            if (!string.IsNullOrWhiteSpace(searchViewModel.CustomerFullName))
            {
                Action<QueryDescriptor<ECommerce>> query = (q) => q.Match(m => m
                                                .Field(f => f.CustomerFullName)
                                                .Query(searchViewModel.CustomerFullName));
                listQuery.Add(query);
            }

            if (searchViewModel.OrderDateStart.HasValue)
            {
                Action<QueryDescriptor<ECommerce>> query = (q) => q.Range(r => r
                                                                    .DateRange(dr => dr
                                                                    .Field(f => f.OrderDate)
                                                                    .Gte(searchViewModel.OrderDateStart)));
                listQuery.Add(query);
            }

            if (searchViewModel.OrderDateEnd.HasValue)
            {
                Action<QueryDescriptor<ECommerce>> query = (q) => q.Range(r => r
                                                                    .DateRange(dr => dr
                                                                    .Field(f => f.OrderDate)
                                                                    .Lte(searchViewModel.OrderDateEnd)));
                listQuery.Add(query);
            }

            if (!string.IsNullOrWhiteSpace(searchViewModel.Gender))
            {
                Action<QueryDescriptor<ECommerce>> query = (q) => q.Term(t=> t.Field(f=> f.Gender).Value(searchViewModel.Gender).CaseInsensitive());
                listQuery.Add(query);
            }

            if (!listQuery.Any())
            {
                listQuery.Add(g => g.MatchAll());
            }

            return await CalculateResultSet(page, pageSize, listQuery);

        }

        private async Task<(List<ECommerce> list, long count)> CalculateResultSet(int page, int pageSize, List<Action<QueryDescriptor<ECommerce>>> listQuery)
        {
            var pageFrom = (page - 1) * pageSize;

            var result = await _elasticsearchClient.SearchAsync<ECommerce>(s => s.Index(indexName).Size(pageSize).From(pageFrom)
            .Query(q => q
                .Bool(b => b
                    .Must(listQuery.ToArray()))));

            foreach (var item in result.Hits) item.Source.Id = item.Id;

            return (result.Documents.ToList(), result.Total);
        }
    }
}
