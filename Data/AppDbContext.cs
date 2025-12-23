using Microsoft.EntityFrameworkCore;
using AppBoleteriaApi.Model;
using AppBoleteriaApi.Configurations;

namespace AppBoleteriaApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; } 
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketType> TicketTypes { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<AccessLog> AccessLogs { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<MenuRoute> MenuRoutes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");

            modelBuilder.ApplyConfiguration(new CompanyConfig());
            modelBuilder.ApplyConfiguration(new VenueConfig());
            modelBuilder.ApplyConfiguration(new UserConfig());
            modelBuilder.ApplyConfiguration(new RoleConfig());
            modelBuilder.ApplyConfiguration(new PermissionConfig()); 
            modelBuilder.ApplyConfiguration(new RolePermissionConfig()); 
            modelBuilder.ApplyConfiguration(new MenuRouteConfig()); 
            modelBuilder.ApplyConfiguration(new EventConfig());
            modelBuilder.ApplyConfiguration(new TicketConfig());
            modelBuilder.ApplyConfiguration(new TicketTypeConfig());
            modelBuilder.ApplyConfiguration(new TransactionConfig());
            modelBuilder.ApplyConfiguration(new AccessLogConfig());
            modelBuilder.ApplyConfiguration(new VenueConfig());
            modelBuilder.ApplyConfiguration(new MenuRouteConfig());
        }
    }
}