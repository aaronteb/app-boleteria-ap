using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppBoleteriaApi.Model;

namespace AppBoleteriaApi.Configurations
{
    public class TicketTypeConfig : IEntityTypeConfiguration<TicketType>
    {
        public void Configure(EntityTypeBuilder<TicketType> builder)
        {
            builder.ToTable("TicketType", "public");

            builder.HasKey(tt => tt.Id);

            builder.Property(tt => tt.Id).HasColumnName("Id");
            builder.Property(tt => tt.CompanyId).HasColumnName("CompanyId"); 
            builder.Property(tt => tt.EventId).HasColumnName("EventId");
            builder.Property(tt => tt.Name).HasColumnName("Name").IsRequired().HasMaxLength(50);
            builder.Property(tt => tt.Price).HasColumnName("Price").HasColumnType("decimal(10,2)");
            builder.Property(tt => tt.Stock).HasColumnName("Stock");
            builder.Property(tt => tt.IsActive).HasColumnName("IsActive").HasDefaultValue(true);

            builder.HasOne(tt => tt.Company)
                .WithMany()
                .HasForeignKey(tt => tt.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(tt => tt.Event)
                .WithMany(e => e.TicketTypes)
                .HasForeignKey(tt => tt.EventId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}