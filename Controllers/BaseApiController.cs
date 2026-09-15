using Microsoft.AspNetCore.Mvc;

namespace JudjodApi.Controllers;

public abstract class BaseApiController : ControllerBase
{
    private readonly IConfiguration _config;
    protected BaseApiController(IConfiguration config) => _config = config;

    protected bool IsAuthorized()
    {
        var key = _config["ApiKey"];
        if (string.IsNullOrEmpty(key)) return false;
        Request.Headers.TryGetValue("X-API-Key", out var fromHeader);
        var fromQuery = Request.Query["key"].ToString();
        return fromHeader == key || fromQuery == key;
    }
}
