namespace AppBoleteriaApi.Model
{
    public class User
    {
        public int Id { get; set; }
        public int? CompanyId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Phone { get; set; }
        public int RoleId { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        public Company? Company { get; set; }
        public Role? Role { get; set; }
        public ICollection<Ticket>? Tickets { get; set; }
        public ICollection<Transaction>? Transactions { get; set; }
        public ICollection<Event>? OrganizedEvents { get; set; }
    }
}