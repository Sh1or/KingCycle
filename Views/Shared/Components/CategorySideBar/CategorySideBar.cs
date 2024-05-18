using App.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Components
{
    [ViewComponent]
    public class CategorySideBar : ViewComponent
    {
        public class CategorySideBarData()
        {

            public List<Category> Categories { get; set; }
            public int level { get; set; }

            public string categorieslug { get; set; }
        }
        public IViewComponentResult Invoke(CategorySideBarData data)
        {
            return View(data);
        }
    }
}