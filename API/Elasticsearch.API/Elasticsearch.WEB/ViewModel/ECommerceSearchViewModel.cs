using System.ComponentModel.DataAnnotations;

namespace Elasticsearch.WEB.ViewModel
{
    public class ECommerceSearchViewModel
    {
        [Display(Name ="Category")]
        public string Category { get; set; } = null!;

        [Display(Name = "Gender")]
        public string Gender { get; set; } = null!;

        [Display(Name = "Order Date Start")]
        [DataType(DataType.Date)]
        public DateTime? OrderDateStart { get; set; }

        [Display(Name = "Order Date End")]
        [DataType(DataType.Date)]
        public DateTime? OrderDateEnd { get; set; }

        [Display(Name = "CustomerFullName")]
        public string CustomerFullName { get; set; } = null!;
    }
}
