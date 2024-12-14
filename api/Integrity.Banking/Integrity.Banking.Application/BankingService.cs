using Integrity.Banking.Domain.Models;
using Integrity.Banking.Domain.Models.Config;
using Integrity.Banking.Domain.Models.Database;
using Integrity.Banking.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System.Data;

namespace Integrity.Banking.Application
{
    public class BankingService(DbConfig dbConfig, IBankingRepository repository, ILogger<BankingService> logger)
    {
        public async Task<TransactionResponse> MakeDepositAsync(TransactionRequest request)
        {
            var response = new TransactionResponse();

            if (request.Amount > 0)
            {
                var updatedCustomerAccount = await SaveTransactionWithRetryAsync(request.CustomerId, request.AccountId, request.TransactionId, request.Amount);
                if (updatedCustomerAccount != null)
                {
                    response.CustomerId = request.CustomerId;
                    response.AccountId = request.AccountId;
                    response.Balance = updatedCustomerAccount.Balance;
                    response.Succeeded = true;
                }
            }           

            return response;
        }

        public async Task<TransactionResponse> MakeWithdrawalAsync(TransactionRequest request)
        {
            var response = new TransactionResponse();

            if (request.Amount > 0)
            {
                var customerAccount = await RunWithRetryAsync(() => repository.GetCustomerAccountAsync(request.CustomerId, request.AccountId));
                if (customerAccount != null)
                {
                    response.CustomerId = request.CustomerId;
                    response.AccountId = request.AccountId;
                    response.Balance = customerAccount.Balance;

                    if (customerAccount.Balance >= request.Amount)
                    {
                        var updatedCustomerAccount = await SaveTransactionWithRetryAsync(request.CustomerId, request.AccountId, request.TransactionId, -request.Amount);
                        if (updatedCustomerAccount != null)
                        {
                            response.Balance = updatedCustomerAccount.Balance;
                            response.Succeeded = true;
                        }
                    }
                }
            }

            return response;
        }

        private async Task<Account?> RunWithRetryAsync(Func<Task<Account?>> callback)
        {
            var retryCount = 0;

            while (retryCount < dbConfig.MaxRetry)
            {
                try
                {
                   return await callback();
                }
                catch (Exception ex) when (ex is MySqlException || ex is DbUpdateException || ex is DBConcurrencyException)
                {
                    await Task.Delay(retryCount * dbConfig.RetryDelayInMs);
                    retryCount += 1;
                }
            }

            return null;
        }

        private async Task<Account?> SaveTransactionWithRetryAsync(int customerId, int accountId, Guid transactionId, decimal amount)
        {
            try
            {
                return await RunWithRetryAsync(() => repository.SaveTransactionAsync(customerId, accountId, transactionId, amount));
            }
            catch (InvalidOperationException ex)
            {
                logger.LogError(ex, "Invalid association {accountId} and {transactionId}", accountId, transactionId);
            }
            return null;
        }

        public async Task<CloseAccountResponse> CloseAccountAsync(CloseAccountRequest request)
        {
            var response = new CloseAccountResponse();

            var customerAccount = await RunWithRetryAsync(async () => {
                var account = await repository.GetCustomerAccountAsync(request.CustomerId, request.AccountId);
                if (account != null && account.Balance == decimal.Zero)
                {
                    account = await repository.MarkAccountAsClosedAsync(request.CustomerId, request.AccountId);
                }
                return account;
            });

            if (customerAccount != null)
            {
                response.CustomerId = request.CustomerId;
                response.AccountId = request.AccountId;
                response.Succeeded = customerAccount.Closed;
            }

            return response;
        }

        public async Task<OpenAccountResponse> OpenAccountAsync(OpenAccountRequest request)
        {
            var response = new OpenAccountResponse();

            if (request.InitialDeposit >= 100m)
            {
                var newAccount = await RunWithRetryAsync(async () =>
                {
                    var customer = await repository.GetCustomerAsync(request.CustomerId);
                    if (customer != null && (customer.Accounts.Count > 0 || request.AccountTypeId == AccountType.Savings))
                    {
                        return await repository.CreateAccountAsync(request.CustomerId, request.InitialDeposit,
                        request.AccountTypeId);
                    }
                    return null;
                });

                if (newAccount != null)
                {
                    response.CustomerId = request.CustomerId;
                    response.AccountId = newAccount.Id;
                    response.AccountTypeId = newAccount.AccountTypeId;
                    response.Balance = newAccount.Balance;
                    response.Succeeded = true;
                }
            }            

            return response;
        }
    }
}
