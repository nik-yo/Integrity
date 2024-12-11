namespace Integrity.Banking.Domain.Models
{
    public class OpenAccountResponse : BaseAccountData
    {
        public AccountType AccountTypeId { get; set; }
        public decimal Balance { get; set; }
        public bool Succeeded { get; set; }
    }
}
