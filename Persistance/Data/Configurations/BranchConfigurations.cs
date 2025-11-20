using Domain.Entities.Vendor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Data.Configurations
{
    public class BranchConfigurations : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.HasOne(b => b.Vendor)
                   .WithMany(v => v.Branches)
                   .HasForeignKey(b => b.VendorId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(b => b.Address)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(b => b.City)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(b => b.State)
                   .IsRequired()
                   .HasMaxLength(100);
        }
    }
}
