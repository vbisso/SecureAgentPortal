using Microsoft.AspNetCore.Identity;
using SecureAgentPortal.Data;
using SecureAgentPortal.Models.Domain;

namespace SecureAgentPortal.Services;

public class AuditService(
    ApplicationDbContext db,
    IHttpContextAccessor http,
    UserManager<IdentityUser> userManager
) : IAuditService
{
    public async Task LogAsync(string action, string entityType, string? entityId, string details = "")
    {
        var ctx = http.HttpContext;
        var user = ctx?.User;
        var identityUser = user is not null ? await userManager.GetUserAsync(user) : null;

        db.AuditLogs.Add(new AuditLog
        {
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Details = details,
            PerformedByUserId = identityUser?.Id ?? "anonymous",
            IpAddress = ctx?.Connection?.RemoteIpAddress?.ToString()
        });

        await db.SaveChangesAsync();
    }
}
