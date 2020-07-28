using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Edura.WebUI.Repository.Abstract;
using Edura.WebUI.Models;
using Edura.WebUI.Entity;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace Edura.WebUI.Controllers
{
    /*Önceki bölümde Catalog2 Category ve Product eklemeyi ayarladık.
     * Bu Bölümde Admin sayfasının detay ayarlarını gösterem.
     * Catalog sayfasında bulunan edit butonlarını aktif hale getirelim.
     * "Daha önce kurduğumuz yapıya göre her kategorinin birden fazla ürünü ve her bir ürünün ise birden fazla kategorisi olabilir."
     * "Dolayısıyla ben edit sayfasına geldiğimde Kategori bilgisini çıkaracam ve  Kategori bilgisinin altına seçmek istediğimiz ürünleri getirmem gerekmektedir."
     * İlk olarak Views>Admin>CatalogList'e gelelim ve catalogliste bulunan edit butonunu aktif hale getirelim.
     * 
     */
    public class AdminController : Controller
    {
        private IUnitOfWork unitofWork;

        public AdminController(IUnitOfWork _unitofWork)
        {
            unitofWork = _unitofWork;
        }

        public IActionResult Index()
        {
            return View();
        }
       // Huzur sağladıktan sonra hareket huzuru bozuyo.
       /* Ne yapmalı
        * Huzuru veya mindful yakalıktan sonra yavaş hareket et.
        * Diyafram nefesi daha hızlı sonuç verir. Derin nefes rahatlamak için kullan, normal nefesi farkındalık için kullan.
        * Dikkatini aktif olan uzuvuna ve nefesine ver.
        * Yavaş hareket et ve etkin uzuvlarının farkındalığında bu hareketleri yap.
        * 
        * Bunu nasıl yapabilirim?
        * İlk olarak bir mini meditasyon yap ve mindful olduktan sonra bun yavaş hareketlerle korumaya çalış.
        * Mindful: Düşüncelerin,duyguların ve bence bunlara bağlıı olarak benin farkındalığıdır.
        */
       
        [HttpGet] //Metodumuz dışarıdan bir id alacak. id'ye göre bilgi çağıracak. 
        public IActionResult EditCategory(int id)  // İlgili kategorileri yükleycem ve yüklerken bunu onunla ilgili ürünleri getirecem.
        { // Bilgi almak için Categoryden CategoryID ve CategoryName geçecem ve oradan ProductCategory tablosuna gitmemiz   oradan Product propertyleri için Product tablosuna geçmemeiz gerekiyor. 
            // Benim ayrı model oluşturmam gerekiyor. "Neden?" Çünkü -->
            //"AdminEditCatalogModels" adını verdik. AdminCatalog sayfasından bizim bütün modellerimizi barındıracak olan bir model olsun, burası.
            var entity = unitofWork.Categories.GetAll()// Bir nesne oluşturduk ve UnitOfWork üzerinde.Categories geçtik ve GetAll metodu ile hepsini çağırdık. IQurable olduğu için bu bizinm filtre uygulayabileceğimiz anlamı taşımakta 
                                .Include(i => i.ProductCategories) // Include loading lazy loading engelliyor. Lazy loading nedir? * Lazy loadingi yerine terchi ediliyor.  Eager Loading ile tek sorgu ile işi halletmek daha iyi.Neden? --> Sorgu anında veri tabanına tek bir sorgu gönderiyoruz.  Include kullanmadığımızda her category için bir sorgu gider veri tabanına. ** Bu yöntem tavsiye edilen yöntem **
                                .ThenInclude(i => i.Product)
                                // daha sonra productcategories üzerinden product tablosuna gider. 
                                .Where(i => i.CategoryId == id)//  yukarıda gelen id sorguluyoruz where ile
                                .Select(i => new AdminEditCategoryModel() // seçme işlemine başlıyoruz.
                                {
                                    CategoryId = i.CategoryId,
                                    CategoryName = i.CategoryName,
                                    Products = i.ProductCategories.Select(a => new AdminEditCategoryProduct()//AdmiEditCategory içinden ProductCategories üzerinden Dto işlemi yapıyoruz.
                                    {
                                        ProductId = a.ProductId,
                                        ProductName = a.Product.ProductName,
                                        Image = a.Product.Image,
                                        IsApproved = a.Product.IsApproved,
                                        IsFeatured = a.Product.IsFeatured,
                                        IsHome = a.Product.IsHome
                                    }).ToList()
                                }).FirstOrDefault();//Sadece tek bir bilgi gelmesini istiyoruz

            return View(entity);
        } // Bu action viewini oluşturdu.   

        [HttpPost]// Güncelleme yapılcak burada. Bu yüzden Category ekleyecez.
        public IActionResult EditCategory(Category entity)
        {
            if (ModelState.IsValid)  //Eğer modelstateIsvalid ise
            {
                unitofWork.Categories.Edit(entity);//  güncelle categoriyi
                unitofWork.SaveChanges();//  kaydet değişikliği

                return RedirectToAction("CatalogList"); // ve yol al cataloglist actiona 
            }

            return View("Error");  // değilse yollan  hata viewi
        }

        [HttpPost] // Post olsun
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromCategory(int ProductId, int CategoryId)  // Dışarıdan int tipinde iki değer alıcak.
        {
            if (ModelState.IsValid) // eğer isvalid ise 
            {
                //silme Klasik sql'i ef aracılığıyla gönder. Aşağıda hem categori hem de productın olduğu ilişkisel tabloya gereksinim var.  EduraContexti kullanmayacağım. bunu EfCategoryRepository üzerinden gönderebilirim
                unitofWork.Categories.RemoveFromCategory(ProductId, CategoryId); //unitofwork'ten categories'i çağır categories'ten RemoveFromCategory ile ProductId ve CategoryId'ye göre kaldır.
                unitofWork.SaveChanges();// Değişiklikleri kaydet.
                return Ok(); // Bir problem yoksa Http200 göndeririz.  // 200 kodu gelirse EDİTCATEGORY DE AJAX KISMINDAKİ SUCCESS ÇALIŞACAK vE GÖRSEL OLARAK SİLECEK AMA BUNU VERİ TABANINDAN SİLMEMİZ GEREKİR. 
            }
            return BadRequest(); // Error function kullandık. 
        }// ajaxı tanımladıktan sonr bu beni iki tane action metodu tanımlamaktan kurtardı. Bu metot async değil.senkron metot. Bu ifade bana ne gibi bir çıkarım verir: ajax  yalnızca senkron bir actionda çalışır.  


        public IActionResult CatalogList()
        {
            var model = new CatalogListModel()
            {
                Categories = unitofWork.Categories.GetAll().ToList(),
                Products = unitofWork.Products.GetAll().ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCategory(Category entity)
        {
            if (ModelState.IsValid)
            {
                unitofWork.Categories.Add(entity);
                unitofWork.SaveChanges();

                return Ok(entity);
            }
            return BadRequest();
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddProduct(Product entity, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\products", file.FileName);
                    var path_tn = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\products\\tn", file.FileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                        entity.Image = file.FileName;
                    }

                    using (var stream = new FileStream(path_tn, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }

                entity.DateAdded = DateTime.Now;
                unitofWork.Products.Add(entity);
                unitofWork.SaveChanges();
                return RedirectToAction("CatalogList");
            }

            return View(entity);
        }
    }
}