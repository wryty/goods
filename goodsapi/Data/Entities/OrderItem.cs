﻿namespace GoodsApi.Data.Entities;
public class OrderItem
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public int Quantity { get; set; }
    public long OrderId { get; set; }
    public Product Product { get; set; }
}