using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PEF.Models;

namespace PEF.Components
{
    public class FooterMenuViewComponent : ViewComponent
    {
        private readonly DataContext _context;

        public FooterMenuViewComponent(DataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Menu> ParentMenus = _context.Menus.Where(a => a.LocationId == 3 && (a.ParentId == 0 || a.ParentId == null) && a.Deleted == 0 && a.Active == 1).OrderByDescending(a => a.Priority).OrderBy(a => a.Id)
                .Include(a => a.MenuLocation)
                .ToList();

            List<Menu> SubMenus = _context.Menus.Where(a => a.LocationId == 3 && a.ParentId != 0 && a.ParentId != null && a.Deleted == 0 && a.Active == 1).OrderByDescending(a => a.Priority).OrderBy(a => a.Id).ToList();

            List<Menu> FooterMenus = _context.Menus.Where(a => a.LocationId == 4 && (a.ParentId == 0 || a.ParentId == null) && a.Deleted == 0 && a.Active == 1).OrderByDescending(a => a.Priority).OrderBy(a => a.Id)
                .Include(a => a.MenuLocation)
                .ToList();

            var footerDesc = _context.Pages.Where(a => a.Deleted == false && a.Active == true && a.Title.Contains("شرح الصندوق - سفلي")).FirstOrDefault();
			var footerContact = _context.Pages.Where(a => a.Deleted == false && a.Active == true && a.Title.Contains("اتصل بنا - سفلي")).FirstOrDefault();

            ViewBag.FooterContact = footerContact;
			ViewBag.FooterDesc = footerDesc;
            ViewBag.ParentMenus = ParentMenus;
            ViewBag.FooterMenus = FooterMenus;
            ViewBag.SubMenus = SubMenus;

            var visits = _context.Visits.FirstOrDefault();
            if (visits != null)
            {
                ViewBag.Visits = visits.VisitsCount;
                visits.VisitsCount = visits.VisitsCount + 1;
                _context.Visits.Update(visits);
                _context.SaveChanges();
            }

            return View("Default");
        }
    }
}