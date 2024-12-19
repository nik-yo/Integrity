using Integrity.Banking.Domain.Models;
using Integrity.Banking.Domain.Models.Config;

namespace Integrity.Banking.Domain
{
    public abstract class BaseHandler(DbConfig dbConfig)
    {
        public async Task<CustomerAccountData?> RunWithRetryAsync(Func<Task<CustomerAccountData?>> callback)
        {
            var retryCount = 0;

            while (retryCount < dbConfig.MaxRetry)
            {
                try
                {
                   return await callback();
                }
                catch (Exception)
                {
                    await Task.Delay(retryCount * dbConfig.RetryDelayInMs);
                    retryCount += 1;
                }
            }

            return null;
        }

        //private async Task<Account?> SaveTransactionWithRetryAsync(int customerId, int accountId, Guid transactionId, decimal amount)
        //{
        //    try
        //    {
        //        return await RunWithRetryAsync(() => repository.SaveTransactionAsync(customerId, accountId, transactionId, amount));
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        logger.LogError(ex, "Invalid association {accountId} and {transactionId}", accountId, transactionId);
        //    }
        //    return null;
        //}

        
    }
}
