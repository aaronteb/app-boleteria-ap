using AppBoleteriaApi.Model;

public class Company
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string? Logo { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public ICollection<User>? Users { get; set; }
    public ICollection<Event>? Events { get; set; }

    public string? PayPhoneToken { get; set; }
    public bool PayPhoneEnabled { get; set; } = false;

    public string? PayPhoneStoreId { get; set; }
    public string? PayPhonePhoneNumber { get; set; }  
    public string? PayPhoneCountryCode { get; set; } = "593";
    public string? PayPhoneCurrency { get; set; } = "USD";
    public int? PayPhoneTimeZone { get; set; } = -5;
}