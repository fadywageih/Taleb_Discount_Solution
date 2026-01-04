using Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Data.Configurations
{
    public class VendorConfigurations : IEntityTypeConfiguration<Vendor>
    {
        public void Configure(EntityTypeBuilder<Vendor> builder)
        {
            builder.HasOne(v => v.User)
                   .WithOne()
                   .HasForeignKey<Vendor>(v => v.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.Property(v => v.BusinessName)
                   .IsRequired()
                   .HasMaxLength(200);
            builder.Property(v => v.Description)
                   .IsRequired()
                   .HasMaxLength(1000);
            builder.Property(v => v.Address)
                   .IsRequired()
                   .HasMaxLength(500);
            builder.Property(v => v.BusinessImages)
                   .HasConversion(
                       v => string.Join(';', v),
                       v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()
                   )
                   .Metadata
                   .SetValueComparer(
                       new ValueComparer<List<string>>(
                           (c1, c2) => c1 != null && c2 != null ? c1.SequenceEqual(c2) : c1 == c2,
                           c => c != null ? c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())) : 0,
                           c => c != null ? c.ToList() : new List<string>()
                       )
                   );
            builder.Property(v => v.Website)
                .HasMaxLength(2000)
                .IsRequired(false);
            builder.Property(v => v.FacebookUrl)
                   .HasMaxLength(2000)
                   .IsRequired(false);
            builder.Property(v => v.LogoUrl)
                   .HasColumnType("nvarchar(max)")
                   .IsRequired(false);
            builder.Property(v => v.Address2)
                   .HasMaxLength(2000)
                   .IsRequired(false);
        }
    }
}