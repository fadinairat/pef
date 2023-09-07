using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PEF.Models;
using PEF.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace PEF.Areas.Control.Controllers
{
    [Area("Control")]
    [Authorize]
    [PEF.AuthorizedAction]
    public class ProjectsController : Controller
    {
        private readonly DataContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _environment;

        public ProjectsController(DataContext context, Microsoft.AspNetCore.Hosting.IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> addProjectEmployer([Bind("Id","ProjectId","EmployerId")] ProjectsEmployers projects)
        {
            if (ModelState.IsValid)
            {
                if(!_context.ProjectsEmployers.Where(a=> a.EmployerId == projects.EmployerId && a.ProjectId == projects.ProjectId).Any())
                {
                    _context.Add(projects);
                    await _context.SaveChangesAsync();
                    return Json(new
                    {
                        result = true,
                        msg = "Employer have been assigned to project!"
                    });
                }
                else
                {
                    return Json(new
                    {
                        result = false,
                        msg = "Employer already assigned to project!"
                    }); 
                }
                
            }
            else
            {
                return Json(new
                {
                    result = false,
                    msg = "Failed to add employer!"
                }); ; ;
            }
               
        }
        public async Task<IActionResult> removeProjectEmployer(int item)
        {
            ProjectsEmployers result = await _context.ProjectsEmployers.Where(a => a.Id == item).FirstOrDefaultAsync();
            if (result!=null)
            {
                _context.ProjectsEmployers.Remove(result);
                await _context.SaveChangesAsync();
                return Json(new
                {
                    result = true,
                    msg = "Item remove successfully..."
                });
            }
            else
            {
                return Json(new
                {
                    result = false,
                    msg = "Failed to find record!"
                });
            }
        }

        public IActionResult loadEmployers(int project)
        {
            var employers = _context.ProjectsEmployers.Where(a => a.ProjectId == project && a.Employer.Deleted == false)
                .Include(a=> a.Employer)
                .Include(a=> a.Project)
                .OrderByDescending(a => a.Employer.Name).ToList();

            return Json(new
            {
                result = true,
                employers = employers
            });
        }

        // GET: Control/Projects
        public async Task<IActionResult> Index(string? keyword, int? city,int? donor,int? program, int PageNumber = 1)
        {
            int PageSize = 20;
            Nullable<int> EmployerId = null;
            Boolean IsSuper = false;
            Nullable<int> UserId = null;
            if (HttpContext.Session.GetString("id") != null)
            {
                UserId = int.Parse(HttpContext.Session.GetString("id"));
            }

            if (HttpContext.Session.GetString("is_super_admin") == "True") IsSuper = true;
            if (!String.IsNullOrEmpty(HttpContext.Session.GetString("type")) && HttpContext.Session.GetString("type") == "Employer")
            {
                EmployerId = int.Parse(HttpContext.Session.GetString("id"));
            }
            

            ViewBag.Cities = _context.City.OrderBy(a => a.Name).OrderBy(a=> a.Priority).ToList();
            ViewBag.Donors = _context.Donors.Where(a=> a.Deleted== false && a.Active == true).OrderBy(a => a.Name).ToList();
            ViewBag.Programs = _context.LookupProjectsProgs.Where(a => a.Deleted == false).OrderBy(a => a.Name).ToList();

            var dataContext = _context.Projects.Where(a => a.Deleted == false
            && (keyword == null || a.Name.Contains(keyword) || a.ArName.Contains(keyword))
            && (city == null || a.Location == city)
            && (donor == null || a.DonorId == donor)
            && (program == null || a.ProgramType == program)
            && (EmployerId == null || (a.ProjectEmployers.Where(p => p.EmployerId == EmployerId).Any()))    
            && ((IsSuper == true || EmployerId != null || UserId ==null) || (a.UsersProjects !=null && a.UsersProjects.Any(b => b.UserId == UserId)))
            )
            .AsQueryable();

            var projects = dataContext.Include(p => p.City).Include(p => p.Currency).Include(p => p.Program).Include(p => p.User).Include(p => p.Donor)
            .OrderByDescending(p => p.Id)
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize);

            int dbPages = _context.Projects
           .Where(a => a.Deleted == false
            && (keyword == null || a.Name.Contains(keyword) || a.ArName.Contains(keyword))
            && (city == null || a.Location == city)
            && (donor == null || a.DonorId == donor)
            && (program == null || a.ProgramType == program)
            && (EmployerId == null || (a.ProjectEmployers.Where(p => p.EmployerId == EmployerId).Any()))
            && ((IsSuper == true || EmployerId != null || UserId == null) || (a.UsersProjects != null && a.UsersProjects.Any(b => b.UserId == UserId)))
           )
           .Count();

            float paging = (float)dbPages / PageSize;
            double TotalPages = Math.Round(paging);
            
            ViewBag.Keyword = keyword;
            ViewBag.City = city;
            ViewBag.Donor = donor;
            ViewBag.Program = program;

            ViewBag.PagesCount = TotalPages;
            ViewBag.DBPages = dbPages;

            ViewBag.Employers = _context.Employers.Where(a => a.Deleted == false).OrderBy(a => a.Name).ToList();

            ViewBag.TotalProjects = dataContext.Count();
            ViewBag.ActiveProjects = dataContext.Where(a => a.EndDate >= DateTime.Now.Date).Count();
            ViewBag.FinishedProjects = dataContext.Where(a => a.EndDate < DateTime.Now.Date).Count();

            return View(await projects.ToListAsync());
        }

        // GET: Control/Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var employers = _context.ProjectsEmployers.Where(a => a.Employer.Deleted == false && a.ProjectId == id)
                .Include(a => a.Employer)
                .OrderBy(a => a.Employer.Name)
                .ToList();

            var projects = await _context.Projects
                .Include(p => p.City)
                .Include(p => p.Currency)
                .Include(p => p.Program)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projects == null)
            {
                return NotFound();
            }

            ViewBag.Employers = employers;

            return View(projects);
        }

        // GET: Control/Projects/Create
        public IActionResult Create()
        {
            ViewData["Location"] = new SelectList(_context.City, "Id", "Name");
            ViewData["CurrencyId"] = new SelectList(_context.LookupCurrencies, "Id", "Name");
            ViewData["ProgramType"] = new SelectList(_context.LookupProjectsProgs, "Id", "Name");
            ViewData["AddedBy"] = new SelectList(_context.Users, "Id", "FullName");
            ViewData["Donors"] = _context.Donors.Where(a=> a.Deleted == false && a.Active==true).ToList();
            ViewData["Forms"] = _context.Forms.Where(a => a.Deleted == false && a.Active == true && a.Type==1).ToList();
            ViewData["InternalForms"] = _context.Forms.Where(a => a.Deleted == false && a.Active == true && a.Type==2).ToList();
            return View();
        }

        // POST: Control/Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProgramType,Program,ProjectType,Name,ArName,DonorId,Donor,Description,ArDescription,CurrencyId,Partners,ArPartners,ProjectActivities,ArProjectActivities,TargetGroup,ArTargetGroup,BeneficiaryNumbers,Location,Duration,DurationUnit,StartDate,EndDate,ProjectCoordinator,ArProjectCoordinator,Outputs,ArOutputs,Conditions,ArConditions,ProjectImage,Standards,ArStandards,Notes,ArNotes,TargetLocations,Budget,AddedBy,User,AddedTime,EditedBy,LastEdit,Active,Deleted,FormId,InternalFormId,District,DonorName")] Projects projects)
        {
            ModelState.Remove("Program");
            ModelState.Remove("Currency");
            ModelState.Remove("City");
            ModelState.Remove("User");
            ModelState.Remove("Donor");

            if (HttpContext.Session.GetString("id") != "")
            {
                projects.AddedBy = int.Parse(HttpContext.Session.GetString("id") ?? "1");
            }
            else projects.AddedBy = 1;

            if (ModelState.IsValid)
            {
                //if (HttpContext.Request.Form.Files.Count > 0)
                //{
                //    var ImageUrl = ImagesUplaod.UploadSingleImage(HttpContext, "files/image/ProjectsImg/", _environment.WebRootPath, "DonorLogo");

                //    if (ImageUrl.Item1.ToString() != "") projects.DonorLogo = ImageUrl.Item1;
                //}
                //else projects.DonorLogo = null;
                projects.Name = Functions.RemoveHtml(projects.Name);
                projects.ArName = Functions.RemoveHtml(projects.ArName);

                projects.Description = Functions.FilterHtml(projects.Description);
                projects.ArDescription = Functions.FilterHtml(projects.ArDescription);
                projects.Partners = Functions.FilterHtml(projects.Partners);
                projects.ArPartners = Functions.FilterHtml(projects.ArPartners);
                projects.Notes = Functions.FilterHtml(projects.Notes);
                projects.ArNotes = Functions.FilterHtml(projects.ArNotes);
                projects.Standards = Functions.FilterHtml(projects.Standards);
                projects.ArStandards = Functions.FilterHtml(projects.ArStandards);
                projects.ArProjectActivities = Functions.FilterHtml(projects.ArProjectActivities);
                projects.ProjectActivities = Functions.FilterHtml(projects.ProjectActivities);
                projects.TargetGroup = Functions.FilterHtml(projects.TargetGroup);
                projects.ArTargetGroup = Functions.FilterHtml(projects.ArTargetGroup);
                projects.Conditions = Functions.FilterHtml(projects.Conditions);
                projects.ArConditions = Functions.FilterHtml(projects.ArConditions);
                projects.TargetLocations = Functions.FilterHtml(projects.TargetLocations);


                if (HttpContext.Request.Form.Files.Count > 0)
                {
                    var ImageUrl = ImagesUplaod.UploadSingleImage(HttpContext, "files/image/ProjectsImg/", _environment.WebRootPath, "ProjectImage");

                    if (ImageUrl.Item1.ToString() != "") projects.ProjectImage = ImageUrl.Item1;
                }
                else projects.ProjectImage = null;

                projects.AddedTime = DateTime.Now;
                TempData["success"] = "Project added successfully...";

                _context.Add(projects);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Location"] = new SelectList(_context.City, "Id", "Name", projects.Location);
            ViewData["CurrencyId"] = new SelectList(_context.LookupCurrencies, "Id", "Name", projects.CurrencyId);
            ViewData["ProgramType"] = new SelectList(_context.LookupProjectsProgs, "Id", "Name", projects.ProgramType);
            ViewData["AddedBy"] = new SelectList(_context.Users, "Id", "FullName", projects.AddedBy);
            ViewData["Donors"] = _context.Donors.Where(a => a.Deleted == false && a.Active == true).ToList();
            ViewData["Forms"] = _context.Forms.Where(a => a.Deleted == false && a.Active == true && a.Type == 1).ToList();
            ViewData["InternalForms"] = _context.Forms.Where(a => a.Deleted == false && a.Active == true && a.Type == 2).ToList();
            return View(projects);
        }

        // GET: Control/Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var projects = await _context.Projects.FindAsync(id);
            if (projects == null)
            {
                return NotFound();
            }
            ViewData["Location"] = new SelectList(_context.City, "Id", "Name", projects.Location);
            ViewData["CurrencyId"] = new SelectList(_context.LookupCurrencies, "Id", "Name", projects.CurrencyId);
            ViewData["ProgramType"] = new SelectList(_context.LookupProjectsProgs, "Id", "Name", projects.ProgramType);
            ViewData["AddedBy"] = new SelectList(_context.Users, "Id", "FullName", projects.AddedBy);
            ViewData["Donors"] = _context.Donors.Where(a => a.Deleted == false && a.Active == true).ToList();
            ViewData["Forms"] = _context.Forms.Where(a => a.Deleted == false && a.Active == true && a.Type == 1).ToList();
            ViewData["InternalForms"] = _context.Forms.Where(a => a.Deleted == false && a.Active == true && a.Type == 2).ToList();

            return View(projects);
        }

        // POST: Control/Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProgramType,ProjectType,Name,ArName,DonorId,Description,ArDescription,CurrencyId,Partners,ArPartners,ProjectActivities,ArProjectActivities,TargetGroup,ArTargetGroup,BeneficiaryNumbers,Location,Duration,DurationUnit,StartDate,EndDate,ProjectCoordinator,ArProjectCoordinator,Outputs,ArOutputs,Conditions,ArConditions,ProjectImage,Standards,ArStandards,Notes,ArNotes,AddedBy,AddedTime,EditedBy,LastEdit,Active,Deleted,FormId,InternalFormId,District")] Projects projects)
        {
            ModelState.Remove("Program");
            ModelState.Remove("Currency");
            ModelState.Remove("City");
            ModelState.Remove("User");
            ModelState.Remove("Donor");

            if (id != projects.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Projects.Attach(projects);
                    _context.Entry(projects).State = EntityState.Modified;

                    _context.Entry(projects).Property(a => a.AddedTime).IsModified = false;
                    _context.Entry(projects).Property(a => a.AddedBy).IsModified = false;
                    _context.Entry(projects).Property(a => a.Active).IsModified = false;
                    //_context.Entry(projects).Property(a => a.LastEdit).IsModified = false;

                    //Filter the Content against Vulnerable Data
                    projects.Name = Functions.RemoveHtml(projects.Name);
                    projects.ArName = Functions.RemoveHtml(projects.ArName);

                    projects.Description = Functions.FilterHtml(projects.Description);
                    projects.ArDescription = Functions.FilterHtml(projects.ArDescription);
                    projects.Partners = Functions.FilterHtml(projects.Partners);
                    projects.ArPartners = Functions.FilterHtml(projects.ArPartners);
                    projects.Notes = Functions.FilterHtml(projects.Notes);
                    projects.ArNotes = Functions.FilterHtml(projects.ArNotes);
                    projects.Standards = Functions.FilterHtml(projects.Standards);
                    projects.ArStandards = Functions.FilterHtml(projects.ArStandards);
                    projects.ArProjectActivities = Functions.FilterHtml(projects.ArProjectActivities);
                    projects.ProjectActivities = Functions.FilterHtml(projects.ProjectActivities);
                    projects.TargetGroup = Functions.FilterHtml(projects.TargetGroup);
                    projects.ArTargetGroup = Functions.FilterHtml(projects.ArTargetGroup);
                    projects.Conditions = Functions.FilterHtml(projects.Conditions);
                    projects.ArConditions = Functions.FilterHtml(projects.ArConditions);
                    projects.TargetLocations = Functions.FilterHtml(projects.TargetLocations);

                    if (HttpContext.Session.GetString("id") != "")
                    {
                        projects.EditedBy = int.Parse(HttpContext.Session.GetString("id") ?? "1");
                    }
                    else projects.EditedBy = 1;

                    projects.LastEdit = DateTime.Now;

                    if (HttpContext.Request.Form.Files.Count > 0)
                    {
                        var ImageUrl = ImagesUplaod.UploadSingleImage(HttpContext, "files/image/ProjectsImg/", _environment.WebRootPath, "ProjectImage");

                        if (ImageUrl.Item1.ToString() != "") projects.ProjectImage = ImageUrl.Item1;
                    }
                    else
                    {
                        _context.Entry(projects).Property(a => a.ProjectImage).IsModified = false;
                    }

                    TempData["success"] = "Project details has been edited successfully...";
                    

                    //_context.Update(projects);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectsExists(projects.Id))
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
            ViewData["Location"] = new SelectList(_context.City, "Id", "Name", projects.Location);
            ViewData["CurrencyId"] = new SelectList(_context.LookupCurrencies, "Id", "Name", projects.CurrencyId);
            ViewData["ProgramType"] = new SelectList(_context.LookupProjectsProgs, "Id", "Name", projects.ProgramType);
            ViewData["AddedBy"] = new SelectList(_context.Users, "Id", "FullName", projects.AddedBy);
            ViewData["Donors"] = _context.Donors.Where(a => a.Deleted == false && a.Active == true).ToList();
            ViewData["Forms"] = _context.Forms.Where(a => a.Deleted == false && a.Active == true && a.Type == 1).ToList();
            ViewData["InternalForms"] = _context.Forms.Where(a => a.Deleted == false && a.Active == true && a.Type == 2).ToList();
            return View(projects);
        }

        // GET: Control/Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }
            

            var projects = await _context.Projects
                .Include(p => p.City)
                .Include(p => p.Currency)
                .Include(p => p.Program)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projects == null)
            {
                return NotFound();
            }

            return View(projects);
        }

        // POST: Control/Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Projects == null)
            {
                return Problem("Entity set 'DataContext.Projects'  is null.");
            }

            var jobs = _context.Jobs.Where(a => a.Deleted == false && a.ProjectId == id).Count();
            if (jobs > 0)
            {
                TempData["error"] = "Cannot delete project! some Jobs are related to this project.";
                return RedirectToAction(nameof(Index));
            }

            var projects = await _context.Projects.FindAsync(id);
            if (projects != null)
            {
                TempData["success"] = "Project deleted successfully...";
                projects.Deleted = true;
                //_context.Projects.Remove(projects);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectsExists(int id)
        {
          return (_context.Projects?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
