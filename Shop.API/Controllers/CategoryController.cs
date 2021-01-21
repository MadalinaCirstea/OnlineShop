using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Serilog.Formatting.Json;
using Shop.Data;
using Shop.Data.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Shop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly Services.ICategory _categoryService;

        public CategoryController(Services.ICategory categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public IEnumerable<Category> GetCategories()
        {
            return _categoryService.GetAll();
        }

        [HttpGet("{id}")]
        public Category GetCategoryById(int id)
        {
           return  _categoryService.GetById(id);
        }

        
        [HttpDelete("{id}")]
        public void DeleteCategory(int id)
        {
           _categoryService.DeleteCategory(id);
        }
    }
}
