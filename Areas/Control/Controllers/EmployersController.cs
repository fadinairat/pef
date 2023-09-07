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
using OfficeOpenXml;
using System.Drawing.Printing;
using OfficeOpenXml.Style;
using System.Xml;
using Microsoft.Extensions.Localization;

namespace PEF.Areas.Control.Controllers
{
    [Area("Control")]
    [Authorize]
    [PEF.AuthorizedAction]
    public class EmployersController : Controller
    {
        private readonly DataContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _environment;
        private readonly string currentCulture;
        private readonly IStringLocalizer<HomeController> _localizer;

        public EmployersController (DataContext context, Microsoft.AspNetCore.Hosting.IWebHostEnvironment environment, IStringLocalizer<HomeController> localizer)
        {
            _context = context;
            _environment = environment;
            currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            _localizer = localizer;
        }

        public IActionResult getSectors(int type)
        {
            var sectors = _context.LookupEmployersSectors.Where(a => a.Deleted == false && a.Type == type).OrderBy(a => a.Id).ToList();

            return Json(new
            {
                result = true,
                sectors = sectors
            });
        }

        [HttpGet]
        public IActionResult ExportToExcel(string? keyword, int? type, int? sector, int? worksector,int? city, int? Districtarea)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var package = new ExcelPackage();

            var worksheet = package.Workbook.Worksheets.Add("Sheet1");

            var employersList = _context.Employers.Where(a => a.Deleted == false
            && (keyword == null || (a.Name.Contains(keyword) || a.ArName.Contains(keyword)))
            && (type == null || a.Type == type)
            && (sector == null || a.Sector == sector)
            && (worksector == null || a.WorkSectorId == worksector)
            && (city == null || a.CityId == city)
            && (Districtarea == null || a.AreaId == Districtarea)
            )
            .Include(e => e.EmployerSector)
            .Select(e=> new
            {
                Employer = e,
                City = e.City !=null ? e.City : null,
                Area = e.Area !=null ? e.Area : null,
                WorkSector = e.WorkSector !=null ? e.WorkSector : null,
                JobsCount = e.Jobs.Where(a => a.Deleted == false).Count(),
                AppsCount = e.Jobs.SelectMany(a => a.JobsApplications.Where(j => j.Deleted == false && j.Job.Deleted == false)).Count(),
                EmployedCount = e.Jobs.SelectMany(a => a.JobsApplications.Where(j => j.Deleted == false && j.Job.Deleted == false && (j.Status == 1 || j.Status == 5))).Count(),
            })
            .ToList();

            //Console.WriteLine("Result Count:" + employersList.Count());

            //return Json(new
            //{
            //    results = employersList
            //});
            //This row to export all model data

            int index = 0;
            // Set the header row
            worksheet.Cells[1, ++index].Value = "#";
            worksheet.Cells[1, ++index].Value = "الاسم بالعربي";
            worksheet.Cells[1, ++index].Value = "الاسم بالانجليزي";
            worksheet.Cells[1, ++index].Value = "الاختصار";
            worksheet.Cells[1, ++index].Value = "نوع الحساب";
            worksheet.Cells[1, ++index].Value = "نوع المؤسسة";
            worksheet.Cells[1, ++index].Value = "القطاع";
            worksheet.Cells[1, ++index].Value = "سنة التأسيس";
            worksheet.Cells[1, ++index].Value = "رقم التسجيل";
            worksheet.Cells[1, ++index].Value = "رقم المشتغل المرخص";
            worksheet.Cells[1, ++index].Value = "المحافظة";
            worksheet.Cells[1, ++index].Value = "المنطقة";
            worksheet.Cells[1, ++index].Value = "العنوان";

            worksheet.Cells[1, ++index].Value = "الهاتف";
            worksheet.Cells[1, ++index].Value = "رقم الجوال";
            worksheet.Cells[1, ++index].Value = "رقم الجوال 2";
            worksheet.Cells[1, ++index].Value = "البريد الالكتروني";

            worksheet.Cells[1, ++index].Value = "اسم  شخص التواصل";
            worksheet.Cells[1, ++index].Value = "المسمى الوظيفي";
            worksheet.Cells[1, ++index].Value = "رقم الجوال";
            worksheet.Cells[1, ++index].Value = "رقم الجوال 2";
            worksheet.Cells[1, ++index].Value = "البريد الالكتروني";
            worksheet.Cells[1, ++index].Value = "مجال العمل بالعربي";
            worksheet.Cells[1, ++index].Value = "بيانات اضافية بالعربي";
            worksheet.Cells[1, ++index].Value = "عدد الموظفين الدائمين - ذكور";
            worksheet.Cells[1, ++index].Value = "عدد الموظفين الدائمين - اناث";
            worksheet.Cells[1, ++index].Value = "عدد الموظفين بدوام كامل - ذكور";
            worksheet.Cells[1, ++index].Value = "عدد الموظفين بدوام كامل - اناث";
            worksheet.Cells[1, ++index].Value = "عدد الموظفين بدوام جزئي - ذكور";
            worksheet.Cells[1, ++index].Value = "عدد الموظفين بدوام جزئي - اناث";
            worksheet.Cells[1, ++index].Value = "عدد أيام الدوام في الأسبوع";
            worksheet.Cells[1, ++index].Value = "ساعات العمل اليومية";
            worksheet.Cells[1, ++index].Value = "هل يوجد لدى المشغل / تامين إصابات عمل";
            worksheet.Cells[1, ++index].Value = "هل يوجد لدى المشغل تامين صحي للموظفين";
            worksheet.Cells[1, ++index].Value = "هل المؤسسة/ الشركة على استعداد لاستضافة متدربين على رأس العمل ";
            worksheet.Cells[1, ++index].Value = "عدد الفروع للمؤسسة / الشركة";
            worksheet.Cells[1, ++index].Value = "عناوين الفروع";
            worksheet.Cells[1, ++index].Value = "إجراءات السلامة والصحة المهنية بالعربية";
            worksheet.Cells[1, ++index].Value = "تقارير مالية";
            worksheet.Cells[1, ++index].Value = "تقارير ادارية";
            worksheet.Cells[1, ++index].Value = "شهادة ترخيص المؤسسة / الشركة";
            worksheet.Cells[1, ++index].Value = "السجل التجاري";
            worksheet.Cells[1, ++index].Value = "مرفقات اخرى";
            // worksheet.Cells[1, 25].Value = "Jobs Count";
            //worksheet.Cells[1, 26].Value = "Jobs Applications Count";
            //worksheet.Cells[1, 27].Value = "Employed Count";

            // Format header row
            using (var range = worksheet.Cells[1, 1, 1, index])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.BlueViolet);
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                //range.Style.WrapText = true; // Wrap text if needed
                range.Style.Font.Size = 12; // Set font size                
                worksheet.Row(1).Height += 7; // Add extra height to the row
            }


            // Set the data rows
            int i = 2;
            int count = employersList.Count();


            foreach (var item in employersList)
            {
                index = 0;
                Employers employer = item.Employer;

                worksheet.Cells[i, ++index].Value = employer.Id;
                worksheet.Cells[i, ++index].Value = employer.ArName;
                worksheet.Cells[i, ++index].Value = employer.Name;
                worksheet.Cells[i, ++index].Value = employer.Shortcut;
                if (employer.Type == 1) worksheet.Cells[i, ++index].Value = "شركة";
                else worksheet.Cells[i, ++index].Value = "مؤسسة";

                worksheet.Cells[i, ++index].Value = employer.EmployerSector?.ArName;
                worksheet.Cells[i, ++index].Value = employer.WorkSector?.ArName;

                worksheet.Cells[i, ++index].Value = employer.EstablishYear;
                worksheet.Cells[i, ++index].Value = employer.RegNumber;
                worksheet.Cells[i, ++index].Value = employer.OperationNumber;
                worksheet.Cells[i, ++index].Value = item.City?.ArName;
                worksheet.Cells[i, ++index].Value = item.Area?.ArName;
                worksheet.Cells[i, ++index].Value = employer.Village;


                worksheet.Cells[i, ++index].Value = employer.Tel;
                worksheet.Cells[i, ++index].Value = employer.Mobile;
                worksheet.Cells[i, ++index].Value = employer.Mobile;
                worksheet.Cells[i, ++index].Value = employer.Email;

                worksheet.Cells[i, ++index].Value = employer.ContactName;
                worksheet.Cells[i, ++index].Value = employer.contactJobTitle;
                worksheet.Cells[i, ++index].Value = employer.ContactMobile;
                worksheet.Cells[i, ++index].Value = employer.ContactMobile2;
                worksheet.Cells[i, ++index].Value = employer.ContactEmail;

                worksheet.Cells[i, ++index].Value = employer.ArDescription;
                worksheet.Cells[i, ++index].Value = employer.ArExtraData;


                worksheet.Cells[i, ++index].Value = employer.PermanentEmpCount;
                worksheet.Cells[i, ++index].Value = employer.PermanentFemaleEmpCount;
                worksheet.Cells[i, ++index].Value = employer.FulltimeEmpCount;
                worksheet.Cells[i, ++index].Value = employer.FulltimeFemaleEmpCount;
                worksheet.Cells[i, ++index].Value = employer.ParttimeEmpCount;
                worksheet.Cells[i, ++index].Value = employer.ParttimeFemaleEmpCount;
                worksheet.Cells[i, ++index].Value = employer.WeekWorkDays;
                worksheet.Cells[i, ++index].Value = employer.DayWorkHours;
                worksheet.Cells[i, ++index].Value = employer.InjuredInsurance == true ? "نعم" : "لا";
                worksheet.Cells[i, ++index].Value = employer.HealthInsurance == true ? "نعم" : "لا";
                worksheet.Cells[i, ++index].Value = employer.AcceptTrainers == true ? "نعم" : "لا";
                worksheet.Cells[i, ++index].Value = employer.Branches;
                worksheet.Cells[i, ++index].Value = employer.BranchesLocation;
                worksheet.Cells[i, ++index].Value = employer.ArSafetyProcedures;

                if (!String.IsNullOrEmpty(employer.FinancialReport)) worksheet.Cells[i, ++index].Value = Url.Action("GetFile", "Home", new { Area = "Control", path = employer.FinancialReport }, protocol: Request.Scheme);
                else worksheet.Cells[i, ++index].Value = "";

                if (!String.IsNullOrEmpty(employer.AnnualReport)) worksheet.Cells[i, ++index].Value = Url.Action("GetFile", "Home", new { Area = "Control", path = employer.AnnualReport }, protocol: Request.Scheme);
                else worksheet.Cells[i, ++index].Value = "";

                if (!String.IsNullOrEmpty(employer.RegistrationDocument)) worksheet.Cells[i, ++index].Value = Url.Action("GetFile", "Home", new { Area = "Control", path = employer.RegistrationDocument }, protocol: Request.Scheme);
                else worksheet.Cells[i, ++index].Value = "";

                if (!String.IsNullOrEmpty(employer.CommercialRegister)) worksheet.Cells[i, ++index].Value = Url.Action("GetFile", "Home", new { Area = "Control", path = employer.CommercialRegister }, protocol: Request.Scheme);
                else worksheet.Cells[i, ++index].Value = "";

                if (!String.IsNullOrEmpty(employer.OtherAttachments)) worksheet.Cells[i, ++index].Value = Url.Action("GetFile", "Home", new { Area = "Control", path = employer.OtherAttachments }, protocol: Request.Scheme);
                else worksheet.Cells[i, ++index].Value = "";
                i++;
            }
            // Auto-fit the columns
            worksheet.Cells.AutoFitColumns();

            // Set the content type and file name for the file result
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = "employers.xlsx";

            // Return the file as a FileResult
            return File(package.GetAsByteArray(), contentType, fileName);
        }

        // GET: Control/Employers
        public async Task<IActionResult> Index(string? keyword,int? type,int? sector, int? worksector,int? city,int? area, int PageNumber=1)
        {
            int PageSize = 20;
            ViewBag.Cities = _context.City.OrderBy(a => a.Name).ToList();
            ViewBag.WorkSectors = _context.LookupWorkSectors.OrderBy(a => a.Name).ToList();
            if (city != null)
            {
                ViewBag.Areas = _context.Villages.Where(a => a.CityId == city && a.Deleted == false).OrderBy(a => a.Name).ToList();
            }

            var dataContext = _context.Employers.Where(a => a.Deleted == false
            && (keyword == null || (a.Name.Contains(keyword) || a.ArName.Contains(keyword)))
            && (type == null || a.Type == type)
            && (sector == null || a.Sector == sector)
            && (worksector == null || a.WorkSectorId == worksector)
            && (city ==null || a.CityId == city)
            && (area == null || a.AreaId == area)
            )
            .AsQueryable();

            var employers = dataContext.Include(e => e.City).Include(e => e.WorkSector).Include(e=> e.EmployerSector)
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize);

            int dbPages = _context.Employers.Where(a => a.Deleted == false
            && (keyword == null || (a.Name.Contains(keyword) || a.ArName.Contains(keyword)))
            && (type == null || a.Type == type)
            && (sector == null || a.Sector == sector)
            && (worksector == null || a.WorkSectorId == worksector)
            && (city == null || a.CityId == city)
            && (area == null || a.AreaId == area)
            )
            .Include(e => e.City).Include(e => e.WorkSector)
            .Count();

            float paging = (float) dbPages / PageSize;
            double TotalPages = Math.Round(paging);
            ViewBag.Type = type;
            ViewBag.Sector = sector;
            ViewBag.City = city;
            ViewBag.Area = area;
            ViewBag.WorkSector = worksector;
            ViewBag.Keyword = keyword;
            ViewBag.PagesCount = TotalPages;
            ViewBag.DBPages = dbPages;
            ViewBag.CurrentPage = PageNumber;

            ViewBag.TotalEmployers = dataContext.Count();
            ViewBag.TotalCompanies = dataContext.Where(a => a.Type == 1).Count();
            ViewBag.TotalInstitutions = dataContext.Where(a => a.Type == 2).Count();
            ViewData["Sectors"] = _context.LookupEmployersSectors.Where(a => a.Deleted == false).OrderBy(a => a.Id).ToList();

            return View(await employers.ToListAsync());
        }

        // GET: Control/Employers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Employers == null)
            {
                return NotFound();
            }

            var employers = await _context.Employers
                .Include(e => e.City)
                .Include(e => e.WorkSector)
                .Include(e => e.EmployerSector)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employers == null)
            {
                return NotFound();
            }

            return View(employers);
        }

        // GET: Control/Employers/Create
        public IActionResult Create()
        {
            ViewData["CityId"] = _context.City.OrderBy(a=> a.Id).ToList();            
            ViewData["EmpTypes"] = _context.Lookups.Where(a => a.Deleted==false && a.Type == "EmpType").OrderBy(a => a.Priority).ToList();
            //ViewData["EmpSectors"] = _context.Lookups.Where(a => a.Deleted == false && a.Type == "EmpSector").OrderBy(a => a.Priority).ToList();
            ViewData["Sectors"] = _context.LookupEmployersSectors.Where(a => a.Deleted == false && a.Type == 1).OrderBy(a => a.Id).ToList();
            ViewData["WorkSectorId"] = new SelectList(_context.LookupWorkSectors, "Id", (currentCulture=="ar-AE" ? "ArName" : "Name"));
            return View();
        }

        // POST: Control/Employers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ArName,Shortcut,Type,Sector,WorkSectorId,EstablishYear,RegNumber,OperationNumber,CityId,AreaId,Village,Mobile,Tel,Email,Address,ContactName,contactJobTitle,ContactMobile,ContactMobile2,ContactEmail,Description,ArDescription,ExtraData,ArExtraData,SafetyProcedures,ArSafetyProcedures,PermanentEmpCount,ParttimeEmpCount,WeekWorkDays,DayWorkHours,FinancialReport,AnnualReport,RegistrationDocument,Branches,BranchesLocation,InjuredInsurance,HealthInsurance,AcceptTrainers,ParttimeFemaleEmpCount,FulltimeEmpCount,FulltimeFemaleEmpCount,PermanentFemaleEmpCount,CommercialRegister,Password,ConfirmPassword,Logo,Active,Deleted,WorkSectorOther,SectorOther")] Employers employers)
        {
            ViewData["CityId"] = _context.City.OrderBy(a => a.Id).ToList();
            ViewData["VillageId"] = _context.Villages.Where(a => a.Deleted == false && a.CityId == employers.CityId).OrderBy(a => a.Name).ToList();
            ViewData["EmpTypes"] = _context.Lookups.Where(a => a.Deleted == false && a.Type == "EmpType").OrderBy(a => a.Priority).ToList();
            //ViewData["EmpSectors"] = _context.Lookups.Where(a => a.Deleted == false && a.Type == "EmpSector").OrderBy(a => a.Priority).ToList();
            ViewData["WorkSectorId"] = new SelectList(_context.LookupWorkSectors, "Id", (currentCulture=="ar-AE" ? "ArName" : "Name"));
            ViewData["Sectors"] = _context.LookupEmployersSectors.Where(a => a.Deleted == false && a.Type == employers.Type).OrderBy(a => a.Id).ToList();

            if (string.IsNullOrEmpty(employers.Name)) employers.Name = employers.ArName;

            if (ModelState.IsValid)
            {
                if (employers.Email != null && EmployerExistByEmail(employers.Email, null))
                {
                    TempData["error"] = _localizer["Employer Email Exists"].Value;
                    return View(employers);
                }

                employers.Active = true;
                employers.Password = Encryption.Encrypt(employers.Password, true);
                employers.ConfirmPassword = Encryption.Encrypt(employers.ConfirmPassword, true);
                employers.AddDate = DateTime.Now;

                //Filter Submitted data against Vulnerable content
                employers.ArName = Functions.RemoveHtml(employers.ArName);
                employers.Name = Functions.RemoveHtml(employers.Name);
                employers.Address = Functions.RemoveHtml(employers.Address);
                employers.AnnualReport = Functions.RemoveHtml(employers.AnnualReport);
                employers.CommercialRegister = Functions.RemoveHtml(employers.CommercialRegister);
                employers.ContactEmail = Functions.RemoveHtml(employers.ContactEmail);
                employers.contactJobTitle = Functions.RemoveHtml(employers.contactJobTitle);
                employers.ContactMobile = Functions.RemoveHtml(employers.ContactMobile);
                employers.ContactMobile2 = Functions.RemoveHtml(employers.ContactMobile2);
                employers.ContactName = Functions.RemoveHtml(employers.ContactName);
                employers.Email = Functions.RemoveHtml(employers.Email);
                employers.FinancialReport = Functions.RemoveHtml(employers.FinancialReport);
                employers.OperationNumber = Functions.RemoveHtml(employers.OperationNumber);
                employers.RegistrationDocument = Functions.RemoveHtml(employers.RegistrationDocument);
                employers.RegNumber = Functions.RemoveHtml(employers.RegNumber);

                employers.Description = Functions.FilterHtml(employers.Description);
                employers.ArDescription = Functions.FilterHtml(employers.ArDescription);
                employers.ArExtraData = Functions.FilterHtml(employers.ArExtraData);
                employers.ExtraData = Functions.FilterHtml(employers.ExtraData);
                employers.SafetyProcedures = Functions.FilterHtml(employers.SafetyProcedures);
                employers.ArSafetyProcedures = Functions.FilterHtml(employers.ArSafetyProcedures);
                employers.BranchesLocation = Functions.FilterHtml(employers.BranchesLocation);

                if (HttpContext.Request.Form.Files.Count > 0)
                {
                    var filePath = ImagesUplaod.UploadFile(HttpContext, "files/file/EmployersDocuments/", _environment.WebRootPath, "FinancialReport");
                    employers.FinancialReport = filePath;
                }
                else employers.FinancialReport = null;

                if (HttpContext.Request.Form.Files.Count > 0)
                {
                    var filePath = ImagesUplaod.UploadFile(HttpContext, "files/file/EmployersDocuments/", _environment.WebRootPath, "AnnualReport");
                    employers.AnnualReport = filePath;
                }
                else employers.AnnualReport = null;

                if (HttpContext.Request.Form.Files.Count > 0)
                {
                    var filePath = ImagesUplaod.UploadFile(HttpContext, "files/file/EmployersDocuments/", _environment.WebRootPath, "RegistrationDocument");
                    employers.RegistrationDocument = filePath;
                    var commPath = ImagesUplaod.UploadFile(HttpContext, "files/file/EmployersDocuments/", _environment.WebRootPath, "CommercialRegister");
                    employers.CommercialRegister = commPath;
                }
                else employers.RegistrationDocument = null;

                if (HttpContext.Request.Form.Files.Count > 0)
                {
                    var ImageUrl = ImagesUplaod.UploadSingleImage(HttpContext, "files/image/EmployersImg/", _environment.WebRootPath, "Logo");

                    if (ImageUrl.Item1.ToString() != "") employers.Logo = ImageUrl.Item1;
                }
                else employers.Logo = null;

                employers.Password = Encryption.Encrypt(employers.Password, true);
                employers.ConfirmPassword = Encryption.Encrypt(employers.ConfirmPassword, true);

                _context.Add(employers);
                await _context.SaveChangesAsync();
                TempData["success"] = _localizer["Item Added"].Value;

                return RedirectToAction(nameof(Index));
            }
            //ViewData["CityId"] = new SelectList(_context.City, "Id", "Name", employers.CityId);
            //ViewData["WorkSectorId"] = new SelectList(_context.LookupWorkSectors, "Id", "Name", employers.WorkSectorId);
            return View(employers);
        }

        // GET: Control/Employers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Employers == null)
            {
                return NotFound();
            }

            var employers = await _context.Employers.FindAsync(id);
            if (employers == null)
            {
                return NotFound();
            }
            //Only the employer who own this job allowed to do action on it
            if (HttpContext.Session.GetString("type") == "Employer" && employers.Id != int.Parse(HttpContext.Session.GetString("id")))
            {
                return RedirectToAction("AccessDenied", "Users");
            }

            ViewData["CityId"] = new SelectList(_context.City, "Id", (currentCulture == "ar-AE" ? "ArName" : "Name"), employers.CityId);
            if (!string.IsNullOrEmpty(employers.CityId.ToString()))
            {
                ViewData["AreaId"] = _context.Villages.Where(a => a.CityId == employers.CityId).OrderBy(a => a.Name).ToList();
            }

            ViewData["Sectors"] = _context.LookupEmployersSectors.Where(a => a.Deleted == false && a.Type == employers.Type).OrderBy(a => a.Id).ToList();
            ViewData["WorkSectorId"] = new SelectList(_context.LookupWorkSectors, "Id", (currentCulture == "ar-AE" ? "ArName" : "Name"), employers.WorkSectorId);
            return View(employers);
        }

        // POST: Control/Employers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ArName,Shortcut,Type,Sector,WorkSectorId,EstablishYear,RegNumber,OperationNumber,CityId,AreaId,Village,Mobile,Tel,Email,Address,ContactName,contactJobTitle,ContactMobile,ContactMobile2,ContactEmail,Description,ArDescription,ExtraData,ArExtraData,SafetyProcedures,ArSafetyProcedures,PermanentEmpCount,ParttimeEmpCount,WeekWorkDays,DayWorkHours,FinancialReport,AnnualReport,RegistrationDocument,Branches,BranchesLocation,InjuredInsurance,HealthInsurance,AcceptTrainers,ParttimeFemaleEmpCount,FulltimeEmpCount,FulltimeFemaleEmpCount,PermanentFemaleEmpCount,CommercialRegister,Password,ConfirmPassword,Logo,Active,Deleted,WorkSectorOther,SectorOther")] Employers employers)
        {
            if (id != employers.Id)
            {
                return NotFound();
            }
            //Only the employer who own this job allowed to do action on it
            if (HttpContext.Session.GetString("type") == "Employer" && employers.Id != int.Parse(HttpContext.Session.GetString("id")))
            {
                return RedirectToAction("AccessDenied", "Users");
            }
            ViewData["AreaId"] = _context.Villages.Where(a => a.CityId == employers.CityId).OrderBy(a => a.Name).ToList();
            ViewData["Sectors"] = _context.LookupEmployersSectors.Where(a => a.Deleted == false && a.Type == employers.Type).OrderBy(a => a.Id).ToList();
            ViewData["CityId"] = new SelectList(_context.City, "Id", (currentCulture == "ar-AE" ? "ArName" : "Name"), employers.CityId);
            ViewData["WorkSectorId"] = new SelectList(_context.LookupWorkSectors, "Id", (currentCulture == "ar-AE" ? "ArName" : "Name"), employers.WorkSectorId);


            if (ModelState.IsValid)
            {
                if (employers.Email != null && EmployerExistByEmail(employers.Email, id))
                {
                    TempData["error"] = _localizer["Employer Email Exists"].Value;
                    return View(employers);

                    //return Redirect(Request.Headers["Referer"].ToString());
                }

                try
                {
                    _context.Employers.Attach(employers);
                    _context.Entry(employers).State = EntityState.Modified;
                    _context.Entry(employers).Property(a => a.AddDate).IsModified = false;
                    _context.Entry(employers).Property(a => a.LastLogin).IsModified = false;
                    _context.Entry(employers).Property(a => a.Active).IsModified = false;
                    _context.Entry(employers).Property(e => e.AnnualReport).IsModified = false;
                    _context.Entry(employers).Property(e => e.FinancialReport).IsModified = false;
                    _context.Entry(employers).Property(e => e.RegistrationDocument).IsModified = false;
                    _context.Entry(employers).Property(e => e.CommercialRegister).IsModified = false;
                    _context.Entry(employers).Property(e => e.Logo).IsModified = false;

                    //Filter Submitted data against Vulnerable content
                    employers.ArName = Functions.RemoveHtml(employers.ArName);
                    employers.Name = Functions.RemoveHtml(employers.Name);
                    employers.Address = Functions.RemoveHtml(employers.Address);
                    employers.AnnualReport = Functions.RemoveHtml(employers.AnnualReport);
                    employers.CommercialRegister = Functions.RemoveHtml(employers.CommercialRegister);
                    employers.ContactEmail = Functions.RemoveHtml(employers.ContactEmail);
                    employers.contactJobTitle = Functions.RemoveHtml(employers.contactJobTitle);
                    employers.ContactMobile = Functions.RemoveHtml(employers.ContactMobile);
                    employers.ContactMobile2 = Functions.RemoveHtml(employers.ContactMobile2);
                    employers.ContactName = Functions.RemoveHtml(employers.ContactName);
                    employers.Email = Functions.RemoveHtml(employers.Email);
                    employers.FinancialReport = Functions.RemoveHtml(employers.FinancialReport);
                    employers.OperationNumber = Functions.RemoveHtml(employers.OperationNumber);
                    employers.RegistrationDocument = Functions.RemoveHtml(employers.RegistrationDocument);
                    employers.RegNumber = Functions.RemoveHtml(employers.RegNumber);

                    employers.Description = Functions.FilterHtml(employers.Description);
                    employers.ArDescription = Functions.FilterHtml(employers.ArDescription);
                    employers.ArExtraData = Functions.FilterHtml(employers.ArExtraData);
                    employers.ExtraData = Functions.FilterHtml(employers.ExtraData);
                    employers.SafetyProcedures = Functions.FilterHtml(employers.SafetyProcedures);
                    employers.ArSafetyProcedures = Functions.FilterHtml(employers.ArSafetyProcedures);
                    employers.BranchesLocation = Functions.FilterHtml(employers.BranchesLocation);

                    if (employers.Password != "" && employers.Password != "******")
                    {
                        employers.Password = Encryption.Encrypt(employers.Password, true);
                        employers.Password = Encryption.Encrypt(employers.ConfirmPassword, true);
                    }
                    else
                    {
                        _context.Entry(employers).Property(a => a.Password).IsModified = false;
                        _context.Entry(employers).Property(a => a.ConfirmPassword).IsModified = false;
                    }

                    if (HttpContext.Request.Form.Files.Count > 0)
                    {
                        var filePath = ImagesUplaod.UploadFile(HttpContext, "files/file/EmployersDocuments/", _environment.WebRootPath, "FinancialReport");
                        if (filePath != "")
                        {
                            _context.Entry(employers).Property(a => a.FinancialReport).IsModified = false;
                            employers.FinancialReport = filePath;
                        }
                    }
                    

                    if (HttpContext.Request.Form.Files.Count > 0)
                    {
                        var filePath = ImagesUplaod.UploadFile(HttpContext, "files/file/EmployersDocuments/", _environment.WebRootPath, "AnnualReport");
                        if (filePath != "")
                        {
                            _context.Entry(employers).Property(a => a.AnnualReport).IsModified = false;
                            employers.AnnualReport = filePath;
                        }
                    }
                    

                    if (HttpContext.Request.Form.Files.Count > 0)
                    {
                        var filePath = ImagesUplaod.UploadFile(HttpContext, "files/file/EmployersDocuments/", _environment.WebRootPath, "RegistrationDocument");
                        if (filePath != "")
                        {
                            _context.Entry(employers).Property(a => a.RegistrationDocument).IsModified = true;
                            employers.RegistrationDocument = filePath;
                        }
                        var commPath = ImagesUplaod.UploadFile(HttpContext, "files/file/EmployersDocuments/", _environment.WebRootPath, "CommercialRegister");
                        if (commPath != "")
                        {
                            _context.Entry(employers).Property(a => a.CommercialRegister).IsModified = true;
                            employers.CommercialRegister = commPath;
                        }
                    }
                    

                    if (HttpContext.Request.Form.Files.Count > 0)
                    {
                        var ImageUrl = ImagesUplaod.UploadSingleImage(HttpContext, "files/image/EmployersImg/", _environment.WebRootPath, "Logo");

                        if (ImageUrl.Item1.ToString() != "") employers.Logo = ImageUrl.Item1;
                    }
                    else _context.Entry(employers).Property(a => a.Logo).IsModified = false;

                    TempData["success"] = _localizer["Item Updated"].Value;
                    //_context.Update(employers);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployersExists(employers.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //To Redirect to Employer Profile
                if (HttpContext.Session.GetString("type") == "Employer")
                {
                    return RedirectToAction("Details", "Employers", new {id= employers.Id});
                }

                return RedirectToAction(nameof(Index));
            }
            
            return View(employers);
        }

        // GET: Control/Employers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Employers == null)
            {
                return NotFound();
            }

            var employers = await _context.Employers
                .Include(e => e.City)
                .Include(e => e.WorkSector)
                .Include(e => e.EmployerSector)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employers == null)
            {
                return NotFound();
            }

            return View(employers);
        }

        // POST: Control/Employers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Employers == null)
            {
                return Problem("Entity set 'DataContext.Employers'  is null.");
            }

            var jobs = _context.Jobs.Where(a => a.Deleted == false && a.EmployerId == id).Count();
            if (jobs > 0)
            {
                TempData["error"] = _localizer["Employer Cannot Delete"].Value;
                return RedirectToAction(nameof(Index));
            }

            var employers = await _context.Employers.FindAsync(id);
            if (employers != null)
            {
                TempData["success"] = _localizer["Item Deleted"].Value;
                employers.Deleted = true;
                _context.Update(employers);
                //_context.Employers.Remove(employers);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Activate(int? id)
        {
            if (id == null || _context.Employers == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
            }
            var employers = await _context.Employers
                .Include(m => m.City)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employers == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
            }

            employers.Active = true;
            _context.Update(employers);
            await _context.SaveChangesAsync();
            TempData["success"] = _localizer["Item Actived"].Value;
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Deactivate(int? id)
        {
            if (id == null || _context.Employers == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
            }
            var employers = await _context.Employers
                .Include(m => m.City)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employers == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
            }

            employers.Active = false;
            _context.Update(employers);
            await _context.SaveChangesAsync();
            TempData["success"] = _localizer["Item Deactived"].Value;
            return RedirectToAction("Index");
        }

        private bool EmployersExists(int id)
        {
          return (_context.Employers?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private bool EmployerExistByEmail(string email, int? id)
        {
            //Make sure that there is no user in the system use the same email address
            Boolean employer_exist = (_context.Employers?.Any(e => e.Email == email && e.Deleted == false && (id == null || e.Id != id))).GetValueOrDefault();
            Boolean donor_exist = (_context.Donors?.Any(e => e.Email == email && e.Deleted == false)).GetValueOrDefault();
            Boolean member_exist = (_context.Members?.Any(e => e.Email == email && e.Deleted == false)).GetValueOrDefault();

            return (member_exist || employer_exist || donor_exist);
            //return (_context.Employers?.Any(e => e.Email == email
            //&& (id == null || e.Id != id)))
            //.GetValueOrDefault();
        }
    }
}
