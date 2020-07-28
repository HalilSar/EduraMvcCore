using Edura.WebUI.Repository.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edura.WebUI.Entity;
using System.Linq.Expressions;
using Edura.WebUI.Models;
using Microsoft.EntityFrameworkCore;

namespace Edura.WebUI.Repository.Concrete.EntityFramework
{// EduraContexte DbSet yok-->"CategoyProduct" bu yüzden buraya Categori ve Productın ilişkisel tablosunu girecez.  aşağıda bununla ilgili sql gidecek bi sorfu yazalım.
    public class EfCategoryRepository : EfGenericRepository<Category>, ICategoryRepository
    {
        public EfCategoryRepository(EduraContext context) : base(context)
        {
        }

        public EduraContext EduraContext
        {
            get { return context as EduraContext; }
        }

        public IEnumerable<CategoryModel> GetAllWithProductCount()
        {
            return GetAll().Select(i => new CategoryModel()
            {
                CategoryId = i.CategoryId,
                CategoryName = i.CategoryName,
                Count = i.ProductCategories.Count()
            });
        }

        public Category GetByName(string name)
        {
            return EduraContext.Categories
                .Where(i => i.CategoryName == name)
                .FirstOrDefault();
        }

        public void RemoveFromCategory(int ProductId, int CategoryId) // bunu ICategoryRepositorye tanımladık, ilk olarak. implement ettik
        {// sql sorgumu göndercem nereye database göndercem. 
            var cmd = $"delete from ProductCategory where ProductId={ProductId} and CategoryId={CategoryId}"; // klasik sql sorgusu
            context.Database.ExecuteSqlCommand(cmd);// commandımızı oluşturduktan sonra context içinden executsqlcommand ile cmd aktif hale getiyoruz.
        }
    }
}
