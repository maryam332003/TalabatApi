using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order;
using Talabat.Core.Repositories.Interfaces;
using Talabat.Core.Services.Interfaces;
using Talabat.Core.Specifications.OrderSpecs;

namespace Talabat.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        public OrderService(IBasketRepository basketRepository,IUnitOfWork unitOfWork,IPaymentService paymentService)                  
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }


        public async Task<Order?> CreateOrderAsync(string buyerEmail, string BasketId, int DeliveryMethodId, Address ShippingAddress)
        {
            //1. Get Basket From Basket Repo
            var Basket =await _basketRepository.GetBasketAsync(BasketId);
            //2. Get Selected Items From Basket
            var OrderItems = new List<OrderItem>();
            if (Basket?.Items.Count() > 0)
            {
                foreach (var item in Basket.Items)
                {
                    var Product =await _unitOfWork.Repository<Product>().GetAsync(item.Id);
                    var ProductItemOrdered = new ProductItemOrdered(Product.Id, Product.Name, Product.PictureUrl);
                    var OrderItem = new OrderItem(ProductItemOrdered, item.Price, item.Quantity);
                    OrderItems.Add(OrderItem);
                }
            }

            //3. Calculate SubTotal
            var SubTotal = OrderItems.Sum(OI => OI.Price * OI.Quantity);

            //4. Get DeliveryMethod From DataBase(هكلم الريبو الخاص بال DeliveryMethod)
            var DeliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetAsync(DeliveryMethodId);
            //check if payment intent Id exists for another order
            var spec = new OrderWithPaymentIntentSpecification(Basket.PaymentIntentId);
            var ExOrder=await _unitOfWork.Repository<Order>().GetWithSpecAsync(spec);
            if (ExOrder is not null)
            {
                _unitOfWork.Repository<Order>().Delete(ExOrder);
                Basket = await _paymentService.CreateOrUpdatePaymentIntent(BasketId);

            }

            ///5. Create Order
            var Order = new Order(buyerEmail, ShippingAddress, DeliveryMethod, OrderItems, SubTotal,Basket.PaymentIntentId);

            //6. Add Orderb To DataBase Locally
            await _unitOfWork.Repository<Order>().AddAsync(Order);

            //7. Add Order To DataBase
            var changes =await _unitOfWork.CompleteAsync();  //Save Changes to DataBase
            if (changes <= 0) return null;
            return Order;


        }

        public async Task<IReadOnlyList<Order>> GetOrderForSpecificUserAsync(string buyerEmail)
        {
            var Spec = new OrderSpecifications(buyerEmail);
            var Orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(Spec);
            return Orders;
        }

        public async Task<Order?> GetOrderBySpecificIdForSpecificUser(string buyerEmail, int OrderId)
        {
            var Spec = new OrderSpecifications(buyerEmail,OrderId);
            var Orders = await _unitOfWork.Repository<Order>().GetWithSpecAsync(Spec);
            if (Orders is null) return null;
            return Orders;
        }

    }
}
