using Microsoft.AspNetCore.Identity;
using SecureAgentPortal.Models.Domain;
using SecureAgentPortal.Repositories;

namespace SecureAgentPortal.Services;

public class TransactionService(
    ITransactionRepository repo,
    IAuditService audit,
    UserManager<IdentityUser> userManager,
    IHttpContextAccessor http
)
{
    public Task<List<Transaction>> GetAllAsync() => repo.GetAllAsync();

    public Task<Transaction?> GetByIdAsync(int id) => repo.GetByIdAsync(id);

    public async Task<int> CreateAsync(string payeeName, decimal amount, string currency)
    {
        var user = http.HttpContext?.User;
        var identityUser = user is not null ? await userManager.GetUserAsync(user) : null;

        var tx = new Transaction
        {
            PayeeName = payeeName,
            Amount = amount,
            Currency = currency,
            CreatedByUserId = identityUser?.Id ?? ""
        };

        await repo.AddAsync(tx);
        await audit.LogAsync("Created", "Transaction", tx.Id.ToString(), $"Created tx for {payeeName} amount {amount} {currency}");
        return tx.Id;
    }

    public async Task ApproveAsync(int id)
        {
            var tx = await repo.GetByIdAsync(id);
            if (tx is null) throw new InvalidOperationException("Transaction not found.");

            tx.Status = "Approved";
            await repo.UpdateAsync(tx);

            await audit.LogAsync("Approved", "Transaction", tx.Id.ToString(), $"Approved tx {tx.ReferenceId}");
        }

        public async Task RejectAsync(int id, string reason)
        {
            var tx = await repo.GetByIdAsync(id);
            if (tx is null) throw new InvalidOperationException("Transaction not found.");

            tx.Status = "Rejected";
            await repo.UpdateAsync(tx);

            await audit.LogAsync("Rejected", "Transaction", tx.Id.ToString(), $"Rejected tx {tx.ReferenceId}. Reason: {reason}");
        }

}
