using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppBoleteriaApi.Model;

namespace AppBoleteriaApi.Configurations
{
    public class MenuRouteConfig : IEntityTypeConfiguration<MenuRoute>
    {
        public void Configure(EntityTypeBuilder<MenuRoute> builder)
        {
            builder.ToTable("MenuRoute", "public");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Name).IsRequired().HasMaxLength(100);
            builder.Property(m => m.Path).IsRequired().HasMaxLength(200);
            builder.Property(m => m.Icon).HasMaxLength(50);
            builder.Property(m => m.RequiredRole).IsRequired().HasMaxLength(50);
            builder.Property(m => m.Order).HasDefaultValue(0);

            builder.HasOne(m => m.Parent)
                .WithMany(m => m.Children)
                .HasForeignKey(m => m.ParentId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        }
    }
}