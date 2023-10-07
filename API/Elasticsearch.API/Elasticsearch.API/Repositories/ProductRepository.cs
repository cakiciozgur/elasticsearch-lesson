using Elasticsearch.API.DTOs;
using Elasticsearch.API.Model;
using Nest;
using System.Collections.Immutable;

namespace Elasticsearch.API.Repositories
{
    public class ProductRepository
    { 
        private readonly ElasticClient _client;
        private const string indexName = "products";
        public ProductRepository(ElasticClient client)
        {
            _client = client;
        }

        public async Task<Product?> SaveAsync(Product product)
        {
            product.Created = DateTime.Now;

            var response = await _client.IndexAsync(product,x=> x.Index(indexName).Id(Guid.NewGuid().ToString()));

            if(!response.IsValid) // fast fail
                return null;

            product.Id = response.Id;

            return product;
        }

        public async Task<ImmutableList<Product>> GetAllAsync()
        {
            var result = await _client.SearchAsync<Product>(s => s
            .Index(indexName)
            .Query(q=> q.MatchAll()));

            foreach (var item in result.Hits) item.Source.Id = item.Id;

            return result.Documents.ToImmutableList();
        }

        public async Task<Product> GetByIdAsync(string id)
        {
            var result = await _client.GetAsync<Product>(id, s => s
            .Index(indexName));

            if (!result.IsValid)
                return null;

            result.Source.Id = result.Id;
            return result.Source;
        }

        public async Task<bool> UpdateAsync(ProductUpdateDto updateProduct)
        {
            var result = await _client.UpdateAsync<Product,ProductUpdateDto>(updateProduct.id,x=> x.Index(indexName).Doc(updateProduct));

            return result.IsValid;

        }
        /// <summary>
        /// Hata yönetimi için bu method ele alınmıştır!
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<DeleteResponse> DeleteAsync(string id)
        {
            var result = await _client.DeleteAsync<Product>(id, s => s.Index(indexName));

            return result;

        }
    }
}
