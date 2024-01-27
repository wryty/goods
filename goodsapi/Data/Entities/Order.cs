namespace GoodsApi.Data.Entities;
public class Order
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string OrderDate { get; set; }
    public List<OrderItem> Items { get; set; }
    public List<long> ItemsId { get; set; }
    public ApplicationUser User { get; set; }
}