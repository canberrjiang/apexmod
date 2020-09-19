using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Core.Entities.OrderAggregate;
using Core.Specifications;
using System;
using static Core.Specifications.BaseProductWithTagsAndCategoriesSpecification;
using static Core.Specifications.ProductWithTagsAndCategoriesSpecification;

namespace Infrastructure.Services
{
  public class OrderService : IOrderService
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBasketRepository _basketRepo;
    public OrderService(IUnitOfWork unitOfWork, IBasketRepository basketRepo)
    {
      _unitOfWork = unitOfWork;
      _basketRepo = basketRepo;
    }

    public async Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethodId, string basketId, Address shippingAddress)
    {
      // get basket from the repo
      var basket = await _basketRepo.GetBasketAsync(basketId);
      // get items from the product repo
      var items = new List<OrderItem>();
      foreach (var item in basket.Items)
      {
        var productDescription = "";
        if (item.ChildProducts.Count > 0)
        {
          var productItemSpec = new ProductsWithTagAndCategorySpecification(item.Id);
          var productItem = await _unitOfWork.Repository<Product>().GetEntityWithSpec(productItemSpec);
          decimal calcPrice = 0;
          foreach (var subItem in item.ChildProducts)
          {
            var childItemId = subItem.FirstOrDefault().Value;
            var childItem = await _unitOfWork.Repository<BaseProduct>().GetByIdAsync(childItemId);
            productDescription += childItem.Name + Environment.NewLine;
            calcPrice += childItem.Price;
          }
          var itemOrdered = new ProductItemOrdered(productItem.Id, productItem.Name, productDescription,
                  productItem.Photos.FirstOrDefault(x => x.IsMain)?.PictureUrl);

          var orderItem = new OrderItem(itemOrdered, calcPrice, item.Quantity);
          items.Add(orderItem);
        }
        else
        {
          var childProductSpec = new BaseProductsWithTagsAndCategoriesSpecification(item.Id);
          var childProductItem = await _unitOfWork.Repository<BaseProduct>().GetEntityWithSpec(childProductSpec);
          var itemOrdered = new ProductItemOrdered(childProductItem.Id, childProductItem.Name, productDescription,
                  childProductItem.Photos.FirstOrDefault(x => x.IsMain)?.PictureUrl);

          if (item.Price != childProductItem.Price)
          {
            item.Price = childProductItem.Price;
          }
          var orderItem = new OrderItem(itemOrdered, item.Price, item.Quantity);
          items.Add(orderItem);
        }
      }
      // get delivery method from repo
      var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);

      // calc subtotal
      var subtotal = (decimal)items.Sum(item => item.Price * item.Quantity);

      // check to see if order exists
      // var spec = new OrderByPaymentIntentIdSpecification(basket.PaymentIntentId);
      // var existingOrder = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);

      // if (existingOrder != null)
      // {
      //   _unitOfWork.Repository<Order>().Delete(existingOrder);
      //   await _paymentService.CreateOrUpdatePaymentIntent(basket.Id);
      // }

      // check to see if order exists
      if (basket.OrderId != null)
      {
        var existingOrder = await _unitOfWork.Repository<Order>().GetByIdAsync((int)basket.OrderId);
        if (existingOrder != null)
        {
          _unitOfWork.Repository<Order>().Delete(existingOrder);
        }
      }


      // create order
      var order = new Order(items, buyerEmail, shippingAddress, deliveryMethod, subtotal);
      _unitOfWork.Repository<Order>().Add(order);

      // save to db
      var result = await _unitOfWork.Complete();

      if (result <= 0) return null;

      // return order
      return order;
    }

    public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
    {
      return await _unitOfWork.Repository<DeliveryMethod>().ListAllAsync();
    }

    public async Task<Order> GetOrderByIdAsync(int id, string buyerEmail)
    {
      var spec = new OrdersWithItemsAndOrderingSpecification(id, buyerEmail);

      return await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
    }

    public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
    {
      var spec = new OrdersWithItemsAndOrderingSpecification(buyerEmail);

      return await _unitOfWork.Repository<Order>().ListAsync(spec);
    }

    public async Task<IReadOnlyList<Order>> GetAllOrders()
    {
      var spec = new OrdersWithItemsAndOrderingSpecification();
      var result = await _unitOfWork.Repository<Order>().GetEntitiesWithSpec(spec);
      return result;
    }

    public async Task<Order> DeleteOrder(Order order)
    {
      _unitOfWork.Repository<Order>().Delete(order);
      var result = await _unitOfWork.Complete();
      if (result <= 0) return null;
      return order;
    }
  }
}