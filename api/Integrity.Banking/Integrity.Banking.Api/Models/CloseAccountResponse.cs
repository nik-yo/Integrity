namespace Integrity.Banking.Api.Models
{
    public class CloseAccountResponse
    {
        public int CustomerId { get; set; }
        public int AccountId { get; set; }
        public bool Succeeded { get; set; }
    }
}
