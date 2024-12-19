using Integrity.Banking.Application;
using Integrity.Banking.Domain;
using Integrity.Banking.Domain.Models;
using Integrity.Banking.Domain.Models.Config;
using Microsoft.Extensions.Logging;

namespace Integrity.Banking.Tests
{
    public class MakeDepositUnitTest
    {
        private readonly TestRepository repository = new();
        private readonly DbConfig dbConfig = new();

        [Fact]
        public async Task MakeValidDeposit_ReturnBalanceAdded()
        {
            var logger = TestLoggerFactory.CreateLogger();
            var depositHandler = new DepositHandler(repository, dbConfig);
            var depositService = new DepositService(depositHandler);

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
                Amount = 110
            };

            var actualOutput = await depositService.ProcessDepositAsync(inputData);

            Assert.NotNull(actualOutput);
            if (actualOutput != null)
            {
                Assert.Equal(expectedOutput.CustomerId, actualOutput.CustomerId);
                Assert.Equal(expectedOutput.AccountId, actualOutput.AccountId);
                Assert.Equal(expectedOutput.Amount, actualOutput.Amount);
            }
        }

        [Fact]
        public async Task MakeDepositWithInvalidCustomer_ReturnNull()
        {
            var logger = TestLoggerFactory.CreateLogger();
            var depositHandler = new DepositHandler(repository, dbConfig);
            var depositService = new DepositService(depositHandler);

            var inputData = new TransactionData
            {
                CustomerId = 3,
                AccountId = 1,
                Amount = 10
            };

            var actualOutput = await depositService.ProcessDepositAsync(inputData);

            Assert.Null(actualOutput);
        }

        [Fact]
        public async Task MakeDepositWithInvalidAccount_ReturnNull()
        {
            var logger = TestLoggerFactory.CreateLogger();
            var depositHandler = new DepositHandler(repository, dbConfig);
            var depositService = new DepositService(depositHandler);

            var inputData = new TransactionData
            {
                CustomerId = 1,
                AccountId = 3,
                Amount = 10
            };

            var actualOutput = await depositService.ProcessDepositAsync(inputData);

            Assert.Null(actualOutput);
        }

        [Fact]
        public async Task MakeDepositWithZeroAmount_ReturnNull()
        {
            var logger = TestLoggerFactory.CreateLogger();
            var depositHandler = new DepositHandler(repository, dbConfig);
            var depositService = new DepositService(depositHandler);

            var inputData = new TransactionData
            {
                CustomerId = 1,
                AccountId = 1,
                Amount = 0
            };

            var actualOutput = await depositService.ProcessDepositAsync(inputData);

            Assert.Null(actualOutput);
        }

        [Fact]
        public async Task MakeDuplicateDeposits_ReturnCorrectBalance()
        {
            var logger = TestLoggerFactory.CreateLogger();
            var depositHandler = new DepositHandler(repository, dbConfig);
            var depositService = new DepositService(depositHandler);

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
                Amount = 110
            };

            await depositService.ProcessDepositAsync(inputData);

            var actualOutput = await depositService.ProcessDepositAsync(inputData);

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
