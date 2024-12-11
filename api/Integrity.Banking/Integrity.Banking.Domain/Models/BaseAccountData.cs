namespace Integrity.Banking.Domain.Models
{
    public abstract class BaseAccountData
    {
        public int CustomerId { get; set; }
        public int AccountId { get; set; }
    }
}
