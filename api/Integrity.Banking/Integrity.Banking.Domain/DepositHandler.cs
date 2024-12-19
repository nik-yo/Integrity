using Integrity.Banking.Domain.Models;
using Integrity.Banking.Domain.Models.Config;
using Integrity.Banking.Domain.Repositories;

namespace Integrity.Banking.Domain
{
    public class DepositHandler(IBankingRepository repository, DbConfig dbConfig): BaseHandler(dbConfig)
    {
        public async Task<TransactionData?> MakeDepositAsync(TransactionData data)
        {
            if (data.Amount > 0)
            {
                var updatedCustomerAccount = await RunWithRetryAsync(async () =>
                {

                    return await repository.SaveTransactionAsync(data.CustomerId, data.AccountId, data.TransactionId, data.Amount);
                });

                if (updatedCustomerAccount != null)
                {
                    data.Amount = updatedCustomerAccount.Balance;
                    return data;
                }
            }

            return null;
        }
    }
}
