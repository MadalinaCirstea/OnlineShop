using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shop.API.Services;
using Shop.Data.Models;

namespace Shop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        private readonly IFood _foodService;

        public FoodController(IFood foodService)
        {
            _foodService = foodService;
        }

        [HttpGet]
        public IEnumerable<Food> GetFoods()
        {
            return _foodService.GetAll();
        }

        [HttpGet("{id}")]
        public Food GetFoodById(int id)
        {
            return _foodService.GetById(id);
        }

        [HttpGet("preferred/{id}")]
        public IEnumerable<Food> GetPreferred(int id)
        {
            return _foodService.GetPreferred(id);
        }

        [HttpGet("category/{id}")]
        public IEnumerable<Food> GetFoodsByCategoryId(int id)
        {
            return _foodService.GetFoodsByCategoryId(id);
        }

        [HttpDelete("{id}")]
        public void DeleteFood(int id)
        {
            _foodService.DeleteFood(id);
        }
    }
}
