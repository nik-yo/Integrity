using Integrity.Banking.Domain;
using Integrity.Banking.Domain.Models;

namespace Integrity.Banking.Application
{
    public class DepositService(DepositHandler depositHandler)
    {
        public async Task<TransactionData?> ProcessDepositAsync(TransactionData data)
        {
            return await depositHandler.MakeDepositAsync(data);
        }
    }
}
