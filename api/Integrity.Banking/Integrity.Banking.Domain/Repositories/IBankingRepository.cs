using Integrity.Banking.Domain.Models;

namespace Integrity.Banking.Domain.Repositories
{
    public interface IBankingRepository
    {
        public Task<CustomerData?> GetCustomerAsync(int customerId);
        public Task<CustomerAccountData?> GetCustomerAccountAsync(int customerId, int accountId);

        public Task<CustomerAccountData?> SaveTransactionAsync(int customerId, int accountId, Guid transactionId, decimal amount);

        public Task<CustomerAccountData?> MarkAccountAsClosedAsync(int customerId, int accountId);
        
        public Task<CustomerAccountData?> CreateAccountAsync(int customerId, decimal balance, AccountType accountTypeId);
    }
}
