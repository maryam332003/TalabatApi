using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order;

namespace Talabat.Repository.Data
{
    public class StoreDbContextSeed
    {
        //Seed Data
       
       public static async Task SeedAsync(StoreDbContext _context)
       {
            if (_context.ProductBrands.Count() == 0)
            {
                //Brands
                //1.Read Data From Json File
                var brandData = File.ReadAllText("../Talabat.Repository/Data/DataSeeding/brands.json");
                //2.Convert Json string to the needed Type
                var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandData);

                if (brands?.Count() > 0)
                {
                    foreach (var brand in brands)
                    {
                        _context.Set<ProductBrand>().Add(brand);
                    }
                    await _context.SaveChangesAsync();
                }
            }

            //================================

            if (_context.ProductCategories.Count() == 0)
            {
                //category
                //1.Read Data From Json File
                var categoryData = File.ReadAllText(path: "../Talabat.Repository/Data/DataSeeding/categories.json");
                //2.Convert Json string to the needed Type
                var categories = JsonSerializer.Deserialize<List<ProductCategory>>(categoryData);

                if (categories?.Count() > 0)
                {
                    foreach (var category in categories)
                    {
                        _context.Set<ProductCategory>().Add(category);
                    }
                    await _context.SaveChangesAsync();
                }
            }


            //=======================

            if (_context.Products.Count() == 0)
            {
                //Product
                //1.Read Data From Json File
                var productData = File.ReadAllText(path: "../Talabat.Repository/Data/DataSeeding/products.json");
                //2.Convert Json string to the needed Type
                var products = JsonSerializer.Deserialize<List<Product>>(productData);

                if (products?.Count() > 0)
                {
                    foreach (var product in products)
                    {
                        _context.Set<Product>().Add(product);
                    }
                    await _context.SaveChangesAsync();
                }

            }
            
            
            //=======================

            if (_context.DeliveryMethods.Count() == 0)
            {
                //Product
                //1.Read Data From Json File
                var deliveryData = File.ReadAllText(path: "../Talabat.Repository/Data/DataSeeding/delivery.json");
                //2.Convert Json string to the needed Type
                var deliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryData);

                if (deliveryMethods?.Count() > 0)
                {
                    foreach (var deliveryMethod in deliveryMethods)
                    {
                        _context.DeliveryMethods.Add(deliveryMethod);
                    }
                    await _context.SaveChangesAsync();
                }

            }
        }

    }
}
