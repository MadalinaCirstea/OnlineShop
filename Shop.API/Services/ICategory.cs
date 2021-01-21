using Shop.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.API.Services
{
    public interface ICategory
    {
        IEnumerable<Category> GetAll();
        Category GetById(int id);
        void DeleteCategory(int id);
    }
}
