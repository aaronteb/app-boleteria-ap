using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppBoleteriaApi.Model;

namespace AppBoleteriaApi.Configurations
{
    public class TicketConfig : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.ToTable("Ticket", "public");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id).HasColumnName("Id");
            builder.Property(t => t.CompanyId).HasColumnName("CompanyId"); 
            builder.Property(t => t.UserId).HasColumnName("UserId");
            builder.Property(t => t.TicketTypeId).HasColumnName("TicketTypeId");
            builder.Property(t => t.QrCode).HasColumnName("QrCode").IsRequired();
            builder.Property(t => t.Used).HasColumnName("Used").HasDefaultValue(false);
            builder.Property(t => t.PurchaseDate).HasColumnName("PurchaseDate").HasDefaultValueSql("now()");
            builder.Property(t => t.IsActive).HasColumnName("IsActive").HasDefaultValue(true);

            builder.HasOne(t => t.Company)
                .WithMany()
                .HasForeignKey(t => t.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.User)
                .WithMany(u => u.Tickets)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.TicketType)
                .WithMany(tt => tt.Tickets)
                .HasForeignKey(t => t.TicketTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(t => t.AccessLogs);
        }
    }
}