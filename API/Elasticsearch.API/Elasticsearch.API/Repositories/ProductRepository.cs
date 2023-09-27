using Elasticsearch.API.Model;
using Nest;

namespace Elasticsearch.API.Repositories
{
    public class ProductRepository
    { 
        private readonly ElasticClient _client;
        public ProductRepository(ElasticClient client)
        {
            _client = client;
        }

        public async Task<Product?> SaveAsync(Product product)
        {
            product.Created = DateTime.Now;

            var response = await _client.IndexAsync(product,x=> x.Index("products").Id(Guid.NewGuid().ToString()));

            if(!response.IsValid) // fast fail
                return null;

            product.Id = response.Id;

            return product;
        }
    }
}
