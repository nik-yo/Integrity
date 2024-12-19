using Integrity.Banking.Domain.Models;
using Integrity.Banking.Domain.Models.Config;
using Integrity.Banking.Domain.Repositories;

namespace Integrity.Banking.Domain
{
    public class AccountHandler(IBankingRepository repository, DbConfig dbConfig) : BaseHandler(dbConfig)
    {
        public async Task<CloseAccountData?> CloseAccountAsync(CloseAccountData data)
        {
            var customerAccount = await RunWithRetryAsync(async () => {
                var account = await repository.GetCustomerAccountAsync(data.CustomerId, data.AccountId);
                if (account != null && account.Balance == decimal.Zero)
                {
                    account = await repository.MarkAccountAsClosedAsync(data.CustomerId, data.AccountId);
                }
                return account;
            });

            if (customerAccount != null)
            {
                return data;
            }

            return null;
        }

        public async Task<OpenAccountData?> OpenAccountAsync(OpenAccountData data)
        {
            if (data.Amount >= 100m)
            {
                var newAccount = await RunWithRetryAsync(async () =>
                {
                    var customer = await repository.GetCustomerAsync(data.CustomerId);
                    if (customer != null && (customer.AccountCount > 0 || data.AccountTypeId == AccountType.Savings))
                    {
                        return await repository.CreateAccountAsync(data.CustomerId, data.Amount, data.AccountTypeId);
                    }
                    return null;
                });

                if (newAccount != null)
                {
                    return data;
                }
            }

            return null;
        }
    }
}
