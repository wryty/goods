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
[Route("/api/Product")]
public class ProductController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public ProductController(ITokenService tokenService, DataContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _tokenService = tokenService;
        _context = context;
        _userManager = userManager;
        _configuration = configuration;
    }


    [Authorize(AuthenticationSchemes = "Bearer", Roles = RoleConsts.Administrator)]
    [HttpPost("/api/products")]
    public async Task<ActionResult> CreateProduct([FromBody] ProductRequest request)
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

        var Product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price
        };

        _context.Products.Add(Product);
        await _context.SaveChangesAsync();

        return Ok(new { ProductId = Product.Id, Message = "Product created successfully" });
    }

    [Authorize(AuthenticationSchemes = "Bearer", Roles = RoleConsts.Administrator)]
    [HttpPut("/api/Product/{id}")]
    public async Task<ActionResult> UpdateProduct(long id, [FromBody] ProductRequest request)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return NotFound("User not found");
        }

        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            return NotFound("Product not found");
        }

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;

        _context.Products.Update(product);
        await _context.SaveChangesAsync();

        return Ok("Product updated successfully");
    }

    [Authorize(AuthenticationSchemes = "Bearer", Roles = RoleConsts.Administrator)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(long id)
    {
        var Product = await _context.Products.FindAsync(id);

        var user = await _userManager.GetUserAsync(User);

        if (Product == null)
        {
            return NotFound("Product not found");
        }
        _context.Products.Remove(Product);

        var result = await _context.SaveChangesAsync();

        if (result > 0)
        {
            return Ok("Product deleted successfully");
        }
        else
        {
            return BadRequest("Failed to delete Product");
        }
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductByIdAdmin(long id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound("Product not found");
        return Ok(new { product.Id, product.Name, product.Description, product.Price, product.PreviewImageFileName, product.DetailImageFileName });
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _context.Products.Select(p => new { p.Id, p.Name, p.Description, p.Price, p.PreviewImageFileName, p.DetailImageFileName }).ToListAsync();
        return Ok(products);
    }


    [HttpPost("/api/Product/UploadImage/{id}")]
    public async Task<ActionResult> UploadImage(long id, IFormFile file, [FromForm] string imageType)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            return NotFound("Product not found");
        }

        var uploadPath = Path.Combine("..", "Goods", "wwwroot", "uploads");

        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
        var filePath = Path.Combine(uploadPath, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        if (imageType == "preview")
        {
            product.PreviewImageFileName = uniqueFileName;
        }
        else if (imageType == "detail")
        {
            product.DetailImageFileName = uniqueFileName;
        }
        else
        {
            return BadRequest("Invalid image type");
        }

        _context.Products.Update(product);
        await _context.SaveChangesAsync();

        return Ok("Image uploaded successfully");
    }




}
