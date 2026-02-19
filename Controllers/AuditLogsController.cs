using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureAgentPortal.Data;

namespace SecureAgentPortal.Controllers;

[Authorize(Roles = "Admin,Auditor")]
public class AuditLogsController : Controller
{
    private readonly ApplicationDbContext _db;

    public AuditLogsController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var logs = await _db.AuditLogs
            .OrderByDescending(a => a.TimestampUtc)
            .Take(200)
            .ToListAsync();

        return View(logs);
    }
}
