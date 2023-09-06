using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Model.Tree;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using PEF.Helpers;
using PEF.Models;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace PEF.Controllers
{
    //[PEFAuth.AuthorizedMembers]
    public class JobsController : Controller
    {
        private DataContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        public JobsController(DataContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult LoadJobs(string? keyword, int? city, int[]? JFields, IEnumerable<string>? ExpLevel, int? SortOption, string? Type, int? ResCountOption = 1, int PageNumber = 1, int? WorkNature=-1)
        {
            ViewBag.Cities = _context.City.OrderBy(a => a.ArName).ToList();
            ViewBag.JobFields = _context.LookupJobsFields.Where(a => a.Deleted == false).OrderBy(a => a.ArName).ToList();

            int PageSize = 10;
            if (ResCountOption != null)
            {
                if (ResCountOption == 1) PageSize = 10;
                else if (ResCountOption == 2) PageSize = 20;
                else if (ResCountOption == 3) PageSize = 30;
            }

            /*
            var query = _context.Jobs.AsQueryable();
            var countQuery = _context.Jobs.AsQueryable();

            query = query.Where(a => a.Deleted == false && a.Approved == true
            && (keyword == null || a.ArName.Contains(keyword) || a.Name.Contains(keyword) || a.Employer.Name.Contains(keyword) || a.Employer.ArName.Contains(keyword) || a.Project.Name.Contains(keyword) || a.Project.ArName.Contains(keyword))
            && (city == null || a.CityId == city)
            && (a.StartDate == null || a.StartDate <= DateTime.Now)
            && (a.ExpireDate == null || a.ExpireDate >= DateTime.Now)
            && (WorkNature == null || WorkNature == -1 || a.WorkNature == WorkNature)
            && ((Type == null || Type == "NewOpp") || (Type == "Featured" && a.Featured == true) || (Type == "Jobs" && a.Type == 1) || (Type == "PaidTraining" && a.Type == 2) || (Type == "NonPaidTraining" && a.Type == 3))
            );

            countQuery = countQuery.Where(a => a.Deleted == false && a.Approved == true
            && (keyword == null || a.ArName.Contains(keyword) || a.Name.Contains(keyword))
            && (city == null || a.CityId == city)
            && (a.StartDate == null || a.StartDate < DateTime.Now)
            && (a.ExpireDate == null || a.ExpireDate > DateTime.Now)
            && (WorkNature == null || WorkNature == -1 || a.WorkNature == WorkNature)
            && ((Type == null || Type == "NewOpp") || (Type == "Featured" && a.Featured == true) || (Type == "Jobs" && a.Type == 1) || (Type == "PaidTraining" && a.Type == 2) || (Type == "NonPaidTraining" && a.Type == 3))
            );


            //If logged in, return only jobs with creteria applied to job seeker
            if (!String.IsNullOrEmpty(HttpContext.Session.GetString("mem_id")))
            {
                //Make sure that user logged in
                var MemberId = int.Parse(HttpContext.Session.GetString("mem_id"));
                Members member = _context.Members.Where(a => a.Id == MemberId).Include(a => a.City).FirstOrDefault();

                int age = CalculateAge(member.BirthDate);

                query = query.Where(a => a.Deleted == false
                && a.JobsApplications.Where(p => p.MemberId == MemberId).Count() == 0  //Show only jobs that not applied    
                && (a.SelFromAge == null || a.SelFromAge <= age)
                && (a.SelToAge == null || a.SelToAge >= age)
                && (a.SelSocialId == null || a.SelSocialId == member.SocialId)
                && (a.SelGender == null || a.SelGender == member.Gender)
                && (a.SelDistrict == null || a.SelDistrict == "Both" || a.SelDistrict == member.City.District)
                && (a.SelCity == null || a.SelCityId == member.CityId)
                && ((Type == null || Type == "NewOpp") || (Type == "Featured" && a.Featured == true) || (Type == "Jobs" && a.Type == 1) || (Type == "PaidTraining" && a.Type == 2) || (Type == "NonPaidTraining" && a.Type == 3))
                );

                countQuery = countQuery.Where(a => a.Deleted == false
                && a.JobsApplications.Where(p => p.MemberId == MemberId).Count() == 0  //Show only jobs that not applied    
                && (a.SelFromAge == null || a.SelFromAge <= age)
                && (a.SelToAge == null || a.SelToAge >= age)
                && (a.SelSocialId == null || a.SelSocialId == member.SocialId)
                && (a.SelGender == null || a.SelGender == member.Gender)
                && (a.SelDistrict == null || a.SelDistrict == "Both" || a.SelDistrict == member.City.District)
                && (a.SelCity == null || a.SelCityId == member.CityId)
                && ((Type == null || Type == "NewOpp") || (Type == "Featured" && a.Featured == true) || (Type == "Jobs" && a.Type == 1) || (Type == "PaidTraining" && a.Type == 2) || (Type == "NonPaidTraining" && a.Type == 3))
                );
            }

            query = query.Include(a => a.City)
            .Include(a => a.Employer).ThenInclude(a => a.EmployerSector)
            .Include(a => a.Currency);

            //countQuery = countQuery.Include(a => a.City)
            //.Include(a => a.Employer)
            //.Include(a => a.Currency);



            if ((Type != null && Type == "NewOpp") || SortOption == 2) query = query.OrderByDescending(a => a.AddDate).ThenByDescending(a => a.Id); //Sort by add date descending
            else if (SortOption == 1) query = query.OrderBy(a => a.AddDate).ThenByDescending(a => a.Id); //Sort by add date ascending
            else query = query.OrderByDescending(a => a.Id);

            query = query.Skip((PageNumber - 1) * PageSize)
            .Take(PageSize);


            int dbPages = countQuery.Count();
            float paging = (float)dbPages / PageSize;
            double TotalPages = Math.Round(paging);
            */

            //var jobsList = query.Select(a => new
            //{
            //    Id = a.Id,
            //    Name = a.Name,
            //    ArName = a.ArName,
            //    StartDate = a.StartDate,
            //    ExpireDate = a.ExpireDate,
            //    EmpArName = a.Employer.ArName,
            //    EmpName = a.Employer.Name,
            //    EmpSectorName = a.Employer.EmployerSector.Name,
            //    EmpSectorArName = a.Employer.EmployerSector.ArName,
            //    Logo = a.Employer.Logo,
            //    Salary = a.Salary,
            //    CityName = a.City.Name,
            //    CityArName = a.City.ArName,
            //    WorkNature = a.WorkNature,
            //    CurIconHtml = a.Currency.IconHtml,
            //    BeneficiaryCount = a.BeneficiaryCount
            //}).ToList();



            var query = _context.Jobs.AsQueryable();
            var countQuery = _context.Jobs.AsQueryable();

            query = query.Where(a => a.Deleted == false && a.Approved == true
                && (keyword == null || a.ArName.Contains(keyword) || a.Name.Contains(keyword))
                && (city == null || a.CityId == city)
                && (WorkNature == null || WorkNature == -1 || a.WorkNature == WorkNature)
                && (a.StartDate == null || a.StartDate <= DateTime.Now)
                && (a.ExpireDate == null || a.ExpireDate >= DateTime.Now.AddDays(-1))
                && ((Type == null || Type == "NewOpp") || (Type == "Featured" && a.Featured == true) || (Type == "Jobs" && a.Type == 1) || (Type == "PaidTraining" && a.Type == 2) || (Type == "NonPaidTraining" && a.Type == 3))
            );

            countQuery = countQuery.Where(a => a.Deleted == false && a.Approved == true
                && (keyword == null || a.ArName.Contains(keyword) || a.Name.Contains(keyword))
                && (city == null || a.CityId == city)
                && (WorkNature == null || WorkNature == -1 || a.WorkNature == WorkNature)
                && (a.StartDate == null || a.StartDate <= DateTime.Now)
                && (a.ExpireDate == null || a.ExpireDate >= DateTime.Now.AddDays(-1))
                && ((Type == null || Type == "NewOpp") || (Type == "Featured" && a.Featured == true) || (Type == "Jobs" && a.Type == 1) || (Type == "PaidTraining" && a.Type == 2) || (Type == "NonPaidTraining" && a.Type == 3))
            );

            //If logged in, return only jobs with creteria applied to job seeker
            if (!String.IsNullOrEmpty(HttpContext.Session.GetString("mem_id")))
            {
                //Make sure that user logged in
                var MemberId = int.Parse(HttpContext.Session.GetString("mem_id"));
                Members member = _context.Members.Where(a => a.Id == MemberId).Include(a => a.City).FirstOrDefault();

                int age = CalculateAge(member.BirthDate);

                query = query.Where(a => a.Deleted == false
                && a.JobsApplications.Where(p => p.MemberId == MemberId).Count() == 0  //Show only jobs that not applied    
                && (a.SelFromAge == null || a.SelFromAge <= age)
                && (a.SelToAge == null || a.SelToAge >= age)
                && (a.SelSocialId == null || a.SelSocialId == member.SocialId)
                && (a.SelGender == null || a.SelGender == member.Gender)
                && (a.SelDistrict == null || a.SelDistrict == "Both" || a.SelDistrict == member.City.District)
                && (a.SelCity == null || a.SelCityId == member.CityId)
                && ((Type == null || Type == "NewOpp") || (Type == "Featured" && a.Featured == true) || (Type == "Jobs" && a.Type == 1) || (Type == "PaidTraining" && a.Type == 2) || (Type == "NonPaidTraining" && a.Type == 3))
                );

                countQuery = countQuery.Where(a => a.Deleted == false
                && a.JobsApplications.Where(p => p.MemberId == MemberId).Count() == 0  //Show only jobs that not applied    
                && (a.SelFromAge == null || a.SelFromAge <= age)
                && (a.SelToAge == null || a.SelToAge >= age)
                && (a.SelSocialId == null || a.SelSocialId == member.SocialId)
                && (a.SelGender == null || a.SelGender == member.Gender)
                && (a.SelDistrict == null || a.SelDistrict == "Both" || a.SelDistrict == member.City.District)
                && (a.SelCity == null || a.SelCityId == member.CityId)
                && ((Type == null || Type == "NewOpp") || (Type == "Featured" && a.Featured == true) || (Type == "Jobs" && a.Type == 1) || (Type == "PaidTraining" && a.Type == 2) || (Type == "NonPaidTraining" && a.Type == 3))
                );
            }

            query = query.Include(a => a.Fields)
            .Include(a => a.City)
            .Include(a => a.Employer).ThenInclude(a => a.EmployerSector)
            .Include(a => a.Currency);

            if (!JFields.IsNullOrEmpty())
            {
                query = query.Where(a => a.Fields != null && a.Fields.Where(f => JFields != null && JFields.Contains(f.FieldId)).Any());
                countQuery = countQuery.Where(a => a.Fields != null && a.Fields.Where(f => JFields != null && JFields.Contains(f.FieldId)).Any());
            }

            if ((Type != null && Type == "NewOpp") || SortOption == 2) query = query.OrderByDescending(a => a.AddDate).ThenByDescending(a => a.Id); //Sort by add date descending
            else if (SortOption == 1) query = query.OrderBy(a => a.AddDate).ThenBy(a => a.Id); //Sort by add date ascending            
            else query = query.OrderByDescending(a => a.Id);

            query = query.Skip((PageNumber - 1) * PageSize)
            .Take(PageSize);

            var jobsList = query.Select(a => new
            {
                Id = a.Id,
                Name = a.Name,
                ArName = a.ArName,
                StartDate = a.StartDate,
                ExpireDate = a.ExpireDate,
                EmpArName = a.Employer.ArName,
                EmpName = a.Employer.Name,
                EmpSectorName = a.Employer.EmployerSector.Name,
                EmpSectorArName = a.Employer.EmployerSector.ArName,
                Logo = a.Employer.Logo,
                Salary = a.Salary,
                CityName = a.City.Name,
                CityArName = a.City.ArName,
                WorkNature = a.WorkNature,
                CurIconHtml = a.Currency.IconHtml,
                BeneficiaryCount = a.BeneficiaryCount
            }).ToList();

            int dbPages = countQuery.Count();
            float paging = (float)dbPages / PageSize;
            double TotalPages = Math.Round(paging);

            string paginate = "";
            
            if(TotalPages > 1)
            {
                paginate += "<div class='text-center d-flex justify-content-center'>";
                paginate += "<nav aria-label='Page navigation example'>";
                paginate += "<ul class='pagination'>";

                string FirstAction = Url.Action("Index", "Jobs", new { keyword = keyword, city = city, JFields = JFields, ExpLevel = ExpLevel, SortOption = SortOption, Type = Type, ResCountOption = ResCountOption, WorkNature = WorkNature, PageNumber = PageNumber });
                string LastAction = Url.Action("Index", "Jobs", new { keyword = keyword, city = city, JFields = JFields, ExpLevel = ExpLevel, SortOption = SortOption, Type = Type, ResCountOption = ResCountOption, WorkNature = WorkNature, PageNumber = PageNumber });

                paginate += "<li class='page-item'><a class='page-link' href='" + FirstAction + "'>الأولى</a></li>";
                for(int i = 1; i <= TotalPages; i++)
                {
                    string CurrentAction = Url.Action("Index", "Jobs", new { keyword = keyword, city = city, JFields = JFields, ExpLevel = ExpLevel, SortOption = SortOption, Type = Type, ResCountOption = ResCountOption, WorkNature = WorkNature, PageNumber = i });
                    string activePage = "";
                    if (PageNumber == i) activePage = " active";
                    paginate += "<li class='page-item "+activePage+ "'><a class='page-link' href='" + CurrentAction  + "'> " + i + "</a></li>";
                }
                paginate += "<li class='page-item'><a class='page-link' href='" + LastAction + "'>الأخيرة</a></li>";
                paginate += "</ul>";
                paginate += "</nav>";
                paginate += "</div>";
            }

            return Json(new
            {
                result = true,
                jobs = jobsList,
                paginate = paginate,
                pagesCount = TotalPages,
                pageNumber = PageNumber
            }) ;
        }

        public IActionResult Index(string? keyword,int? city, string[]? JFields,int? SortOption, string? Type, int? ResCountOption = 1,int PageNumber = 1)
        {
            
            ViewBag.Cities = _context.City.OrderBy(a=> a.ArName).ToList();
            ViewBag.JobFields = _context.LookupJobsFields.Where(a => a.Deleted == false).OrderBy(a => a.ArName).ToList();

            int PageSize = 10;
            if (ResCountOption != null)
            {
                if (ResCountOption == 1) PageSize = 10;
                else if (ResCountOption == 2) PageSize = 20;
                else if (ResCountOption == 3) PageSize = 30;
            }
            

            var query = _context.Jobs.AsQueryable();
            var countQuery = _context.Jobs.AsQueryable();

            query = query.Where(a => a.Deleted == false && a.Approved == true
            && (keyword == null || a.ArName.Contains(keyword) || a.Name.Contains(keyword) || a.Employer.Name.Contains(keyword) || a.Employer.ArName.Contains(keyword) || a.Project.Name.Contains(keyword) || a.Project.ArName.Contains(keyword))
            && (city == null || a.CityId == city)
            && (a.StartDate == null || a.StartDate <= DateTime.Now)
            && (a.ExpireDate == null || a.ExpireDate >= DateTime.Now.AddDays(-1))
            && ((Type == null || Type == "NewOpp") || (Type =="Featured" && a.Featured == true) || (Type =="Jobs" && a.Type == 1) || (Type == "PaidTraining" && a.Type ==2 ) || (Type == "NonPaidTraining" && a.Type == 3))
            );

            countQuery = countQuery.Where(a => a.Deleted == false && a.Approved == true
            && (keyword == null || a.ArName.Contains(keyword) || a.Name.Contains(keyword))
            && (city == null || a.CityId == city)
            && (a.StartDate == null || a.StartDate < DateTime.Now)
            && (a.ExpireDate == null || a.ExpireDate > DateTime.Now.AddDays(-1))
            && ((Type == null || Type == "NewOpp") || (Type == "Featured" && a.Featured == true) || (Type == "Jobs" && a.Type == 1) || (Type == "PaidTraining" && a.Type == 2) || (Type == "NonPaidTraining" && a.Type == 3))
            );
            

            //If logged in, return only jobs with creteria applied to job seeker
            if (!String.IsNullOrEmpty(HttpContext.Session.GetString("mem_id")))
            {
                //Make sure that user logged in
                var MemberId = int.Parse(HttpContext.Session.GetString("mem_id"));
                Members member = _context.Members.Where(a => a.Id == MemberId).Include(a => a.City).FirstOrDefault();

                int age = CalculateAge(member.BirthDate);

                query = query.Where(a => a.Deleted == false
                && a.JobsApplications.Where(p => p.MemberId == MemberId).Count() == 0  //Show only jobs that not applied    
                && (a.SelFromAge ==null || a.SelFromAge <= age)
                && (a.SelToAge ==null || a.SelToAge >= age)
                && (a.SelSocialId ==null || a.SelSocialId == member.SocialId)
                && (a.SelGender ==null || a.SelGender == member.Gender)
                && (a.SelDistrict ==null || a.SelDistrict=="Both" || a.SelDistrict == member.City.District)
                && (a.SelCity==null || a.SelCityId == member.CityId)
                && ((Type == null || Type == "NewOpp") || (Type == "Featured" && a.Featured == true) || (Type == "Jobs" && a.Type == 1) || (Type == "PaidTraining" && a.Type == 2) || (Type == "NonPaidTraining" && a.Type == 3))
                );

                countQuery = countQuery.Where(a => a.Deleted == false
                && a.JobsApplications.Where(p => p.MemberId == MemberId).Count() == 0  //Show only jobs that not applied    
                && (a.SelFromAge == null || a.SelFromAge <= age)
                && (a.SelToAge == null || a.SelToAge >= age)
                && (a.SelSocialId == null || a.SelSocialId == member.SocialId)
                && (a.SelGender == null || a.SelGender == member.Gender)
                && (a.SelDistrict == null || a.SelDistrict == "Both" || a.SelDistrict == member.City.District)
                && (a.SelCity == null || a.SelCityId == member.CityId)
                && ((Type == null || Type == "NewOpp") || (Type == "Featured" && a.Featured == true) || (Type == "Jobs" && a.Type == 1) || (Type == "PaidTraining" && a.Type == 2) || (Type == "NonPaidTraining" && a.Type == 3))
                );
            }

            query = query.Include(a => a.City)
            .Include(a => a.Employer).ThenInclude(a=> a.EmployerSector)
            .Include(a => a.Currency);

            //countQuery = countQuery.Include(a => a.City)
            //.Include(a => a.Employer)
            //.Include(a => a.Currency);



            if ((Type != null && Type == "NewOpp") || SortOption == 2) query = query.OrderByDescending(a => a.AddDate).ThenByDescending(a => a.Id); //Sort by add date descending
            else if (SortOption == 1) query = query.OrderBy(a => a.AddDate).ThenByDescending(a => a.Id); //Sort by add date ascending
            else query = query.OrderByDescending(a => a.Id);

            query = query.Skip((PageNumber - 1) * PageSize)
            .Take(PageSize);
            

            int dbPages = countQuery.Count();
            float paging = (float) dbPages / PageSize;
            double TotalPages = Math.Round(paging);

            var jobsList = query.ToList();
            ViewBag.Jobs = jobsList;
            if(jobsList.Count() == 0)
            {
                ViewBag.Msg = "لم يتم العثور على نتائج!";
            }
            ViewBag.Keyword = keyword;
            ViewBag.City = city;
            ViewBag.Field = JFields;
            ViewBag.Type = Type;
            ViewBag.SortOption = SortOption;
            ViewBag.ResCountOption = ResCountOption;
            ViewBag.PagesCount = TotalPages;
            ViewBag.PageNumber = PageNumber;

            return View();
        }

        public IActionResult Applied(int PageNumber = 1)
        {
            //If logged in, return only jobs with creteria applied to job seeker
            if (!String.IsNullOrEmpty(HttpContext.Session.GetString("mem_id")))
            {
                //Make sure that user logged in
                var MemberId = int.Parse(HttpContext.Session.GetString("mem_id"));
                Members member = _context.Members.Where(a => a.Id == MemberId).Include(a => a.City).FirstOrDefault();

                ViewBag.Cities = _context.City.OrderBy(a => a.ArName).ToList();
                ViewBag.JobFields = _context.LookupJobsFields.Where(a => a.Deleted == false).OrderBy(a => a.ArName).ToList();

                int PageSize = 10;
                //var query = _context.Jobs.AsQueryable();
                //query = query.Where(a => a.Deleted == false && a.Approved == true && a.JobsApplications.Where(p => p.MemberId == MemberId).Count() > 0);
                var jobsList = _context.JobsApplications.Where(a => a.MemberId == MemberId && a.Job.Deleted == false && a.Job.Approved == true)
                    .Include(a => a.Member)
                    .Include(a => a.Job)
                    .Include(a => a.Job.City)
                    .Include(a => a.Job.Employer).ThenInclude(a=> a.EmployerSector)
                    .Include(a => a.Job.Currency)
                    .OrderByDescending(a => a.ApplyDate)
                    .ThenBy(a => a.JobId)
                    .Skip((PageNumber - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
                    

                int dbPages = _context.JobsApplications.Where(a => a.MemberId == MemberId && a.Job.Deleted == false && a.Job.Approved == true).Count();

                float paging = (float)dbPages / PageSize;
                double TotalPages = Math.Round(paging);

                ViewBag.Jobs = jobsList;
                if (jobsList.Count() == 0)
                {
                    ViewBag.Msg = "لم يتم العثور على نتائج!";
                }

                String route = "<a href='" + Url.Action("Index", "Home") + "' class='rtText' >الرئيسية &raquo;</a>";
                route += " <a href='" + Url.Action("Index", "Jobs") + "' class='rtText' >قائمة فرص التشغيل &raquo;</a>";
                ViewBag.Route = route;

                ViewBag.PagesCount = TotalPages;
                ViewBag.DBPages = dbPages;
                return View();
            }
            else
            {
                TempData["error"] = "عليك تسجيل الدخول أولا...";
                return RedirectToAction("Login","Home");
            }
        }

        public IActionResult Apply(int id)
        {
            if (!String.IsNullOrEmpty(HttpContext.Session.GetString("mem_id")))
            {
                var MemberId = int.Parse(HttpContext.Session.GetString("mem_id"));
                Members member = _context.Members.Where(a => a.Id == MemberId).Include(a => a.City).Include(a=> a.MembersEducation).FirstOrDefault();

                if (id == null)
                {
                    TempData["error"] = "لم يتم العثور على تفاصيل!";
                    return RedirectToAction("Index");
                }

                Jobs? job = _context.Jobs
                .Include(a => a.Project)
                .Include(a => a.Project.Form)
                .Include(a => a.Employer)
                .Include(a => a.SocialStatus)
                .Include(a => a.SelCity)
                .FirstOrDefault(a => a.Id == id && a.Deleted == false);
                if (job == null)
                {
                    TempData["error"] = "لم يتم العثور على تفاصيل!";
                    return RedirectToAction("Index");
                }

                ViewBag.ApplyActive = true; //To Activate the Apply Button
                ViewBag.JobApplied = false;

                Boolean ApplyActive = true;
                Boolean JobApplied = false;

                //Check the creteria matching
                //Check if Job Already Applied
                var appsCount = _context.JobsApplications.Where(a => a.JobId == id && a.MemberId == int.Parse(HttpContext.Session.GetString("mem_id")) && a.Deleted == false).Count();
                if (appsCount > 0)
                {
                    JobApplied = true; //Member already applied for this job
                    ApplyActive = false; //Not allowed to apply for this job again
                }

                //Disable Apply to job if member doen't have education history
                if (member.MembersEducation?.Where(a => a.Deleted == false).Count() == 0)
                {
                    //Not Have Education
                    TempData["error"] = "لا يمكن التقدم الى فرصة التشغيل / التدريب لانه لا يوجد لديك مؤهلات علمية!";
                    return RedirectToAction("Details", "Jobs", new { id = id });
                }

                //Age Validation
                int age = CalculateAge(member.BirthDate);
                if (job.SelFromAge != null && age < job.SelFromAge) ApplyActive = false;
                if (job.SelToAge != null && age > job.SelToAge) ApplyActive = false;

                //Social Status Validation
                if (job.SocialStatus != null && job.SelSocialId != member.SocialId) ApplyActive = false;

                //Gender Validation
                if (job.SelGender != null && job.SelGender != 0 && (job.SelGender - 1) == member.Gender) ApplyActive = false;

                //City Validation
                if (job.SelCity != null && (job.SelCityId != member.CityId || member.City == null)) ApplyActive = false;
                //District Validation
                if (!String.IsNullOrEmpty(job.SelDistrict) && job.SelDistrict != "Both" && (member.City.District != job.SelDistrict)) ApplyActive = false;


                ViewBag.JobApplied = JobApplied;
                ViewBag.ApplyActive = ApplyActive;


                if (job.Project != null && job.Project.Form != null)
                {
                    ViewBag.ShowBtn = false;
                    //There is a form must fill to apply for job
                    Forms AppForm = _context.Forms.Where(a => a.Deleted == false && a.Id == job.Project.FormId).FirstOrDefault();
                    if (AppForm != null)
                    {
                        List<FormsFields> Fields = _context.FormsFields.Where(a => a.Deleted == false && a.FormId == AppForm.Id)                        
                        .Include(a=> a.Options)
                        .OrderBy(a => a.Priority)
                        .ThenBy(a => a.Id)
                        .ToList();

                        ViewBag.JobFields = _context.JobsFields.Where(a => a.JobId == id).Include(a => a.Field).ToList();
                        ViewBag.JobId = id;
                        ViewBag.FormId = AppForm.Id;
                        ViewBag.AppForm = AppForm;
                        ViewBag.Fields = Fields;
                        ViewData["title"] = "التقدم الى وظيفة (" + job.ArName + ")";
                        return View("Apply_Form",job);
                    }
                    else
                    {
                        TempData["error"] = "Something error in form!";
                        return RedirectToAction("Index");                            
                    }
                }
                else
                {                    
                    if(appsCount == 0)
                    {
                        //Add the application entry to Seekers Applications table
                        JobsApplications jobApp = new JobsApplications { JobId = id, MemberId = MemberId, ApplyDate = DateTime.Now, Status = 0 };
                        //Insert the object and save changes
                        _context.JobsApplications.Add(jobApp);
                        _context.SaveChanges();


                        TempData["success"] = "تهانينا! تم التقدم الى الوظيفة بنجاح...";
                        //ViewBag.Msg = "تهانينا! تم التقدم الى الوظيفة بنجاح...";
                        Console.Write("Job Added");
                        return RedirectToAction("Details", "Jobs", new { id = id });
                    }
                    else
                    {
                        //Already Applied
                        TempData["error"] = "لقد قمت بالتقدم الى هذه الوظيفة مسبقا!";                        
                        return RedirectToAction("Details", "Jobs", new { id = id });
                    }
                    
                }
            }
            else
            {
                //No session found
                TempData["error"] = "يجب عليك تسجيل الدخول أولا...";
                return RedirectToAction("Login","Home");
            }   
            
        }

        [HttpPost]
        public async Task<IActionResult> SaveJobApp(int JobId, int FormId)
        {
            if (!String.IsNullOrEmpty(HttpContext.Session.GetString("mem_id")))
            {
                int MemberId = int.Parse(HttpContext.Session.GetString("mem_id"));
                if(_context.JobsApplications.Where(a=> a.MemberId == MemberId && a.JobId == JobId).Any())
                {
                    TempData["error"] = "لقد قدمت بالتقدم الى هذه الوظيفة مسبقا...";
                    return RedirectToAction("Details", new { id = JobId });
                }
                else
                {
                    //Collect submitted data and apply for the job
                    List<FormsFields> Fields = _context.FormsFields.Where(a => a.Deleted == false && a.FormId == FormId)
                        .Include(a => a.Options)
                        .OrderBy(a => a.Priority)
                        .ThenBy(a => a.Id)
                        .ToList();
                                        
                    //Add Form submission record to formEntries table
                    FormsEntries Entry = new FormsEntries
                    {
                        MemberId = MemberId,
                        FormId = FormId,
                        JobId = JobId,
                        Type = "Job",
                        IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                        AddedTime = DateTime.Now
                    };
                    

                    Entry.Entries = new List<FormsEntriesFields>();
                    //Loop for each field to add FormsEntriesFields to DB
                    foreach (FormsFields field in Fields)
                    {
                        //
                        if(field.Type !="header" && field.Type != "button" && field.Type!="hidden" && field.Type!="paragraph" && field.Type!="")
                        {
                            string Answer = Request.Form[field.Title];
                            if (Answer == null) Answer = "";

                            if (field.Type == "file")
                            {
                                if (HttpContext.Request.Form.Files.Count > 0)
                                {
                                    string FilePath = ImagesUplaod.UploadFile(HttpContext, "files/file/MembersFiles/", _environment.WebRootPath, field.Title);
                                    if (FilePath != "") Answer = FilePath;
                                }
                            }
                            else if (field.Type == "checkbox-group")
                            {
                                Answer = "";
                                List<FormsFieldsOptions> options = _context.FormsFieldsOptions.Where(a => a.FieldId == field.Id && a.Deleted == false).ToList();
                                foreach (FormsFieldsOptions opt in options)
                                {
                                    string checkboxValue = Request.Form["opt-" + opt.Id];
                                    bool isChecked = !string.IsNullOrEmpty(checkboxValue) && checkboxValue.ToLower() == "on";
                                    if (isChecked)
                                    {
                                        if (!string.IsNullOrEmpty(Answer)) Answer += "," + opt.ArValue;
                                        else Answer = opt.ArValue;
                                    }
                                }
                                string otherValue = Request.Form["opt-other-" + field.Id];
                                if (!string.IsNullOrEmpty(otherValue) && otherValue.ToLower() == "on")
                                {
                                    //Other checkbox is checked
                                    string otherText = Request.Form["opt-otherTxt-" + field.Id];
                                    if (!string.IsNullOrEmpty(otherText))
                                    {
                                        if (!string.IsNullOrEmpty(Answer)) Answer += "," + otherText;
                                        else Answer = otherText;
                                    }
                                }
                            }

                            FormsEntriesFields EntryField = new FormsEntriesFields
                            {
                                EntryId = Entry.Id,
                                FieldId = field.Id,
                                Title = field.Title,
                                Label = field.ArLabel,
                                Type = field.Type,
                                Answer = Answer
                            };

                            //Add the item to List
                            Entry.Entries.Add(EntryField);
                        }
                    }
                    //Add the entry to database
                    _context.FormsEntries.Add(Entry);

                    //Add the application record
                    _context.JobsApplications.Add(new JobsApplications
                    {
                        JobId = JobId,
                        MemberId = MemberId,
                        ApplyDate = DateTime.Now,
                        Status = 0
                    });

                    await _context.SaveChangesAsync();

                    TempData["success"] = "تهانينا... تم تقديم الطلب بنجاح...";
                    return RedirectToAction("Details", new { id = JobId });
                }
            }
            else
            {
                //No session found
                return RedirectToAction("Index");
            }
        }

        public IActionResult Details(int id)
        {
            if(id == null)
            {
                TempData["error"] = "لم يتم العثور على تفاصيل!";
                return RedirectToAction("Index");
            }

            Jobs? job = _context.Jobs
            .Include(a=> a.Project)
            .Include(a=> a.Employer)
            .Include(a=> a.SocialStatus)
            .Include(a=> a.JobsCities)
            .FirstOrDefault(a => a.Id == id && a.Deleted==false && a.Approved==true);
            if(job == null)
            {
                TempData["error"] = "لم يتم العثور على تفاصيل!";
                return RedirectToAction("Index");
            }
            //Get the skills fields for jobs
            var jobFields = _context.JobsFields.Where(a => a.JobId == id).Include(a => a.Field).ToList();
            //Get Latest Jobs According Date Descending
            ViewBag.LatestJobs = _context.Jobs.Where(a => a.Deleted == false && a.Id!=id && a.Approved == true
            && (a.StartDate == null || a.StartDate <= DateTime.Now)
            && (a.ExpireDate == null || a.ExpireDate >= DateTime.Now.AddDays(-1))
            )
            .Include(a => a.City)
            .Include(a => a.Employer)
            .Include(a => a.Currency)
            .OrderByDescending(a=> a.AddDate )
            .Take(5)
            .ToList();

            ViewBag.ApplyActive = true; //To Activate the Apply Button
            ViewBag.JobApplied = false;
            ViewBag.HaveEducation = true;
            ViewBag.OgImage = job.Employer.Logo;

            if (!String.IsNullOrEmpty(HttpContext.Session.GetString("mem_id")))
            {
                //Make sure that user logged in
                var MemberId = int.Parse(HttpContext.Session.GetString("mem_id"));
                Members member = _context.Members.Where(a => a.Id == MemberId && a.Deleted == false).Include(a => a.City).Include(a=> a.MembersEducation).FirstOrDefault();

                if (member != null)
                {
					//Check if Job Already Applied
					var appsCount = _context.JobsApplications.Where(a => a.JobId == id && a.MemberId == int.Parse(HttpContext.Session.GetString("mem_id")) && a.Deleted == false).Count();
					if (appsCount > 0)
					{
						ViewBag.JobApplied = true; //Member already applied for this job
						ViewBag.ApplyActive = false; //Not allowed to apply for this job again
						ViewBag.Application = _context.JobsApplications.Where(a => a.JobId == id && a.MemberId == MemberId).FirstOrDefault();
					}

					//Age Validation
					int age = CalculateAge(member.BirthDate);
					if (job.SelFromAge != null && age < job.SelFromAge) ViewBag.ApplyActive = false;
					if (job.SelToAge != null && age > job.SelToAge) ViewBag.ApplyActive = false;

					//Social Status Validation
					if (job.SocialStatus != null && job.SelSocialId != member.SocialId) ViewBag.ApplyActive = false;

					//Gender Validation
					if (job.SelGender != null && job.SelGender != 0 && (job.SelGender - 1) == member.Gender) ViewBag.ApplyActive = false;

                    //City Validation
                    if ((member.City == null && job.JobsCities?.Count()>0) || (job.JobsCities?.Count() > 0 && !job.JobsCities.Any(a => a.JobId == job.Id && a.CityId == member.CityId))) ViewBag.ApplyActive = false;
					//if (job.SelCity != null && (job.SelCityId != member.CityId || member.City == null)) ViewBag.ApplyActive = false;
					//District Validation
					if (!String.IsNullOrEmpty(job.SelDistrict) && job.SelDistrict != "Both" && (member.City.District != job.SelDistrict)) ViewBag.ApplyActive = false;

                    //Disable Apply to job if member doen't have education history
                    if (member.MembersEducation?.Where(a=> a.Deleted==false).Count() == 0)
                    {
                        ViewBag.ApplyActive = false;
                        ViewBag.HaveEducation = false;
                    }

                    //Village Validation
                    //if (job.SelVillageId != null && job.SelVillageId != member.VillageId) ViewBag.ApplyActive = false;
                }
			}
            //Check if Apply Button is Active
            //Check if date still valid for application
            if (DateTime.Now.Date < job.StartDate || DateTime.Now.AddDays(-1).Date > job.ExpireDate)
            {
                ViewBag.ApplyActive = false;
                ViewBag.JobExpired = true;
            }
            //Check if job Createria applied to member
            ViewData["title"] = job.ArName;
            ViewBag.JobFields = jobFields;
            return View(job);
        }

        static int CalculateAge(DateTime Dob)
        {
            DateTime Now = DateTime.Now;
            int Years = new DateTime(DateTime.Now.Subtract(Dob).Ticks).Year - 1;
            DateTime PastYearDate = Dob.AddYears(Years);
            int Months = 0;
            for (int i = 1; i <= 12; i++)
            {
                if (PastYearDate.AddMonths(i) == Now)
                {
                    Months = i;
                    break;
                }
                else if (PastYearDate.AddMonths(i) >= Now)
                {
                    Months = i - 1;
                    break;
                }
            }

            int Days = Now.Subtract(PastYearDate.AddMonths(Months)).Days;
            int Hours = Now.Subtract(PastYearDate).Hours;
            int Minutes = Now.Subtract(PastYearDate).Minutes;
            int Seconds = Now.Subtract(PastYearDate).Seconds;
            //return String.Format("Age: {0} Year(s) {1} Month(s) {2} Day(s) {3} Hour(s) {4} Second(s)",
            // Years, Months, Days, Hours, Seconds);
            return Years;
        }
    }
}
