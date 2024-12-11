using Integrity.Banking.Domain.Models;
using Integrity.Banking.Domain.Repositories;

namespace Integrity.Banking.Application
{
    public class BankingService(IBankingRepository repository)
    {
        public async Task<TransactionResponse> MakeDepositAsync(TransactionRequest request)
        {
            var response = new TransactionResponse();

            if (request.Amount > 0)
            {
                var customerAccount = await repository.GetCustomerAccountAsync(request.CustomerId, request.AccountId);
                if (customerAccount != null)
                {
                    response.CustomerId = request.CustomerId;
                    response.AccountId = request.AccountId;
                    response.Balance = customerAccount.Balance + request.Amount;
                    var updatedCustomerAccount = await repository.UpdateAccountBalanceAsync(request.CustomerId, request.AccountId, response.Balance);
                    response.Succeeded = updatedCustomerAccount != null;
                }
            }           

            return response;
        }

        public async Task<TransactionResponse> MakeWithdrawalAsync(TransactionRequest request)
        {
            var response = new TransactionResponse();

            if (request.Amount > 0)
            {
                var customerAccount = await repository.GetCustomerAccountAsync(request.CustomerId, request.AccountId);
                if (customerAccount != null)
                {
                    response.CustomerId = request.CustomerId;
                    response.AccountId = request.AccountId;
                    response.Balance = customerAccount.Balance;

                    if (customerAccount.Balance >= request.Amount)
                    {
                        response.Balance -= request.Amount;
                        var updatedCustomerAccount = await repository.UpdateAccountBalanceAsync(request.CustomerId, request.AccountId, response.Balance);
                        response.Succeeded = updatedCustomerAccount != null;
                    }
                }
            }

            return response;
        }

        public async Task<CloseAccountResponse> CloseAccountAsync(CloseAccountRequest request)
        {
            var response = new CloseAccountResponse();

            var customerAccount = await repository.GetCustomerAccountAsync(request.CustomerId, request.AccountId);
            if (customerAccount != null && customerAccount.Balance == decimal.Zero)
            {
                response.CustomerId = request.CustomerId;
                response.AccountId = request.AccountId;
                var updatedCustomerAccount = await repository.MarkAccountAsClosedAsync(request.CustomerId, request.AccountId);
                response.Succeeded = updatedCustomerAccount != null && updatedCustomerAccount.Closed;
            }

            return response;
        }

        public async Task<OpenAccountResponse> OpenAccountAsync(OpenAccountRequest request)
        {
            var response = new OpenAccountResponse();

            if (request.InitialDeposit >= 100m)
            {
                var customer = await repository.GetCustomerAsync(request.CustomerId);
                if (customer != null)
                {
                    var newAccount = await repository.CreateAccountAsync(request.CustomerId, request.InitialDeposit,
                        customer.Accounts.Count == 0 ? 
                        AccountType.Savings : 
                        request.AccountTypeId);

                    if (newAccount != null)
                    {
                        response.CustomerId = request.CustomerId;
                        response.AccountId = newAccount.Id;
                        response.AccountTypeId = newAccount.AccountTypeId;
                        response.Balance = newAccount.Balance;
                        response.Succeeded = true;
                    }
                }
            }            

            return response;
        }
    }
}
