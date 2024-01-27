using GoodsApi.Data;
using GoodsApi.Data.Entities;
using GoodsApi.Models;
using GoodsApi.Models.Identity;
using GoodsApi.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoodsApi.Controllers;


[ApiController]
[Route("/api/Order")]
public class OrderController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public OrderController(ITokenService tokenService, DataContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _tokenService = tokenService;
        _context = context;
        _userManager = userManager;
        _configuration = configuration;
    }


    [Authorize]
    [HttpPost("/api/Order")]
    public async Task<ActionResult> AddOrder(List<long> itemsId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return NotFound("User not found");
        }

        var items = _context.CartItems.Where(i => i.UserId == user.Id).ToList();
        var orderItems = new List<OrderItem>();

        var order = new Order
        {
            UserId = user.Id,
            ItemsId = itemsId,
            OrderDate = DateTime.Now.ToString()
        };


        _context.Orders.Add(order);


        await _context.CartItems.Where(i => i.UserId == user.Id).ExecuteDeleteAsync();


        await _context.SaveChangesAsync();

        foreach (var item in items)
        {
            orderItems.Add(new()
            {
                OrderId = order.Id,
                Product = item.Product,
                Quantity = item.Quantity,
                ProductId = item.ProductId,
            });
        }

        _context.OrderItems.AddRange(orderItems);


        await _context.SaveChangesAsync();

        return Ok(new { orderId = order.Id, Message = "Order added successfully" });
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> ClearOrder(long id)
    {
        var user = await _userManager.GetUserAsync(User);

        var order = await _context.Orders.FindAsync(id);

        _context.OrderItems.RemoveRange(_context.OrderItems.Where(i => i.OrderId == id));
        
        _context.Orders.Remove(order);

        var result = await _context.SaveChangesAsync();

        if (result > 0)
        {
            return Ok("Order deleted successfully");
        }
        else
        {
            return BadRequest("Failed to delete Order");
        }
    }


    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(long id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound("user not found");
        var order = await _context.Orders.FindAsync(id);
        return Ok(new { order.Id, order.Items});
    }

    [Authorize]
    [HttpGet("/items/{id}")]
    public async Task<IActionResult> GetOrderItems(long id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound("user not found");
        var orderItems = _context.OrderItems.Where(i => i.OrderId == id).ToList();
        return Ok(orderItems);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound("user not found");
        var items = await _context.Orders.Select(item => new { item.Id, item.UserId, item.ItemsId, item.Items, item.OrderDate}).Where(item => item.UserId == user.Id).ToListAsync();
        return Ok(items);
    }

}
