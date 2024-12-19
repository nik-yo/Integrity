using Integrity.Banking.Application;
using Integrity.Banking.Domain;
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
            var withdrawalHandler = new WithdrawalHandler(repository, dbConfig);
            var withdrawalService = new WithdrawalService(withdrawalHandler);

            var inputData = new TransactionData
            {
                CustomerId = 1,
                AccountId = 1,
                Amount = 10
            };

            var expectedOutput = new TransactionData
            {
                CustomerId = 1,
                AccountId = 1,
                Amount = 90
            };

            var actualOutput = await withdrawalService.ProcessWithdrawalAsync(inputData);

            Assert.NotNull(actualOutput);
            if (actualOutput != null)
            {
                Assert.Equal(expectedOutput.CustomerId, actualOutput.CustomerId);
                Assert.Equal(expectedOutput.AccountId, actualOutput.AccountId);
                Assert.Equal(expectedOutput.Amount, actualOutput.Amount);
            }
        }

        [Fact]
        public async Task MakeWithdrawalWithInvalidCustomer_ReturnNull()
        {
            var logger = TestLoggerFactory.CreateLogger();
            var withdrawalHandler = new WithdrawalHandler(repository, dbConfig);
            var withdrawalService = new WithdrawalService(withdrawalHandler);

            var inputData = new TransactionData
            {
                CustomerId = 2,
                AccountId = 1,
                Amount = 10
            };

            var actualOutput = await withdrawalService.ProcessWithdrawalAsync(inputData);

            Assert.Null(actualOutput);
        }

        [Fact]
        public async Task MakeWithdrawalWithInvalidAccount_ReturnNull()
        {
            var logger = TestLoggerFactory.CreateLogger();
            var withdrawalHandler = new WithdrawalHandler(repository, dbConfig);
            var withdrawalService = new WithdrawalService(withdrawalHandler);

            var inputData = new TransactionData
            {
                CustomerId = 1,
                AccountId = 3,
                Amount = 10
            };

            var actualOutput = await withdrawalService.ProcessWithdrawalAsync(inputData);

            Assert.Null(actualOutput);
        }

        [Fact]
        public async Task MakeWithdrawalWithZeroAmount_ReturnNull()
        {
            var logger = TestLoggerFactory.CreateLogger();
            var withdrawalHandler = new WithdrawalHandler(repository, dbConfig);
            var withdrawalService = new WithdrawalService(withdrawalHandler);

            var inputData = new TransactionData
            {
                CustomerId = 1,
                AccountId = 1,
                Amount = 0
            };

            var actualOutput = await withdrawalService.ProcessWithdrawalAsync(inputData);

            Assert.Null(actualOutput);
        }

        [Fact]
        public async Task MakeWithdrawalWithAmountMoreThanBalance_ReturnNull()
        {
            var logger = TestLoggerFactory.CreateLogger();
            var withdrawalHandler = new WithdrawalHandler(repository, dbConfig);
            var withdrawalService = new WithdrawalService(withdrawalHandler);

            var inputData = new TransactionData
            {
                CustomerId = 1,
                AccountId = 1,
                Amount = 150
            };

            var actualOutput = await withdrawalService.ProcessWithdrawalAsync(inputData);

            Assert.Null(actualOutput);
        }

        [Fact]
        public async Task MakeDuplicateWithdrawals_ReturnCorrectBalance()
        {
            var logger = TestLoggerFactory.CreateLogger();
            var withdrawalHandler = new WithdrawalHandler(repository, dbConfig);
            var withdrawalService = new WithdrawalService(withdrawalHandler);

            var inputData = new TransactionData
            {
                CustomerId = 1,
                AccountId = 1,
                Amount = 10
            };

            var expectedOutput = new TransactionData
            {
                CustomerId = 1,
                AccountId = 1,
                Amount = 90
            };

            await withdrawalService.ProcessWithdrawalAsync(inputData);

            var actualOutput = await withdrawalService.ProcessWithdrawalAsync(inputData);

            Assert.NotNull(actualOutput);
            if (actualOutput != null)
            {
                Assert.Equal(expectedOutput.CustomerId, actualOutput.CustomerId);
                Assert.Equal(expectedOutput.AccountId, actualOutput.AccountId);
                Assert.Equal(expectedOutput.Amount, actualOutput.Amount);
            }
        }
    }
}
