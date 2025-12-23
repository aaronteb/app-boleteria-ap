using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppBoleteriaApi.Model;

namespace AppBoleteriaApi.Configurations
{
    public class AccessLogConfig : IEntityTypeConfiguration<AccessLog>
    {
        public void Configure(EntityTypeBuilder<AccessLog> builder)
        {
            builder.ToTable("AccessLog", "public");

            builder.HasKey(al => al.Id);

            builder.Property(al => al.Id).HasColumnName("Id");
            builder.Property(al => al.TicketId).HasColumnName("TicketId");
            builder.Property(al => al.StaffId).HasColumnName("StaffId");
            builder.Property(al => al.ScannedAt).HasColumnName("ScannedAt").HasDefaultValueSql("now()");
            builder.Property(al => al.IsActive).HasColumnName("IsActive").HasDefaultValue(true); 

            builder.Ignore(al => al.Ticket);
            builder.Ignore(al => al.Staff);
        }
    }
}