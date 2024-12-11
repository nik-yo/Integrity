using Integrity.Banking.Domain.Models;
using Integrity.Banking.Domain.Models.Database;
using Integrity.Banking.Domain.Repositories;
using Integrity.Banking.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Integrity.Banking.Infrastructure.Repositories
{
    public class BankingRepository(BankingDbContext dbContext) : IBankingRepository
    {
        public async Task<Customer?> GetCustomerAsync(int customerId)
        {
            return await dbContext.Customers.Include(c => c.Accounts).FirstOrDefaultAsync(c => c.Id == customerId);
        }

        public async Task<Account?> GetCustomerAccountAsync(int customerId, int accountId)
        {
            return await dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId && a.Customers.Count(u => u.Id == customerId) > 0 && !a.Closed);
        }

        public async Task<Account?> UpdateAccountBalanceAsync(int customerId, int accountId, decimal balance)
        {
            var customerAccount = await GetCustomerAccountAsync(customerId, accountId);
            if (customerAccount != null)
            {
                customerAccount.Balance = balance;
                await dbContext.SaveChangesAsync();
            }
            return customerAccount;
        }

        public async Task<Account?> MarkAccountAsClosedAsync(int customerId, int accountId)
        {
            var customerAccount = await GetCustomerAccountAsync(customerId, accountId);
            if (customerAccount != null)
            {
                customerAccount.Closed = true;
                await dbContext.SaveChangesAsync();
            }
            return customerAccount;
        }

        public async Task<Account?> CreateAccountAsync(int customerId, decimal balance, AccountType accountTypeId)
        {
            var customer = await GetCustomerAsync(customerId);
            if (customer != null)
            {
                var newAccount = new Account
                {
                    Balance = balance,
                    AccountTypeId = accountTypeId
                };
                customer.Accounts.Add(newAccount);
                await dbContext.SaveChangesAsync();
                return newAccount;
            }

            return null;
        }
    }
}
