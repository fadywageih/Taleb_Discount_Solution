using Domain.Entities.transcation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Data.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("Transactions");

            builder.HasKey(t => t.Id);
            builder.Property(t => t.TransactionNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.DiscountCode)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(t => t.ProductName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.ProductPictureUrl)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(t => t.VendorName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.CustomerName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.CustomerEmail)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(t => t.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(t => t.Quantity)
                .IsRequired()
                .HasDefaultValue(1);
            builder.Property(t => t.Status)
                .IsRequired()
                .HasConversion<string>() 
                .HasMaxLength(20)
                .HasDefaultValue(TransactionStatus.Pending);

            builder.Property(t => t.RejectionReason)
                .HasMaxLength(500)
                .IsRequired(false);

            // Relationships
            builder.HasOne(t => t.Product)
                .WithMany()
                .HasForeignKey(t => t.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Vendor)
                .WithMany()
                .HasForeignKey(t => t.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Customer)
                .WithMany()
                .HasForeignKey(t => t.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(t => t.TransactionNumber).IsUnique();
            builder.HasIndex(t => t.VendorId);
            builder.HasIndex(t => t.CustomerId);
            builder.HasIndex(t => t.ProductId);
            builder.HasIndex(t => t.Status);
            builder.HasIndex(t => t.TransactionDate);
            builder.HasIndex(t => t.CreatedAt);

            // Default values
            builder.Property(t => t.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}