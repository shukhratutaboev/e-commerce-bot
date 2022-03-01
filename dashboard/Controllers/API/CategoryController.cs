using dashboard.Entities;
using dashboard.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace dashboard.Controllers.API;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ILogger<CategoryController> _logger;
    private readonly IService<Category> _cts;
    public CategoryController(ILogger<CategoryController> logger,
                                IService<Category> cts)
    {
        _logger = logger;
        _cts = cts;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var json = JsonConvert.SerializeObject(
            await _cts.GetAllAsync(), Formatting.Indented,
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
            await _cts.GetAsync(id), Formatting.Indented,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }
        );
        return Ok(json);
    }
}