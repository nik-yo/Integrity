using System.Text.Json.Serialization;

namespace Integrity.Banking.Domain.Models
{
    public class TransactionResponse : BaseAccountData
    {
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Balance { get; set; }
        public bool Succeeded { get; set; }

    }
}
