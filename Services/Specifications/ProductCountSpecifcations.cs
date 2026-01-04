namespace Services.Specifications
{
    public class ProductCountSpecifcations : Specifications<Product>
    {
        public ProductCountSpecifcations(ProductParameterSpecifications productParameter)
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
        }
    }
}