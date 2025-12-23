using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppBoleteriaApi.Model;

namespace AppBoleteriaApi.Configurations
{
    public class CompanyConfig : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Company", "public");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id).HasColumnName("Id");
            builder.Property(c => c.Name).HasColumnName("Name").IsRequired().HasMaxLength(200);
            builder.Property(c => c.Slug).HasColumnName("Slug").IsRequired().HasMaxLength(100);
            builder.Property(c => c.Logo).HasColumnName("Logo");
            builder.Property(c => c.ContactEmail).HasColumnName("ContactEmail").HasMaxLength(150);
            builder.Property(c => c.ContactPhone).HasColumnName("ContactPhone").HasMaxLength(20);
            builder.Property(c => c.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("now()");
            builder.Property(c => c.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
            builder.HasIndex(c => c.Slug).IsUnique();
        }
    }
}