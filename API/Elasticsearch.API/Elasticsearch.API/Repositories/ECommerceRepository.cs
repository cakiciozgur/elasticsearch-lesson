using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elasticsearch.API.Model.ECommerceModel;
using System.Collections.Immutable;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using Elasticsearch.API.Extensions;

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

        public async Task<ImmutableList<ECommerce>> TermQueryAsync(string customerFirstName) // aranacak kelime birebir geçmesi gerekir o yüzden keyword'de aranır
        {
            // way 1
            //var result = await _client.SearchAsync<ECommerce>(s =>s.Index(indexName).Query(y => y.Term(t => t.Field("customer_first_name.keyword").Value(customerFirstName))));

            //way 2
            //var result = await _client.SearchAsync<ECommerce>(s =>s.Index(indexName).Query(y => y.Term(t => t.CustomerFirstName.Suffix("keyword"),customerFirstName)));

            //way 3
            var termQuery = new TermQuery("customer_first_name.keyword") { Value = customerFirstName, CaseInsensitive = true };
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(termQuery));


            var response = Document.MoveDocumentId(result);
            return response.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> TermsQueryAsync(List<string> customerFirstNameList) // aranacak kelimeler birebir geçmesi gerekir o yüzden keyword'de aranır
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

            var response = Document.MoveDocumentId(result);
            return response.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> PrefixQueryAsync(string customerFullName) // Örneğin Sultan Al Mayer, Sultan Al Brat gibi veriler var ise ve ben Sultan diye aratırsam tüm veriler gelir.
        {
            //way 1
            var result = await _client.SearchAsync<ECommerce>(s =>s.Index(indexName)
            .Query(y => y
            .Prefix(p => p
            .Field(f=> f.CustomerFullName
            .Suffix("keyword"))
            .Value(customerFullName))));

            var response = Document.MoveDocumentId(result);
            return response.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> RangeQueryAsync(double fromPrice, double toPrice) // belirli bir fiyat aralığındakileri getirir
        {
            //way 1
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
            .Query(q => q
            .Range(r => r
            .NumberRange(nr => nr
            .Field(f => f.TaxfulTotalPrice)
            .Gte(fromPrice)
            .Lte(toPrice)))));

            var response = Document.MoveDocumentId(result);
            return response.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MatchAllQueryAsync() // o index içerisindeki tüm datayı getirir
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
            .Size(25)
            .Query(q=> q
            .MatchAll()));

            var response = Document.MoveDocumentId(result);
            return response.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> PaginationQueryAsync(int page,int pageSize) // sayfalama kurgusu
        {
            // page 0 gelirse hata verir business katmanında kontrolü yapılmalı
            var pageFrom = (page-1) * pageSize;
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
            .Size(pageSize).From(pageFrom)
            .Query(q => q
            .MatchAll()));

            var response = Document.MoveDocumentId(result);
            return response.Documents.ToImmutableList();
        }
        public async Task<ImmutableList<ECommerce>> WildCardQueryAsync(string customerFullName) // örneğin 1=> "mav*" sorgusu mav ile başlayanları getirir 2=> "*ze" sorgusu ze ile bitenleri getirir 3=> "d?ta*" d ile başlayan ikinci karakteri öenmli değil joker olacak ta ile devam edenleri getirir
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
            .Query(q => q
            .Wildcard(w=> w
            .Field(f=> f.CustomerFullName.Suffix("keyword"))
            .Wildcard(customerFullName))));

            var response = Document.MoveDocumentId(result);
            return response.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> FuzzyQueryAsync(string customerName) // örneğin lacivert kelimesi içerisinde geçen farklı harflerin kaç tanesi yanlış olabilir fuzziness = 1 ise lacivrt ile bulabilir. fuzziness = 2 ise lacvrt olarak arattığımızda bulabilir. ek olarak fuzziness otomatik bırakılabilir. 
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
            .Query(q => q
            .Fuzzy(fu => fu
            .Field(f => f.CustomerFirstName.Suffix("keyword")).Value(customerName)
            .Fuzziness(new Fuzziness(1)))).Sort(sort=> sort
            .Field(f=> f.TaxfulTotalPrice,new FieldSort() { Order = SortOrder.Asc })));

            var response = Document.MoveDocumentId(result);
            return response.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MatchQueryFullTextAsync(string category) // örneğin Women's Clothing ile arama yaparsak Women's OR Clothing şeklinde arar. Ek olarak operator vererek bunu değiştirebiliriz.
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Size(100)
            .Query(q => q
            .Match(m=> m
            .Field(f=> f.Category).Query(category).Operator(Operator.And))));

            var response = Document.MoveDocumentId(result);
            return response.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MatchBoolPrefixQueryFullTextAsync(string customerFullName) //Sultan Al Me ile arama yaparsak Sultan OR Al ve Me ile başlayanlar sonuncu kelime prefix gibi davranır
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Size(100)
            .Query(q => q
            .MatchBoolPrefix(m=> m
            .Field(f=> f.CustomerFullName)
            .Query(customerFullName))));

            var response = Document.MoveDocumentId(result);
            return response.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MatchPhraseQueryFullTextAsync(string customerFullName) //Sultan Al Meyer ile arama yaparsak birebir tutması gerekir. Sultan Al ile arama yaparsak bu öbeğin sonunda kelimeler olabilir örneğin Sultan Al Prat,Sultan Al Brey
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Size(100)
            .Query(q => q
            .MatchPhrase(m => m
            .Field(f => f.CustomerFullName)
            .Query(customerFullName))));

            var response = Document.MoveDocumentId(result);
            return response.Documents.ToImmutableList();
        }
    }
}
