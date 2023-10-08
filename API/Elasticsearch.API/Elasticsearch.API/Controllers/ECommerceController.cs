using Elasticsearch.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Elasticsearch.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ECommerceController : ControllerBase
    {
        private readonly ECommerceRepository _eCommerceRepository;

        public ECommerceController(ECommerceRepository eCommerceRepository)
        {
            _eCommerceRepository = eCommerceRepository;
        }

        [HttpGet]
        public async Task<IActionResult> TermQuery(string customerFirstName)
        {
            var response = await _eCommerceRepository.TermQueryAsync(customerFirstName);

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> TermsQuery(List<string> customerFirstNameList)
        {
            var response = await _eCommerceRepository.TermsQueryAsync(customerFirstNameList);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> PrefixQuery(string customerFullName)
        {
            var response = await _eCommerceRepository.PrefixQueryAsync(customerFullName);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> RangeQuery(double fromPrice, double toPrice)
        {
            var response = await _eCommerceRepository.RangeQueryAsync(fromPrice,toPrice);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> MatchAllQuery()
        {
            var response = await _eCommerceRepository.MatchAllQueryAsync();

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> PaginationQuery(int page=1, int pageSize=3)
        {
            var response = await _eCommerceRepository.PaginationQueryAsync(page,pageSize);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> WildCardQuery(string customerFullName)
        {
            var response = await _eCommerceRepository.WildCardQueryAsync(customerFullName);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> FuzzyQuery(string customerName)
        {
            var response = await _eCommerceRepository.FuzzyQueryAsync(customerName);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> MatchQueryFullTextAsync(string category)
        {
            var response = await _eCommerceRepository.MatchQueryFullTextAsync(category);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> MultiMatchQueryFullTextAsync(string name)
        {
            var response = await _eCommerceRepository.MultiMatchQueryFullTextAsync(name);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> MatchBoolPrefixQueryFullText(string customerFullName)
        {
            var response = await _eCommerceRepository.MatchBoolPrefixQueryFullTextAsync(customerFullName);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> MatchPhraseQueryFullText(string customerFullName)
        {
            var response = await _eCommerceRepository.MatchPhraseQueryFullTextAsync(customerFullName);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> CompoundQueryExampleOne(string cityName, double taxtfulTotalPrice, string category, string manufactur)
        {
            var response = await _eCommerceRepository.CompoundQueryExampleOneAsync(cityName, taxtfulTotalPrice, category, manufactur);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> CompoundQueryExampleTwo(string customerFullName)
        {
            var response = await _eCommerceRepository.CompoundQueryExampleTwoAsync(customerFullName);

            return Ok(response);
        }
    }
}
