namespace Services.Specifications
{
    public class ProductWithCategorySpecifications : Specifications<Product>
    {
        public ProductWithCategorySpecifications(int id) : base(p => p.Id == id)
        {
            AddInclude(Product => Product.ProductCategory);
            AddInclude(Product => Product.Vendor);
        }
        public ProductWithCategorySpecifications(Guid vendorId) : base(p => p.VendorId == vendorId)
        {
            AddInclude(p => p.ProductCategory);
            AddInclude(p => p.Vendor);
        }
        public ProductWithCategorySpecifications(ProductParameterSpecifications productParameter)
            : base(Product =>
                (!productParameter.CategoryId.HasValue || Product.CategoryId == productParameter.CategoryId.Value) &&
                (string.IsNullOrEmpty(productParameter.Search) ||
                 Product.Name.ToLower().Contains(productParameter.Search.ToLower()) ||
                 Product.Description.ToLower().Contains(productParameter.Search.ToLower()) ||
                 Product.ProductCategory.Name.ToLower().Contains(productParameter.Search.ToLower())) &&
                (!productParameter.MinPrice.HasValue ||
                 (Product.DiscountPrice ?? Product.Price) >= productParameter.MinPrice.Value) &&
                (!productParameter.MaxPrice.HasValue ||
                 (Product.DiscountPrice ?? Product.Price) <= productParameter.MaxPrice.Value))
        {
            AddInclude(Product => Product.ProductCategory);
            AddInclude(Product => Product.Vendor);

            if (productParameter.Sort is not null)
            {
                switch (productParameter.Sort)
                {
                    case ProductSortOption.NameDesc:
                        SetOrderByDescending(Product => Product.Name);
                        break;
                    case ProductSortOption.PriceAsc:
                        SetOrderBy(Product => Product.DiscountPrice ?? Product.Price);
                        break;
                    case ProductSortOption.PriceDesc:
                        SetOrderByDescending(Product => Product.DiscountPrice ?? Product.Price);
                        break;
                    case ProductSortOption.Newest:
                        SetOrderByDescending(Product => Product.Id);
                        break;
                    case ProductSortOption.DiscountDesc:
                        SetOrderByDescending(Product =>
                            Product.Price > 0 ?
                            ((Product.Price - (Product.DiscountPrice ?? Product.Price)) * 100 / Product.Price)
                            : 0);
                        break;
                    default: 
                        SetOrderBy(Product => Product.Name);
                        break;
                }
            }
            else
            {
                SetOrderByDescending(Product => Product.Id);
            }
            ApplyPagination(productParameter.PageIndex, productParameter.PageSize);
        }
    }
}