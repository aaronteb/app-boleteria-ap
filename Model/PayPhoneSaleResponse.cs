namespace AppBoleteriaApi.Services
{
    /// <summary>
    /// Respuesta de la API Sale de PayPhone
    /// </summary>
    public class PayPhoneSaleResponse
    {
        public long TransactionId { get; set; }
        public string? Message { get; set; }
        public string? ErrorCode { get; set; }
    }

    /// <summary>
    /// Respuesta del estado de una transacción en PayPhone
    /// </summary>
    public class PayPhoneStatusResponse
    {
        public int StatusCode { get; set; } // 1=Pendiente, 2=Rechazado, 3=Aprobado
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
    }

    /// <summary>
    /// Respuesta al iniciar un pago
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
        public string? PayPhoneTransactionId { get; set; }
        public string? PaymentUrl { get; set; }
    }

    /// <summary>
    /// Errores de PayPhone parseados
    /// </summary>
    internal class PayPhoneErrorResponse
    {
        public string? Message { get; set; }
        public int? ErrorCode { get; set; }
    }

    /// <summary>
    /// Request interno para la API Sale de PayPhone
    /// </summary>
    internal class PayPhoneSaleRequest
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public int Amount { get; set; } // En CENTAVOS
        public int AmountWithoutTax { get; set; }
        public int AmountWithTax { get; set; }
        public int Tax { get; set; }
        public string ClientTransactionId { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public string StoreId { get; set; } = string.Empty;
        public string Currency { get; set; } = "USD";
        public int TimeZone { get; set; } = -5;
        public string? ResponseUrl { get; set; }
    }
}