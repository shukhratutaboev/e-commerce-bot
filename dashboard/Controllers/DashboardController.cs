using System.ComponentModel;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using dashboard.Models;
using dashboard.Sevices;
using dashboard.Entities;
using dashboard.Mappers;

namespace dashboard.Controllers;

public class DashboardController : Controller
{
    private readonly ILogger<DashboardController> _logger;
    private readonly IService<Category> _cts;
    private readonly IService<Item> _its;

    public DashboardController(ILogger<DashboardController> logger,
                                IService<Category> cts,
                                IService<Item> its)
    {
        _logger = logger;
        _cts = cts;
        _its = its;
    }
    public async Task<IActionResult> Home()
    { 
        return View();
    }
    public async Task<IActionResult> Categories()
    {
        var categoryList = await _cts.GetAllAsync();
        return View(categoryList);
    }
    public async Task<IActionResult> CategoryCreate()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CategoryCreate(NewCategory obj)
    {
        if(ModelState.IsValid)
        {
            await _cts.CreateAsync(obj.ToEntity());
            return RedirectToAction("Categories");
        }
        return View(obj);
    }
    public async Task<IActionResult> Items()
    {
        var itemList = await _its.GetAllAsync();
        return View(itemList);
    }
    public async Task<IActionResult> ItemCreate()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ItemCreate(NewItem obj)
    {
        if(ModelState.IsValid)
        {
            await _its.CreateAsync(obj.ToEntity(await _cts.GetAsync(obj.CategoryId)));
            return RedirectToAction("Items");
        }
        return View(obj);
    }
    public async Task<IActionResult> ItemEdit(Guid id)
    {
        var item = await _its.GetAsync(id);
        if(id == null && item == default) return NotFound();
        return View(item.ToModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ItemEdit(NewItem obj, Guid id)
    {
        if(ModelState.IsValid)
        {
            var item = await _its.GetAsync(id);
            item.Name = obj.Name;
            item.Cost = obj.Cost;
            item.CategoryId = obj.CategoryId;
            item.Category = await _cts.GetAsync(obj.CategoryId);
            if(obj.ImageUrl != default)
            {
                item.ImageUrl = obj.ImageUrl.toByte();
            }
            await _its.UpdateAsync(item);
            return RedirectToAction("Items");
        }
        return View(obj);
    }
}
