using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Data;
using Shop.Data.Models;
using Shop.Web.Models.Category;
using Shop.Web.Models.Food;
using System.Linq;
using Shop.Web.DataMapper;
using System.Net;
using System;
using System.Net.Http;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http.Formatting;
using Newtonsoft.Json;
using System.IO;

namespace Shop.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategory _categoryService;
        private readonly IFood _foodService;
        private readonly Mapper _mapper;

        public CategoryController(ICategory categoryService, IFood foodService)
        {
            _categoryService = categoryService;
            _foodService = foodService;
            _mapper = new Mapper();
        }

        public async Task<IActionResult> Index()
        {
            List<Category> reservationList = new List<Category>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:44391/api/Category"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    reservationList = JsonConvert.DeserializeObject<List<Category>>(apiResponse);
                }
            }

            //var categories = _categoryService.GetAll().
            var categories = reservationList.
                Select(category => new CategoryListingModel
                {
                    Name = category.Name,
                    Description = category.Description,
                    Id = category.Id,
                    ImageUrl = category.ImageUrl
                });

            var model = new CategoryIndexModel
            {
                CategoryList = categories
            };

            return View(model);
        }

        public async Task<IActionResult> Topic(int id, string searchQuery)
        {
            Category categoryFromResponse = new Category();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:44391/api/Category/" + id.ToString()))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    categoryFromResponse = JsonConvert.DeserializeObject<Category>(apiResponse);
                }
            }
            //var category = _categoryService.GetById(id);

            var category = categoryFromResponse;
            var foods = _foodService.GetFilteredFoods(id, searchQuery);

            var foodListings = foods.Select(food => new FoodListingModel
            {
                Id = food.Id,
                Name = food.Name,
                InStock = food.InStock,
                Price = food.Price,
                ShortDescription = food.ShortDescription,
                Category = _mapper.FoodToCategoryListing(food),
                ImageUrl = food.ImageUrl
            });

            var model = new CategoryTopicModel
            {
                Category = _mapper.CategoryToCategoryListing(category),
                Foods = foodListings
            };

            return View(model);
        }

        public IActionResult Search(int id, string searchQuery)
        {
            return RedirectToAction("Topic", new { id, searchQuery });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult New()
        {
            ViewBag.ActionText = "create";
            ViewBag.Action = "New";
            ViewBag.CancelAction = "Index";
            ViewBag.SubmitText = "Create Category";
            return View("CreateEdit");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult New(CategoryListingModel model)
        {
            if (ModelState.IsValid)
            {
                var category = _mapper.CategoryListingToModel(model);
                _categoryService.NewCategory(category);
                return RedirectToAction("Topic", new { id = category.Id, searchQuery = "" });
            }

            ViewBag.ActionText = "create";
            ViewBag.Action = "New";
            ViewBag.CancelAction = "Index";
            ViewBag.SubmitText = "Create Category";

            return View("CreateEdit", model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.ActionText = "change";
            ViewBag.Action = "Edit";
            ViewBag.CancelAction = "Topic";
            ViewBag.SubmitText = "Save Changes";
            ViewBag.RouteId = id;
            Category categoryFromResponse = new Category();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:44391/api/Category/" + id.ToString()))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    categoryFromResponse = JsonConvert.DeserializeObject<Category>(apiResponse);
                }
            }
            //var category = _categoryService.GetById(id);

            var category = categoryFromResponse;
            if (category != null)
            {
                var model = _mapper.CategoryToCategoryListing(category);
                return View("CreateEdit", model);
            }

            return View("CreateEdit");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(CategoryListingModel model)
        {
            if (ModelState.IsValid)
            {
                var category = _mapper.CategoryListingToModel(model);
                _categoryService.EditCategory(category);
                return RedirectToAction("Topic", new { id = category.Id, searchQuery = "" });
            }

            ViewBag.ActionText = "change";
            ViewBag.Action = "Edit";
            ViewBag.CancelAction = "Topic";
            ViewBag.SubmitText = "Save Changes";
            ViewBag.RouteId = model.Id;

            return View("CreateEdit", model);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            _categoryService.DeleteCategory(id);

            return RedirectToAction("Index");
        }
    }
}
