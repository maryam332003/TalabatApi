using Talabat.Core.Entities.Order;

namespace Talabat.Core.Specifications.OrderSpecs
{
    public class OrderSpecifications:BaseSpecifications<Order>
    {
        public OrderSpecifications(string email):base(O=>O.BuyerEmail==email) 
        {
            Includes.Add(O => O.DeliveryMethod);
            Includes.Add(O => O.Items);
            AddOrderByDesc(O => O.OrderDate);

            
        }
        public OrderSpecifications(string email,int Id)
                                  : base(O => O.BuyerEmail == email && O.Id == Id)
        {
            Includes.Add(O => O.DeliveryMethod);
            Includes.Add(O => O.Items);
            AddOrderByDesc(O => O.OrderDate);

            
        }
    }
}
