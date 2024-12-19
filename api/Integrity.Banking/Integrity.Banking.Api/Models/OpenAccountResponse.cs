using Integrity.Banking.Domain.Models;

namespace Integrity.Banking.Api.Models
{
    public class OpenAccountResponse
    {
        public int CustomerId { get; set; }
        public int AccountId { get; set; }
        public AccountType AccountTypeId { get; set; }
        public decimal Balance { get; set; }
        public bool Succeeded { get; set; }
    }
}
