using Microsoft.AspNetCore.Identity;

namespace GoodsApi.Data.Entities;

public class ApplicationUser : IdentityUser<long>
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public List<Order> Orders { get; set; }
    public List<CartItem> Items { get; set; }
}

