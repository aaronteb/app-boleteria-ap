using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppBoleteriaApi.Model;

namespace AppBoleteriaApi.Configurations
{
    public class PermissionConfig : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("Permission", "public");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Code).IsRequired().HasMaxLength(100);
            builder.HasIndex(p => p.Code).IsUnique();
            builder.Property(p => p.Description).HasMaxLength(300);
        }
    }
}