﻿using Integrity.Banking.Application;
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
                Balance = 110,
                Succeeded = true,
            };

            var actualResponse = await bankingService.MakeDepositAsync(transactionRequest);

            Assert.Equal(expectedResponse.CustomerId, actualResponse.CustomerId); //Customer exists
            Assert.Equal(expectedResponse.AccountId, actualResponse.AccountId); //Account exists
            Assert.Equal(expectedResponse.Balance, actualResponse.Balance);
            Assert.True(actualResponse.Succeeded);
        }

        [Fact]
        public async Task MakeDepositWithInvalidCustomer_ReturnFailed()
        {
            var logger = TestLoggerFactory.CreateLogger();
            var bankingService = new BankingService(dbConfig, repository, logger);

            var transactionRequest = new TransactionRequest
            {
                CustomerId = 3,
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

            var actualResponse = await bankingService.MakeDepositAsync(transactionRequest);

            Assert.Equal(expectedResponse.CustomerId, actualResponse.CustomerId);
            Assert.Equal(expectedResponse.AccountId, actualResponse.AccountId);
            Assert.Equal(expectedResponse.Balance, actualResponse.Balance);
            Assert.False(actualResponse.Succeeded);
        }

        [Fact]
        public async Task MakeDepositWithInvalidAccount_ReturnFailed()
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

            var actualResponse = await bankingService.MakeDepositAsync(transactionRequest);

            Assert.Equal(expectedResponse.CustomerId, actualResponse.CustomerId);
            Assert.Equal(expectedResponse.AccountId, actualResponse.AccountId);
            Assert.Equal(expectedResponse.Balance, actualResponse.Balance);
            Assert.False(actualResponse.Succeeded);
        }

        [Fact]
        public async Task MakeDepositWithZeroAmount_ReturnFailed()
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

            var actualResponse = await bankingService.MakeDepositAsync(transactionRequest);

            Assert.Equal(expectedResponse.CustomerId, actualResponse.CustomerId);
            Assert.Equal(expectedResponse.AccountId, actualResponse.AccountId);
            Assert.Equal(expectedResponse.Balance, actualResponse.Balance);
            Assert.False(actualResponse.Succeeded);
        }

        [Fact]
        public async Task MakeDuplicateDeposits_ReturnCorrectBalance()
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
                Balance = 110,
                Succeeded = true,
            };

            await bankingService.MakeDepositAsync(transactionRequest);

            var actualResponse = await bankingService.MakeDepositAsync(transactionRequest);

            Assert.Equal(expectedResponse.CustomerId, actualResponse.CustomerId);
            Assert.Equal(expectedResponse.AccountId, actualResponse.AccountId);
            Assert.Equal(expectedResponse.Balance, actualResponse.Balance);
            Assert.True(actualResponse.Succeeded);
        }
    }
}
