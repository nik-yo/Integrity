using Integrity.Banking.Application;
using Integrity.Banking.Domain.Models;
using Integrity.Banking.Domain.Models.Config;

namespace Integrity.Banking.Tests
{
    public class MakeWithdrawalUnitTest
    {
        private readonly TestRepository repository = new();
        private readonly DbConfig dbConfig = new();

        [Fact]
        public async Task MakeValidWithdrawal_ReturnBalanceSubtracted()
        {
            var logger = TestLoggerFactory.CreateLogger();
            var bankingService = new BankingService(dbConfig, repository, logger);

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
            var logger = TestLoggerFactory.CreateLogger();
            var bankingService = new BankingService(dbConfig, repository, logger);

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
            var logger = TestLoggerFactory.CreateLogger();
            var bankingService = new BankingService(dbConfig, repository, logger);

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
            var logger = TestLoggerFactory.CreateLogger();
            var bankingService = new BankingService(dbConfig, repository, logger);

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
            var logger = TestLoggerFactory.CreateLogger();
            var bankingService = new BankingService(dbConfig, repository, logger);

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

        [Fact]
        public async Task MakeDuplicateWithdrawals_ReturnCorrectBalance()
        {
            var logger = TestLoggerFactory.CreateLogger();
            var bankingService = new BankingService(dbConfig, repository, logger);

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

            await bankingService.MakeWithdrawalAsync(transactionRequest);

            var actualResponse = await bankingService.MakeWithdrawalAsync(transactionRequest);

            Assert.Equal(expectedResponse.CustomerId, actualResponse.CustomerId);
            Assert.Equal(expectedResponse.AccountId, actualResponse.AccountId);
            Assert.Equal(expectedResponse.Balance, actualResponse.Balance);
            Assert.True(actualResponse.Succeeded);
        }
    }
}
