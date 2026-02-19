using Microsoft.EntityFrameworkCore;
using SecureAgentPortal.Data;
using SecureAgentPortal.Models.Domain;

namespace SecureAgentPortal.Repositories;

public class TransactionRepository(ApplicationDbContext db) : ITransactionRepository
{
    public Task<List<Transaction>> GetAllAsync() =>
        db.Transactions.OrderByDescending(t => t.CreatedAtUtc).ToListAsync();

    public Task<Transaction?> GetByIdAsync(int id) =>
        db.Transactions.FirstOrDefaultAsync(t => t.Id == id);

    public async Task AddAsync(Transaction transaction)
    {
        db.Transactions.Add(transaction);
        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Transaction transaction)
    {
        db.Transactions.Update(transaction);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Transaction transaction)
    {
        db.Transactions.Remove(transaction);
        await db.SaveChangesAsync();
    }
}
