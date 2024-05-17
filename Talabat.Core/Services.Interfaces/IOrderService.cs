using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order;

namespace Talabat.Core.Services.Interfaces
{
    public interface IOrderService
    {
        //Create order
        Task<Order?> CreateOrderAsync (string buyerEmail,string BasketId,int DeliveryMethodId,Address ShiipingAddress);
        //GetOrderForSpecificUser-->بجيب الاوردر الخاص بيوزر معين
        Task<IReadOnlyList<Order>> GetOrderForSpecificUserAsync(string buyerEmail);
        //getOrderById-->بجيب اوردر معين ليوزر معين
        Task<Order?> GetOrderBySpecificIdForSpecificUser(string buyerEmail, int OrderId);
    }
}
