using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureAgentPortal.Services;

namespace SecureAgentPortal.Controllers;

[Authorize(Roles = "Admin,Agent")]
public class TransactionsController : Controller
{
    private readonly TransactionService _service;

    public TransactionsController(TransactionService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        var txs = await _service.GetAllAsync();
        return View(txs);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string payeeName, decimal amount, string currency = "USD")
    {
        if (string.IsNullOrWhiteSpace(payeeName))
            ModelState.AddModelError("", "Payee name is required.");

        if (amount <= 0)
            ModelState.AddModelError("", "Amount must be greater than 0.");

        if (!ModelState.IsValid)
            return View();

        await _service.CreateAsync(payeeName.Trim(), amount, currency.Trim().ToUpperInvariant());
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        await _service.ApproveAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id, string reason)
    {
        await _service.RejectAsync(id, reason ?? "");
        return RedirectToAction(nameof(Index));
    }

}
