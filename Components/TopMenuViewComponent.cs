﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PEF.Models;

namespace PEF.Components
{
    public class TopMenuViewComponent : ViewComponent
    {
        private readonly DataContext _context;

        public TopMenuViewComponent(DataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Menu> ParentMenus = _context.Menus.Where(a => a.LocationId == 2 && (a.ParentId == 0 || a.ParentId==null) && a.Deleted == 0 && a.Active == 1).OrderByDescending(a => a.Priority).OrderBy(a => a.Id)
                .Include(a => a.MenuLocation)
                .ToList();

            List<Menu> SubMenus = _context.Menus.Where(a => a.LocationId == 2 && a.ParentId != 0 && a.ParentId!=null &&  a.Deleted == 0 && a.Active == 1).OrderByDescending(a => a.Priority).OrderBy(a => a.Id).ToList();

            ViewBag.ParentMenus = ParentMenus;
            ViewBag.SubMenus = SubMenus;
            return View("Default");
        }
    }
}
