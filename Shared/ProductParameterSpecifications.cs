// في Shared/ProductParameterSpecifications.cs
namespace Shared
{
    public class ProductParameterSpecifications
    {
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public ProductSortOption? Sort { get; set; }
        public string? Search { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int PageIndex { get; set; } = 1;
        private const int MaxPageSize = 50;
        private const int DefaultPageSize = 12;
        private int _pageSize = DefaultPageSize;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }

    public enum ProductSortOption
    {
        NameAsc,
        NameDesc,
        PriceAsc,
        PriceDesc,
        Newest,
        DiscountDesc 
    }
}