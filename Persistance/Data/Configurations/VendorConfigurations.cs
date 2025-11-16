

using Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
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
                   );
        }
    }
}
