namespace Integrity.Banking.Domain.Models
{
    public class CustomerAccountData
    {
        public int AccountId { get; set; }
        public int CustomerId { get; set; }
        public decimal Balance { get; set; }
    }
}
