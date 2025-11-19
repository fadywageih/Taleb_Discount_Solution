using Domain.Contracts;
using Domain.Entities.Product;
using Shared;

namespace Services.Specifications
{
    public class ProductCountSpecifcations : Specifications<Product>
    {
        public ProductCountSpecifcations(ProductParameterSpecifications productParameter) : base(Product =>
            (!productParameter.CategoryId.HasValue || Product.CategoryId == productParameter.CategoryId.Value) && // CategoryId بدلاً من categoryId
            (string.IsNullOrWhiteSpace(productParameter.Search) || Product.Name.ToLower().Contains(productParameter.Search.ToLower().Trim())))
        { }
    }
}