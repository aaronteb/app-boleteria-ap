using AppBoleteriaApi.DTOs;

namespace AppBoleteriaApi.Services
{
    public interface IPayPhoneService
    {
        Task<InitiatePaymentResponse> InitiatePaymentAsync(int userId, InitiatePaymentDto dto);
        Task<CajitaConfirmResponse> ConfirmPaymentFromCajitaAsync(ConfirmPaymentFromCajitaDto dto);
        Task<PayPhoneStatusResponse> CheckPaymentStatusAsync(string transactionId);
        Task<TransactionStatusDto?> GetTransactionStatusAsync(string transactionId);
        Task VerifyAndUpdatePendingTransactionsAsync(int userId);

        [Obsolete("Usar ConfirmPaymentFromCajitaAsync")]
        Task<bool> ConfirmPaymentAsync(string transactionId);
    }
}