using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using Talabat.APIs.Errors;
using Talabat.Repository.Data;

namespace Talabat.APIs.Controllers
{
    public class BuggyController : BaseApiController
    {
        private readonly StoreDbContext _context;

        public BuggyController(StoreDbContext  context)
        {
            _context = context;
        }

        [HttpGet(template:"notfound")]
        public ActionResult GetNotFoundRequest()
        {
            var product = _context.Products.Find(keyValues: 1000);
            if(product is null)
            {
                return NotFound(value: new ApiResponse(statusCode: 404));
            }
            return Ok(product);
        }
        [HttpGet(template:"servererror")]
        public ActionResult GetServerError() 
        {
            var product = _context.Products.Find(keyValues: 1000);
            var result = product.ToString();
            return Ok(result);
        }
        [HttpGet(template:"badrequest")]
        public ActionResult GetBadRequest() 
        {
            return BadRequest(error: new ApiResponse(statusCode: 400));
        }
        [HttpGet(template: "badrequest/{id}")]
        public ActionResult GetBadRequest(int? id)
        {
            return Ok();
        }

        [HttpGet(template: "unauthroized")]
        public ActionResult GetUnauthorizedError() 
        {
            return Unauthorized(value: new ApiResponse(statusCode: 401));
        }
    }
}
