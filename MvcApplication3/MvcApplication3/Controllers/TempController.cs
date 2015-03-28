using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Configuration;
using System.Web.Configuration;
using System.Collections.ObjectModel;

namespace MvcApplication3.Controllers
{
    public class Phone
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public string Producer { get; set; }
    }
    public class PageInfo
    {
        public int PageNumber { get; set; } // номер текущей страницы
        public int PageSize { get; set; } // кол-во объектов на странице
        public int TotalItems { get; set; } // всего объектов
        public int TotalPages  // всего страниц
        {
            get { return (int)Math.Ceiling((decimal)TotalItems / PageSize); }
        }
    }
    public class IndexViewModel
    {
        public IEnumerable<Phone> Phones { get; set; }
        public PageInfo PageInfo { get; set; }
    }


    public class TempController : Controller
    {
        List<Phone> phones;
        public TempController()
        {
            phones = new List<Phone>();
            phones.Add(new Phone { Id = 1, Model = "Samsung Galaxy III", Producer = "Samsung" });
            phones.Add(new Phone { Id = 2, Model = "Samsung Ace II", Producer = "Samsung" });
            phones.Add(new Phone { Id = 3, Model = "HTC Hero", Producer = "HTC" });
            phones.Add(new Phone { Id = 4, Model = "HTC One S", Producer = "HTC" });
            phones.Add(new Phone { Id = 5, Model = "HTC One X", Producer = "HTC" });
            phones.Add(new Phone { Id = 6, Model = "LG Optimus 3D", Producer = "LG" });
            phones.Add(new Phone { Id = 7, Model = "Nokia N9", Producer = "Nokia" });
            phones.Add(new Phone { Id = 8, Model = "Samsung Galaxy Nexus", Producer = "Samsung" });
            phones.Add(new Phone { Id = 9, Model = "Sony Xperia X10", Producer = "SONY" });
            phones.Add(new Phone { Id = 10, Model = "Samsung Galaxy II", Producer = "Samsung" });
        }
        public Collection<Phone> Index(int page = 1)
    {
        phones = new List<Phone>();
        phones.Add(new Phone { Id = 1, Model = "Samsung Galaxy III", Producer = "Samsung" });
        phones.Add(new Phone { Id = 2, Model = "Samsung Ace II", Producer = "Samsung" });
        phones.Add(new Phone { Id = 3, Model = "HTC Hero", Producer = "HTC" });
        phones.Add(new Phone { Id = 4, Model = "HTC One S", Producer = "HTC" });
        phones.Add(new Phone { Id = 5, Model = "HTC One X", Producer = "HTC" });
        phones.Add(new Phone { Id = 6, Model = "LG Optimus 3D", Producer = "LG" });
        phones.Add(new Phone { Id = 7, Model = "Nokia N9", Producer = "Nokia" });
        phones.Add(new Phone { Id = 8, Model = "Samsung Galaxy Nexus", Producer = "Samsung" });
        phones.Add(new Phone { Id = 9, Model = "Sony Xperia X10", Producer = "SONY" });
        phones.Add(new Phone { Id = 10, Model = "Samsung Galaxy II", Producer = "Samsung" });

        return new Collection<Phone>
                    {
                      new Phone {Id = phones[1].Id,Model = phones[1].Model,Producer = phones[1].Producer}
                    };
    }

    }
}

