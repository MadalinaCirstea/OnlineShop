using Shop.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.API.Services
{
    public interface IFood
    {
        IEnumerable<Food> GetAll();
        IEnumerable<Food> GetPreferred(int count);
        IEnumerable<Food> GetFoodsByCategoryId(int categoryId);
        Food GetById(int id);
        void DeleteFood(int id);
    }
}
