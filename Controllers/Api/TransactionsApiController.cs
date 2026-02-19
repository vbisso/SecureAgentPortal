using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureAgentPortal.Services;

namespace SecureAgentPortal.Controllers.Api;

[ApiController]
[Route("api/transactions")]
[Authorize(Roles = "Admin,Agent")]
public class TransactionsApiController : ControllerBase
{
    private readonly TransactionService _service;

    public TransactionsApiController(TransactionService service)
    {
        _service = service;
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetCount()
    {
        var txs = await _service.GetAllAsync();
        return Ok(new { count = txs.Count });
    }
}
