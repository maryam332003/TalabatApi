using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Interfaces;
using Talabat.Core.Specifications.Employee_Specs;

namespace Talabat.APIs.Controllers
{
 
    public class EmployeeController : BaseApiController
    {
        private readonly IGenericRepository<Employee> _employeeRepo;
        public EmployeeController(IGenericRepository<Employee> employeeRepo)
        {
            _employeeRepo = employeeRepo;
        }

        [HttpGet]
        public  async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            var spec = new EmployeeWithDepartmentAndCategorySpecifications();
            var employee = await _employeeRepo.GetAllWithSpecAsync(spec);

            return Ok(employee);

        }
        [ProducesResponseType(typeof(Employee),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]

        [HttpGet(template: "{id}")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeeById(int id)
        {
            var spec = new EmployeeWithDepartmentAndCategorySpecifications(id);
            var employee = await _employeeRepo.GetWithSpecAsync(spec);
            if (employee is null)
                return NotFound(value: new ApiResponse(statusCode: 404));

            return Ok(employee);
            
        }

    }
}
