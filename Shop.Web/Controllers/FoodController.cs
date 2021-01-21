using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Shop.Data;
using Shop.Data.Models;
using Shop.Web.DataMapper;
using Shop.Web.Models.Category;
using Shop.Web.Models.Food;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shop.Web.Controllers
{
	public class FoodController : Controller
	{
		private readonly ICategory _categoryService;
		private readonly IFood _foodService;
        private readonly Mapper _mapper;

		public FoodController(ICategory categoryService, IFood foodService)
		{
			_categoryService = categoryService;
			_foodService = foodService;
            _mapper = new Mapper();
		}
        
        [Route("Foods/{id}")]
		public async Task<IActionResult> Index(int id)
		{
			Food foodFromResponse = new Food();
			using (var httpClient = new HttpClient())
			{
				using (var response = await httpClient.GetAsync("https://localhost:44391/api/Food/" + id.ToString()))
				{
					string apiResponse = await response.Content.ReadAsStringAsync();
					foodFromResponse = JsonConvert.DeserializeObject<Food>(apiResponse);
				}
			}
			//var food = _foodService.GetById(id);

			var food = foodFromResponse;

			var model = new FoodIndexModel
			{
				Id = food.Id,
				Name = food.Name,
				ImageUrl = food.ImageUrl,
				InStock = food.InStock,
				Price = food.Price,
				Description = food.ShortDescription + "\n" + food.LongDescription,
				CategoryId = food.Category.Id,
				CategoryName = food.Category.Name
			};

			return View(model);
		}

		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> New(int categoryId = 0)
		{
			await GetCategoriesForDropDownList();
			NewFoodModel model = new NewFoodModel
			{
				CategoryId = categoryId
			};

			ViewBag.ActionText = "create";
			ViewBag.Action = "New";
			ViewBag.CancelAction = "Topic";
			ViewBag.SubmitText = "Create Food";
            ViewBag.RouteId = categoryId;
            ViewBag.ControllerName = "Category";

			if(categoryId == 0)
			{
				ViewBag.CancelAction = "Index";
				ViewBag.ControllerName = "Home";
			}

			return View("CreateEdit",model);
		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> New(NewFoodModel model)
		{
			if (ModelState.IsValid && _categoryService.GetById(model.CategoryId.Value) != null)
			{
				var food = _mapper.NewFoodModelToFood(model, true, _categoryService);
				_foodService.NewFood(food);
				return RedirectToAction("Index", new { id = food.Id });
			}
		    await GetCategoriesForDropDownList();

			ViewBag.ActionText = "create";
			ViewBag.Action = "New";
            ViewBag.ControllerName = "Category";
			ViewBag.CancelAction = "Topic";
			ViewBag.SubmitText = "Create Food";
            ViewBag.RouteId = model.CategoryId;

			return View("CreateEdit",model);
		}

		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Edit(int id)
		{
			ViewBag.ActionText = "change";
			ViewBag.Action = "Edit";
			ViewBag.CancelAction = "Index";
			ViewBag.SubmitText = "Save Changes";
            ViewBag.ControllerName = "Food";
			ViewBag.RouteId = id;

			await GetCategoriesForDropDownList();

			Food foodFromResponse = new Food();
			using (var httpClient = new HttpClient())
			{
				using (var response = await httpClient.GetAsync("https://localhost:44391/api/Food/" + id.ToString()))
				{
					string apiResponse = await response.Content.ReadAsStringAsync();
					foodFromResponse = JsonConvert.DeserializeObject<Food>(apiResponse);
				}
			}
			//var food = _foodService.GetById(id);

			var food = foodFromResponse;

			if (food != null)
			{
				var model = _mapper.FoodToNewFoodModel(food);
				return View("CreateEdit", model);
			}

			return View("CreateEdit");
		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Edit(NewFoodModel model)
		{
			if (ModelState.IsValid)
			{
				var food = _mapper.NewFoodModelToFood(model, false, _categoryService);
				_foodService.EditFood(food);
				return RedirectToAction("Index", new { id = model.Id });
			}
            
			ViewBag.ActionText = "change";
			ViewBag.Action = "Edit";
			ViewBag.CancelAction = "Index";
			ViewBag.SubmitText = "Save Changes";
            ViewBag.ControllerName = "Food";
			ViewBag.RouteId = model.Id;
			await GetCategoriesForDropDownList();

			return View("CreateEdit", model);
		}

		[Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var categoryId = _foodService.GetById(id).CategoryId;
            _foodService.DeleteFood(id);

            return RedirectToAction("Topic", "Category", new { id = categoryId, searchQuery = "" });
        }

		private async Task GetCategoriesForDropDownList()
		{
			List<Category> categoryList = new List<Category>();
			using (var httpClient = new HttpClient())
			{
				using (var response = await httpClient.GetAsync("https://localhost:44391/api/Category"))
				{
					string apiResponse = await response.Content.ReadAsStringAsync();
					categoryList = JsonConvert.DeserializeObject<List<Category>>(apiResponse);
				}
			}

			var categories = categoryList.Select(category => new CategoryDropdownModel
			{
				Id = category.Id,
				Name = category.Name
			});
			ViewBag.Categories = new SelectList(categories, "Id", "Name");
		}
	}
}
