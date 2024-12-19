using Integrity.Banking.Domain.Models;
using Integrity.Banking.Domain.Models.Config;
using Integrity.Banking.Domain.Repositories;

namespace Integrity.Banking.Domain
{
    public class WithdrawalHandler(IBankingRepository repository, DbConfig dbConfig) : BaseHandler(dbConfig)
    {
        public async Task<TransactionData?> MakeWithdrawalAsync(TransactionData data)
        {
            if (data.Amount > 0)
            {
                var customerAccount = await RunWithRetryAsync(async () =>
                {
                    var account = await repository.GetCustomerAccountAsync(data.CustomerId, data.AccountId);
                    if (account != null && account.Balance >= data.Amount)
                    {
                        var updatedCustomerAccount = await repository.SaveTransactionAsync(data.CustomerId, data.AccountId, data.TransactionId, -data.Amount);
                        if (updatedCustomerAccount != null)
                        {
                            return updatedCustomerAccount;
                        }
                    }
                    return null;
                });

                if (customerAccount != null)
                {
                    data.Amount = customerAccount.Balance;
                    return data;
                }
            }

            return null;
        }
    }
}
