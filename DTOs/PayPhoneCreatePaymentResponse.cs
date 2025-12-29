namespace AppBoleteriaApi.DTOs
{
    // ============================================
    // RESPUESTAS DE PAYPHONE API
    // ============================================

    public class PayPhoneCreatePaymentResponse
    {
        public string PayWithCard { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string PaymentId { get; set; } = string.Empty;
    }

    public class PayPhoneConfirmPaymentResponse
    {
        public int TransactionStatus { get; set; } 
        public string TransactionId { get; set; } = string.Empty;
        public string ClientTransactionId { get; set; } = string.Empty;
        public string StatusCode { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    // ============================================
    // DTOs DE TU API
    // ============================================

    public class InitiatePaymentDto
    {
        public int TicketTypeId { get; set; }
        public int Quantity { get; set; }
    }

    public class InitiatePaymentResponse
    {
        public string PaymentUrl { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }

    public class TransactionStatusDto
    {
        public int Id { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string? PayPhoneTransactionId { get; set; }
    }

    // ============================================
    // DTOs PARA CONFIGURAR PAYPHONE DE COMPAÑÍA
    // ============================================

    public class CompanyPayPhoneConfigDto
    {
        public string PayPhoneToken { get; set; } = string.Empty;
        public bool PayPhoneEnabled { get; set; }
    }

    public class CompanyPayPhoneStatusDto
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public bool PayPhoneEnabled { get; set; }
        public bool HasToken { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}