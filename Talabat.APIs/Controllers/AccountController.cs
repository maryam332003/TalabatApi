    using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.APIs.Models;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services.Interfaces;

namespace Talabat.APIs.Controllers
{

    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public AccountController(UserManager<AppUser> userManager 
            , SignInManager<AppUser> signInManager
            , ITokenService tokenService
            ,IMapper mapper
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }
        #region Login
        //login
        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null) { return Unauthorized(value: new ApiResponse(statusCode: 401)); }
            var Result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);
            if (!Result.Succeeded) { return Unauthorized(value: new ApiResponse(statusCode: 401)); }
            return Ok(value: new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _tokenService.CreateTokenAsync(user, _userManager),
            });

        } 
        #endregion

        #region Register
        //Register
        [HttpPost(template: "Register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto model)
        {
            if (CheckEmailExists(model.Email).Result.Value)
            {
                return BadRequest(error: new ApiResponse(statusCode: 400, message: "Email is already exists!"));
            }
            var user = new AppUser()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName = model.Email.Split(separator: "@")[0],
                PhoneNumber = model.PhoneNumber,
            };
            var Result = await _userManager.CreateAsync(user, model.Password);
            if (!Result.Succeeded) return BadRequest(error: new ApiResponse(statusCode: 400));
            var ReturnUser = new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _tokenService.CreateTokenAsync(user, _userManager),
            };
            return Ok(ReturnUser);
        }

        #endregion

        
        //public async Task<IActionResult> TestEmail()
        //{
        //    var message = new Message();

        //}
        
        
        
        
        #region GetCurrentUser
        [Authorize]
        [HttpGet(template: "GetCurrentUser")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(userEmail);
            return Ok(value: new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _tokenService.CreateTokenAsync(user, _userManager),
            });
        }
        #endregion

        #region CurrentUserAddress
        [Authorize]
        [HttpGet("CurrentUserAddress")]
        public async Task<ActionResult<AddressDto>> GetCurrentUserAddress()
        {
            var user = await _userManager.FindUserWithAddressAsync(User);
            if(user is not  null)
            {
                var MappedAddress = _mapper.Map<Address, AddressDto>(user.Address);
                //var MappedAddress = _mapper.Map<Address, AddressDto>(user.Address);
                return Ok(MappedAddress);
            }
            return BadRequest();
       
        }
        #endregion

        #region UpdateCurrentUserAddress
        [Authorize]
        [HttpPut(template: "address")]
        public async Task<ActionResult<AddressDto>> UpdateCurrentUserAddress(AddressDto model)
        {
            var user = await _userManager.FindUserWithAddressAsync(User);
            var address = _mapper.Map<AddressDto, Address>(model);
            user.Address = address;
            var Result = await _userManager.UpdateAsync(user);
            if (!Result.Succeeded) return BadRequest(error: new ApiResponse(statusCode: 400));
            return Ok(model);
        }
        #endregion

        #region CheckEmailExists
        [HttpPut(template: "emailExists")]
        public async Task<ActionResult<bool>> CheckEmailExists(string email)
        {
            //var user = await _userManager.FindByEmailAsync(email);
            //if (user is null) return false;
            //return true;
            return await _userManager.FindByEmailAsync(email) is not null;
        } 
        #endregion
    }
}