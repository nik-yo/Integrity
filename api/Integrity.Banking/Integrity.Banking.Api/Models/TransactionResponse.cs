using System.Text.Json.Serialization;

namespace Integrity.Banking.Api.Models
{
    public class TransactionResponse
    {
        public int CustomerId { get; set; }
        public int AccountId { get; set; }

        [JsonConverter(typeof(DecimalConverter))]
        public decimal Balance { get; set; }
        public bool Succeeded { get; set; }

    }
}
