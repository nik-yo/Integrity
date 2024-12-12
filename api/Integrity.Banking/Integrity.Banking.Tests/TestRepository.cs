using Integrity.Banking.Domain.Models;
using Integrity.Banking.Domain.Models.Database;
using Integrity.Banking.Domain.Repositories;

namespace Integrity.Banking.Tests
{
    public class TestRepository : IBankingRepository
    {
        private readonly Customer _customer = new Customer()
        {
            Id = 1
        };

        private readonly Customer _secondCustomer = new Customer()
        {
            Id = 2
        };

        private readonly Account _account = new Account()
        {
            Id = 1,
            AccountTypeId = AccountType.Savings,
            Balance = 100m
        };

        private readonly Account _secondAccount = new Account()
        {
            Id = 2,
            AccountTypeId = AccountType.Checking,
            Balance = decimal.Zero
        };

        public List<Account> Accounts { get; set; } = [];
        private List<Customer> Customers { get; set; } = [];

        private List<Tuple<Customer, Account>> _xref = [];

        public TestRepository()
        {
            Customers.Add(_customer);
            Customers.Add(_secondCustomer);
            Accounts.Add(_account);
            Accounts.Add(_secondAccount);
            _xref.Add(new(_customer,_account));
            _xref.Add(new(_customer, _secondAccount));
        }

        public async Task<Customer?> GetCustomerAsync(int customerId)
        {
            var customer = Customers.FirstOrDefault(c => c.Id == customerId);
            if (customer != null)
            {
                var accounts = _xref.Where(x => x.Item1 == customer).Select(x => x.Item2);
                customer.Accounts.AddRange(accounts);
            }
            return customer;
        }

        public async Task<Account?> GetCustomerAccountAsync(int customerId, int accountId)
        {
            return _xref.FirstOrDefault(x => x.Item1.Id == customerId && x.Item2.Id == accountId && !x.Item2.Closed)?.Item2;
        }

        public async Task<Account?> UpdateAccountBalanceAsync(int customerId, int accountId, decimal balance)
        {
            var customerAccount = await GetCustomerAccountAsync(customerId, accountId);
            if (customerAccount != null)
            {
                customerAccount.Balance = balance;
            }
            return customerAccount;
        }

        public async Task<Account?> MarkAccountAsClosedAsync(int customerId, int accountId)
        {
            var customerAccount = await GetCustomerAccountAsync(customerId, accountId);
            if (customerAccount != null)
            {
                customerAccount.Closed = true;
            }
            return customerAccount;
        }

        public async Task<Account?> CreateAccountAsync(int customerId, decimal balance, AccountType accountTypeId)
        {
            var customer = await GetCustomerAsync(customerId);
            if (customer != null)
            {
                var newAccount = new Account
                {
                    Id = Accounts.Max(a => a.Id) + 1,
                    Balance = balance,
                    AccountTypeId = accountTypeId
                };
                Accounts.Add(newAccount);
                _xref.Add(new(customer, newAccount));
                return newAccount;
            }
            return null;
        }        
    }
}
