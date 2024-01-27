namespace GoodsApi.Data.Entities;
public class CartItem
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public int Quantity { get; set; }
    public long UserId { get; set; }
    public Product Product { get; set; }
    public ApplicationUser User { get; set; }
}