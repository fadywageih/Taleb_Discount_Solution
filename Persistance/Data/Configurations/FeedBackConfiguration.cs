using Domain.Entities.FeedBack;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Data.Configurations
{
    public class FeedBackConfiguration : IEntityTypeConfiguration<FeedBack>
    {
        public void Configure(EntityTypeBuilder<FeedBack> builder)
        {
            builder.ToTable("FeedBacks");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(f => f.Category)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(f => f.Rating)
                .IsRequired();

            builder.Property(f => f.Suggestions)
                .HasMaxLength(2000)
                .IsRequired(false);

            builder.Property(f => f.CreatedAt)
                .IsRequired();

            builder.HasIndex(f => f.Email);
            builder.HasIndex(f => f.Category);
            builder.HasIndex(f => f.CreatedAt);
            builder.Property(f => f.CreatedAt)
                .HasDefaultValueSql("GETDATE()"); 
                                                  
        }
    }
}