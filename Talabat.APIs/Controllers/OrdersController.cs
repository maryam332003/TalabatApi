using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.Dtos;
using Talabat.APIs.Dtos.OrderDtos;
using Talabat.APIs.Errors;
using Talabat.Core.Entities.Order;
using Talabat.Core.Repositories.Interfaces;
using Talabat.Core.Services.Interfaces;

namespace Talabat.APIs.Controllers
{

    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OrdersController(IOrderService orderService,IMapper mapper,IUnitOfWork unitOfWork)
        {
            _orderService = orderService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }



        [HttpPost("CreateOrder")]
        [Authorize]
        public async Task<ActionResult<OrderToReturnDto>> CreateOrder(OrderDto model)
        {
            var BuyerEmail=User.FindFirstValue(ClaimTypes.Email);
            var ShippingAddress = _mapper.Map<AddressDto, Address>(model.ShippingAddress);
            var order = await _orderService.CreateOrderAsync(BuyerEmail, model.BasketId, model.DeliveryMethodId, ShippingAddress);
            if (order is null) return BadRequest(BadRequest(new ApiResponse(400, "There Is a Problem with your Order!")));
            var Result= _mapper.Map<Order, OrderToReturnDto>(order);
            return Ok(Result); 


        }


        [HttpGet("GetOrdersForUser")]
        [Authorize]
        public async Task<ActionResult<IReadOnlyList< Order>>> GetOrdersForUser()
        {
            var BuyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var Orders = await _orderService.GetOrderForSpecificUserAsync(BuyerEmail);
            if (Orders is null) return NotFound(new ApiResponse(404, "There is no orders for you"));
            return Ok(Orders);
        }
        
        [HttpGet("GetOrderByIdForUser/{id}")]
        [Authorize]
        public async Task<ActionResult<Order>> GetOrderByIdForUser(int Id)
        {
            var user=User.FindFirstValue(ClaimTypes.Email);
            var Orders = await _orderService.GetOrderBySpecificIdForSpecificUser(user, Id);
            if (Orders is null) return NotFound(new ApiResponse(404, $"There is no orders With this id {Id} for u"));
            return Ok(Orders);
        }

        
        [HttpGet("GetDeliveryMethods")]
        public async Task<ActionResult<DeliveryMethod>> GetDeliveryMethods()
        {
           var DeliveryMethods=await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
            return Ok(DeliveryMethods);

        }

        

    }
}
