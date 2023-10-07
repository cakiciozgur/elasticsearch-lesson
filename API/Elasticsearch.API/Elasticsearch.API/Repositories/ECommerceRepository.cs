using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elasticsearch.API.Model.ECommerceModel;
using System.Collections.Immutable;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elasticsearch.API.Repositories
{
    public class ECommerceRepository
    {
        private readonly ElasticsearchClient _client;
        private const string indexName = "kibana_sample_data_ecommerce";

        public ECommerceRepository(ElasticsearchClient client)
        {
            _client = client;
        }

        public async Task<ImmutableList<ECommerce>> TermQuery(string customerFirstName)
        {
            // way 1
            //var result = await _client.SearchAsync<ECommerce>(s =>s.Index(indexName).Query(y => y.Term(t => t.Field("customer_first_name.keyword").Value(customerFirstName))));

            //way 2
            //var result = await _client.SearchAsync<ECommerce>(s =>s.Index(indexName).Query(y => y.Term(t => t.CustomerFirstName.Suffix("keyword"),customerFirstName)));

            //way 3
            var termQuery = new TermQuery("customer_first_name.keyword") { Value = customerFirstName, CaseInsensitive = true };
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(termQuery));


            foreach (var item in result.Hits) item.Source.Id = item.Id;
            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> TermsQuery(List<string> customerFirstNameList)
        {
            List<FieldValue> terms = new List<FieldValue>();
            foreach (var item in customerFirstNameList)
            {
                terms.Add(item);
            }
            //way 1
            //var termsQuery = new TermsQuery()
            //{
            //    Field = "customer_first_name.keyword",
            //    Terms = new TermsQueryField(terms.AsReadOnly()),
            //};

            //var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(termsQuery));

            //way 2
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
            .Size(100)
            .Query(q=> q
            .Terms(t=> t
            .Field(f=> f.CustomerFirstName
            .Suffix("keyword"))
            .Terms(new TermsQueryField(terms.AsReadOnly())))));

            foreach (var item in result.Hits) item.Source.Id = item.Id;
            return result.Documents.ToImmutableList();
        }
    }
}
