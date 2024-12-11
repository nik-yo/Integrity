using Integrity.Banking.Domain.Models;
using Integrity.Banking.Domain.Models.Database;

namespace Integrity.Banking.Domain.Repositories
{
    public interface IBankingRepository
    {
        public Task<Customer?> GetCustomerAsync(int customerId);
        public Task<Account?> GetCustomerAccountAsync(int customerId, int accountId);

        public Task<Account?> UpdateAccountBalanceAsync(int customerId, int accountId, decimal balance);

        public Task<Account?> MarkAccountAsClosedAsync(int customerId, int accountId);
        
        public Task<Account?> CreateAccountAsync(int customerId, decimal balance, AccountType accountTypeId);
    }
}
