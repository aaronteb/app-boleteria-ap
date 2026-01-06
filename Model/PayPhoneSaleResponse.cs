namespace AppBoleteriaApi.Services
{
    /// <summary>
    /// Respuesta del estado de una transacción en PayPhone
    /// </summary>
    public class PayPhoneStatusResponse
    {
        public int StatusCode { get; set; } // 1=Pendiente, 2=Rechazado, 3=Aprobado, 4=Cancelado
        public string? TransactionStatus { get; set; }
        public string? ClientTransactionId { get; set; }
        public string? AuthorizationCode { get; set; }
        public long TransactionId { get; set; }
        public int Amount { get; set; }
        public string? Message { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Currency { get; set; }
        public string? Reference { get; set; }
        public DateTime? Date { get; set; }
        public string? Email { get; set; }
        public string? CardType { get; set; }
        public string? Bin { get; set; }
        public string? LastDigits { get; set; }
        public string? CardBrand { get; set; }
    }

    /// <summary>
    /// Respuesta al iniciar un pago (datos para la Cajita)
    /// </summary>
    public class InitiatePaymentResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public int Quantity { get; set; }

        // 🆕 DATOS PARA LA CAJITA DE PAGOS
        public string? Token { get; set; }
        public string? StoreId { get; set; }
        public int AmountInCents { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Currency { get; set; }
        public int TimeZone { get; set; }
    }

    /// <summary>
    /// DTO para confirmar pago desde la URL de respuesta de la Cajita
    /// </summary>
    public class ConfirmPaymentFromCajitaDto
    {
        public long Id { get; set; } // TransactionId de PayPhone
        public string ClientTxId { get; set; } = string.Empty; // Reference de tu BD
    }

    /// <summary>
    /// Respuesta de confirmación de la Cajita (API Button/V2/Confirm)
    /// </summary>
    public class CajitaConfirmResponse
    {
        public string? Email { get; set; }
        public string? CardType { get; set; }
        public string? Bin { get; set; }
        public string? LastDigits { get; set; }
        public bool Deferred { get; set; }
        public string? CardBrand { get; set; }
        public int Amount { get; set; }
        public string? ClientTransactionId { get; set; }
        public string? PhoneNumber { get; set; }
        public int StatusCode { get; set; } // 2=Cancelado, 3=Aprobado
        public string? TransactionStatus { get; set; }
        public string? AuthorizationCode { get; set; }
        public string? Message { get; set; }
        public int MessageCode { get; set; }
        public long TransactionId { get; set; }
        public string? Document { get; set; }
        public string? Currency { get; set; }
        public string? StoreName { get; set; }
        public DateTime Date { get; set; }
        public string? Reference { get; set; }
    }

    /// <summary>
    /// Errores de PayPhone parseados
    /// </summary>
    internal class PayPhoneErrorResponse
    {
        public string? Message { get; set; }
        public int? ErrorCode { get; set; }
    }
}