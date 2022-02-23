using dashboard.Entities;
using dashboard.Sevices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace dashboard.Controllers.API;

[ApiController]
[Route("api/[controller]")]
public class ItemController : ControllerBase
{
    private readonly ILogger<ItemController> _logger;
    private readonly IService<Item> _its;
    public ItemController(ILogger<ItemController> logger,
                                IService<Item> cts)
    {
        _logger = logger;
        _its = cts;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var json = JsonConvert.SerializeObject(
            await _its.GetAllAsync(), Formatting.Indented,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }
        );
        return Ok(json);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var json = JsonConvert.SerializeObject(
            await _its.GetAsync(id), Formatting.Indented,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }
        );
        return Ok(json);
    }
}