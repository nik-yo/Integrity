using Integrity.Banking.Domain;
using Integrity.Banking.Domain.Models;

namespace Integrity.Banking.Application
{
    public class WithdrawalService(WithdrawalHandler withdrawalHandler)
    {
        public async Task<TransactionData?> ProcessWithdrawalAsync(TransactionData data)
        {
            return await withdrawalHandler.MakeWithdrawalAsync(data);
        }
    }
}
