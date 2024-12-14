using System.Text.Json.Serialization;

namespace Integrity.Banking.Domain.Models
{
    public class TransactionRequest : BaseAccountData
    {
        public decimal Amount { get; set; }
        [JsonIgnore] // Remove if we allow frontend to set
        public Guid TransactionId { get; set; } = Guid.NewGuid();
    }
}
