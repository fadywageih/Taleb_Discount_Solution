using Domain.Contracts;
using Domain.Entities.Product;
using Shared;

namespace Services.Specifications
{
    public class ProductWithCategorySpecifications : Specifications<Product>
    {
        public ProductWithCategorySpecifications(int id) : base(p => p.Id == id)
        {
            AddInclude(Product => Product.ProductCategory);
            AddInclude(Product => Product.Vendor); // إضافة Vendor
        }
        public ProductWithCategorySpecifications(Guid vendorId) : base(p => p.VendorId == vendorId)
        {
            AddInclude(p => p.ProductCategory);
            AddInclude(p => p.Vendor); // ✅ أضف هذا السطر
        }

        public ProductWithCategorySpecifications(ProductParameterSpecifications productParameter)
            : base(Product =>
                (!productParameter.CategoryId.HasValue || Product.CategoryId == productParameter.CategoryId.Value) &&
                (string.IsNullOrEmpty(productParameter.Search) || Product.Name.ToLower().Contains(productParameter.Search.ToLower())))
        {
            AddInclude(Product => Product.ProductCategory);
            AddInclude(Product => Product.Vendor); // إضافة Vendor

            if (productParameter.Sort is not null)
            {
                switch (productParameter.Sort)
                {
                    case ProductSortOption.NameDesc:
                        SetOrderByDescending(Product => Product.Name);
                        break;
                    default:
                        SetOrderBy(Product => Product.Name);
                        break;
                }
            }
            ApplyPagination(productParameter.PageIndex, productParameter.PageSize);
        }
    }
}