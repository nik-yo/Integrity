using Integrity.Banking.Application;
using Integrity.Banking.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrity.Banking.Tests
{
    public class MakeWithdrawalUnitTest
    {
        private readonly TestRepository repository = new();

        [Fact]
        public async Task MakeValidWithdrawal_ReturnBalanceSubtracted()
        {
            var bankingService = new BankingService(repository);

            var transactionRequest = new TransactionRequest
            {
                CustomerId = 1,
                AccountId = 1,
                Amount = 10
            };

            var expectedResponse = new TransactionResponse
            {
                CustomerId = 1,
                AccountId = 1,
                Balance = 90,
                Succeeded = true,
            };

            var actualResponse = await bankingService.MakeWithdrawalAsync(transactionRequest);

            Assert.Equal(expectedResponse.CustomerId, actualResponse.CustomerId); //Customer exists
            Assert.Equal(expectedResponse.AccountId, actualResponse.AccountId); //Account exists
            Assert.Equal(expectedResponse.Balance, actualResponse.Balance);
            Assert.True(actualResponse.Succeeded);
        }

        [Fact]
        public async Task MakeWithdrawalWithInvalidCustomer_ReturnFailed()
        {
            var bankingService = new BankingService(repository);

            var transactionRequest = new TransactionRequest
            {
                CustomerId = 2,
                AccountId = 1,
                Amount = 10
            };

            var expectedResponse = new TransactionResponse
            {
                CustomerId = 0,
                AccountId = 0,
                Balance = 0,
                Succeeded = false,
            };

            var actualResponse = await bankingService.MakeWithdrawalAsync(transactionRequest);

            Assert.Equal(expectedResponse.CustomerId, actualResponse.CustomerId);
            Assert.Equal(expectedResponse.AccountId, actualResponse.AccountId);
            Assert.Equal(expectedResponse.Balance, actualResponse.Balance);
            Assert.False(actualResponse.Succeeded);
        }

        [Fact]
        public async Task MakeWithdrawalWithInvalidAccount_ReturnFailed()
        {
            var bankingService = new BankingService(repository);

            var transactionRequest = new TransactionRequest
            {
                CustomerId = 1,
                AccountId = 3,
                Amount = 10
            };

            var expectedResponse = new TransactionResponse
            {
                CustomerId = 0,
                AccountId = 0,
                Balance = 0,
                Succeeded = false,
            };

            var actualResponse = await bankingService.MakeWithdrawalAsync(transactionRequest);

            Assert.Equal(expectedResponse.CustomerId, actualResponse.CustomerId);
            Assert.Equal(expectedResponse.AccountId, actualResponse.AccountId);
            Assert.Equal(expectedResponse.Balance, actualResponse.Balance);
            Assert.False(actualResponse.Succeeded);
        }

        [Fact]
        public async Task MakeWithdrawalWithZeroAmount_ReturnFailed()
        {
            var bankingService = new BankingService(repository);

            var transactionRequest = new TransactionRequest
            {
                CustomerId = 1,
                AccountId = 1,
                Amount = 0
            };

            var expectedResponse = new TransactionResponse
            {
                CustomerId = 0,
                AccountId = 0,
                Balance = 0,
                Succeeded = false,
            };

            var actualResponse = await bankingService.MakeWithdrawalAsync(transactionRequest);

            Assert.Equal(expectedResponse.CustomerId, actualResponse.CustomerId);
            Assert.Equal(expectedResponse.AccountId, actualResponse.AccountId);
            Assert.Equal(expectedResponse.Balance, actualResponse.Balance);
            Assert.False(actualResponse.Succeeded);
        }

        [Fact]
        public async Task MakeWithdrawalWithAmountMoreThanBalance_ReturnFailed()
        {
            var bankingService = new BankingService(repository);

            var transactionRequest = new TransactionRequest
            {
                CustomerId = 1,
                AccountId = 1,
                Amount = 150
            };

            var expectedResponse = new TransactionResponse
            {
                CustomerId = 1,
                AccountId = 1,
                Balance = 100,
                Succeeded = false,
            };

            var actualResponse = await bankingService.MakeWithdrawalAsync(transactionRequest);

            Assert.Equal(expectedResponse.CustomerId, actualResponse.CustomerId);
            Assert.Equal(expectedResponse.AccountId, actualResponse.AccountId);
            Assert.Equal(expectedResponse.Balance, actualResponse.Balance);
            Assert.False(actualResponse.Succeeded);
        }
    }
}
