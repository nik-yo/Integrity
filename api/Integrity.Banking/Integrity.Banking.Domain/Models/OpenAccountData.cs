namespace Integrity.Banking.Domain.Models
{
    public class OpenAccountData
    {
        public int CustomerId { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public AccountType AccountTypeId { get; set; }
    }
}
