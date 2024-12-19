namespace Integrity.Banking.Domain.Models.Database
{
    public class Customer
    {
        public int Id { get; set; }

        public List<Account> Accounts { get; set; } = [];
    }
}
