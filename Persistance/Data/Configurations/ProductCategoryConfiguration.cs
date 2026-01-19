using Domain.Entities.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Data.Configurations
{
    public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
    {
        public void Configure(EntityTypeBuilder<ProductCategory> builder)
        {
            builder.HasKey(pc => pc.Id);

            builder.Property(pc => pc.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.HasData(
                new ProductCategory { Id = 1, Name = "Supplies" },
                new ProductCategory { Id = 2, Name = "Technology" },
                new ProductCategory { Id = 3, Name = "Medical" },
                new ProductCategory { Id = 4, Name = "Engineering" },
                new ProductCategory { Id = 5, Name = "Workspaces" },
                new ProductCategory { Id = 6, Name = "Uniform" }
            );
        }
    }
}
