namespace Integrity.Banking.Domain.Models
{
    public class TransactionRequest : BaseAccountData
    {
        public decimal Amount { get; set; }
    }
}
