using Integrity.Banking.Domain.Models;

namespace Integrity.Banking.Api.Models
{
    public class OpenAccountRequest
    {
        public int CustomerId { get; set; }
        public decimal InitialDeposit {  get; set; }
        public AccountType AccountTypeId { get; set; }
    }
}
