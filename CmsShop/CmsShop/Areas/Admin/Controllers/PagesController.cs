using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CmsShop.Areas.Admin.Models.Data;
using CmsShop.Areas.Admin.ViewModels.Pages;

namespace CmsShop.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            // Deklaracja listy PageVM
            List<PageVM> pagesList;

            using (Db db = new Db())
            {
                // Inicjalizacja listy
                pagesList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }

            //Zwracamy strony do widoku
            return this.View(pagesList);
        }

        // GET Admin/Pages
        [HttpGet]
        public ActionResult AddPage()
        {
            return this.View();
        }

        // Post Admin/Pages
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            // Sprawdzanie model state

            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            using (Db db = new Db())
            {
                string slug;
                // Inicjalizacja PageDTO
                PageDTO dto = new PageDTO();

                // Jełsi Slug nie jest wypełniony zastępujemy go tytułem

                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                // Zapobiegamy dodaniu takiej samej nazwy strony
                if (db.Pages.Any(x => x.Title == model.Title) || db.Pages.Any(x => x.Slug == slug))
                {
                    this.ModelState.AddModelError("", "ten tytuł lub adress już istnieje");
                    return this.View(model);
                }

                dto.Title = model.Title;
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSideBar = model.HasSideBar;
                dto.Sorting = 1000;

                // Zapis DTO
                db.Pages.Add(dto);
                db.SaveChanges();
            }

            this.TempData["SM"] = "Dodałeś nową stronęęęę";

            return RedirectToAction("AddPage");
        }
    }
}