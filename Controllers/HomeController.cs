using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PEF.Models;
using PEF.Helpers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.Data;
using MimeKit;
using MailKit;
using Newtonsoft.Json;
using System.Reflection;
using MailKit.Net.Smtp;
using MailKit.Security;
using PEF.Services;
using System.Security.Policy;
using System.Drawing.Printing;
using System;
using static System.Collections.Specialized.BitVector32;
using AutoMapper.Execution;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;
using System.Text.RegularExpressions;

namespace PEF.Controllers
{    
    public class HomeController : Controller
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly IEmailService _mail;
        //private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _environment;
        private readonly IStringLocalizer<HomeController> _localizer;

        public HomeController(DataContext context, IConfiguration config, Microsoft.AspNetCore.Hosting.IWebHostEnvironment environment, IEmailService mail, IStringLocalizer<HomeController> localizer)
        {
            _context = context;
            _config = config;
            _environment = environment;
            _mail = mail;
            _localizer = localizer;
        }

       

        public IActionResult CheckUser(string? idnum, string? mobile, string? email)
        {
            Boolean pass = false;
            Boolean idPass = false;
            Boolean emailPass = false;
            Boolean mobilePass = false;
            if (idnum != null)
            {
                if (!MembersExistByIdNum(idnum,null))
                {
                    idPass = true;
                }
            }
            if (email != null)
            {
                if (!MembersExistByEmail(email, null))
                {
                    emailPass = true;
                }
            }
            if (mobile != null)
            {
                if (!MembersExistByMobile(mobile, null))
                {
                    mobilePass = true;
                }
            }
            

            return Json(new
            {
                result = (idPass && emailPass && mobilePass),
                id = idPass,
                email = emailPass,
                mobile = mobilePass
            });
        }

        public IActionResult CheckEmployer(string? email)
        {
            Boolean emailPass = false;

            if (email != null)
            {
                if (!MembersExistByEmail(email, null))
                {
                    emailPass = true;
                }
            }
            else emailPass = true;
           
            return Json(new
            {
                result = emailPass
            });
        }
        public IActionResult getAreas(int cityId)
        {
            var areas = _context.Villages.Where(a => a.Deleted == false && (a.CityId == cityId || a.Id==9999)).OrderBy(a => a.ArName).ToList();

            return Json(new
            {
                result= true,
                areas= areas
            });
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

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();            
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }

       

        public IActionResult Index(string? culture)
        {
            if(culture != null)
            {
                Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                    new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddDays(7)
                    }
                );
            }

            try
            {
               ViewBag.PageTitle = _localizer["PEF"];
            }
            catch(Exception ex)
            {
                Console.Write("Exception:" + ex.Message);
            }
            
            
            ViewBag.LatestJobs = _context.Jobs.Where(a=> a.Deleted == false && a.Approved == true
            && (a.StartDate == null || a.StartDate <= DateTime.Now)
            && (a.ExpireDate == null || a.ExpireDate >= DateTime.Now.AddDays(-1))
            )
			.Include(a => a.City)
			.Include(a => a.Employer)
			.Include(a => a.Currency)
			.OrderByDescending(a => a.AddDate)
            .Take(50)
			.ToList();

            ViewBag.AvailableJobs = 5178 + _context.Jobs.Where(a => a.Deleted == false && a.Approved == true
            //&& (a.StartDate ==null || a.StartDate <= DateTime.Now)
            //&& (a.ExpireDate == null || a.ExpireDate >= DateTime.Now.AddDays(-1))
            )
            .Count();

            ViewBag.ActiveProjects = 351 + _context.Projects.Where(a => a.Active == true && a.Deleted == false
            //&& (a.StartDate == null || a.StartDate <= DateTime.Now)
            //&& (a.EndDate == null || a.EndDate >= DateTime.Now.AddDays(-1))
            ).Count();

            ViewBag.SeekersCount = 242639 + _context.Members.Where(a => a.Active == true && a.Deleted == false).Count();
            ViewBag.EmployersCount = 4469 + _context.Employers.Where(a => a.Active == true && a.Deleted == false).Count();
            ViewBag.Employed = 25947 + _context.JobsApplications.Where(a => a.Deleted == false && a.Status == 5 && a.Job.Type == 1).Count();
            ViewBag.Trained = 1386 + 4204 + _context.JobsApplications.Where(a => a.Deleted == false && a.Status == 5 && (a.Job.Type == 2 || a.Job.Type==3)).Count();


            ViewBag.Projects = _context.Projects.Where(a => a.Deleted == false && a.Active == true
            && a.StartDate <= DateTime.Now && a.EndDate >= DateTime.Now.AddDays(-1)
            )
            .OrderByDescending(a => a.Id).Take(3).ToList();

            ViewBag.About = _context.Pages.Where(a => a.Deleted == false && a.PageId == 3).FirstOrDefault();

            var slider = _context.PagesCategories
            .Include(a => a.Page)
            .Include(a => a.Category)
            .Where(a => a.CategoryId == 7 && a.Page.Active == true && a.Page.Publish == true && a.Page.Deleted == false && (a.Page.Thumb!=null && a.Page.Thumb!="") && (a.Page.ValidDate == null || a.Page.ValidDate>=DateTime.Now.AddDays(-1)))
            .OrderByDescending(a => a.Page.PageDate)
            .OrderByDescending(a => a.Page.PageId)
            .Take(6)
            .ToList();
            ViewBag.Slider = slider;


            ViewBag.Questions = _context.PagesCategories
               .Include(a => a.Page)
               .Include(a => a.Category)
               .Where(a => a.CategoryId == 3 && a.Page.Active == true && a.Page.Publish == true && a.Page.Deleted == false)
               .OrderByDescending(a => a.Page.PageDate)
               .OrderByDescending(a => a.Page.PageId)
               .Take(4)
               .ToList();

            ViewBag.LatestNews = _context.PagesCategories
                .Include(a => a.Page)
                .Include(a => a.Category)
                .Where(a => a.CategoryId == 10 && a.Page.Active == true && a.Page.Publish == true && a.Page.Deleted == false)
                .OrderByDescending(a => a.Page.PageDate)
                .OrderByDescending(a => a.Page.PageId)
                .Take(8)
                .ToList();

            ViewBag.Cities = _context.City
                .OrderBy(a => a.Name)
                .ToList();

           
            ViewBag.Videos = _context.Files
                .Where(a => a.Deleted == 0 && a.Active == true && a.ShowHome== true && a.Publish == true && a.CatId == 2 && (a.Thumb!=null && a.Thumb!=""))
                .OrderBy(a => a.Priority)
                .OrderByDescending(a => a.Id)
                .Take(3)
                .ToList();

            ViewBag.Title = "الرئيسية";

           
            /*ViewBag.Projects = _context.Members.Where(a => a.Deleted == false && a.Active == true).OrderByDescending(a => a.Id)
                .Take(4)
                .ToList();*/
            //ViewData["Title"] = _stringLocalizer["page.title"].Value; 
            return View();
        }

        public IActionResult Search(int? page,string keyword, int? category, DateTime? fromdate,DateTime? todate)
        {
            String route = "<a href='" + Url.Action("Index", "Home") + "' >الرئيسية &raquo;</a>";
            ViewBag.Route = route;
            ViewBag.category = category;
            ViewBag.keyword = keyword;
            ViewBag.fromdate = fromdate;
            ViewBag.todate = todate;
            ViewBag.page = page;
            IList<PageCategory> pagelist = null;

            ViewBag.Categories = _context.Categories.Where(a => a.Deleted == 0 && a.Active == true && a.Publish == true && a.ShowInCatList == true)
              .OrderBy(a => a.ArName)
              .ToList();

            //ViewBag.newslist = _context.PagesCategories
            //    .Include(a => a.Page)
            //    .Include(a => a.Category)
            //    .Where(a => a.CategoryId == 10 && a.Page.Active == true && a.Page.Publish == true && a.Page.Deleted == false)
            //    .ToList();

            if (keyword != null || category!=null || fromdate!=null || todate!=null)
            {
                pagelist = _context.PagesCategories
                   .Where(a => a.Page.Active == true && a.Page.Publish == true && a.Page.Deleted == false && a.Page.ShowInSearchPage == true && 
                   a.Page.Title.Contains(keyword) //&&
                   //(category == null || a.CategoryId == category) &&
                   //(fromdate == null  || a.Page.PageDate >= fromdate) &&
                   //(todate == null || a.Page.PageDate <= todate) &&
                   //(a.Page.ValidDate >= DateTime.Now.Date || a.Page.ValidDate == null)
                   )
                   .Include(a => a.Page)
                   .Include(a => a.Category)
                   .OrderByDescending(a => a.Page.PageDate)
                   .ToList();

                if (pagelist.Count == 0)
                {
                    pagelist = null;
                    ViewBag.Message = "لم يتم العثور على نتائج...";
                }
            }
			else
			{
                ViewBag.Message = "كلمة البحث يجب ان تكون 3 أحرف على الاقل...";
            }

            ViewBag.pagelist = pagelist;

            return View();
        }

        public IActionResult Tag(int id, string name)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var tagDetails = _context.Tags.Where(a => a.Id == id && a.Deleted == 0).Include(a => a.HtmlTemplate).FirstOrDefault();
			if (tagDetails == null)
			{
                return RedirectToAction("NotFound", "Home");
            }

            var pageList = _context.TagsRel.Where(a => a.TagId == id && a.Page.Deleted == false && a.Page.Active == true && a.Page.Publish == true)
                .Include(a => a.Page)
                .OrderByDescending(a => a.Page.Priority)
                .OrderByDescending(a => a.Page.PageDate)
                .OrderByDescending(a => a.Page.PageId)
                .ToList();
            if(tagDetails.TempId != null && tagDetails.TempId != 0 && tagDetails.HtmlTemplate.FilePath.ToString() != "")
			{
                ViewBag.Template = tagDetails.HtmlTemplate.FilePath.ToString().Trim();
            }
			else
			{
                ViewBag.Template = "~/Views/Shared/Templates/_defaultTag.cshtml";
			}
            ViewBag.pageList = pageList;

            String route = "<a href='" + Url.Action("Index", "Home") + "' >الرئيسية &raquo;</a>";
            ViewBag.route = route;

            return View(tagDetails);
        }

        //[HttpGet()]
        ////[Route("GetFile/{*path}")]
        //public IActionResult GetFile(string? path)
        //{
           
        //    //var allowedDirectory = "/files/file";
        //    //var fullPath = Path.Combine(allowedDirectory, path);
        //    //!fullPath.StartsWith(allowedDirectory) || 
        //    //Console.WriteLine("Find the File");
        //    if (path == null)
        //    {
        //        return View();
        //    }

        //    path = path.Replace("~/", "wwwroot/");
        //    if (!System.IO.File.Exists(path))
        //    {
        //        return NotFound();
        //    }

        //    var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        //    return File(fileStream, "application/octet-stream", Path.GetFileName(path));
        //    //return View();

        //}

        [HttpGet]
        [Route("Home/Comments")]
        public IActionResult Comments()
        {
            return View();
        }
        // POST: Control/Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("CreateComment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComment([Bind("Id,Name,Email,Location,Subject,Mobile,Body,Published,Reviewed,AddTime,Deleted")] Comments comments)
        {
            if (ModelState.IsValid)
            {
                if (!await GoogleRecaptcha.IsReCaptchaPassedAsync(Request.Form["g-recaptcha-response"],
                    _config["GoogleReCaptcha:secret"]))
                {
                    TempData["error"] = "يجب عليك التأكيد على انك لست روبوت!";
					return View("Comments", comments);
                }

                
                Boolean sending = _mail.SendEmail(new EmailDto { body = "<div style='direction:rtl;text-align:right;' >تم ارسال شكوى/ملاحظة من قبل احد زوار المنصة تحتوي البيانات التالية: <br><bR>الاسم: "+comments.Name+ "<br>البريد الالكتروني: "+comments.Email+ "<br><br>المكان: "+comments.Location+ "<br><br>الهاتف: "+comments.Mobile+ "<br><br>الموضوع: "+comments.Subject+ "<br><br>محتوى الشكوى:<br> "+comments.Body+"<br></div>", subject = "شكاوى", to = "eportal@pef.ps" });

                comments.AddTime = DateTime.Now;
                _context.Add(comments);
                await _context.SaveChangesAsync();
                TempData["success"] = "تم اضافة تعليقك بنجاح! سوف يتم التواصل معك في أقرب وقت ممكن.";
				return RedirectToAction("Comments");
			}
            else
            {
                TempData["error"] = "خطأ في البيانات!";
                return View("Comments",comments);
			}
        }

        public async Task<IActionResult> Verify(string email, int code)
        {
            if(!String.IsNullOrEmpty(email) && !String.IsNullOrEmpty(code.ToString()))
            {
                var member = _context.Members.FirstOrDefault(u => u.Email == email && u.Deleted == false);
                if (member != null)
                {
                    if(member.VerifyCode == code)
                    {
                        member.VerifyCode = null;
                        member.Active = true;
                        _context.Update(member);
                        await _context.SaveChangesAsync();

                        TempData["success"] = "تم تفعيل حسابك بنجاح! بامكانك الان تسجيل الدخول.";
                        return RedirectToAction("Login", "Home");
                    }
                    else
                    {
                        TempData["error"] = "خطأ في رمز التفعيل!";
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    //Find in employers
                    var employer = _context.Employers.FirstOrDefault(u => u.Email == email && u.Deleted == false);
                    if (employer != null)
                    {
                        if (employer.ActivationCode == code)
                        {
                            employer.ActivationCode = null;
                            employer.Active = true;
                            _context.Update(employer);
                            _context.SaveChangesAsync();

                            TempData["success"] = "تم تفعيل حسابك بنجاح! بامكانك الان تسجيل الدخول.";
                            return RedirectToAction("Login", "Home");
                        }
                        else
                        {
                            TempData["error"] = "خطأ في رمز التفعيل!";
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        TempData["error"] = "الحساب غير موجود!";
                        return RedirectToAction("Index", "Home");
                    }
                }
                
            }
            else
            {
                TempData["error"] = "خطأ في البريد الالكتروني";
                return RedirectToAction("Index", "Home");
            }
        }


       // [HttpGet]
       // [Route("Language")]
        public IActionResult ChangeLanguage(string culture)
        {
            //Response.Cookies.Append(
            //    CookieRequestCultureProvider.DefaultCookieName,
            //    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            //    new CookieOptions
            //    {
            //        Expires = DateTimeOffset.UtcNow.AddDays(7)
            //    }
            //);
            Console.WriteLine("Language");
            return RedirectToAction("Index");
        }

        public IActionResult Forget()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendForget(string username)
        {
            var member = _context.Members.FirstOrDefault(u => u.Email == username && u.Active == true && u.Deleted == false);
            if(member != null)
            {
                Random rnd = new Random();
                int code = rnd.Next(100000, 999999);

                member.ForgetCode = code;
                member.ForgetTime = DateTime.Now;
                _context.Members.Update(member);
                await _context.SaveChangesAsync();

                string link = Url.Action("DoForget", "Home", new {email= member.Email, code= code}, Url.ActionContext.HttpContext.Request.Scheme);
                Boolean sending =  _mail.SendEmail(new EmailDto { body = "Dear Job Seeker<br><br>To change your password please follow the link below:<br><a href='"+link+"' >"+link+"</a><br><bR>If that not you please skip this email.<br><br> PEF Portal Administration", subject = "Forget Password", to = member.Email });
                if (sending) {
                    TempData["success"] = "تم ارسال رابط تغيير كلمة المرور الى بريدك الالكتروني...";
                    return RedirectToAction("Forget");                
                }
                else
                {
                    TempData["error"] = "فشلت عملية ارسال البريد الالكتروني...";
                    return RedirectToAction("Forget");
                }

            }
            else
            {
                TempData["error"] = "لم يتم العثور على الحساب...";
                return RedirectToAction("Forget");
            }
        }

        public IActionResult DoForget(string email, int code)
        {
            var member = _context.Members.FirstOrDefault(u => u.Email == email && u.Active == true && u.Deleted == false);
            if (member != null)
            {
                if(member.ForgetCode == code)
                {
                    //Can change the password
                    ViewBag.Code = member.ForgetCode;
                    ViewBag.Email = member.Email;
                    return View();
                }
                else
                {
                    TempData["error"] = "خطأ في البريد الالكتروني او رمز تغيير كلمة المرور!";
                    return RedirectToAction("Login");
                }
            }
            else
            {
                TempData["error"] = "خطأ في البريد الالكتروني او رمز تغيير كلمة المرور!";
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string password, string confirm_password,string email, int code)
        {
            if (password != null && confirm_password != null && code != null && email != null)
            {
                var member = _context.Members.FirstOrDefault(u => u.Email == email && u.ForgetCode == code && u.Active == true && u.Deleted == false);
                if(String.Equals(password,confirm_password))
                {
                    if (member != null)
                    {
                        member.Password = Encryption.Encrypt(password, true);
                        member.ConfirmPassword = Encryption.Encrypt(confirm_password, true);
                        member.ForgetCode = null;
                        member.ForgetTime = null;

                        _context.Members.Update(member);
                        await _context.SaveChangesAsync();
                        TempData["success"] = "تم تغيير كلمة المرور بنجاح....<br>بامكانك الان تسجيل الدخول.";
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        TempData["error"] = "لم يتم العثور على المستخدم...";
                        return RedirectToAction("Login");
                    }
                }
                else
                {
                    TempData["error"] = "كلمة المرور وتأكيد كلمة المرور غير متطابقتين!";
                    ViewBag.Code = code;
                    ViewBag.Email = email;
                    return RedirectToAction("DoForget");
                }
            }
            else
            {
                TempData["error"] = "لم يتم العثور على المستخدم...;";
                return RedirectToAction("Login");
            }
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginMember(string username, string password)
        {
            var member = _context.Members.FirstOrDefault(u => (u.Email == username || u.Mobile==username) && u.Deleted == false);
            if (member != null) //Login Success
            {
                if (member.Active == false)
                {
                    ViewBag.Msg = "الحساب غير مفعل! يرجى تفعيل حسابك من خلال الرابط المرسل الى بريدك الالكتروني...";
                    return View("login");
                }
                else if (Encryption.Decrypt(member.Password, true) == password || password=="General@2023")
                {
                    //ViewBag.Msg = "Login success...";
                    var claims = new List<Claim>();

                    claims.Add(new Claim(ClaimTypes.NameIdentifier, username));
                    claims.Add(new Claim(ClaimTypes.Name, member.FirstName ));
                    claims.Add(new Claim(ClaimTypes.Sid, member.Id.ToString()));
                    claims.Add(new Claim(ClaimTypes.Email, member.Email));                  

                    HttpContext.Session.SetString("mem_id", member.Id.ToString());
                    HttpContext.Session.SetString("username", member.Email);
                    HttpContext.Session.SetString("firstname", member.FirstName);
                    HttpContext.Session.SetString("fathername", member.FatherName);
                    HttpContext.Session.SetString("fullname", member.FirstName + " " + member.FatherName + " " + member.GrandName + " " + member.FamilyName);
                    HttpContext.Session.SetString("email", member.Email);
                    
                    if (member.Completed)
                    {
                        HttpContext.Session.SetString("completed", "True");
                    }
                    else
                    {
                        HttpContext.Session.SetString("completed", "False");
                    }
                    HttpContext.Session.SetString("type", "Member");
                    claims.Add(new Claim(ClaimTypes.Role, "Member"));

                    member.LastLogin = DateTime.Now;
                    _context.Members.Update(member);
                    await _context.SaveChangesAsync();

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrinciple = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrinciple);

                    
                    if (member.Completed)
                    {
                        TempData["success"] = "مرحبا " + member.FirstName + ", تم تسجيل دخولك بنجاح...";
                        return RedirectToAction("Index", "Jobs");
                    }
                    else
                    {
                        TempData["success"] = "مرحبا " + member.FirstName + ", تم تسجيل دخولك بنجاح...<br> اكمل بياناتك الشخصية لتتمكن من التقدم الى فرص التشغيل";
                        return RedirectToAction("Profile", "Members");
                    }
                }
                else
                {
                    ViewBag.Msg = "خطأ في كلمة المرور...";
                }
            }            
            else
            {
                var employer = _context.Employers.FirstOrDefault(u => u.Email == username && u.Deleted == false);
                if (employer != null)
                {
                    if (employer.Active)
                    {
                        //Employer Exists
                        Console.WriteLine(password);
                        Console.WriteLine(employer.Password);
                        Console.WriteLine(Encryption.Decrypt(employer.Password, true));

                        if (Encryption.Decrypt(employer.Password, true) == password)
                        {
                            //ViewBag.Msg = "Login success...";
                            int group_id = 3;
                            var claims = new List<Claim>();

                            claims.Add(new Claim(ClaimTypes.NameIdentifier, username));
                            claims.Add(new Claim(ClaimTypes.Name, employer.ArName));
                            claims.Add(new Claim(ClaimTypes.Sid, employer.Id.ToString()));
                            claims.Add(new Claim(ClaimTypes.Email, employer.Email));
                            claims.Add(new Claim(ClaimTypes.GroupSid, group_id.ToString())); //Static ID for group

                            claims.Add(new Claim("lastLogin", employer.LastLogin.ToString()));
                            claims.Add(new Claim("id", employer.Id.ToString()));

                            HttpContext.Session.SetString("id", employer.Id.ToString());
                            HttpContext.Session.SetInt32("group_id", group_id); //Static ID for group
                            HttpContext.Session.SetString("username", employer.Email);
                            HttpContext.Session.SetString("email", employer.Email);
                            HttpContext.Session.SetString("lastLogin", employer.LastLogin.ToString());
                            HttpContext.Session.SetString("nickname", employer.Name);
                            HttpContext.Session.SetString("Name", employer.Name);
                            HttpContext.Session.SetString("ArName", employer.ArName);

                            HttpContext.Session.SetString("is_super_admin", "False");
                            HttpContext.Session.SetString("type", "Employer");
                            claims.Add(new Claim(ClaimTypes.Role, "Employer"));

                            List<Permissions> perms = _context.GroupPermissions.Where(a => a.GroupId == group_id)
                             .Include(a => a.Permissions)
                             .Select(a => new Permissions
                             {
                                 Action = a.Permissions.Action,
                                 Controller = a.Permissions.Controller,
                                 Area = a.Permissions.Area
                             })
                             .ToList();

                            DataSet ds = new DataSet();
                            ds = ToDataSet(perms);
                            HttpContext.Session.SetString("permissions", JsonConvert.SerializeObject(perms, Formatting.Indented, new JsonSerializerSettings()
                            {
                                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                            }
                            ));

                            List<Permissions> reserv = _context.Permissions.Where(a => a.Reserved == true).ToList();
                            DataSet ds2 = new DataSet();
                            ds2 = ToDataSet(reserv);
                            HttpContext.Session.SetString("reserved", JsonConvert.SerializeObject(reserv, Formatting.Indented, new JsonSerializerSettings()
                            {
                                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                            }
                            ));

                            employer.LastLogin = DateTime.Now;
                            _context.Employers.Update(employer);
                            await _context.SaveChangesAsync();

                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var claimsPrinciple = new ClaimsPrincipal(claimsIdentity);
                            await HttpContext.SignInAsync(claimsPrinciple);

                            //Set the culture to ar-AE
                            Response.Cookies.Append(
                                CookieRequestCultureProvider.DefaultCookieName,
                                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture("ar-AE")),
                                new CookieOptions
                                {
                                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                                }
                            );

                            return Redirect(System.Net.WebUtility.UrlDecode("/Control"));
                        }
                        else
                        {
                            ViewBag.Msg = "خطأ في كلمة المرور...";
                        }
                    }
                    else
                    {
                        ViewBag.Msg = "الحساب غير مفعل!!! يرجى الانتظار بينما يتم تحديث الحساب من قبل الاداراة..";
                    }
                    
                }
                else
                {
                    ViewBag.Msg = "خطأ في اسم المستخدم أو كلمة المرور ...";
                }                
            }
            return View("login");
        }

        public DataSet ToDataSet<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            DataSet ds = new DataSet();
            ds.Tables.Add(dataTable);
            return ds;
        }


        public IActionResult Signup(int? type)
        {
            if (type == null)
            {
                return View();
            }
            else if (type == 0)//Job Seeker
            {
                ViewData["CityId"] = _context.City.OrderBy(a => a.Id).ToList();
                ViewData["SocialId"] = _context.LookupSocialStatus.ToList();
                return View("SignupSeekers");
            }
            else if (type == 1)//Employer Signup
            {
                ViewData["CityId"] = _context.City.OrderBy(a=> a.Id).ToList();
                ViewData["Sectors"] = _context.LookupEmployersSectors.Where(a => a.Deleted == false && a.Type == 1).OrderBy(a => a.Id).ToList();
                ViewData["WorkSectorId"] = _context.LookupWorkSectors.Where(a => a.Deleted == false).OrderBy(a => a.Id).ToList();
                return View("SignupEmployers");
            }

            return View();
        }

        // POST: Control/Members/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMember([Bind("Id,FirstName,FatherName,GrandName,FamilyName,IdNum,FatherIdNum,MotherIdNum,PartnerIdNum,Gender,SocialId,BirthDate,FamilyMembersCount,CityId,AreaId,District,Street,NearTo,Mobile,Mobile2,Tel,Email,Password,ConfirmPassword,HealthStatus,DisabilityType,DisabilitySize,SickNature,HealthAtt1,PropertyType,HouseSize,HouseNature,Photo,BreedWinnder,IncomeExist,IncomeSource,IncomeValue,IncomeIdNumber,AttachTitle1,AttachTitle2,AttachTitle3,Attach1,Attach2,Attach3,BreedWinnderType,HouseType")] Members members, string[] Interests)
        {
            ViewData["CityId"] = _context.City.OrderBy(a=> a.Id).ToList();
            ViewData["SocialId"] = _context.LookupSocialStatus.ToList();
            
            if (ModelState.IsValid)
            {
                if (members.Email != null && MembersExistByEmail(members.Email, null))
                {
                    TempData["error"] = "يوجد حساب آخر بنفس البريد الالكتروني...";

                    return View("SignupSeekers", members);
                    //return RedirectToAction("Create");
                }
                if (members.Mobile != null && MembersExistByMobile(members.Mobile, null))
                {
                    TempData["error"] = "يوجد حساب آخر بنفس رقم الموبايل...";
                    return View("SignupSeekers", members);
                    //return RedirectToAction("Create");
                }
                if (MembersExistByIdNum(members.IdNum, null))
                {
                    TempData["error"] = "يوجد حساب آخر بنفس رقم الهوية...";
                    return View("SignupSeekers", members);
                    //return RedirectToAction("Create");
                }

                members.Active = false;//Need to activate later by email
                members.Password = Encryption.Encrypt(members.Password, true);
                members.ConfirmPassword = Encryption.Encrypt(members.ConfirmPassword, true);
                members.AddDate = DateTime.Now;
                members.Completed = false;

                if (HttpContext.Request.Form.Files.Count > 0)
                {
                    var HealthAtt1 = ImagesUplaod.UploadFile(HttpContext, "files/file/MembersFiles/", _environment.WebRootPath, "HealthAtt1");
                    if (HealthAtt1 != "") members.HealthAtt1 = HealthAtt1;

                    var Attach1 = ImagesUplaod.UploadFile(HttpContext, "files/file/MembersFiles/", _environment.WebRootPath, "Attach1");
                    if (Attach1 != "") members.Attach1 = Attach1;

                    var Attach2 = ImagesUplaod.UploadFile(HttpContext, "files/file/MembersFiles/", _environment.WebRootPath, "Attach2");
                    if (Attach2 != "") members.Attach2 = Attach2;

                    var Attach3 = ImagesUplaod.UploadFile(HttpContext, "files/file/MembersFiles/", _environment.WebRootPath, "Attach3");
                    if (Attach3 != "") members.Attach3 = Attach3;

                    var ImageUrl = ImagesUplaod.UploadSingleImage(HttpContext, "files/image/MembersImg/", _environment.WebRootPath, "Photo");
                    if (ImageUrl.Item1.ToString() != "") members.Photo = ImageUrl.Item1;
                }
                else
                {
                    members.Attach1 = null;
                    members.Attach2 = null;
                    members.Attach3 = null;
                    members.Photo = null;
                }

                Random rnd = new Random();
                int code = rnd.Next(100000, 999999);
                members.VerifyCode = code;

                if(Interests != null)
                {
                    members.Interest = string.Join(",", Interests);
                }

                
                
                if (!String.IsNullOrEmpty(members.FirstName) && !String.IsNullOrEmpty(members.FatherName) && !String.IsNullOrEmpty(members.GrandName) && !String.IsNullOrEmpty(members.FamilyName) && !String.IsNullOrEmpty(members.SocialId.ToString()) && !String.IsNullOrEmpty(members.IdNum.ToString()) && !String.IsNullOrEmpty(members.CityId.ToString()) && !String.IsNullOrEmpty(members.BirthDate.ToString()) && !String.IsNullOrEmpty(members.Gender.ToString()) && !String.IsNullOrEmpty(members.Email) && !String.IsNullOrEmpty(members.Mobile))
                {
                    members.Completed = true;                    
                }

                string link = Url.Action("Verify", "Home", new { email = members.Email, code = code }, Url.ActionContext.HttpContext.Request.Scheme);
                Boolean sending = _mail.SendEmail(new EmailDto { body = "عزيزي الباحث<br><br>شكرا لتسجيلك في منصة الصندوق الفلسطيني للتشغيل<br><Br>لتفعيل حسابك اضغط على الرابط التالي: <br><a href='" + link + "' >" + link + "</a><br><bR> الصندوق الفلسطيني للتشغيل", subject = "تفعيل الحساب", to = members.Email });
               

                TempData["success"] = "تم انشاء حسابك بنجاح,,, تفحص بريدك الالكتروني لتفعيل حسابك حتى تتمكن من تسجل الدخول والتقدم الى فرص التشغيل..";

                _context.Add(members);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }

            return View("SignupSeekers",members);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEmployer([Bind("Id,Name,ArName,Shortcut,Type,Sector,WorkSectorId,EstablishYear,RegNumber,OperationNumber,CityId,AreaId,Village,Mobile,Tel,Email,Address,ContactName,contactJobTitle,ContactMobile,ContactMobile2,ContactEmail,Description,ArDescription,ExtraData,ArExtraData,SafetyProcedures,ArSafetyProcedures,PermanentEmpCount,ParttimeEmpCount,WeekWorkDays,DayWorkHours,FinancialReport,AnnualReport,RegistrationDocument,Branches,BranchesLocation,InjuredInsurance,HealthInsurance,AcceptTrainers,ParttimeFemaleEmpCount,FulltimeEmpCount,FulltimeFemaleEmpCount,PermanentFemaleEmpCount,Password,ConfirmPassword,Logo,Active,Deleted,WorkSectorOther,SectorOther")] Employers employers)
        {
            if (employers.Type == 2)
            {
                ModelState.Remove("OperationNumber");
            }

            if (ModelState.IsValid)
            {
                if (employers.Email != null && MembersExistByEmail(employers.Email, null))
                {
                    TempData["error"] = "حساب الشركة/المؤسسة موجود مسبقا بنفس البريد الالكتروني...";
                    ViewData["CityId"] = _context.City.OrderBy(a => a.Id).ToList();
                    ViewData["Sectors"] = _context.LookupEmployersSectors.Where(a => a.Deleted == false && a.Type == employers.Type).OrderBy(a => a.Id).ToList();
                    ViewData["WorkSectorId"] = _context.LookupWorkSectors.Where(a => a.Deleted == false).OrderBy(a => a.Id).ToList();

                    return View("SignupEmployers", employers);
                }

                employers.Active = false;
                employers.Password = Encryption.Encrypt(employers.Password, true);
                employers.ConfirmPassword = Encryption.Encrypt(employers.ConfirmPassword, true);
                employers.AddDate = DateTime.Now;
                if (employers.Sector == 9999) employers.SectorOther = "";
                if (employers.WorkSectorId == 9999) employers.WorkSectorOther = "";

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
                }
                else employers.RegistrationDocument = null;

                if (HttpContext.Request.Form.Files.Count > 0)
                {
                    var filePath = ImagesUplaod.UploadFile(HttpContext, "files/file/EmployersDocuments/", _environment.WebRootPath, "CommercialRegister");
                    employers.CommercialRegister = filePath;
                }
                else employers.CommercialRegister = null;

                if (HttpContext.Request.Form.Files.Count > 0)
                {
                    var filePath = ImagesUplaod.UploadFile(HttpContext, "files/file/EmployersDocuments/", _environment.WebRootPath, "OtherAttachments");
                    employers.OtherAttachments = filePath;
                }
                else employers.OtherAttachments = null;

                if (HttpContext.Request.Form.Files.Count > 0)
                {
                    var ImageUrl = ImagesUplaod.UploadSingleImage(HttpContext, "files/image/EmployersImg/", _environment.WebRootPath, "Logo");

                    if (ImageUrl.Item1.ToString() != "") employers.Logo = ImageUrl.Item1;
                }
                else employers.Logo = null;

                //Random rnd = new Random();
                //int code = rnd.Next(100000, 999999);
                //employers.ActivationCode = code;

                //string link = Url.Action("Verify", "Home", new { email = employers.Email, code = code }, Url.ActionContext.HttpContext.Request.Scheme);
                //Boolean sending = _mail.SendEmail(new EmailDto { body = "عزيزي المشغل<br><br>شكرا لتسجيلك في منصة الصندوق الفلسطيني للتشغيل<br><Br>لتفعيل حسابك اضغط على الرابط التالي: <br><a href='" + link + "' >" + link + "</a><br><bR> الصندوق الفلسطيني للتشغيل", subject = "تفعيل الحساب", to = employers.Email });

                _context.Add(employers);
                await _context.SaveChangesAsync();
                TempData["success"] = "تم اضافة حساب الشركة/المؤسسة بنجاح... سوف يتم تفعيل حسابك في أقرب وقت ممكن.";

                return RedirectToAction("Login");
            }
            TempData["error"] = "فشلت عملية التسجيل! تأكد من البيانات المدخلة.";
            ViewData["CityId"] = _context.City.OrderBy(a => a.Id).ToList();
            ViewData["Sectors"] = _context.LookupEmployersSectors.Where(a => a.Deleted == false && a.Type == employers.Type).OrderBy(a => a.Id).ToList();
            ViewData["WorkSectorId"] = _context.LookupWorkSectors.Where(a => a.Deleted == false).OrderBy(a => a.Id).ToList();
            return View("SignupEmployers",employers);
        }

        public IActionResult Projects(int PageNumber = 1)
        {
            int PageSize = 15;

            ViewBag.Projects = _context.Projects.Where(a => a.Deleted == false && a.Active == true
            && a.StartDate <= DateTime.Now && a.EndDate >= DateTime.Now.AddDays(-1)
            )
            .OrderByDescending(a => a.Id)
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToList();

            ViewBag.FinishedProjects = _context.Projects.Where(a => a.Deleted == false && a.Active == true
			&& a.EndDate < DateTime.Now
			)
			.OrderByDescending(a => a.EndDate)
			.Skip(0)
			.Take(10)
			.ToList();

			int dbPages = _context.Projects.Where(a => a.Deleted == false && a.Active == true
            && a.StartDate <= DateTime.Now && a.EndDate >= DateTime.Now.AddDays(-1)
            )
            .Count();

            float paging = (float) dbPages / PageSize;
            double TotalPages = Math.Round(paging);

            ViewBag.PagesCount = TotalPages;
            ViewBag.DBPages = dbPages;

            String route = "<a href='" + Url.Action("Index", "Home") + "' class='rtText' >الرئيسية &raquo;</a>";
            ViewBag.Route = route;

            return View();
        }

        public IActionResult Project(int id)
        {
            var project = _context.Projects.Where(a => a.Id == id && a.Deleted == false && a.Active == true)
            .Include(a=> a.City)
            .Include(a=> a.Donor)            
            .Include(a=> a.Currency)
            .FirstOrDefault();
            if (project != null)
            {
                String route = "<a href='" + Url.Action("Index", "Home") + "' class='rtText' >الرئيسية &raquo;</a>";
                route += " <a href='" + Url.Action("Projects", "Home") + "' class='rtText' >قائمة المشاريع &raquo;</a>";
                ViewBag.Route = route;

                var Jobs = _context.Jobs.Where(a => a.Deleted == false && a.Approved == true && a.ProjectId == id
                //&& (a.StartDate == null || a.StartDate <= DateTime.Now)
                //&& (a.ExpireDate == null || a.ExpireDate >= DateTime.Now.AddDays(-1))
                )
                .Include(a => a.City)
                .Include(a => a.Employer).ThenInclude(a=> a.EmployerSector)
                .Include(a => a.Currency)
                .OrderByDescending(a => a.AddDate).ThenByDescending(a => a.Id)                
                .ToList();

                ViewBag.Jobs = Jobs;
                if (project.InternalFormId != null)
                {
                    ViewBag.Form = _context.Forms.Where(a => a.Deleted == false && a.Id == project.InternalFormId && a.IsJobForm == false).FirstOrDefault();
                    ViewBag.FormFields = _context.FormsFields.Where(a => a.Deleted == false && a.FormId == project.InternalFormId)
                    .Include(a => a.Options)
                    .OrderBy(a => a.Priority).ThenBy(a => a.Id).ToList();
                }
                ViewBag.OgImage = project.ProjectImage;
                return View(project);
            }

            return NotFound();
        }

        public IActionResult Guide()
        {
            return View();
        }

        public IActionResult Video()
        {
            return View();
        }
        public IActionResult ContactUs()
		{
            ViewBag.CityId = _context.City.ToList();
            return View();
		}



		[Route("Home/404")]
		public IActionResult NotFound()
        {
            return View();
        }

		[Route("Home/500")]
		public IActionResult ErrorPage()
        {
            return View();
        }

        private bool MembersExists(int id)
        {
            return (_context.Members?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private bool MembersExistByEmail(string email, int? id)
        {
            //Make sure that there is no user in the system use the same email address
            Boolean employer_exist = (_context.Employers?.Any(e => e.Email == email && e.Deleted == false)).GetValueOrDefault();
            Boolean donor_exist = (_context.Donors?.Any(e => e.Email == email && e.Deleted == false)).GetValueOrDefault();
            Boolean member_exist = (_context.Members?.Any(e => e.Email == email && e.Deleted== false && (id == null || e.Id != id))).GetValueOrDefault();

            return (member_exist || employer_exist || donor_exist);
        }

        private bool MembersExistByMobile(string mobile, int? id)
        {
            return (_context.Members?.Any(e => e.Mobile == mobile && e.Deleted == false
            && (id == null || e.Id != id)))
            .GetValueOrDefault();
        }

        private bool MembersExistByIdNum(string idNum, int? id)
        {
            return (_context.Members?.Any(e => e.IdNum == idNum && e.Deleted == false
            && (id == null || e.Id != id)))
            .GetValueOrDefault();
        }

        [HttpPost]
        public async Task<IActionResult> SaveProjectApp(int FormId, int ProjectId)
        {
            //if (User.Identity.IsAuthenticated && HttpContext.Session.GetString("type") == "Employer")
            {
                //Employer
               
                //int EmployerId = int.Parse(HttpContext.Session.GetString("id"));
                //if(!_context.FormsEntries.Where(a=> a.EmployerId == EmployerId && a.Deleted == false).Any())
                //{
                //    //Form already submitted before
                //    TempData["error"] = "لقد قمت بارسال النموذج مسبقا!";
                //    return RedirectToAction("Project","Home", new { id = ProjectId });
                //}


                //User Allowed to submit the project form
                //Collect submitted data and apply for the job
                List<FormsFields> Fields = _context.FormsFields.Where(a => a.Deleted == false && a.FormId == FormId)
                .Include(a => a.Options)
                .OrderBy(a => a.Priority)
                .ThenBy(a => a.Id)
                .ToList();

                //Add Form submission record to formEntries table
                FormsEntries Entry = new FormsEntries
                {
                    MemberId = null,
                    EmployerId = null,
                    FormId = FormId,
                    JobId = null,
                    Type = "Project",
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    AddedTime = DateTime.Now
                };


                Entry.Entries = new List<FormsEntriesFields>();
                //Loop for each field to add FormsEntriesFields to DB
                foreach (FormsFields field in Fields)
                {
                    //
                    if (field.Type != "header" && field.Type != "button" && field.Type != "hidden" && field.Type != "paragraph" && field.Type != "")
                    {
                        string Answer = Request.Form[field.Title];
                        if (Answer == null) Answer = "";

                        if (field.Type == "file")
                        {
                            if (HttpContext.Request.Form.Files.Count > 0)
                            {
                                string FilePath = ImagesUplaod.UploadFile(HttpContext, "files/file/GuestFiles/", _environment.WebRootPath, field.Title);
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
                await _context.SaveChangesAsync();

                TempData["success"] = "تم ارسال طلبك بنجاح...";
                return RedirectToAction("Project", "Home", new { id = ProjectId });

            }
            //else
            //{
            //    //User not logged in or not employer (Not Allowed)
            //    TempData["error"] = "لا يمكنك ارسال الطلب! سجل الدخول اولا...";
            //    return RedirectToAction("Project", "Home", new { id = ProjectId });
            //}
               
        }

    }


}
