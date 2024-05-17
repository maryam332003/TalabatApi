using AutoMapper;
using AutoMapper.Execution;
using Talabat.APIs.Dtos.OrderDtos;
using Talabat.Core.Entities.Order;

namespace Talabat.APIs.Helpers
{
    public class OrderPictureUrlResolver : IValueResolver<OrderItem, OrderItemDto, string>
    {
        public string Resolve(OrderItem source, OrderItemDto destination, string destMember, ResolutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
