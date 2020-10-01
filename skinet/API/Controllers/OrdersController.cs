using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using API.Extensions;
using AutoMapper;
using Core.Interfaces;
using Core.Entities.OrderAggregate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
  public class OrdersController : BaseApiController
  {
    private readonly IOrderService _orderService;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    public OrdersController(IOrderService orderService, IMapper mapper, IUnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _orderService = orderService;
    }

    [HttpPost]
    public async Task<ActionResult<OrderToReturnDto>> CreateOrder(OrderDto orderDto)
    {
      var email = HttpContext.User.RetrieveEmailFromPrincipal();

      var address = _mapper.Map<AddressDto, Address>(orderDto.ShipToAddress);

      var order = await _orderService.CreateOrderAsync(email, orderDto.DeliveryMethodId, orderDto.BasketId, address);

      if (order == null) return BadRequest(new ApiResponse(400, "Problem creating order"));

      return Ok(_mapper.Map<Order, OrderToReturnDto>(order));
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetAllOrders()
    {
      var orders = await _orderService.GetAllOrders();
      return Ok(_mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders));
    }

    [HttpGet("all/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<OrderToReturnDto>> GetOrderById(int id)
    {
      var orders = await _orderService.GetOrderByIdAsync(id);
      return Ok(_mapper.Map<Order, OrderToReturnDto>(orders));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser()
    {
      var email = HttpContext.User.RetrieveEmailFromPrincipal();

      var orders = await _orderService.GetOrdersForUserAsync(email);

      return Ok(_mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderToReturnDto>> GetOrderByIdForUser(int id)
    {
      var email = HttpContext.User.RetrieveEmailFromPrincipal();

      var order = await _orderService.GetOrderByIdAsync(id, email);

      if (order == null) return NotFound(new ApiResponse(404));

      return _mapper.Map<Order, OrderToReturnDto>(order);
    }

    [HttpGet("deliveryMethods")]
    public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
    {
      var deliveryMethods = await _orderService.GetDeliveryMethodsAsync();

      return Ok(deliveryMethods);
    }

    [HttpGet("deliveryMethods/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<DeliveryMethod>> GetDeliveryMethodById(int id)
    {
      var deliveryMethods = await _orderService.GetDeliveryMethodByIdAsync(id);

      return Ok(deliveryMethods);
    }

    [HttpPut("deliveryMethods/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<DeliveryMethod>> UpdateDeliveryMethods(int id, DeliveryMethodToCreate deliveryMethodToUpdate)
    {
      var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(id);
      _mapper.Map(deliveryMethodToUpdate, deliveryMethod);
      _unitOfWork.Repository<DeliveryMethod>().Update(deliveryMethod);
      var result = await _unitOfWork.Complete();

      if (result <= 0) return BadRequest(new ApiResponse(400, "Problem updating delivery method"));

      return Ok(deliveryMethod);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Order>> DeleteOrder(int id)
    {
      var email = HttpContext.User.RetrieveEmailFromPrincipal();
      var order = await _orderService.GetOrderByIdAsync(id, email);
      var result = await _orderService.DeleteOrder(order);
      if (result == null) return BadRequest("Failed to delete order");
      return Ok(result);
    }
  }
}