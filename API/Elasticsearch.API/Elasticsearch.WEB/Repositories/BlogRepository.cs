using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elasticsearch.WEB.Models;

namespace Elasticsearch.WEB.Repositories
{
    public class BlogRepository
    {
        private readonly ElasticsearchClient _elasticsearchClient;
        private const string IndexName = "blog";

        public BlogRepository(ElasticsearchClient elasticsearchClient)
        {
            _elasticsearchClient = elasticsearchClient;
        }

        public async Task<Blog?> SaveAsync(Blog newBlog)
        {
            newBlog.Created = DateTime.Now;

            var response = await _elasticsearchClient.IndexAsync(newBlog, x => x.Index(IndexName).Id(Guid.NewGuid().ToString()));

            if (!response.IsValidResponse) // fast fail
                return null;

            newBlog.Id = response.Id;

            return newBlog;
        }

        public async Task<List<Blog>> SearchAsync(string searchText)
        { /* 
           
            V1

            // title
            // content

            //title => fulltext
            //content => fulltext

            //should => term 1 or term2 or term3
            var result = await _elasticsearchClient.SearchAsync<Blog>(s => s.Index(IndexName).Size(100)
            .Query(q => q
            .Bool(b => b
                .Should(s => s //should => term 1 or term2 or term3
                    .Match(m => m
                        .Field(f => f.Content)
                        .Query(searchText)),   // match bitiminde nokta ile match bool prefix dersek şu yapıda arama yapıyor ( (term1 and term2) or term3)
                    s=> s.MatchBoolPrefix(p => p // shoul params aldığı için lambda ile ikinci şartı ekliyoruz ve 2 şartı or olarak arıyor.
                        .Field(f => f.Title)
                        .Query(searchText))))));

            */


            // V2
            List<Action<QueryDescriptor<Blog>>> listQuery = new();


            Action<QueryDescriptor<Blog>> matchAll = (q) => q.MatchAll();

            Action<QueryDescriptor<Blog>> matchContent = (q) => q.Match(m => m
                                                            .Field(f => f.Content)
                                                            .Query(searchText));

            Action<QueryDescriptor<Blog>> matchBoolPrefixTitle = (q) => q.MatchBoolPrefix(p => p
                                                             .Field(f => f.Title)
                                                             .Query(searchText));

            Action<QueryDescriptor<Blog>> tagTermQuery = (q) => q.Term(t=> t.Field(f=> f.Tags).Value(searchText));


            if (string.IsNullOrWhiteSpace(searchText))
            {
                listQuery.Add(matchAll);
            }
            else
            {
                listQuery.Add(matchContent);
                listQuery.Add(matchBoolPrefixTitle);
                listQuery.Add(tagTermQuery);
            }


            var result = await _elasticsearchClient.SearchAsync<Blog>(s => s.Index(IndexName).Size(100)
            .Query(q => q
            .Bool(b => b
                .Should(listQuery.ToArray()))));

            foreach (var item in result.Hits) item.Source.Id = item.Id;

            return result.Documents.ToList();
        }
    }
}
