using AppBoleteriaApi.DTOs;

namespace AppBoleteriaApi.Services
{
    public interface IPayPhoneService
    {
        Task<InitiatePaymentResponse> InitiatePaymentAsync(int userId, InitiatePaymentDto dto);
        Task<bool> ConfirmPaymentAsync(string transactionId);
        Task<TransactionStatusDto?> GetTransactionStatusAsync(string transactionId);
        Task<PayPhoneStatusResponse> CheckPaymentStatusAsync(string transactionId);
    }
}