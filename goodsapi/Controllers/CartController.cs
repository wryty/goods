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
[Route("/api/Cart")]
public class CartController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public CartController(ITokenService tokenService, DataContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _tokenService = tokenService;
        _context = context;
        _userManager = userManager;
        _configuration = configuration;
    }


    [Authorize]
    [HttpPost("/api/cartitem")]
    public async Task<ActionResult> AddCartItem([FromBody] CartItemRequest request)
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

        var found = _context.CartItems.Where(i => i.ProductId == request.ProductId).ToList();
        if (found.Any()) 
        {
            found[0].Quantity += 1;
            await _context.SaveChangesAsync();
            return Ok(new { cartItemId = found[0].Id, Message = "Cart item updated successfully" });
        }
        
        var cartItem = new CartItem
        {
            ProductId = request.ProductId,
            UserId = user.Id,
            Quantity = request.Quantity,
            Product = await _context.Products.FindAsync(request.ProductId)
        };

        _context.CartItems.Add(cartItem);

        await _context.SaveChangesAsync();

        return Ok(new { cartItemId = cartItem.Id, Message = "Cart updated successfully" });
    }

    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        var user = await _userManager.GetUserAsync(User);

        await _context.CartItems.Where((i => i.UserId == user.Id)).ExecuteDeleteAsync();

        var result = await _context.SaveChangesAsync();

        if (result > 0)
        {
            return Ok("Cart deleted successfully");
        }
        else
        {
            return BadRequest("Failed to delete Cart");
        }
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveFromCart(long id)
    {
        var user = await _userManager.GetUserAsync(User);

        var cartItem = await _context.CartItems.FindAsync(id);


        _context.CartItems.Remove(cartItem);

        var result = await _context.SaveChangesAsync();

        if (result > 0)
        {
            return Ok("CartItem deleted successfully");
        }
        else
        {
            return BadRequest("Failed to delete CartItem");
        }
    }


    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCartItemById(long id)
    {

        var cartItem = await _context.CartItems.FindAsync(id);


        if (cartItem == null)
        {
            return Ok(cartItem);
        }
        else
        {
            return BadRequest("Failed to find CartItem");
        }
        
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> ChangeCartQuantity(long id, [FromBody] CartItemRequest request)
    {
        var cartItem = await _context.CartItems.FindAsync(id);

        cartItem.Quantity = request.Quantity;

        var result = await _context.SaveChangesAsync();

        if (result > 0)
        {
            return Ok("CartItem updated successfully");
        }
        else
        {
            return BadRequest("Failed to update CartItem");
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound("user not found");
        var items = await _context.CartItems.Select(item => new { item.Id, item.UserId, item.Quantity, item.Product }).Where(item => item.UserId == user.Id).ToListAsync();
        return Ok(items);
    }

}
