﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Dtos.OrderDtos;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Interfaces;

namespace Talabat.APIs.Controllers
{

    public class BasketController : BaseApiController
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketController(IBasketRepository basketRepository,IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<CustomerBasket>> GetBasket(string id)
        {
            var basket = await _basketRepository.GetBasketAsync(id);
            if (basket is null) return Ok(value: new CustomerBasket() { Id = id });
            return Ok(basket);
        
        }

        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdateBasket(CustomerBasketDto basket)
        {
           var mappedBasket= _mapper.Map<CustomerBasket>(basket);

            var createdOrUpdatedBasket = await _basketRepository.UpdateBasketAsync(mappedBasket);
            if (createdOrUpdatedBasket is null) return BadRequest(error: new ApiResponse(statusCode: 400));
            return Ok(createdOrUpdatedBasket);
        }

        [HttpDelete]
        public async Task DeleteBasket(string id)
        {
            await _basketRepository.DeleteBasketAsync(id);
        }


    }
}
