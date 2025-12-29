using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppBoleteriaApi.Model;

namespace AppBoleteriaApi.Configurations
{
    public class TransactionConfig : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("Transaction", "public");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id).HasColumnName("Id");
            builder.Property(t => t.CompanyId).HasColumnName("CompanyId"); 
            builder.Property(t => t.UserId).HasColumnName("UserId");
            builder.Property(t => t.TicketId).HasColumnName("TicketId");
            builder.Property(t => t.Amount).HasColumnName("Amount").HasColumnType("decimal(10,2)");
            builder.Property(t => t.PaymentMethod).HasColumnName("PaymentMethod").HasMaxLength(50);
            builder.Property(t => t.Status).HasColumnName("Status").HasMaxLength(20);
            builder.Property(t => t.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("now()");
            builder.Property(t => t.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
            builder.Property(t => t.PayPhoneTransactionId).HasColumnName("PayPhoneTransactionId").HasMaxLength(100);
            builder.Property(t => t.PayPhonePaymentId).HasColumnName("PayPhonePaymentId").HasMaxLength(100);
            builder.Property(t => t.Reference).HasColumnName("Reference").HasMaxLength(100);
            builder.Property(t => t.TicketTypeId).HasColumnName("TicketTypeId");
            builder.Property(t => t.Quantity).HasColumnName("Quantity");
            builder.Property(t => t.UpdatedAt).HasColumnName("UpdatedAt");

            builder.HasOne(t => t.Company)
                .WithMany()
                .HasForeignKey(t => t.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(t => t.User);
            builder.Ignore(t => t.Ticket);
        }
    }
}