using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppBoleteriaApi.Model;

namespace AppBoleteriaApi.Configurations
{
    public class VenueConfig : IEntityTypeConfiguration<Venue>
    {
        public void Configure(EntityTypeBuilder<Venue> builder)
        {
            builder.ToTable("Venue", "public");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.Name).IsRequired().HasMaxLength(200);
            builder.Property(v => v.Address).IsRequired().HasMaxLength(300);
            builder.Property(v => v.City).IsRequired().HasMaxLength(100);
            builder.Property(v => v.State).IsRequired().HasMaxLength(100);
            builder.Property(v => v.Country).IsRequired().HasMaxLength(100);
            builder.Property(v => v.PostalCode).HasMaxLength(20);
            builder.Property(v => v.Phone).HasMaxLength(20);
            builder.Property(v => v.Capacity).IsRequired();
            builder.Property(v => v.CreatedAt).HasDefaultValueSql("now()");
            builder.Property(v => v.IsActive).HasDefaultValue(true);

            builder.HasOne(v => v.Company)
                .WithMany()
                .HasForeignKey(v => v.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(v => v.CompanyId);
        }
    }
}