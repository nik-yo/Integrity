using Integrity.Banking.Application;
using Integrity.Banking.Domain;
using Integrity.Banking.Domain.Models;
using Integrity.Banking.Domain.Models.Config;

namespace Integrity.Banking.Tests
{
    public class OpenAccountUnitTest
    {
        private readonly TestRepository repository = new();
        private readonly DbConfig dbConfig = new();

        [Fact]
        public async Task OpenAccountWithInvalidCustomer_ReturnNull()
        {
            var logger = TestLoggerFactory.CreateLogger();
            var accountHandler = new AccountHandler(repository, dbConfig);
            var accountService = new AccountService(accountHandler);

            var inputData = new OpenAccountData
            {
                CustomerId = 3,
                Amount = 100m,
                AccountTypeId = AccountType.Checking
            };

            var actualOutput = await accountService.ProcessOpenAccountAsync(inputData);

            Assert.NotNull(actualOutput);
        }

        [Fact]
        public async Task OpenAccountWithLessThan100_ReturnNull()
        {
            var logger = TestLoggerFactory.CreateLogger();
            var accountHandler = new AccountHandler(repository, dbConfig);
            var accountService = new AccountService(accountHandler);

            var inputData = new OpenAccountData
            {
                CustomerId = 1,
                Amount = 75m,
                AccountTypeId = AccountType.Checking
            };

            var actualOutput = await accountService.ProcessOpenAccountAsync(inputData);

            Assert.Null(actualOutput);
        }

        [Fact]
        public async Task OpenFirstCheckingAccount_ReturnNull()
        {
            var logger = TestLoggerFactory.CreateLogger();
            var accountHandler = new AccountHandler(repository, dbConfig);
            var accountService = new AccountService(accountHandler);

            var inputData = new OpenAccountData
            {
                CustomerId = 2,
                Amount = 100m,
                AccountTypeId = AccountType.Checking
            };

            var actualOutput = await accountService.ProcessOpenAccountAsync(inputData);

            Assert.Null(actualOutput);
        }

        [Fact]
        public async Task OpenFirstSavingsAccount_ReturnSucceededTrue()
        {
            var logger = TestLoggerFactory.CreateLogger();
            var accountHandler = new AccountHandler(repository, dbConfig);
            var accountService = new AccountService(accountHandler);

            var inputData = new OpenAccountData
            {
                CustomerId = 2,
                Amount = 100m,
                AccountTypeId = AccountType.Savings
            };

            var expectedOutput = new OpenAccountData
            {
                CustomerId = 2,
                AccountId = 3,
                Amount = 100m,
                AccountTypeId = AccountType.Savings
            };

            var actualOutput = await accountService.ProcessOpenAccountAsync(inputData);

            Assert.NotNull(actualOutput);
            if (actualOutput != null)
            {
                Assert.Equal(expectedOutput.AccountId, actualOutput.AccountId);
                Assert.Equal(expectedOutput.CustomerId, actualOutput.CustomerId);
                Assert.Equal(expectedOutput.Amount, expectedOutput.Amount);
            }
        }

        [Fact]
        public async Task OpenSecondAccount_ReturnSucceededTrue()
        {
            var logger = TestLoggerFactory.CreateLogger();
            var accountHandler = new AccountHandler(repository, dbConfig);
            var accountService = new AccountService(accountHandler);

            var inputData = new OpenAccountData
            {
                CustomerId = 1,
                Amount = 120m,
                AccountTypeId = AccountType.Savings
            };

            var expectedOutput = new OpenAccountData
            {
                CustomerId = 1,
                AccountId = 3,
                Amount = 120m,
                AccountTypeId = AccountType.Savings
            };

            var actualOutput = await accountService.ProcessOpenAccountAsync(inputData);

            Assert.NotNull(actualOutput);
            if (actualOutput != null)
            {
                Assert.Equal(expectedOutput.CustomerId, actualOutput.CustomerId);
                Assert.Equal(expectedOutput.AccountId, actualOutput.AccountId);
                Assert.Equal(expectedOutput.Amount, actualOutput.Amount);
                Assert.Equal(expectedOutput.AccountTypeId, actualOutput.AccountTypeId);
            }
        }
    }
}
