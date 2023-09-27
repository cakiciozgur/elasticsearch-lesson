using Elasticsearch.API.DTOs;
using Elasticsearch.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Elasticsearch.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : BaseController
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            this._productService = productService;
        }
        [HttpPost]
        public async Task<IActionResult> Save(ProductCreateDto requst)
        {
            return CreateActionResult(await _productService.SaveAsync(requst));
        }
    }
}
