using System.Text.Json.Serialization;

namespace Integrity.Banking.Api.Models
{
    public class TransactionRequest
    {
        public int CustomerId { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        [JsonIgnore] // Remove if we allow frontend to set
        public Guid TransactionId { get; } = Guid.NewGuid();
    }
}
