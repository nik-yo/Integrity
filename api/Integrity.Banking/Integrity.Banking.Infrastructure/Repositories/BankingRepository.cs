using Integrity.Banking.Domain.Models;
using Integrity.Banking.Domain.Models.Database;
using Integrity.Banking.Domain.Repositories;
using Integrity.Banking.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Integrity.Banking.Infrastructure.Repositories
{
    public class BankingRepository(BankingDbContext dbContext) : IBankingRepository
    {
        public async Task<CustomerData?> GetCustomerAsync(int customerId)
        {
            var customer = await GetCustomerFromDbAsync(customerId);

            if (customer != null)
            {
                return new CustomerData
                {
                    Id = customerId,
                    AccountCount = customer.Accounts.Count(),
                };
            }

            return null;
        }

        public async Task<CustomerAccountData?> GetCustomerAccountAsync(int customerId, int accountId)
        {
            var account = await GetAccountFromDbAsync(customerId, accountId);
            if (account != null)
            {
                return new CustomerAccountData
                {
                    CustomerId = customerId,
                    AccountId = account.Id,
                    Balance = account.Balance,
                };
            }

            return null;
        }

        private async Task<Customer?> GetCustomerFromDbAsync(int customerId)
        {
            return await dbContext.Customers.Include(c => c.Accounts).FirstOrDefaultAsync(c => c.Id == customerId);
        }

        private async Task<Account?> GetAccountFromDbAsync(int customerId, int accountId)
        {
            return await dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId && a.Customers.Count(u => u.Id == customerId) > 0 && !a.Closed);
        }


        public async Task<CustomerAccountData?> SaveTransactionAsync(int customerId, int accountId, Guid transactionId, decimal amount)
        {
            var customerAccount = await GetAccountFromDbAsync(customerId, accountId);
            if (customerAccount != null)
            {
                var accountTransaction = dbContext.Transactions.FirstOrDefault(t => t.Id == transactionId);
                if (accountTransaction == null)
                {
                    dbContext.Transactions.Add(new Transaction
                    {
                        Id = transactionId,
                        Timestamp = DateTimeOffset.UtcNow,
                        Amount = amount,
                        AccountId = customerAccount.Id,
                        Account = customerAccount,
                    });

                    customerAccount.Balance += amount;

                    // https://learn.microsoft.com/en-us/ef/core/saving/transactions#default-transaction-behavior
                    await dbContext.SaveChangesAsync();
                }
                else if (accountTransaction.AccountId != customerAccount.Id)
                {
                    throw new InvalidOperationException("Invalid account id");
                }

                return new CustomerAccountData
                {
                    AccountId = customerAccount.Id,
                    CustomerId = customerId,
                    Balance = customerAccount.Balance
                };
            }
            return null;
        }

        public async Task<CustomerAccountData?> MarkAccountAsClosedAsync(int customerId, int accountId)
        {
            var customerAccount = await GetAccountFromDbAsync(customerId, accountId);
            if (customerAccount != null)
            {
                customerAccount.Closed = true;
                await dbContext.SaveChangesAsync();

                return new CustomerAccountData
                {
                    CustomerId = customerId,
                    AccountId = accountId,
                    Balance = customerAccount.Balance
                };
            }

            return null;
        }

        public async Task<CustomerAccountData?> CreateAccountAsync(int customerId, decimal balance, AccountType accountTypeId)
        {
            var customer = await GetCustomerFromDbAsync(customerId);
            if (customer != null)
            {
                var newAccount = new Account
                {
                    Balance = balance,
                    AccountTypeId = accountTypeId,
                };
                customer.Accounts.Add(newAccount);
                await dbContext.SaveChangesAsync();
                return new CustomerAccountData
                {
                    CustomerId = customerId,
                    AccountId = newAccount.Id,
                    Balance = newAccount.Balance
                };
            }

            return null;
        }
    }
}
