using AppBoleteriaApi.DTOs;

namespace AppBoleteriaApi.Services
{
    public interface ITicketService
    {
        Task<List<TicketResponseDto>> PurchaseTicketsAsync(int userId, TicketPurchaseDto dto);
        Task<TicketResponseDto?> GetByIdAsync(int id);
        Task<TicketResponseDto?> GetByQrCodeAsync(string qrCode);
        Task<IEnumerable<TicketResponseDto>> GetMyTicketsAsync(int userId);
        Task<bool> UseTicketAsync(string qrCode, int staffId);
    }
}