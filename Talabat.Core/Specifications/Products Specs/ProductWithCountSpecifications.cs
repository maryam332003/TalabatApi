using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications.Products_Specs
{
    public class ProductWithCountSpecifications : BaseSpecifications<Product>
    {
        public ProductWithCountSpecifications(ProductSpecParams productSpec) 
                   : base(P =>
            (string.IsNullOrEmpty(productSpec.Search) || P.Name.ToLower().Contains(productSpec.Search))&&
            (!productSpec.BrandId.HasValue || P.BrandId == productSpec.BrandId.Value)
            &&
            (!productSpec.CategoryId.HasValue || P.CategoryId == productSpec.CategoryId.Value)
            )
        {

        }
    }
}
