﻿using Integrity.Banking.Application;
using Integrity.Banking.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrity.Banking.Tests
{
    public class CloseAccountUnitTest
    {
        private readonly TestRepository repository = new();

        [Fact]
        public async Task CloseInvalidAccount_ReturnSucceededFalse()
        {
            var bankingService = new BankingService(repository);

            var closeAccountRequest = new CloseAccountRequest
            {
                CustomerId = 1,
                AccountId = 3
            };

            var expectedResponse = new CloseAccountResponse
            {
                CustomerId = 0,
                AccountId = 0,
                Succeeded = false,
            };

            var actualResponse = await bankingService.CloseAccountAsync(closeAccountRequest);

            Assert.Equal(expectedResponse.CustomerId, actualResponse.CustomerId);
            Assert.Equal(expectedResponse.AccountId, actualResponse.AccountId);
            Assert.False(actualResponse.Succeeded);
        }

        [Fact]
        public async Task CloseAccountWithNonZeroBalance_ReturnSucceededFalse()
        {
            var bankingService = new BankingService(repository);

            var closeAccountRequest = new CloseAccountRequest
            {
                CustomerId = 1,
                AccountId = 1
            };

            var expectedResponse = new CloseAccountResponse
            {
                CustomerId = 0,
                AccountId = 0,
                Succeeded = false,
            };

            var actualResponse = await bankingService.CloseAccountAsync(closeAccountRequest);

            Assert.Equal(expectedResponse.CustomerId, actualResponse.CustomerId);
            Assert.Equal(expectedResponse.AccountId, actualResponse.AccountId);
            Assert.False(actualResponse.Succeeded);
        }

        [Fact]
        public async Task CloseValidAccount_ReturnAccountNotDeleted()
        {
            var bankingService = new BankingService(repository);

            var closeAccountRequest = new CloseAccountRequest
            {
                CustomerId = 1,
                AccountId = 2
            };

            var expectedResponse = new CloseAccountResponse
            {
                CustomerId = 1,
                AccountId = 2,
                Succeeded = true,
            };

            var actualResponse = await bankingService.CloseAccountAsync(closeAccountRequest);

            Assert.Equal(expectedResponse.CustomerId, actualResponse.CustomerId);
            Assert.Equal(expectedResponse.AccountId, actualResponse.AccountId);
            Assert.True(actualResponse.Succeeded);
            Assert.NotNull(repository.Accounts.FirstOrDefault(a => a.Id == closeAccountRequest.AccountId));
        }
    }
}
