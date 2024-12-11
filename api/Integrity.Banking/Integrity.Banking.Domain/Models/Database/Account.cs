namespace Integrity.Banking.Domain.Models.Database
{
    public class Account
    {
        public int Id { get; set; }
        public decimal Balance { get; set; }
        public List<Customer> Customers { get; set; } = [];
        public AccountType AccountTypeId { get; set; }
        public bool Closed { get; set; }
    }
}
