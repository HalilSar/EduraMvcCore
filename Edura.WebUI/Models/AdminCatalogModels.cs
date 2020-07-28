using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edura.WebUI.Models
{
    // Oluşturduğumuz Bu Modeli AdminControllerda doldurmam gerekiyo şimdi.

    public class AdminEditCategoryModel // Bunu biz direckt olarak Edit sayfasına taşıyacaz.
    {// Sayfaya neleri taşıyacağım
        public int CategoryId { get; set; }  // 1
        public string CategoryName { get; set; } // 2
        public List<AdminEditCategoryProduct> Products { get; set; } //3. olarak sayfaya bir List taşıtacşım. Burada List içine "<>" Product yazıp Entitydeki Productı kullanabiliriz. Lakin ben buradan sadece istediklerimi seçmek istiyorum.Bu işlem  "DTO" olabilir mi? --> Büyük ihtimalle evet.
        /* Dolasıyla ben nurada ayrı bir sınıf oluşturabilirim.
         */
    }

    public class AdminEditCategoryProduct// "Kesinlikle bu bir " dto "---> Bu Categorye bağlı bir product modeli
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
        public bool IsApproved { get; set; }
        public bool IsHome { get; set; }
        public bool IsFeatured { get; set; }
    }
}
