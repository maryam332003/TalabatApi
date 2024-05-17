using Microsoft.Extensions.Configuration;
using Stripe;
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
using Product = Talabat.Core.Entities.Product;

namespace Talabat.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public PaymentService(IBasketRepository basketRepository,IUnitOfWork unitOfWork,IConfiguration configuration)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }
        public async Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string BasketId)
        {
           //Get Basket
            var basket=await _basketRepository.GetBasketAsync(BasketId);
            if (basket is null) return null;
            //Total Price
            if (basket.Items.Count > 0)
            {
                foreach (var item in basket.Items)
                {
                    var product =await _unitOfWork.Repository<Product>().GetAsync(item.Id);
                    if(item.Price != product.Price)
                    {
                        item.Price = product.Price; 
                    }
                    
                }
            }

            var SubTotal = basket.Items.Sum(I => I.Price * I.Quantity);
            var ShippingPrice = 0m;
            if (basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethod=await _unitOfWork.Repository<DeliveryMethod>().GetAsync(basket.DeliveryMethodId.Value);
                ShippingPrice = deliveryMethod.Cost;
            }
            var Total = SubTotal + ShippingPrice;


            //Call Stripe===>to Call Stripe we need to install package(Stripe.net)
            StripeConfiguration.ApiKey = _configuration["StripeKeys:Secretkey"];
            var Service = new PaymentIntentService();
            PaymentIntent paymentIntent;

            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                //Create new PaymentIntentId
                var options = new PaymentIntentCreateOptions()
                {
                    Amount = (long)(SubTotal * 100 + ShippingPrice),
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>() { "card" }
                };

                paymentIntent = await Service.CreateAsync(options);
                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;
            }
            else
            {
                //Update PaymentIntentId
                var options = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)(SubTotal * 100 + ShippingPrice),
                };

                paymentIntent=await Service.UpdateAsync(basket.PaymentIntentId, options);
            }
            await _basketRepository.UpdateBasketAsync(basket);
            //Return Basket Included PaymentId and Client Secret
            return basket;

        }

        public async Task<Order> UpdatePaymentIntentToSuccessOrFailed(string PaymentIntentId, bool Flag)
        {
            var spec = new OrderWithPaymentIntentSpecification(PaymentIntentId);
            var order = await _unitOfWork.Repository<Order>().GetWithSpecAsync(spec);
            if (Flag)
            {
                order.Status = OrderStatus.PaymentReceived;
            }
            else
            {
                order.Status = OrderStatus.PaymentFailed;

            }
            _unitOfWork.Repository<Order>().Update(order);
            return order;
        }
    }
}
