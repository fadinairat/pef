using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PEF.Helpers;
using PEF.Models;

namespace PEF.Areas.Control.Controllers
{
    [Area("Control")]
    [Authorize]
    [PEF.AuthorizedAction]
    public class DonorsController : Controller
    {
        private readonly DataContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _environment;

        public DonorsController(DataContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment; 
        }

        // GET: Control/Donors
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.Donors.Include(d => d.User);
            return View(await dataContext.ToListAsync());
        }

        // GET: Control/Donors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Donors == null)
            {
                return NotFound();
            }

            var donors = await _context.Donors
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            ViewBag.Projects = _context.Projects.Where(a => a.Deleted == false && a.DonorId == id).OrderBy(a=> a.Name).ToList();

            if (donors == null)
            {
                return NotFound();
            }

            return View(donors);
        }

        // GET: Control/Donors/Create
        public IActionResult Create()
        {
            
            return View();
        }

        // POST: Control/Donors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ArName,Description,ArDescription,Logo,Email,Password,ConfirmPassword,AddDate,LastLogin,AddedBy,Active,Deleted")] Donors donors)
        {
            if (HttpContext.Session.GetString("id") != "")
            {
                donors.AddedBy = int.Parse(HttpContext.Session.GetString("id") ?? "1");
            }
            else donors.AddedBy = 1;

            if (ModelState.IsValid)
            {
                if (donors.Email != null && DonorExistsByEmail(donors.Email, null))
                {
                    TempData["error"] = "Another account with same email already exists...";
                    return View("Create", donors);
                }

                //Filter Submitted data against Vulnerable content
                donors.ArName = Functions.RemoveHtml(donors.ArName);
                donors.Name = Functions.RemoveHtml(donors.Name);
                donors.Email = Functions.RemoveHtml(donors.Email);
                donors.Logo = Functions.RemoveHtml(donors.Logo);

                donors.Description = Functions.FilterHtml(donors.Description);
                donors.ArDescription = Functions.FilterHtml(donors.ArDescription);

                donors.AddDate = DateTime.Now;
                donors.Active = true;                
                donors.Deleted = false;

                if(donors.Password != null && donors.ConfirmPassword!=null)
                {
                    donors.Password = Encryption.Encrypt(donors.Password, true);
                    donors.ConfirmPassword = Encryption.Encrypt(donors.ConfirmPassword, true);
                }

                if (HttpContext.Request.Form.Files.Count > 0)
                {
                    var ImageUrl = ImagesUplaod.UploadSingleImage(HttpContext, "files/image/DonorsImg/", _environment.WebRootPath, "Logo");

                    if (ImageUrl.Item1.ToString() != "") donors.Logo = ImageUrl.Item1;
                }
                else donors.Logo = null;

                _context.Add(donors);
                await _context.SaveChangesAsync();

                TempData["success"] = "Donor created successfully...";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "Failed to create donor...";
            return View(donors);
        }

        // GET: Control/Donors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Donors == null)
            {
                return NotFound();
            }

            var donors = await _context.Donors.FindAsync(id);
            if (donors == null)
            {
                return NotFound();
            }
            
            return View(donors);
        }

        // POST: Control/Donors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ArName,Description,ArDescription,Logo,Email,Password,ConfirmPassword,AddDate,LastLogin,AddedBy,Active,Deleted")] Donors donors)
        {
            if (id != donors.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    if (donors.Email != null && DonorExistsByEmail(donors.Email, id))
                    {
                        TempData["error"] = "Another account with same email already exists...";
                        return View("Edit", donors);
                    }

                    _context.Donors.Attach(donors);
                    _context.Entry(donors).State = EntityState.Modified;
                    _context.Entry(donors).Property(a => a.AddDate).IsModified = false;
                    _context.Entry(donors).Property(a => a.AddedBy).IsModified = false;
                    _context.Entry(donors).Property(a => a.LastLogin).IsModified = false;
                    _context.Entry(donors).Property(a => a.Active).IsModified = false;

                    //Filter Submitted data against Vulnerable content
                    donors.ArName = Functions.RemoveHtml(donors.ArName);
                    donors.Name = Functions.RemoveHtml(donors.Name);
                    donors.Email = Functions.RemoveHtml(donors.Email);
                    donors.Logo = Functions.RemoveHtml(donors.Logo);

                    donors.Description = Functions.FilterHtml(donors.Description);
                    donors.ArDescription = Functions.FilterHtml(donors.ArDescription);

                    if (donors.Password != "" && donors.Password != "******")
                    {
                        donors.Password = Encryption.Encrypt(donors.Password, true);
                        donors.Password = Encryption.Encrypt(donors.ConfirmPassword, true);
                    }
                    else
                    {
                        _context.Entry(donors).Property(a => a.Password).IsModified = false;
                        _context.Entry(donors).Property(a => a.ConfirmPassword).IsModified = false;
                    }

                    if (HttpContext.Request.Form.Files.Count > 0)
                    {
                        var ImageUrl = ImagesUplaod.UploadSingleImage(HttpContext, "files/image/DonorsImg/", _environment.WebRootPath, "Logo");

                        if (ImageUrl.Item1.ToString() != "") donors.Logo = ImageUrl.Item1;
                    }
                    else _context.Entry(donors).Property(a => a.Logo).IsModified = false;

                    TempData["success"] = "Donor details updated successfully...";
                    // _context.Update(donors);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DonorsExists(donors.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            
            return View(donors);
        }

        // GET: Control/Donors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Donors == null)
            {
                return NotFound();
            }

            var donors = await _context.Donors
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (donors == null)
            {
                return NotFound();
            }

            return View(donors);
        }

        // POST: Control/Donors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Donors == null)
            {
                return Problem("Entity set 'DataContext.Donors'  is null.");
            }
            var donors = await _context.Donors.Where(a=> a.Id == id).Include(a=> a.Projects).FirstOrDefaultAsync();
            if (donors != null)
            {
                if(donors.Projects.Count() == 0)
                {
                    //_context.Donors.Remove(donors);
                    donors.Deleted = true;
                    _context.Donors.Update(donors);
                    TempData["success"] = "Donor deleted successfully...";
                }
                else
                {
                    TempData["error"] = "Cannot delete! some project related to this donor.";
                    return RedirectToAction(nameof(Index));
                }
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Activate(int? id)
        {
            if (id == null || _context.Donors == null)
            {
                TempData["error"] = "Donor not found";
            }
            var donors = await _context.Donors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (donors == null)
            {
                TempData["error"] = "Donor not found";
            }

            donors.Active = true;
            _context.Update(donors);
            await _context.SaveChangesAsync();
            TempData["success"] = "Donor activated successfully...";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Deactivate(int? id)
        {
            if (id == null || _context.Donors == null)
            {
                TempData["error"] = "Donor not found";
            }
            var donors = await _context.Donors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (donors == null)
            {
                TempData["error"] = "Donor not found";
            }

            donors.Active = false;
            _context.Update(donors);
            await _context.SaveChangesAsync();
            TempData["success"] = "Donor deactivated successfully...";
            return RedirectToAction("Index");
        }

        private bool DonorsExists(int id)
        {
          return (_context.Donors?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private bool DonorExistsByEmail(string email, int? id)
        {
            //Make sure that there is no user in the system use the same email address
            Boolean employer_exist = (_context.Employers?.Any(e => e.Email == email && e.Deleted == false)).GetValueOrDefault();
            Boolean donor_exist = (_context.Donors?.Any(e => e.Email == email && e.Deleted == false && (id == null || e.Id != id))).GetValueOrDefault();
            Boolean member_exist = (_context.Members?.Any(e => e.Email == email && e.Deleted == false)).GetValueOrDefault();

            return (member_exist || employer_exist || donor_exist);
        }
    }
}
