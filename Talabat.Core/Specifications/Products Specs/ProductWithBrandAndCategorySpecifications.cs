using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications.Products_Specs
{
    public class ProducProductWithBrandAndCategorySpecifications : BaseSpecifications<Product>
    {
        public ProducProductWithBrandAndCategorySpecifications(ProductSpecParams productSpec)
            : base(P => 
            (!productSpec.BrandId.HasValue || P.BrandId == productSpec.BrandId.Value)
            &&
            (!productSpec.CategoryId.HasValue || P.CategoryId == productSpec.CategoryId.Value)
            )
        {
            Includes.Add(P => P.Brand);
            Includes.Add(P => P.Category);

            if (!string.IsNullOrEmpty(productSpec.Sort))
            {
                switch(productSpec.Sort)
                {
                    case "priceAsc":
                        AddOrderBy(P => P.Price);
                        break;
                    case "priceDesc":
                        AddOrderByDesc(P => P.Price);
                        break;
                    default:
                        AddOrderBy(P => P.Name);
                        break;
                }
            }
            else
            {
                AddOrderBy(P => P.Name);
            }
            ApplyPagination(productSpec.PageSize * (productSpec.PageIndex - 1), productSpec.PageSize);
        }
        public ProducProductWithBrandAndCategorySpecifications(int id):base(P => P.Id == id)
        {
            Includes.Add(P => P.Brand);
            Includes.Add(P => P.Category);

        }
    }
}
