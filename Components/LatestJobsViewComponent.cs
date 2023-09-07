using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PEF.Models;

namespace PEF.Components
{
    public class LatestJobsViewComponent : ViewComponent
    {
        private readonly DataContext _context;

        public LatestJobsViewComponent(DataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewBag.LatestJobs = _context.Jobs.Where(a => a.Deleted == false && a.Approved == true
            && (a.StartDate == null || a.StartDate <= DateTime.Now)
            && (a.ExpireDate == null || a.ExpireDate >= DateTime.Now.AddDays(-1))
            )
            .Include(a => a.City)
            .Include(a => a.Employer)
            .Include(a => a.Currency)
            .OrderByDescending(a => a.Id)
            .Take(3)
            .ToList();

            return View("Default");
        }
    }
}
