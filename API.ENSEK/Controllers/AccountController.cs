using CORE.ENSEK.Models;
using CORE.ENSEK.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.ENSEK.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : Controller
{
    private readonly AccountService _accountService;

    public AccountController(AccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost("account-data-upload")]
    public async Task<IActionResult> UploadUsersData([FromBody] UploadRequest request)
    {
        var result = await _accountService.InsertUserAccounts(request.Data);

        return Ok(new
        {
            result
        });
    }
    
    [HttpPost("meter-reading-uploads")]
    public async Task<IActionResult> UploadMeterReadings([FromBody] UploadRequest request)
    {
        var result = await _accountService.InsertReadings(request.Data);

        return Ok(new
        {
            result.failed,
            result.success
        });
    }
    
    [HttpGet("get-all-accounts")]
    public async Task<IActionResult> GetAccounts()
    {
        var result = await _accountService.GetAllAccounts();

        return Ok(new
        {
            result
        });
    }
    
    [HttpGet("get-account-by-id/{id}")]
    public async Task<IActionResult> GetAccountById(string id)
    {
        var result = await _accountService.GetAccount(id);

        return Ok(new
        {
            result
        });
    }
    
}