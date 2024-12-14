using Integrity.Banking.Domain.Models;
using Integrity.Banking.Domain.Models.Database;

namespace Integrity.Banking.Domain.Repositories
{
    public interface IBankingRepository
    {
        public Task<Customer?> GetCustomerAsync(int customerId);
        public Task<Account?> GetCustomerAccountAsync(int customerId, int accountId);

        public Task<Account?> SaveTransactionAsync(int customerId, int accountId, Guid transactionId, decimal amount);

        public Task<Account?> MarkAccountAsClosedAsync(int customerId, int accountId);
        
        public Task<Account?> CreateAccountAsync(int customerId, decimal balance, AccountType accountTypeId);
    }
}
