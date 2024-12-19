using Integrity.Banking.Application;
using Integrity.Banking.Domain;
using Integrity.Banking.Domain.Models;
using Integrity.Banking.Domain.Models.Config;

namespace Integrity.Banking.Tests
{
    public class CloseAccountUnitTest
    {
        private readonly TestRepository repository = new();
        private readonly DbConfig dbConfig = new();

        [Fact]
        public async Task CloseInvalidAccount_ReturnNull()
        {
            var logger = TestLoggerFactory.CreateLogger();
            var accountHandler = new AccountHandler(repository, dbConfig);
            var accountService = new AccountService(accountHandler);

            var inputData = new CloseAccountData
            {
                CustomerId = 1,
                AccountId = 3
            };

            var actualOutput = await accountService.ProcessCloseAccountAsync(inputData);

            Assert.Null(actualOutput);
        }

        [Fact]
        public async Task CloseAccountWithNonZeroBalance_ReturnNull()
        {
            var logger = TestLoggerFactory.CreateLogger();
            var accountHandler = new AccountHandler(repository, dbConfig);
            var accountService = new AccountService(accountHandler);

            var inputData = new CloseAccountData
            {
                CustomerId = 1,
                AccountId = 1
            };

            var actualOutput = await accountService.ProcessCloseAccountAsync(inputData);

            Assert.Null(actualOutput);
        }

        [Fact]
        public async Task CloseValidAccount_ReturnAccountData()
        {
            var logger = TestLoggerFactory.CreateLogger();
            var accountHandler = new AccountHandler(repository, dbConfig);
            var accountService = new AccountService(accountHandler);

            var inputData = new CloseAccountData
            {
                CustomerId = 1,
                AccountId = 2
            };

            var expectedOutput = new CloseAccountData
            {
                CustomerId = 1,
                AccountId = 2,
            };

            var actualOutput = await accountService.ProcessCloseAccountAsync(inputData);

            Assert.NotNull(actualOutput);
            if (actualOutput != null)
            {
                Assert.Equal(expectedOutput.CustomerId, actualOutput.CustomerId);
                Assert.Equal(expectedOutput.AccountId, actualOutput.AccountId);
            }
        }
    }
}
