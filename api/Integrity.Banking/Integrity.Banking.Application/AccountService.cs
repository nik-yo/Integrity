using Integrity.Banking.Domain;
using Integrity.Banking.Domain.Models;

namespace Integrity.Banking.Application
{
    public class AccountService(AccountHandler accountHandler)
    {
        public async Task<CloseAccountData?> ProcessCloseAccountAsync(CloseAccountData data)
        {
            return await accountHandler.CloseAccountAsync(data);
        }

        public async Task<OpenAccountData?> ProcessOpenAccountAsync(OpenAccountData data)
        {
            return await accountHandler.OpenAccountAsync(data);
        }
    }
}
