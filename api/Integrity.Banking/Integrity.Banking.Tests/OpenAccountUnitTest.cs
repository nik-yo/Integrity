using Integrity.Banking.Application;
using Integrity.Banking.Domain.Models;

namespace Integrity.Banking.Tests
{
    public class OpenAccountUnitTest
    {
        private readonly TestRepository repository = new();

        [Fact]
        public async Task OpenAccountWithInvalidCustomer_ReturnSucceededFalse()
        {
            var bankingService = new BankingService(repository);

            var openAccountRequest = new OpenAccountRequest
            {
                CustomerId = 3,
                InitialDeposit = 100m,
                AccountTypeId = AccountType.Checking
            };

            var expectedResponse = new OpenAccountResponse
            {
                CustomerId = 0,
                AccountId = 0,
                Balance = 0,
                Succeeded = false,
            };

            var actualResponse = await bankingService.OpenAccountAsync(openAccountRequest);

            Assert.Equal(expectedResponse.CustomerId, actualResponse.CustomerId);
            Assert.Equal(expectedResponse.AccountId, actualResponse.AccountId);
            Assert.Equal(expectedResponse.Balance, actualResponse.Balance);
            Assert.False(actualResponse.Succeeded);
        }

        [Fact]
        public async Task OpenAccountWithLessThan100_ReturnSucceededFalse()
        {
            var bankingService = new BankingService(repository);

            var openAccountRequest = new OpenAccountRequest
            {
                CustomerId = 1,
                InitialDeposit = 75m,
                AccountTypeId = AccountType.Checking
            };

            var expectedResponse = new OpenAccountResponse
            {
                CustomerId = 0,
                AccountId = 0,
                Balance = 0,
                Succeeded = false,
            };

            var actualResponse = await bankingService.OpenAccountAsync(openAccountRequest);

            Assert.Equal(expectedResponse.CustomerId, actualResponse.CustomerId);
            Assert.Equal(expectedResponse.AccountId, actualResponse.AccountId);
            Assert.Equal(expectedResponse.Balance, actualResponse.Balance);
            Assert.False(actualResponse.Succeeded);
        }

        [Fact]
        public async Task OpenFirstCheckingAccount_ReturnSucceededFalse()
        {
            var bankingService = new BankingService(repository);

            var openAccountRequest = new OpenAccountRequest
            {
                CustomerId = 2,
                InitialDeposit = 100m,
                AccountTypeId = AccountType.Checking
            };

            var expectedResponse = new OpenAccountResponse
            {
                CustomerId = 0,
                AccountId = 0,
                Balance = 0,
                Succeeded = false,
            };

            var actualResponse = await bankingService.OpenAccountAsync(openAccountRequest);

            Assert.Equal(expectedResponse.CustomerId, actualResponse.CustomerId);
            Assert.Equal(expectedResponse.AccountId, actualResponse.AccountId);
            Assert.Equal(expectedResponse.Balance, actualResponse.Balance);
            Assert.False(actualResponse.Succeeded);
        }

        [Fact]
        public async Task OpenFirstSavingsAccount_ReturnSucceededTrue()
        {
            var bankingService = new BankingService(repository);

            var openAccountRequest = new OpenAccountRequest
            {
                CustomerId = 2,
                InitialDeposit = 100m,
                AccountTypeId = AccountType.Savings
            };

            var expectedResponse = new OpenAccountResponse
            {
                CustomerId = 2,
                AccountId = 3,
                Balance = 100m,
                Succeeded = true,
            };

            var actualResponse = await bankingService.OpenAccountAsync(openAccountRequest);

            Assert.Equal(expectedResponse.CustomerId, actualResponse.CustomerId);
            Assert.Equal(expectedResponse.AccountId, actualResponse.AccountId);
            Assert.Equal(expectedResponse.Balance, actualResponse.Balance);
            Assert.True(actualResponse.Succeeded);
        }

        [Fact]
        public async Task OpenSecondAccount_ReturnSucceededTrue()
        {
            var bankingService = new BankingService(repository);

            var openAccountRequest = new OpenAccountRequest
            {
                CustomerId = 1,
                InitialDeposit = 120m,
                AccountTypeId = AccountType.Savings
            };

            var expectedResponse = new OpenAccountResponse
            {
                CustomerId = 1,
                AccountId = 3,
                Balance = 120m,
                Succeeded = true,
            };

            var actualResponse = await bankingService.OpenAccountAsync(openAccountRequest);

            Assert.Equal(expectedResponse.CustomerId, actualResponse.CustomerId);
            Assert.Equal(expectedResponse.AccountId, actualResponse.AccountId);
            Assert.Equal(expectedResponse.Balance, actualResponse.Balance);
            Assert.True(actualResponse.Succeeded);
        }
    }
}
