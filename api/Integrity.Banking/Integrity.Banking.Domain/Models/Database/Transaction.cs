namespace Integrity.Banking.Domain.Models.Database
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public int AccountId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public Account Account { get; set; } = null!;
        public decimal Amount { get; set; }
    }
}
