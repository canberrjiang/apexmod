using System;
using System.Collections.Generic;
using Core.Entities;

namespace Core.Entities.OrderAggregate
{
  public class Order : BaseEntity
  {
    // EF needs a parameterless constructor
    public Order()
    {
    }

    public Order(IReadOnlyList<OrderItem> orderItems, string buyerEmail, Address shipToAddress, DeliveryMethod deliveryMethod, decimal subtotal)
    {
      BuyerEmail = buyerEmail;
      ShipToAddress = shipToAddress;
      DeliveryMethod = deliveryMethod;
      OrderItems = orderItems;
      Subtotal = subtotal;
    }

    // Todo - change this to Buyer Email
    public string BuyerEmail { get; set; }
    public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
    public Address ShipToAddress { get; set; }
    public DeliveryMethod DeliveryMethod { get; set; }
    public IReadOnlyList<OrderItem> OrderItems { get; set; }
    public decimal Subtotal { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal GetTotal()
    {
      return Subtotal + DeliveryMethod.Price;
    }
  }
}