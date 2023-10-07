using Elasticsearch.API.DTOs;
using Elasticsearch.API.Model;
using Elasticsearch.API.Repositories;
using System.Collections.Immutable;

namespace Elasticsearch.API.Services
{
    public class ProductService
    {
        private readonly ProductRepository _productRepository;
        private readonly ILogger _logger;
        public ProductService(ProductRepository repository, ILogger logger)
        {
            this._productRepository = repository;
            _logger = logger;
        }

        public async Task<ResponseDto<ProductDto>> SaveAsync(ProductCreateDto request)
        {
            var response = await _productRepository.SaveAsync(request.CreateProduct());

            if(response == null)
            {
                return ResponseDto<ProductDto>.Fail(new List<string> { "kayıt sırasında hata oluştu!" }, System.Net.HttpStatusCode.ServiceUnavailable);
            }

            return ResponseDto<ProductDto>.Success(response.CreateDto(), System.Net.HttpStatusCode.Created);
        }

        public async Task<ResponseDto<List<ProductDto>>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();
            var productListDto = new List<ProductDto>();

            foreach (var x in products)
            {
                if(x.Feature is null)
                {
                    productListDto.Add(new ProductDto(x.Id, x.Name, x.Price, x.Stock, null));
                    continue;
                }

                productListDto.Add(new ProductDto(x.Id, x.Name, x.Price, x.Stock, new ProductFeatureDto(x.Feature!.Width, x.Feature!.Height, x.Feature!.Color.ToString())));

            }

            return ResponseDto<List<ProductDto>>.Success(productListDto, System.Net.HttpStatusCode.OK);
        }

        public async Task<ResponseDto<ProductDto>> GetByIdAsync(string id)
        {
            var hasProduct = await _productRepository.GetByIdAsync(id);

            if (hasProduct == null)
            {
                return ResponseDto<ProductDto>.Fail("Ürün Bulunamadı", System.Net.HttpStatusCode.NotFound);
            }

            return ResponseDto<ProductDto>.Success(hasProduct.CreateDto(), System.Net.HttpStatusCode.OK);

        }

        public async Task<ResponseDto<bool>> UpdateAsync(ProductUpdateDto updateProduct)
        {
            var isSuccess = await _productRepository.UpdateAsync(updateProduct);

            if (!isSuccess)
            {
                return ResponseDto<bool>.Fail("Update sırasında bir hata meydana geldi", System.Net.HttpStatusCode.InternalServerError);
            }

            return ResponseDto<bool>.Success(true,System.Net.HttpStatusCode.NoContent);

        }

        public async Task<ResponseDto<bool>> DeleteAsync(string id)
        {
            var res = await _productRepository.DeleteAsync(id);

            if (!res.IsValid && res.Result == Nest.Result.NotFound)
            {
                return ResponseDto<bool>.Fail("Silmek istediğiniz ürün bulunamadı!", System.Net.HttpStatusCode.NotFound);
            }

            if (!res.IsValid)
            {
                _logger.LogError(res.OriginalException, res.ServerError.Error.ToString());

                return ResponseDto<bool>.Fail("Product silme sırasında bir hata meydana geldi", System.Net.HttpStatusCode.NotFound);
            }

            return ResponseDto<bool>.Success(true, System.Net.HttpStatusCode.NoContent);
        }
    }
}
