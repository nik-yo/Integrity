namespace Integrity.Banking.Domain.Models
{
    public class TransactionData
    {
        public int CustomerId { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public Guid TransactionId { get; set; }
    }
}
