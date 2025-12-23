using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppBoleteriaApi.Model;

namespace AppBoleteriaApi.Configurations
{
    public class EventConfig : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable("Event", "public");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("Id");
            builder.Property(e => e.CompanyId).HasColumnName("CompanyId"); 
            builder.Property(e => e.OrganizerId).HasColumnName("OrganizerId");
            builder.Property(e => e.Title).HasColumnName("Title").IsRequired().HasMaxLength(200);
            builder.Property(e => e.Description).HasColumnName("Description");
            builder.Property(e => e.Location).HasColumnName("Location").HasMaxLength(200);
            builder.Property(e => e.EventDateTime).HasColumnName("EventDateTime");
            builder.Property(e => e.BannerUrl).HasColumnName("BannerUrl");
            builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("now()");
            builder.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(true);

            builder.HasOne(e => e.Company)
                .WithMany(c => c.Events)
                .HasForeignKey(e => e.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Organizer)
                .WithMany(u => u.OrganizedEvents)
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Property(e => e.VenueId).HasColumnName("VenueId").IsRequired(false);

            builder.HasOne(e => e.Venue)
                .WithMany(v => v.Events)
                .HasForeignKey(e => e.VenueId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        }

    }
}