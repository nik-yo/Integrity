namespace Integrity.Banking.Api.Models
{
    public class CloseAccountRequest
    {
        public int CustomerId { get; set; }
        public int AccountId { get; set; }
    }
}
