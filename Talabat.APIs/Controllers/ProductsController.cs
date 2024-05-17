using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Interfaces;
using Talabat.Core.Specifications.Products_Specs;

namespace Talabat.APIs.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class ProductsController : BaseApiController
    {

        private readonly IGenericRepository<Product> _productRepo;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<ProductBrand> _brandsRepo;
        private readonly IGenericRepository<ProductCategory> _categoriesRepo;

        //getall
        //getbyid

        public ProductsController(
            IGenericRepository<Product> ProductRepo, 
            IMapper mapper,
            IGenericRepository<ProductBrand> brandsRepo,
            IGenericRepository<ProductCategory> categoriesRepo
            )
        {
            _productRepo = ProductRepo;
            _mapper = mapper;
            _brandsRepo = brandsRepo;
            _categoriesRepo = categoriesRepo;
        }


        // api/Products
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProduct([FromQuery]ProductSpecParams productSpec)
        {
            //var Products= await _productRepo.GetAllAsync();

            //JsonResult result = new JsonResult(Products);
            //OkResult result = new OkResult();
            //OkObjectResult result = new OkObjectResult(Products);
            //result.ContentTypes = new Microsoft.AspNetCore.Mvc.Formatters.MediaTypeCollection();
            //result.StatusCode = 200;
            //return result;
            var spec = new ProducProductWithBrandAndCategorySpecifications(productSpec);
            var Products = await _productRepo.GetAllWithSpecAsync(spec);
            var result = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(Products);

            var countSpec = new ProductWithCountSpecifications(productSpec);
            var count = await _productRepo.GetCountAsync(countSpec);
            return Ok(value: new Pagination<ProductToReturnDto>(productSpec.PageIndex,productSpec.PageSize,count,result));
        }

        [ProducesResponseType(typeof(ProductToReturnDto),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]

        [HttpGet(template: "{id}")]
        public async Task<ActionResult<ProductToReturnDto>> GetProductById(int id)
        {

            //var product = await _productRepo.GetAsync(id);
            var spes = new ProducProductWithBrandAndCategorySpecifications(id);
            var product = await _productRepo.GetWithSpecAsync(spes);

            if (product is null)
                return NotFound(value: new ApiResponse(statusCode : 404 ));
            var result = _mapper.Map<Product,ProductToReturnDto>(product);
            return Ok(result);

        }
        [HttpGet(template:"brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
        {
            var brands = await _brandsRepo.GetAllAsync();
            return Ok(brands);
        }
        [HttpGet(template: "categories")]
        public async Task<ActionResult<IReadOnlyList<ProductCategory>>> GetCategories()
        {
            var categories = await _brandsRepo.GetAllAsync();
            return Ok(categories);
        }





    }
}
