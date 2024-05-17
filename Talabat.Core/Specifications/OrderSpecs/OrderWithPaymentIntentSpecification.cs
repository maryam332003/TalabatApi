using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order;

namespace Talabat.Core.Specifications.OrderSpecs
{
    public class OrderWithPaymentIntentSpecification:BaseSpecifications<Order>
    {
        public OrderWithPaymentIntentSpecification(string paymentIntentId):base(O=>O.PaymentIntentId==paymentIntentId)
        {
            
        }
    }
}
