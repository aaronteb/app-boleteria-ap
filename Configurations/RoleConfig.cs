using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppBoleteriaApi.Model;

namespace AppBoleteriaApi.Configurations
{
    public class RoleConfig : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Role", "public");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                .HasColumnName("Id");

            builder.Property(r => r.Name)
                .HasColumnName("Name")
                .IsRequired();

            builder.Property(r => r.IsActive) 
                .HasColumnName("IsActive")
                .HasDefaultValue(true);
        }
    }
}