using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Shop.API.Services
{
    public class FoodService : IFood
    {
        private readonly ApplicationDbContext _context;

        public FoodService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void DeleteFood(int id)
        {
            var food = GetById(id);
            if (food == null)
            {
                throw new ArgumentException();
            }
            _context.Remove(food);
            _context.SaveChanges();
        }

        public IEnumerable<Food> GetAll()
        {
            return _context.Foods
                .Include(food => food.Category);
        }

        public Food GetById(int id)
        {
            return GetAll().FirstOrDefault(food => food.Id == id);
        }

        public IEnumerable<Food> GetFoodsByCategoryId(int categoryId)
        {
            return GetAll().Where(food => food.Category.Id == categoryId);
        }

        public IEnumerable<Food> GetPreferred(int count)
        {
            return GetAll().OrderByDescending(food => food.Id).Where(food => food.IsPreferedFood && food.InStock != 0).Take(count);
        }
    }
}