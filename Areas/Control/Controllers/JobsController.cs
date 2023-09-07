using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PEF.Helpers;
using PEF.Migrations;
using PEF.Models;
using OfficeOpenXml;
using MailKit.Net.Smtp;
using MailKit.Security;


using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using OfficeOpenXml.Style;
using PEF.Services;
using PEF.Pages.Members;
using Microsoft.Extensions.Localization;
using PEF.Pages.Projects;

namespace PEF.Areas.Control.Controllers
{
    [Area("Control")]
    [Authorize]
    [PEF.AuthorizedAction]
    public class JobsController : Controller
    {
        private readonly DataContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        private readonly IEmailService _mail;
        private readonly string currentCulture;
        private readonly IStringLocalizer<HomeController> _localizer;

        public JobsController(DataContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, IEmailService mail, IStringLocalizer<HomeController> localizer)
        {
            _context = context;
            _environment = environment;
            _mail = mail;
            currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            _localizer = localizer;
        }

        [HttpPost]
        [Route("Control/Jobs/UploadAppDocument")]
        public async Task<IActionResult> UploadAppDocument()
        {
            string filePath = "";
            int appId = int.Parse(HttpContext.Request.Form["att_app_id"]);
            string fileName = HttpContext.Request.Form["document_title"];
            if (HttpContext.Request.Form.Files.Count > 0)
            {
                filePath = ImagesUplaod.UploadFile(HttpContext, "files/file/MembersFiles/", _environment.WebRootPath, "Document");
            }

            if(String.IsNullOrEmpty(fileName) || String.IsNullOrEmpty(filePath))
            {
                return Json(new
                {
                    result = false,
                    msg = _localizer["Failed to upload"]
                });
            }

            var app = _context.JobsApplications.FirstOrDefault(a => a.Id == appId);
            if (app!=null)
            {
                _context.Add(new JobsAppsAttachments { FileName = fileName, FilePath = filePath, JobAppId = appId, MemberId = app.MemberId });
                await _context.SaveChangesAsync();
                return Json(new
                {
                    result = true,
                    msg = _localizer["Upload Success"]
                }); ;
            }
            else
            {
                return Json(new
                {
                    result = false,
                    msg = _localizer["Failed to upload"]
                });
            }
           
        }

        [HttpGet]
        [Route("Control/Jobs/ReloadDocuments")]
        public IActionResult ReloadDocuments(int app_id)
        {
            List<JobsAppsAttachments> appAtt = _context.JobsAppsAttachments.Where(a => a.Deleted == false && a.JobAppId == app_id).ToList();

            return Json(new
            {
                result = true,
                documents = appAtt
            });
        }


        private void FillMemberSheet(ref ExcelWorksheet worksheet, Members member, int row, ref int index)
        {
            worksheet.Cells[row, ++index].Value = member.Id;
            worksheet.Cells[row, ++index].Value = member.FirstName + " " + member.FatherName + " " + member.GrandName + " " + member.FamilyName;
            worksheet.Cells[row, ++index].Value = member.IdNum;
            if (member.Gender == 2) worksheet.Cells[row, ++index].Value = "أنثى";
            else worksheet.Cells[row, ++index].Value = "ذكر";
            worksheet.Cells[row, ++index].Value = member.BirthDate.ToString("dd/MM/yyyy");
            worksheet.Cells[row, ++index].Value = Functions.CalculateAge(member.BirthDate).ToString();
            worksheet.Cells[row, ++index].Value = member.SocialStatus?.ArName;

            worksheet.Cells[row, ++index].Value = member.FatherIdNum;
            worksheet.Cells[row, ++index].Value = member.MotherIdNum;
            worksheet.Cells[row, ++index].Value = member.PartnerIdNum;
            worksheet.Cells[row, ++index].Value = member.City?.ArName;
            worksheet.Cells[row, ++index].Value = member.Area?.ArName;
            worksheet.Cells[row, ++index].Value = member.District;
            worksheet.Cells[row, ++index].Value = member.Street;
            worksheet.Cells[row, ++index].Value = member.NearTo;
            worksheet.Cells[row, ++index].Value = member.Tel;
            worksheet.Cells[row, ++index].Value = member.Mobile;
            worksheet.Cells[row, ++index].Value = member.Mobile2;
            worksheet.Cells[row, ++index].Value = member.Email;
            if (member.HealthStatus == 1) worksheet.Cells[row, ++index].Value = "سليم";
            else if (member.HealthStatus == 2) worksheet.Cells[row, ++index].Value = "مريض";
            else if (member.HealthStatus == 3) worksheet.Cells[row, ++index].Value = "ذوي اعاقة";

            if (member.SickNature == 1) worksheet.Cells[row, ++index].Value = "مريض قلب";
            else if (member.SickNature == 2) worksheet.Cells[row, ++index].Value = "مريض ضغط";
            else if (member.SickNature == 3) worksheet.Cells[row, ++index].Value = "مريض سكري";
            else if (member.SickNature == 4) worksheet.Cells[row, ++index].Value = "مريض روماتزم";
            else worksheet.Cells[row, ++index].Value = "";

            if (member.DisabilityType == 1) worksheet.Cells[row, ++index].Value = "حركية";
            else if (member.DisabilityType == 2) worksheet.Cells[row, ++index].Value = "سمعية";
            else if (member.DisabilityType == 3) worksheet.Cells[row, ++index].Value = "بصرية";
            else if (member.DisabilityType == 4) worksheet.Cells[row, ++index].Value = "عقلية";
            else if (member.DisabilityType == 5) worksheet.Cells[row, ++index].Value = "نطقية";
            else worksheet.Cells[row, ++index].Value = "";

            if (member.DisabilitySize == 1) worksheet.Cells[row, ++index].Value = "جزئي";
            else if (member.DisabilitySize == 2) worksheet.Cells[row, ++index].Value = "كلي";
            else worksheet.Cells[row, ++index].Value = "";

            if (member.PropertyType == 0) worksheet.Cells[row, ++index].Value = "أجار";
            else if (member.PropertyType == 1) worksheet.Cells[row, ++index].Value = "ملك";
            else if (member.PropertyType == 2) worksheet.Cells[row, ++index].Value = "استخدام";
            else worksheet.Cells[row, ++index].Value = "";

            if (member.HouseNature == 0) worksheet.Cells[row, ++index].Value = "شقة";
            else if (member.HouseNature == 1) worksheet.Cells[row, ++index].Value = "منزل مستقل";
            else if (member.HouseNature == 2) worksheet.Cells[row, ++index].Value = "أخرى";
            else worksheet.Cells[row, ++index].Value = "";

            worksheet.Cells[row, ++index].Value = member.HouseSize;

            if (member.IncomeExist)
            {
                worksheet.Cells[row, ++index].Value = "نعم";
                worksheet.Cells[row, ++index].Value = member.BreedWinnder;
                worksheet.Cells[row, ++index].Value = member.IncomeIdNumber;
                //worksheet.Cells[row, ++index].Value = "";
                if (member.BreedWinnderType == 0) worksheet.Cells[row, ++index].Value = "نفسه";
                else if (member.BreedWinnderType == 1) worksheet.Cells[row, ++index].Value = "زوج / زوجة";
                else if (member.BreedWinnderType == 2) worksheet.Cells[row, ++index].Value = "أخ / أخت";
                else if (member.BreedWinnderType == 3) worksheet.Cells[row, ++index].Value = "ابن";
                else if (member.BreedWinnderType == 4) worksheet.Cells[row, ++index].Value = "أب";
                else if (member.BreedWinnderType == 5) worksheet.Cells[row, ++index].Value = "أم";
                else if (member.BreedWinnderType == 6) worksheet.Cells[row, ++index].Value = "غير ذلك";
                else worksheet.Cells[row, ++index].Value = "";


                worksheet.Cells[row, ++index].Value = member.IncomeSource;
                worksheet.Cells[row, ++index].Value = member.IncomeValue.ToString();
            }
            else
            {
                worksheet.Cells[row, ++index].Value = "لا";
                worksheet.Cells[row, ++index].Value = "";
                worksheet.Cells[row, ++index].Value = "";
                worksheet.Cells[row, ++index].Value = "";
                worksheet.Cells[row, ++index].Value = "";
                worksheet.Cells[row, ++index].Value = "";
            }
            worksheet.Cells[row, ++index].Value = member.FamilyMembersCount.ToString();

            if (!String.IsNullOrEmpty(member.Attach1)) worksheet.Cells[row, ++index].Value = Url.Action("GetFile", "Home", new { Area = "Control", path = member.Attach1 }, protocol: Request.Scheme);
            else worksheet.Cells[row, ++index].Value = "";

            if (!String.IsNullOrEmpty(member.Attach2)) worksheet.Cells[row, ++index].Value = Url.Action("GetFile", "Home", new { Area = "Control", path = member.Attach2 }, protocol: Request.Scheme);
            else worksheet.Cells[row, ++index].Value = "";
        }

        [HttpGet]
        public IActionResult ExportToExcel(int? Id, string? keyword, DateTime? startDate, DateTime? endDate,int? member, int? status, int? city, int? project)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var package = new ExcelPackage(); 
            var worksheet = package.Workbook.Worksheets.Add("Sheet1");

            Boolean IsSuper = false;
            if (HttpContext.Session.GetString("is_super_admin") == "True") IsSuper = true;
            Nullable<int> UserId = null;
            UserId = int.Parse(HttpContext.Session.GetString("id"));

            int employer = 0;
            if (HttpContext.Session.GetString("type") == "Employer")
            {
                employer = int.Parse(HttpContext.Session.GetString("id"));
            }

            //Jobs? jobDetails = null;
            //if (Id != null)
            //{
            //    jobDetails = _context.Jobs.Where(a => a.Deleted == false)
            //    .Include(a => a.Project)
            //    .FirstOrDefault();
            //}
            
            var jobsApps = _context.JobsApplications.Where(a => a.Deleted == false && a.Job.Deleted == false
            && (Id == null || a.JobId == Id)
            && (startDate == null || a.ApplyDate >= startDate)
            && (endDate == null || a.ApplyDate <= endDate)
            && (status == null || a.Status == status)
            && (city == null || a.Member.CityId == city)
            && (employer == 0 || a.Job.EmployerId == employer)
            && (member ==null || a.MemberId == member)
            && (project == null || a.Job.ProjectId == project)
            && (keyword == null || (a.Member.FirstName + " " + a.Member.FatherName + " " + a.Member.FamilyName + " " + a.Member.GrandName).Contains(keyword) || a.Job.Name.Contains(keyword) || a.Job.ArName.Contains(keyword) || a.Member.Email.Contains(keyword))
            && (employer != 0 || (IsSuper == true || employer != 0 || UserId == null) || (a.Job.Project.UsersProjects != null && a.Job.Project.UsersProjects.Any(b => b.UserId == UserId)))
            )
            .Include(a => a.Member)
            .Include(a=> a.Member.City)
            //.Include(m => m.Member.JobsApplications)
            .Include(m => m.Member.MembersEducation).ThenInclude(me => me.Country)
            .Include(m => m.Member.MembersEducation).ThenInclude(me => me.Specialize)
            .Include(m => m.Member.MembersEducation).ThenInclude(me => me.Education)
            .Include(m => m.Member.MembersTraining)
            .Include(m => m.Member.MembersExpert)
            .Include(m => m.Member.Area)
            .Include(a => a.Job)
            .Include(a => a.Job.Employer)
            .Include(a => a.Member.SocialStatus)
            .Include(a => a.Job.Project).Include(a => a.Job.Project.Donor)
            .OrderBy(a => a.Job.Id).ThenBy(a => a.ApplyDate)
            .ToList();

            //This row to export all model data
            //worksheet.Cells["A1"].LoadFromCollection(jobsApps, true);

            int index = 0;
            // Set the header row
            worksheet.Cells[1, ++index].Value = "#";
            worksheet.Cells[1, ++index].Value = "الاسم الرباعي";
            worksheet.Cells[1, ++index].Value = "رقم الهوية";
            worksheet.Cells[1, ++index].Value = "الجنس";
            worksheet.Cells[1, ++index].Value = "تاريخ الميلاد";
            worksheet.Cells[1, ++index].Value = "العمر";
            worksheet.Cells[1, ++index].Value = "الحالة الاجتماعية";
            worksheet.Cells[1, ++index].Value = "رقم هوية الاب";
            worksheet.Cells[1, ++index].Value = "رقم هوية الام";
            worksheet.Cells[1, ++index].Value = "رقم هوية الزوج/ة";
            worksheet.Cells[1, ++index].Value = "المحافظة";
            worksheet.Cells[1, ++index].Value = "المنطقة";
            worksheet.Cells[1, ++index].Value = "الحي";
            worksheet.Cells[1, ++index].Value = "الشارع";
            worksheet.Cells[1, ++index].Value = "بالقرب من";
            worksheet.Cells[1, ++index].Value = "هاتف";
            worksheet.Cells[1, ++index].Value = "رقم جوال";
            worksheet.Cells[1, ++index].Value = "رقم جوال 2";
            worksheet.Cells[1, ++index].Value = "البريد الالكتروني";
            worksheet.Cells[1, ++index].Value = "الحالة الصحية";
            worksheet.Cells[1, ++index].Value = "طبيعة المرض";
            worksheet.Cells[1, ++index].Value = "نوع الاعاقة";
            worksheet.Cells[1, ++index].Value = "حجم الاعاقة";
            worksheet.Cells[1, ++index].Value = "نوع الملكية";
            worksheet.Cells[1, ++index].Value = "طبيعة السكن";
            worksheet.Cells[1, ++index].Value = "مساحة السكن";
            worksheet.Cells[1, ++index].Value = "هل يوجد مصدر دخل";
            worksheet.Cells[1, ++index].Value = "اسم المعيل";
            worksheet.Cells[1, ++index].Value = "رقم هوية المعيل";
            worksheet.Cells[1, ++index].Value = "صفة المعيل";
            worksheet.Cells[1, ++index].Value = "مصدر الدخل";
            worksheet.Cells[1, ++index].Value = "قيمة مصدر الدخل";
            worksheet.Cells[1, ++index].Value = "عدد افراد الاسرة";
            worksheet.Cells[1, ++index].Value = "صورة الهوية";
            worksheet.Cells[1, ++index].Value = "صورة السيرة الذاتية";

            worksheet.Cells[1, ++index].Value = "نوع المستوى التعليمي";
            worksheet.Cells[1, ++index].Value = "المستوى التعليمي (الدرجة العلمية)";
            worksheet.Cells[1, ++index].Value = "الجامعة";
            worksheet.Cells[1, ++index].Value = "الدولة";
            worksheet.Cells[1, ++index].Value = "سنة التخرج";
            worksheet.Cells[1, ++index].Value = "التخصص";
            worksheet.Cells[1, ++index].Value = "المعدل التراكمي";
            worksheet.Cells[1, ++index].Value = "مرفقات التعليم";

            worksheet.Cells[1, ++index].Value = "عدد الدورات التدريبية";
            worksheet.Cells[1, ++index].Value = "مجموع ساعات الدورات التدريبية";
            worksheet.Cells[1, ++index].Value = "مرفقات الدورات التدريبية";
            worksheet.Cells[1, ++index].Value = "عدد الخبرات";
            worksheet.Cells[1, ++index].Value = "مجموع السنوات";
            worksheet.Cells[1, ++index].Value = "مرفقات الخبرات والمهن";
            //worksheet.Cells[1, ++index].Value = " هل ترغب في الحصول على دورة تدريبية؟ ( نعم – لا)";
            worksheet.Cells[1, ++index].Value = "الحالة العملية حسب بيانات الصندوق الفلسطيني للتشغيل";
            worksheet.Cells[1, ++index].Value = "تاريخ بداية اخر استفادة";
            worksheet.Cells[1, ++index].Value = "تاريخ نهاية اخر استفادة";
            worksheet.Cells[1, ++index].Value = "عدد الأشهر";

            worksheet.Cells[1, ++index].Value = "المشروع";
            worksheet.Cells[1, ++index].Value = "الوظيفة";
            worksheet.Cells[1, ++index].Value = "المشغل";
            worksheet.Cells[1, ++index].Value = "المانح";
            worksheet.Cells[1, ++index].Value = "تاريخ التقدم";
            //Set the column format
            worksheet.Column(index).Style.Numberformat.Format = "dd/MM/yyyy"; // You can use any desired date format

            worksheet.Cells[1, ++index].Value = "الحالة";


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
                worksheet.Row(1).Height += 20; // Add extra height to the row
            }

            int i = 0;
            int counter = 0;
            // Set the data rows
            foreach (JobsApplications app in jobsApps)
            {
				index = 0;
				var workStatus = false;

                FillMemberSheet(ref worksheet, app.Member, (i + 2), ref index);
                
                var educations = app.Member.MembersEducation;
                var eduCounter = i;
                int itCount = 0;
                if (educations != null && educations.Count() > 0)
                {
                    int tmp_index = index;
                    foreach (MembersEducation edu in educations)
                    {
                        if(edu.Deleted == false)
                        {//To Make Sure Education Not Deleted
                            if (itCount > 0)
                            {
                                index = 0;
                                FillMemberSheet(ref worksheet, app.Member, eduCounter + 2, ref index);
                            }
                            else index = tmp_index;
                            
                            if (edu.EducationLevelType == 1) worksheet.Cells[eduCounter + 2, (++index)].Value = "عامل";
                            else if (edu.EducationLevelType == 2) worksheet.Cells[eduCounter + 2, (++index)].Value = "مهني";
                            else if (edu.EducationLevelType == 3) worksheet.Cells[eduCounter + 2, (++index)].Value = "خريج";

                            worksheet.Cells[eduCounter + 2, (++index)].Value = edu.Education?.ArName;
                            worksheet.Cells[eduCounter + 2, (++index)].Value = edu.University;
                            worksheet.Cells[eduCounter + 2, (++index)].Value = (!string.IsNullOrEmpty(edu.CountryName) ? edu.CountryName : edu.Country?.Name);
                            worksheet.Cells[eduCounter + 2, (++index)].Value = edu.GradDate?.ToString("dd/MM/yyyy");
                            worksheet.Cells[eduCounter + 2, (++index)].Value = edu.Specialize?.ArName;
                            worksheet.Cells[eduCounter + 2, (++index)].Value = edu.Grade;

                            if (!String.IsNullOrEmpty(edu.Attachment)) worksheet.Cells[eduCounter + 2, (++index)].Value = Url.Action("GetFile", "Home", new { Area = "Control", path = edu.Attachment }, protocol: Request.Scheme);
                            else worksheet.Cells[eduCounter + 2, (++index)].Value = "";

                            eduCounter++;
                            //if (eduCounter > 1) counter++;//There more than one education 
                            itCount++;
                        }
                    }
				}
                else index = index + 8;
                //8 is the number of columns for education data


                if (itCount == 0) itCount = 1;
                var tmpIndex1 = index;
                for (var jj = 0; jj < itCount; jj++)
                {
                    index = tmpIndex1;
                    worksheet.Cells[i + 2, ++index].Value = app.Member.MembersTraining?.Count;
                    worksheet.Cells[i + 2, ++index].Value = app.Member.MembersTraining?.Sum(a => a.Hours);
                    worksheet.Cells[i + 2, ++index].Value = "";//For Training Attachments
                    worksheet.Cells[i + 2, ++index].Value = app.Member.MembersExpert?.Count;

                    var ExpYears = 0;
                    foreach (MembersExpert exp in app.Member.MembersExpert)
                    {
                        if (exp.StartDate != null && exp.EndDate != null)
                        {
                            DateTime startDate1 = exp.StartDate ?? DateTime.MinValue;
                            DateTime endDate1 = (exp.EndDate != null ? exp.EndDate : DateTime.Now.Date) ?? DateTime.Now.Date;

                            TimeSpan difference = endDate1.Subtract(startDate1);
                            int differenceInYears = (int)(difference.TotalDays / 365.25);
                            ExpYears += differenceInYears;
                        }
                    }
                    worksheet.Cells[i + 2, ++index].Value = ExpYears;
                    worksheet.Cells[i + 2, ++index].Value = ""; //For Expert Attachments
                                                                //worksheet.Cells[i + 2, ++index].Value = (app.Member.NeedTraining == true ? "نعم" : "لا");

                    var activeJobsCount = app.Member.JobsApplications.Where(a => a.Deleted == false && a.Status == 5 && (a.EndDate == null || a.EndDate >= DateTime.Now) &&
                    a.StartDate <= DateTime.Now).Count();
                    var allEmployedCount = app.Member.JobsApplications.Where(a => a.Deleted == false && a.Status == 5).Count();

                    worksheet.Cells[i + 2, ++index].Value = (activeJobsCount > 0 ? "مستفيد حالي" : (allEmployedCount > 0 ? "مستفيد سابق" : "غير مستفيد"));


                    var apps = app.Member.JobsApplications.Where(a => a.Deleted == false && a.Status == 5)
                    .OrderByDescending(a => a.ApplyDate).FirstOrDefault();

                    if (apps != null)
                    {
                        //Start and End Date may be null, so we have to use .HasValue to avoid errors
                        if (apps.StartDate != null) worksheet.Cells[i + 2, ++index].Value = apps.StartDate.HasValue ? apps.StartDate.Value.ToString("dd/MM/yyyy") : string.Empty;
                        else worksheet.Cells[i + 2, ++index].Value = "";

                        if (apps.EndDate != null) worksheet.Cells[i + 2, ++index].Value = apps.EndDate.HasValue ? apps.EndDate.Value.ToString("dd/MM/yyyy") : string.Empty;
                        else worksheet.Cells[i + 2, ++index].Value = "";


                        if (apps.StartDate != null && apps.EndDate != null)
                        {
                            worksheet.Cells[i + 2, ++index].Value = Functions.CalculateMonths((DateTime)apps.StartDate, (DateTime)apps.EndDate);
                        }
                        else worksheet.Cells[i + 2, ++index].Value = "";
                    }
                    else
                    {
                        worksheet.Cells[i + 2, ++index].Value = "";
                        worksheet.Cells[i + 2, ++index].Value = "";
                        worksheet.Cells[i + 2, ++index].Value = "";
                    }

                    worksheet.Cells[i + 2, ++index].Value = app.Job?.Project?.ArName;
                    worksheet.Cells[i + 2, ++index].Value = app.Job?.ArName;
                    worksheet.Cells[i + 2, ++index].Value = app.Job?.Employer?.ArName;
                    worksheet.Cells[i + 2, ++index].Value = app.Job?.Project?.Donor?.ArName;
                    worksheet.Cells[i + 2, ++index].Value = app.ApplyDate?.ToString("dd/MM/yyyy") ?? "";
                    

                    var statusText = "قيد الانتظار";
                    if (app.Status == 0) statusText = "قيد الانتظار";
                    else if (app.Status == 1) statusText = "تم التأكيد";
                    else if (app.Status == 2) statusText = "قيد الدراسة";
                    else if (app.Status == 3) statusText = "قائمة الانتظار";
                    else if (app.Status == 4) statusText = "مقابلة";
                    else if (app.Status == 5) statusText = "تم التعاقد/التوظيف";
                    else if (app.Status == 6) statusText = "تم الرفض";

                    worksheet.Cells[i + 2, ++index].Value = statusText;
                    i++;
                }

                

                //if (counter > i)
                //{
                //    i = counter;
                //}
                //else i++;
                //counter = i;//Set counter to main counter value(i)
            }


            // Auto-fit the columns
            worksheet.Cells.AutoFitColumns();

            // Set the content type and file name for the file result
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = "jobs.xlsx";
            if (project != null)
            {
                var projectDet = _context.Projects.Where(a => a.Deleted == false && a.Id == project).FirstOrDefault();
                if (projectDet != null)
                {
                    fileName = projectDet.Name + ".xlsx";
                }
            }

            // Return the file as a FileResult
            return File(package.GetAsByteArray(), contentType, fileName);
        }

        [HttpGet]
        [Route("Control/Jobs/AppsList")]
        public async Task<IActionResult> AppsList(int? job, string? keyword, DateTime? startDate, DateTime? endDate, int? status, int? city,int? member, int PageNumber = 1)
        {
            Boolean IsSuper = false;
            if (HttpContext.Session.GetString("is_super_admin") == "True") IsSuper = true;
            Nullable<int> UserId = null;
            UserId = int.Parse(HttpContext.Session.GetString("id"));

            int employer = 0;
            if (HttpContext.Session.GetString("type") == "Employer")
            {
                employer = int.Parse(HttpContext.Session.GetString("id"));
            }

            int PageSize = 20;
            var dataContext = _context.JobsApplications.Where(a => a.Deleted == false && a.Job.Deleted == false
            && (job == null || a.JobId == job)
            && (startDate == null || a.ApplyDate >= startDate)
            && (endDate == null || a.ApplyDate <= endDate)
            && (status == null || a.Status == status)
            && (city == null || a.Member.CityId == city)
            && (employer == 0 || a.Job.EmployerId == employer)
            && (member ==null || a.MemberId == member)
            && (keyword == null || (a.Member.FirstName + " " + a.Member.FatherName + " " + a.Member.FamilyName + " " + a.Member.GrandName).Contains(keyword) || a.Job.Name.Contains(keyword) || a.Job.ArName.Contains(keyword) || a.Member.Email.Contains(keyword))
            && (employer !=0 || (IsSuper == true || employer != 0 || UserId == null) || (a.Job.Project.UsersProjects != null && a.Job.Project.UsersProjects.Any(b => b.UserId == UserId)))
            ).AsQueryable();

            var appsList = dataContext
            .Include(a => a.Member).Include(a => a.Job)
            .Include(a=> a.Job.Employer)
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize);

            int dbPages = _context.JobsApplications.Where(a => a.Deleted == false && a.Job.Deleted == false
            && (job == null || a.JobId == job)
            && (startDate == null || a.ApplyDate >= startDate)
            && (endDate == null || a.ApplyDate <= endDate)
            && (status == null || a.Status == status)
            && (city == null || a.Member.CityId == city)
            && (employer == 0 || a.Job.EmployerId == employer)
            && (member == null || a.MemberId == member)
            && (keyword == null || (a.Member.FirstName + " " + a.Member.FatherName + " " + a.Member.FamilyName + " " + a.Member.GrandName).Contains(keyword) || a.Job.Name.Contains(keyword) || a.Job.ArName.Contains(keyword) || a.Member.Email.Contains(keyword))
            && (employer!=0 || (IsSuper == true || employer != 0 || UserId == null) || (a.Job.Project.UsersProjects != null && a.Job.Project.UsersProjects.Any(b => b.UserId == UserId)))
            )
            .Count();

            float paging = (float)dbPages / PageSize;
            double TotalPages = Math.Round(paging);

            ViewBag.PagesCount = TotalPages;
            ViewBag.DBPages = dbPages;
            ViewBag.CurrentPage = PageNumber;

            ViewBag.Cities = _context.City.OrderBy(a => a.Name).ToList();

            ViewBag.City = city;
            ViewBag.Status = status;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.Keyword = keyword;
            ViewBag.PageNumber = PageNumber;
            ViewBag.JobId = job;
            ViewBag.Member = member;
            if (member != null) ViewBag.MemberDetails = _context.Members.Where(a => a.Deleted == false && a.Id == member).FirstOrDefault();

            ViewBag.TotalApps = dataContext.Count();
            

            if (job != null)
            {
                ViewBag.JobDetails = _context.Jobs.Where(a => a.Deleted == false && a.Id == job).FirstOrDefault();
            }

            return View(await appsList.ToListAsync());
        }

        public async Task<IActionResult> changeAppStatus(int newStatus, int appId, DateTime? startDate, DateTime? endDate, string? rejectReason)
        {
            if (newStatus != null && appId != null)
            {
                var jobApp = _context.JobsApplications.Where(a => a.Deleted == false && a.Id == appId).Include(a=> a.Job).Include(a=> a.Member).Include(a=> a.JobsAppsAttachments).FirstOrDefault();
                if (jobApp != null)
                {
                    if(jobApp.Forwarded == false)
                    {
                        //Prevent changing status because the app not forwarded to employer
                        return Json(new { result = false, msg = "Not allowed to change the app status until being approved by the administration!" });
                    }
                    if (jobApp.Status != 1 && jobApp.Status != 6 && jobApp.Status != 5)//Job not approved or rejected
                    {
                        if(newStatus == 1)//for Approved
                        {
                            var statusCheck = _context.JobsStatusTracking.Where(a => a.JobAppId == jobApp.Id && (a.Status == 2 || a.Status == 4)).Count();
                            if(statusCheck == 0)
                            {
                                //Prevent change newStatus if application not passed to "Interview" or "Under Study" newStatus
                                return Json(new { result = false, msg = "Application must be interviewed before being approved!" });
                            }
                        }
                        else if(newStatus == 5)//For Contracted/Employed
                        {
                            var statusCheck = _context.JobsStatusTracking.Where(a => a.JobAppId == jobApp.Id && (a.Status==1 || a.Status==2 || a.Status==2 || a.Status ==4 )).Count();
                            if (statusCheck == 0)
                            {
                                //Prevent change status if application not passed to "Interview" or "Under Study" status
                                return Json(new { result = false, msg = "Application must be approved/Interviewed/Waiting listed/Under study before being contracted!" });
                            }
                            else if (startDate == null)
                            {
                                //Prevent change status if employment date not set
                                return Json(new { result = false, msg = "You have to set the employment starting date!" });
                            }
                        }

                        //Everything is ok, then change the status
                        jobApp.Status = newStatus;
                        if (newStatus == 5)
                        {
                            jobApp.StartDate = startDate;
                            jobApp.EndDate = endDate;

                            string filePath = "";                            
                            string fileName = "Contract Document";
                            if (HttpContext.Request.Form.Files.Count > 0)
                            {
                                filePath = ImagesUplaod.UploadFile(HttpContext, "files/file/MembersFiles/contracts/", _environment.WebRootPath, "contDocument");
                            }
                            if (!String.IsNullOrEmpty(filePath))
                            {
                                if(jobApp.JobsAppsAttachments.Count() ==0) jobApp.JobsAppsAttachments = new List<JobsAppsAttachments>();
                                jobApp.JobsAppsAttachments.Add(new JobsAppsAttachments { FileName = fileName, FilePath = filePath, JobAppId = appId, MemberId = jobApp.MemberId, AddDate = DateTime.Now });
                            }
                        }
                        else if(newStatus == 6)
                        {
                            jobApp.RejectReason = rejectReason;
                        }

                        _context.Update(jobApp);

                        var jobTrack = new JobsStatusTracking { JobAppId = appId, Status = newStatus, ApplyDate = DateTime.Now, EmployerId = jobApp.Job.EmployerId};
                        await _context.AddAsync(jobTrack);
                        await _context.SaveChangesAsync();

                        var statusText = _localizer["Pending"];

                        if (jobApp.Status == 0) statusText = _localizer["Pending"];
                        else if (jobApp.Status == 1) statusText = _localizer["Approved"];
                        else if (jobApp.Status == 2) statusText = _localizer["Under Study"];
                        else if (jobApp.Status == 3) statusText = _localizer["Waiting List"];
                        else if (jobApp.Status == 4) statusText = _localizer["Interview"];
                        else if (jobApp.Status == 5) statusText = _localizer["Contracted/Employed"];
                        else if (jobApp.Status == 6) statusText = _localizer["Rejected"];
                        //Sending notification email
                        string link = Url.Action("Details", "Jobs", new { area="", id = jobApp.Job.Id }, Url.ActionContext.HttpContext.Request.Scheme);
                        if(jobApp.Member != null && !string.IsNullOrEmpty(jobApp.Member.Email))
                        {
                            Boolean sending = _mail.SendEmail(new EmailDto { body = "عزيزنا الباحث<br><br>تم تغيير حالة طلبك على فرصة (<b>" + jobApp.Job.ArName + "</b>) الى " + statusText + "<br><b> لمعاينة الطلبات المقدمة اضغط على رابط التالي او ادخل الى حسابك على موقع البوابة ثم توجه الى شاشة (فرص تقدمت لها) <br><a href='" + link + "' >" + link + "</a><br><bR><br><br> ادارة بوابة التشغيل", subject = "تغيير حالة الطلب", to = jobApp.Member.Email });
                        }
                       
                        return Json(new { result = true, msg = _localizer["Status Changed"] });
                    }
                    else
                    {
                        return Json(new { result = false, msg = _localizer["Status Cannot Change"] });
                    }                   
                }
                else
                {
                    return Json(new { result = false, msg = _localizer["Item Not Found"] });
                }
            }
            else
            {
                return Json(new{result = false,msg = _localizer["Item Not Found"] });
            }

            //return Json(new
            //{
            //    lang = langId,
            //    location = location,
            //    menus = menuList
            //});
        }

        // GET: Control/Jobs
        public async Task<IActionResult> Index(string? keyword, int? city, int? donor, int? project,int? employer, int PageNumber = 1)
        {
            Boolean IsSuper = false;
            if (HttpContext.Session.GetString("is_super_admin") == "True") IsSuper = true;
            Nullable<int> UserId = null;
            UserId = int.Parse(HttpContext.Session.GetString("id"));

            if (HttpContext.Session.GetString("type") == "Employer")
            {
                employer = int.Parse(HttpContext.Session.GetString("id"));
            }

            int PageSize = 20;
            ViewBag.Cities = _context.City.OrderBy(a => a.Name).ToList();
            ViewBag.Donors = _context.Donors.Where(a => a.Deleted == false).OrderBy(a => a.Name).ToList();
            ViewBag.Projects = _context.Projects.Where(a => a.Deleted == false).OrderBy(a => a.Name).ToList();

            var dataContext = _context.Jobs
            .Where(a => a.Deleted == false
            && (city == null || a.CityId == city)
            && (donor == null || (a.Project != null && a.Project.DonorId == donor))
            && (project == null || a.ProjectId == project)
            && (employer == null || a.EmployerId == employer)
            && (employer != null || (IsSuper == true || UserId == null) || (a.Project.UsersProjects != null && a.Project.UsersProjects.Any(b => b.UserId == UserId)))
            && (keyword == null || a.Name.Contains(keyword) || a.ArName.Contains(keyword))
            )
            .AsQueryable();


            var jobs = dataContext.Include(j => j.City)
            .Include(j => j.Employer)
            .Include(j => j.JobsApplications)
            .Include(j=> j.Project)
            .OrderByDescending(p => p.Id)
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize);

            int dbPages = _context.Jobs
            .Where(a => a.Deleted == false
            && (city == null || a.CityId == city)
            && (donor == null || (a.Project != null && a.Project.DonorId == donor))
            && (project == null || a.ProjectId == project)
            && (employer == null || a.EmployerId == employer)
            && (employer != null || (IsSuper == true || UserId == null) || (a.Project.UsersProjects != null && a.Project.UsersProjects.Any(b => b.UserId == UserId)))
            )
            .Count();

            float paging = (float)dbPages / PageSize;
            double TotalPages = Math.Round(paging);

            if (employer != null)
            {
                ViewBag.EmployerDetails = _context.Employers.Where(a => a.Deleted == false && a.Id == employer).FirstOrDefault();
            }

            ViewBag.Keyword = keyword;
            ViewBag.City = city;
            ViewBag.Donor = donor;
            ViewBag.Project = project;
            ViewBag.Employer = employer;

            ViewBag.PagesCount = TotalPages;
            ViewBag.DBPages = dbPages;
            ViewBag.CurrentPage = PageNumber;

            ViewBag.TotalJobs = dataContext.Count();
            ViewBag.PendingJobs = dataContext.Where(a => a.StartDate > DateTime.Now.Date).Count();
            ViewBag.ActiveJobs = dataContext.Where(a => a.StartDate <= DateTime.Now.Date && a.ExpireDate >= DateTime.Now).Count();
            ViewBag.ExpiredJobs = dataContext.Where(a => a.ExpireDate < DateTime.Now.Date).Count();
            ViewBag.FullTime = dataContext.Where(a => a.WorkNature == 1).Count();
            ViewBag.PartTime = dataContext.Where(a => a.WorkNature == 0).Count();

            return View(await jobs.ToListAsync());
        }

       

        public async Task<IActionResult> AppDetails(int? Id)
        {
            JobsApplications Application = await _context.JobsApplications.Where(a => a.Id == Id)
            .Include(a=> a.Job).Include(a=> a.Job.Project).Include(a => a.Job.Project.Form).Include(a=> a.Member).FirstOrDefaultAsync();

            if (Application != null)
            {
                if(Application.Job?.Project?.FormId != null && Application.Job?.Project?.FormId!=0)
                {
                    //A form associated with the project
                    FormsEntries AppEntry = _context.FormsEntries.Where(a => a.MemberId == Application.MemberId && a.JobId == Application.JobId).FirstOrDefault();
                    if (AppEntry != null)
                    {
                        //Get fields results 
                        List<FormsEntriesFields> Fields = _context.FormsEntriesFields.Where(a => a.EntryId == AppEntry.Id).OrderBy(a => a.Id).ToList();
                        ViewBag.Fields = Fields;
                    }
                    ViewBag.Form = Application.Job.Project.Form;
                }
                ViewBag.Member = _context.Members.Where(a => a.Id == Application.MemberId).Include(a=> a.SocialStatus).Include(a=> a.City).Include(a=> a.Area).FirstOrDefault();
                ViewBag.Attachments = _context.JobsAppsAttachments.Where(a => a.Deleted == false && a.JobAppId == Id).ToList();
                return View(Application);
            }
            else
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
                return RedirectToAction("AppsList");
            }
        }

        // GET: Control/Jobs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Jobs == null)
            {
                return NotFound();
            }

            var jobs = await _context.Jobs
                .Include(j => j.City)
                .Include(j => j.Employer)
                .Include(j => j.Project)
                .Include(j => j.Fields).ThenInclude(a=> a.Field)
                .Include(j => j.JobsCities).ThenInclude(a=> a.City)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobs == null)
            {
                return NotFound();
            }
            ViewData["JobFields"] = _context.JobsFields.Where(a => a.JobId == id).Include(a => a.Field).OrderBy(a => a.Id).ToList();
            return View(jobs);
        }

        // GET: Control/Jobs/Create
        public IActionResult Create()
        {
            int employer = 0;            
            if (HttpContext.Session.GetString("type") == "Employer")
            {
                ViewBag.ShowEmployers = false;
                employer = int.Parse(HttpContext.Session.GetString("id"));
                //employer = int.Parse(HttpContext.Session.GetString("id"));
            }
            else
            {
                ViewBag.ShowEmployers = true;
            }

            ViewData["CityId"] = _context.City.OrderBy(a => a.Name).ToList();
            ViewData["VillageId"] = null;
            ViewData["SocialId"] = _context.LookupSocialStatus.OrderBy(a => a.Name).ToList();
            ViewData["CurrencyId"] = new SelectList(_context.LookupCurrencies, "Id", (currentCulture=="ar-AE" ? "ArName": "Name"));
            ViewData["EmployerId"] = _context.Employers.Where(a=> a.Deleted == false && a.Active == true).OrderBy(a => a.Name).ToList();
            ViewData["ProjectId"] = _context.Projects.Where(a=> a.Deleted==false
            && (employer ==0 || a.ProjectEmployers.Where(p=> p.Id !=0 && (employer ==0 || p.EmployerId == employer)).Count() > 0)
            )
            .OrderBy(a => a.Name).ToList();
            ViewData["Fields"] = _context.LookupJobsFields.Where(a => a.Deleted == false).OrderBy(a => a.Name).ToList();
            return View();
        }

        // POST: Control/Jobs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmployerId,Name,ArName,ProjectId,BeneficiaryCount,Description,ArDescription,CityId,Tasks,ArTasks,Conditions,ArConditions,Contacts,ArContacts,AddDate,StartDate,ExpireDate,CurrencyId,Salary,SalaryType,SelCityId,SelDistrict,SelFromAge,SelToAge,SelGender,SelSocialId,SelVillageId,SocialStatusId,WorkNature,Approved,Deleted,Type,Featured")] Jobs jobs, string[] job_fields, string[] cities)
        {
            int employer = 0;
            if (HttpContext.Session.GetString("type") == "Employer")
            {
                //If the user is employer then app must added to employer
                jobs.EmployerId = int.Parse(HttpContext.Session.GetString("id"));
                employer = int.Parse(HttpContext.Session.GetString("id"));
            }

            if (ModelState.IsValid)
            {
                
                if (job_fields != null)
                {
                    //Intialize The Fields
                    jobs.Fields = new List<JobsFields>();
                    foreach (var field in job_fields)
                    {
                        var FieldToAdd = new JobsFields { FieldId = int.Parse(field), JobId = jobs.Id };
                        //_context.JobsFields.Add(FieldToAdd);
                        if(FieldToAdd !=null) jobs.Fields.Add(FieldToAdd);
                    }
                }
                if(cities != null)
                {
                    //Intialize The Fields
                    jobs.JobsCities = new List<JobsCities>();
                    foreach (var city in cities)
                    {
                        var CityToAdd = new JobsCities { CityId = int.Parse(city), JobId = jobs.Id };
                        //_context.JobsFields.Add(FieldToAdd);
                        if (CityToAdd != null) jobs.JobsCities.Add(CityToAdd);
                    }
                }
                jobs.Name = Functions.RemoveHtml(jobs.Name);
                jobs.ArName = Functions.RemoveHtml(jobs.ArName);

                jobs.Description = Functions.FilterHtml(jobs.Description);
                jobs.ArDescription = Functions.FilterHtml(jobs.ArDescription);
                jobs.Tasks = Functions.FilterHtml(jobs.Tasks);
                jobs.ArTasks = Functions.FilterHtml(jobs.ArTasks);
                jobs.Conditions = Functions.FilterHtml(jobs.Conditions);
                jobs.ArConditions = Functions.FilterHtml(jobs.ArConditions);
                jobs.ArContacts = Functions.FilterHtml(jobs.ArContacts);
                jobs.Contacts = Functions.FilterHtml(jobs.Contacts);

                _context.Add(jobs);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CityId"] = _context.City.OrderBy(a => a.Name).ToList();
            ViewData["VillageId"] = _context.Villages.OrderBy(a => a.Name).ToList();
            ViewData["SocialId"] = _context.LookupSocialStatus.OrderBy(a => a.Name).ToList();
            ViewData["CurrencyId"] = new SelectList(_context.LookupCurrencies, "Id", (currentCulture == "ar-AE" ? "ArName" : "Name"));
            ViewData["EmployerId"] = _context.Employers.Where(a => a.Deleted == false && a.Active == true).OrderBy(a => a.Name).ToList();
            ViewData["ProjectId"] = _context.Projects.Where(a => a.Deleted == false
            && (employer == 0 || (a.ProjectEmployers.Where(a => a.EmployerId == employer).Count() > 0))
            ).OrderBy(a => a.Name)
            .ToList();
            ViewData["Fields"] = _context.LookupJobsFields.Where(a => a.Deleted == false).OrderBy(a => a.Name).ToList();
            return View(jobs);
        }

        // GET: Control/Jobs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            int employer = 0;
            if (HttpContext.Session.GetString("type") == "Employer")
            {
                //If the user is employer then app must added to employer
                employer = int.Parse(HttpContext.Session.GetString("id"));
            }

            if (id == null || _context.Jobs == null)
            {
                return NotFound();
            }

            var jobs = await _context.Jobs.FindAsync(id);
            if (jobs == null)
            {
                return NotFound();
            }

            //Only the employer who own this job allowed to do action on it
            if (HttpContext.Session.GetString("type") == "Employer" && jobs.EmployerId != int.Parse(HttpContext.Session.GetString("id")))
            {
                return RedirectToAction("AccessDenied", "Users");
            }
             

            ViewData["CityId"] = _context.City.OrderBy(a => a.Name).ToList();
            if(jobs.SelCity != null)
            {
                ViewData["VillageId"] = _context.Villages.Where(a=> a.CityId == jobs.SelCityId).OrderBy(a => a.Name).ToList();
            }
            
            ViewData["SocialId"] = _context.LookupSocialStatus.OrderBy(a => a.Name).ToList();
            ViewData["CurrencyId"] = new SelectList(_context.LookupCurrencies, "Id", (currentCulture == "ar-AE" ? "ArName" : "Name"));
            ViewData["EmployerId"] = _context.Employers.Where(a => a.Deleted == false && a.Active == true).OrderBy(a => a.Name).ToList();
            ViewData["ProjectId"] = _context.Projects.Where(a => a.Deleted == false
            && (employer==0 || (a.ProjectEmployers.Where(a=> a.EmployerId == employer).Count()>0))
            ).OrderBy(a => a.Name).ToList();
            ViewData["Fields"] = _context.LookupJobsFields.Where(a => a.Deleted == false).OrderBy(a => a.Name).ToList();
            ViewData["JobFields"] = _context.JobsFields.Where(a=> a.JobId == id).Include(a=> a.Field).OrderBy(a => a.Id).ToList();
            ViewData["JobsCities"] = _context.JobsCities.Where(a => a.JobId == id).Include(a => a.City).OrderBy(a => a.Id).ToList();
            return View(jobs);
        }

        // POST: Control/Jobs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmployerId,Name,ArName,ProjectId,BeneficiaryCount,Description,ArDescription,CityId,Tasks,ArTasks,Conditions,ArConditions,Contacts,ArContacts,AddDate,StartDate,ExpireDate,CurrencyId,Salary,SalaryType,SelCityId,SelDistrict,SelFromAge,SelToAge,SelGender,SelSocialId,SelVillageId,SocialStatusId,WorkNature,Approved,Deleted,Type,Featured")] Jobs jobs, string[] job_fields, string[] cities)
        {
            int employer = 0;
            if (HttpContext.Session.GetString("type") == "Employer")
            {
                employer = int.Parse(HttpContext.Session.GetString("id"));
            }

            if (id != jobs.Id)
            {
                return NotFound();
            }
            //Only the employer who own this job allowed to do action on it
            if (HttpContext.Session.GetString("type") == "Employer" && jobs.EmployerId != int.Parse(HttpContext.Session.GetString("id")))
            {
                return RedirectToAction("AccessDenied", "Users");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Jobs.Attach(jobs);
                    _context.Entry(jobs).State = EntityState.Modified;
                    _context.Entry(jobs).Property(a => a.AddDate).IsModified = false;
                    _context.Entry(jobs).Property(a => a.Approved).IsModified = false;
                    _context.Entry(jobs).Property(a => a.EmployerId).IsModified = false;
                    

                    //Update Fields Many to Many
                    IEnumerable<JobsFields> p_fields_list = _context.JobsFields.Where(u => u.JobId == id);
                    _context.JobsFields.RemoveRange(p_fields_list);

                    if (job_fields != null)
                    {
                        foreach (var field in job_fields)
                        {
                            var FieldToAdd = new JobsFields { FieldId = int.Parse(field), JobId = id };
                            _context.JobsFields.Add(FieldToAdd);
                        }
                    }
                    //Update Fields Many to Many
                    IEnumerable<JobsCities> p_cities_list = _context.JobsCities.Where(u => u.JobId == id);
                    _context.JobsCities.RemoveRange(p_cities_list);
                    if (cities != null)
                    {
                        //Intialize The Fields
                        jobs.JobsCities = new List<JobsCities>();
                        foreach (var city in cities)
                        {
                            var CityToAdd = new JobsCities { CityId = int.Parse(city), JobId = jobs.Id };
                            if (CityToAdd != null) jobs.JobsCities.Add(CityToAdd);
                        }
                    }
                    jobs.Name = Functions.RemoveHtml(jobs.Name);
                    jobs.ArName = Functions.RemoveHtml(jobs.ArName);

                    jobs.Description = Functions.FilterHtml(jobs.Description);
                    jobs.ArDescription = Functions.FilterHtml(jobs.ArDescription);
                    jobs.Tasks = Functions.FilterHtml(jobs.Tasks);
                    jobs.ArTasks = Functions.FilterHtml(jobs.ArTasks);
                    jobs.Conditions = Functions.FilterHtml(jobs.Conditions);
                    jobs.ArConditions = Functions.FilterHtml(jobs.ArConditions);
                    jobs.ArContacts = Functions.FilterHtml(jobs.ArContacts);
                    jobs.Contacts = Functions.FilterHtml(jobs.Contacts);

                    //_context.Update(jobs);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobsExists(jobs.Id))
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
            ViewData["CityId"] = new SelectList(_context.City, "Id", "ArName", jobs.CityId);
            ViewData["VillageId"] = _context.Villages.OrderBy(a => a.Name).ToList();
            ViewData["SocialId"] = _context.LookupSocialStatus.OrderBy(a => a.Name).ToList();
            ViewData["CurrencyId"] = new SelectList(_context.LookupCurrencies, "Id", (currentCulture == "ar-AE" ? "ArName" : "Name"));
            ViewData["EmployerId"] = new SelectList(_context.Employers, "Id", "ArName", jobs.EmployerId);
            //ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "ArName", jobs.ProjectId);
            ViewData["ProjectId"] = _context.Projects.Where(a => a.Deleted == false
            && (employer == 0 || (a.ProjectEmployers.Where(a => a.EmployerId == employer).Count() > 0))
            ).OrderBy(a => a.Name).ToList();

            ViewData["Fields"] = _context.LookupJobsFields.Where(a => a.Deleted == false).OrderBy(a => a.Name).ToList();
            return View(jobs);
        }

        // GET: Control/Jobs/AppDelete/5
        public async Task<IActionResult> AppDelete(int? id)
        {
            if (_context.JobsApplications == null)
            {
                return Problem("Entity set 'DataContext.Jobs'  is null.");
            }
            var jobs = await _context.JobsApplications.FindAsync(id);
            if (jobs != null)
            {
                //Only the employer who own this job allowed to do action on it
                if (HttpContext.Session.GetString("type") == "Employer" && (jobs.Job.EmployerId != int.Parse(HttpContext.Session.GetString("id")) || !jobs.Forwarded))
                {
                    return RedirectToAction("AccessDenied", "Users");
                }
               

                TempData["success"] = _localizer["Item Deleted"].Value;
                jobs.Deleted = true;
                _context.Update(jobs);
                //_context.Jobs.Remove(jobs);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("AppliedJobs", new { job = id });
        }

        // GET: Control/Jobs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Jobs == null)
            {
                return NotFound();
            }

            var jobs = await _context.Jobs
                .Include(j => j.City)
                .Include(j => j.Employer)
                .Include(j => j.Project)
                .Include(j => j.Fields).ThenInclude(a => a.Field)
                .Include(j => j.JobsCities).ThenInclude(a => a.City)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobs == null)
            {
                return NotFound();
            }
            //Only the employer who own this job allowed to do action on it
            if (HttpContext.Session.GetString("type") == "Employer" && jobs.EmployerId != int.Parse(HttpContext.Session.GetString("id")))
            {
                return RedirectToAction("AccessDenied", "Users");
            }

            ViewData["JobFields"] = _context.JobsFields.Where(a => a.JobId == id).Include(a => a.Field).OrderBy(a => a.Id).ToList();
            return View(jobs);
        }

        // POST: Control/Jobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Jobs == null)
            {
                return Problem("Entity set 'DataContext.Jobs'  is null.");
            }
            var jobs = await _context.Jobs.FindAsync(id);
            if (jobs != null)
            {
                //Only the employer who own this job allowed to do action on it
                if (HttpContext.Session.GetString("type") == "Employer" && jobs.EmployerId != int.Parse(HttpContext.Session.GetString("id")))
                {
                    return RedirectToAction("AccessDenied", "Users");
                }

                TempData["success"] = _localizer["Item Deleted"].Value;
                jobs.Deleted = true;
                _context.Update(jobs);
                //_context.Jobs.Remove(jobs);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Forward(int? id)
        {
            if (id == null || _context.JobsApplications == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
            }
            var jobs = await _context.JobsApplications
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobs == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
            }            

            jobs.Forwarded = true;
            _context.Update(jobs);
            await _context.SaveChangesAsync();
            TempData["success"] = _localizer["Item Forwarded"].Value;
            return RedirectToAction("AppsList");
        }

        public async Task<IActionResult> Activate(int? id)
        {
            if (id == null || _context.Jobs == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
            }
            var jobs = await _context.Jobs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobs == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
            }
            //Only the employer who own this job allowed to do action on it
            if (HttpContext.Session.GetString("type") == "Employer" && jobs.EmployerId != int.Parse(HttpContext.Session.GetString("id")))
            {
                return RedirectToAction("AccessDenied", "Users");
            }


            jobs.Approved = true;
            _context.Update(jobs);
            await _context.SaveChangesAsync();
            TempData["success"] = _localizer["Item Activated"].Value;
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Deactivate(int? id)
        {
            if (id == null || _context.Jobs == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
            }
            var jobs = await _context.Jobs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobs == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
            }
            //Only the employer who own this job allowed to do action on it
            if (HttpContext.Session.GetString("type") == "Employer" && jobs.EmployerId != int.Parse(HttpContext.Session.GetString("id")))
            {
                return RedirectToAction("AccessDenied", "Users");
            }

            jobs.Approved = false;
            _context.Update(jobs);
            await _context.SaveChangesAsync();
            TempData["success"] = _localizer["Item Activated"].Value;
            return RedirectToAction("Index");
        }

        private bool JobsExists(int id)
        {
          return (_context.Jobs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
