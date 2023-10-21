using Elasticsearch.WEB.Repositories;
using Elasticsearch.WEB.ViewModel;

namespace Elasticsearch.WEB.Services
{
    public class ECommerceService
    {
        private readonly ECommerceRepository _eCommerceRepository;
        public ECommerceService(ECommerceRepository eCommerceRepository)
        {
            _eCommerceRepository = eCommerceRepository;
        }

        public async Task<(List<ECommerceViewModel>,long totalCount, long totalPageLinkCount)> SearchAsync(ECommerceSearchViewModel model, int page, int pageSize)
        {
            // list döneceğiz
            // totalCount döneceğiz
            // 1 2 3 4 5 6 7
            // 78 % 10 kalan 8
            // 7+1 sayfa göster 


            var (list,totalCount) = await _eCommerceRepository.SearchAsync(model, page, pageSize);

            var pageLinkCountCalculate = totalCount % pageSize;
            long pageLinkCount = 0;

            if(pageLinkCountCalculate == 0)
            {
                pageLinkCount = totalCount/pageSize;
            }
            else
            {
                pageLinkCount = (totalCount / pageSize) +1;
            }

            var eCommerceList = list.Select(x => new ECommerceViewModel
            {
                Category = string.Join(",", x.Category),
                CustomerFullName = x.CustomerFullName,
                CustomerFirstName = x.CustomerFirstName,
                OrderDate = x.OrderDate.ToShortDateString(),
                Gender = x.Gender.ToLower(),
                Id = x.Id,
                OrderId = x.OrderId,
                TaxfulTotalPrice = x.TaxfulTotalPrice,
            }).ToList();

            return (eCommerceList, totalCount, pageLinkCount);

        }
    }
}
