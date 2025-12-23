using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppBoleteriaApi.Model;

namespace AppBoleteriaApi.Configurations
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users", "public");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id).HasColumnName("id");
            builder.Property(u => u.CompanyId).HasColumnName("company_id").IsRequired(false); // 👈 NO REQUERIDO
            builder.Property(u => u.FullName).HasColumnName("full_name").IsRequired().HasMaxLength(150);
            builder.Property(u => u.Email).HasColumnName("email").IsRequired().HasMaxLength(150);
            builder.Property(u => u.PasswordHash).HasColumnName("password_hash").IsRequired();
            builder.Property(u => u.Phone).HasColumnName("phone").HasMaxLength(20);
            builder.Property(u => u.RoleId).HasColumnName("role_id").IsRequired();
            builder.Property(u => u.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
            builder.Property(u => u.IsActive).HasColumnName("is_active").HasDefaultValue(true);

            builder.HasOne(u => u.Company)
                .WithMany(c => c.Users)
                .HasForeignKey(u => u.CompanyId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false); 

            builder.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .HasPrincipalKey(r => r.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(u => u.Tickets);
            builder.Ignore(u => u.Transactions);
            builder.Ignore(u => u.OrganizedEvents);

            builder.HasIndex(u => u.Email).IsUnique();
        }
    }
}