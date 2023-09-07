using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.VisualBasic;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PEF.Helpers;
using PEF.Migrations;
using PEF.Models;
using Rotativa.AspNetCore;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PEF.Areas.Control.Controllers
{
    [Area("Control")]
    [Authorize]
    [PEF.AuthorizedAction]
    public class MembersController : Controller
    {
        private readonly DataContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _environment;
        private readonly string currentCulture;
        private readonly IStringLocalizer<HomeController> _localizer;

        public MembersController(DataContext context, Microsoft.AspNetCore.Hosting.IWebHostEnvironment environment, IStringLocalizer<HomeController> localizer)
        {
            _context = context;
            _environment = environment;
            _localizer = localizer;
        }

        private void FillMemberSheet(ref ExcelWorksheet worksheet, Members member, int row,ref int index)
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

        public IActionResult ExportToExcel(string? keyword, int? gender, int? city, int? social, int? health, int? property, int? housenature, int? income, int? familyCountFrom, int? familyCountTo, int? fromAge, int? toAge, int? active, string? cityarea, string? interest, int? educationtype)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var package = new ExcelPackage();

            DateTime today = DateTime.Today;
            Nullable<DateTime> ageFromThreshold = null;
            Nullable<DateTime> ageToThreshold = null;
            if (fromAge != null) ageFromThreshold = today.AddYears((0 - toAge.GetValueOrDefault())); //-20
            if (toAge != null) ageToThreshold = today.AddYears((0 - fromAge.GetValueOrDefault())); //-30

            var worksheet = package.Workbook.Worksheets.Add("Sheet1");

            var members = _context.Members.Where(p => p.Deleted == false
            && (keyword == null || (p.FirstName + " " + p.FatherName + " " + p.GrandName + " " + p.FamilyName).Contains(keyword) || p.Email.Contains(keyword) || p.Mobile.Contains(keyword) || p.IdNum.Contains(keyword))
            && (gender == null || p.Gender == gender)
            && (city == null || p.CityId == city)
            && (social == null || p.SocialId == social)
            && (health == null || p.HealthStatus == health)
            && (property == null || p.PropertyType == property)
            && (housenature == null || p.HouseNature == housenature)
            && (income == null || p.IncomeExist == (income == 1 ? true : false))
            && (familyCountFrom == null || p.FamilyMembersCount >= familyCountFrom)
            && (familyCountTo == null || p.FamilyMembersCount <= familyCountTo)
            && (ageFromThreshold == null || p.BirthDate >= ageFromThreshold)
            && (ageToThreshold == null || p.BirthDate <= ageToThreshold)
            && (active == null || p.Active == (active == 1 ? true : false))
            && (cityarea == null || p.City.District.ToLower() == cityarea.ToLower())
            && (interest == null || p.Interest == interest)
            && (educationtype == null || p.MembersEducation.Where(a => a.EducationLevelType == educationtype).Any())
            )
            .Include(m => m.City)
            .Include(m => m.SocialStatus)
            .Include(m=> m.JobsApplications)
            .Include(m => m.MembersEducation).ThenInclude(me => me.Country)
            .Include(m => m.MembersEducation).ThenInclude(me => me.Specialize)
            .Include(m => m.MembersEducation).ThenInclude(me => me.Education)
            .Include(m=> m.MembersTraining)
            .Include(m=> m.MembersExpert)
            .Include(m=> m.Area)
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
            worksheet.Cells[1, ++index].Value = "عدد الخبرة";
            worksheet.Cells[1, ++index].Value = "مجموع السنوات";
            worksheet.Cells[1, ++index].Value = "مرفقات الخبرات والمهن";
            //worksheet.Cells[1, ++index].Value = " هل ترغب في الحصول على دورة تدريبية؟ ( نعم – لا)";
            worksheet.Cells[1, ++index].Value = "الحالة العملية حسب بيانات الصندوق الفلسطيني للتشغيل";

            worksheet.Cells[1, ++index].Value = "تاريخ بداية اخر استفادة";
            worksheet.Column(index).Style.Numberformat.Format = "dd/MM/yyyy"; // You can use any desired date format
            worksheet.Cells[1, ++index].Value = "تاريخ نهاية اخر استفادة";
            //Set the column format
            worksheet.Column(index).Style.Numberformat.Format = "dd/MM/yyyy"; // You can use any desired date format
            worksheet.Cells[1, ++index].Value = "عدد الأشهر";


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
                                              // Set the WrapText property of the cell to true
               range.Style.WrapText = true;
            }
            // Set the width of column A to 20 pixels
            worksheet.Column(34).Width = 150;
            worksheet.Column(35).Width = 150;
            worksheet.Column(43).Width = 150;

            int i = 0;
            int counter = 0;

            foreach (Members member in members)
            {
                index = 0;
                var workStatus = false;

                FillMemberSheet(ref worksheet,member,(i+2) , ref index);

     //           worksheet.Cells[i + 2, ++index].Value = member.Id;
     //           worksheet.Cells[i + 2, ++index].Value = member.FirstName + " " + member.FatherName + " " + member.GrandName + " " + member.FamilyName;
     //           worksheet.Cells[i + 2, ++index].Value = member.IdNum;
     //           if (member.Gender == 2) worksheet.Cells[i + 2, ++index].Value = "أنثى";
     //           else worksheet.Cells[i + 2, ++index].Value = "ذكر";
     //           worksheet.Cells[i + 2, ++index].Value = member.BirthDate.ToString("dd/MM/yyyy");
     //           worksheet.Cells[i + 2, ++index].Value = Functions.CalculateAge(member.BirthDate).ToString();
     //           worksheet.Cells[i + 2, ++index].Value = member.SocialStatus?.ArName;
                
     //           worksheet.Cells[i + 2, ++index].Value = member.FatherIdNum;
     //           worksheet.Cells[i + 2, ++index].Value = member.MotherIdNum;
     //           worksheet.Cells[i + 2, ++index].Value = member.PartnerIdNum;
     //           worksheet.Cells[i + 2, ++index].Value = member.City?.ArName;
     //           worksheet.Cells[i + 2, ++index].Value = member.Area?.ArName;
     //           worksheet.Cells[i + 2, ++index].Value = member.District;
     //           worksheet.Cells[i + 2, ++index].Value = member.Street;
     //           worksheet.Cells[i + 2, ++index].Value = member.NearTo;
     //           worksheet.Cells[i + 2, ++index].Value = member.Tel;
     //           worksheet.Cells[i + 2, ++index].Value = member.Mobile;
     //           worksheet.Cells[i + 2, ++index].Value = member.Mobile2;
     //           worksheet.Cells[i + 2, ++index].Value = member.Email;
     //           if (member.HealthStatus == 1) worksheet.Cells[i + 2, ++index].Value = "سليم";
     //           else if (member.HealthStatus == 2) worksheet.Cells[i + 2, ++index].Value = "مريض";
     //           else if (member.HealthStatus == 3) worksheet.Cells[i + 2, ++index].Value = "ذوي اعاقة";

     //           if (member.SickNature == 1) worksheet.Cells[i + 2, ++index].Value = "مريض قلب";
     //           else if (member.SickNature == 2) worksheet.Cells[i + 2, ++index].Value = "مريض ضغط";
     //           else if (member.SickNature == 3) worksheet.Cells[i + 2, ++index].Value = "مريض سكري";
     //           else if (member.SickNature == 4) worksheet.Cells[i + 2, ++index].Value = "مريض روماتزم";
     //           else worksheet.Cells[i + 2, ++index].Value = "";

     //           if (member.DisabilityType == 1) worksheet.Cells[i + 2, ++index].Value = "حركية";
     //           else if (member.DisabilityType == 2) worksheet.Cells[i + 2, ++index].Value = "سمعية";
     //           else if (member.DisabilityType == 3) worksheet.Cells[i + 2, ++index].Value = "بصرية";
     //           else if (member.DisabilityType == 4) worksheet.Cells[i + 2, ++index].Value = "عقلية";
     //           else if (member.DisabilityType == 5) worksheet.Cells[i + 2, ++index].Value = "نطقية";
     //           else worksheet.Cells[i + 2, ++index].Value = "";

     //           if (member.DisabilitySize==1) worksheet.Cells[i + 2, ++index].Value = "جزئي";
     //           else if(member.DisabilitySize==2) worksheet.Cells[i + 2, ++index].Value = "كلي";
     //           else worksheet.Cells[i + 2, ++index].Value = "";

     //           if (member.PropertyType ==0) worksheet.Cells[i + 2, ++index].Value = "أجار";
     //           else if (member.PropertyType == 1) worksheet.Cells[i + 2, ++index].Value = "ملك";
     //           else if (member.PropertyType == 2) worksheet.Cells[i + 2, ++index].Value = "استخدام";
     //           else worksheet.Cells[i + 2, ++index].Value = "";

     //           if (member.HouseNature == 0) worksheet.Cells[i + 2, ++index].Value = "شقة";
     //           else if (member.HouseNature == 1) worksheet.Cells[i + 2, ++index].Value = "منزل مستقل";
     //           else if (member.HouseNature == 2) worksheet.Cells[i + 2, ++index].Value = "أخرى";
     //           else worksheet.Cells[i + 2, ++index].Value = "";

     //           worksheet.Cells[i + 2, ++index].Value = member.HouseSize;

     //           if (member.IncomeExist)
     //           {
     //               worksheet.Cells[i + 2, ++index].Value = "نعم";
     //               worksheet.Cells[i + 2, ++index].Value = member.BreedWinnder;
     //               worksheet.Cells[i + 2, ++index].Value = member.IncomeIdNumber;
     //               //worksheet.Cells[i + 2, ++index].Value = "";
     //               if(member.BreedWinnderType == 0 ) worksheet.Cells[i + 2, ++index].Value = "نفسه";
     //               else if(member.BreedWinnderType == 1 ) worksheet.Cells[i + 2, ++index].Value = "زوج / زوجة";
     //               else if(member.BreedWinnderType == 2 ) worksheet.Cells[i + 2, ++index].Value = "أخ / أخت";
     //               else if(member.BreedWinnderType == 3 ) worksheet.Cells[i + 2, ++index].Value = "ابن";
     //               else if(member.BreedWinnderType == 4 ) worksheet.Cells[i + 2, ++index].Value = "أب";
     //               else if(member.BreedWinnderType == 5 ) worksheet.Cells[i + 2, ++index].Value = "أم";
     //               else if(member.BreedWinnderType == 6 ) worksheet.Cells[i + 2, ++index].Value = "غير ذلك";
     //               else worksheet.Cells[i + 2, ++index].Value = "";


					//worksheet.Cells[i + 2, ++index].Value = member.IncomeSource;
     //               worksheet.Cells[i + 2, ++index].Value = member.IncomeValue.ToString();
     //           }
     //           else
     //           {
     //               worksheet.Cells[i + 2, ++index].Value = "لا";
     //               worksheet.Cells[i + 2, ++index].Value = "";
     //               worksheet.Cells[i + 2, ++index].Value = "";
     //               worksheet.Cells[i + 2, ++index].Value = "";
     //               worksheet.Cells[i + 2, ++index].Value = "";
     //               worksheet.Cells[i + 2, ++index].Value = "";
     //           }
     //           worksheet.Cells[i + 2, ++index].Value = member.FamilyMembersCount.ToString();

     //           if (!String.IsNullOrEmpty(member.Attach1)) worksheet.Cells[i + 2, ++index].Value = Url.Action("GetFile", "Home", new { Area="Control", path = member.Attach1 }, protocol: Request.Scheme);
     //           else worksheet.Cells[i + 2, ++index].Value = "";

     //           if (!String.IsNullOrEmpty(member.Attach2)) worksheet.Cells[i + 2, ++index].Value = Url.Action("GetFile", "Home", new { Area = "Control", path = member.Attach2 }, protocol: Request.Scheme);
     //           else worksheet.Cells[i + 2, ++index].Value = "";

                var educations = member.MembersEducation;
                var eduCounter = i;
                int itCount = 0;
                if(educations != null && educations.Count()>0)
                {
                    int tmpIndex = index;
                   
                    foreach (MembersEducation edu in educations)
                    {
                        if(edu.Deleted == false)
                        {                            
                            if(itCount > 0)
                            {
                                index = 0;
                                FillMemberSheet(ref worksheet, member, eduCounter + 2, ref index);
                            }
                            else index = tmpIndex;

                            if (edu.EducationLevelType == 1) worksheet.Cells[eduCounter + 2, ++index].Value = "عامل";
                            else if (edu.EducationLevelType == 2) worksheet.Cells[eduCounter + 2, ++index].Value = "مهني";
                            else if (edu.EducationLevelType == 3) worksheet.Cells[eduCounter + 2, ++index].Value = "خريج";
                            else worksheet.Cells[eduCounter + 2, ++index].Value = "";

                            worksheet.Cells[eduCounter + 2, ++index].Value = edu.Education?.ArName;
                            worksheet.Cells[eduCounter + 2, ++index].Value = edu.University;
                            worksheet.Cells[eduCounter + 2, ++index].Value = (!string.IsNullOrEmpty(edu.CountryName) ? edu.CountryName: edu.Country?.Name);
                            worksheet.Cells[eduCounter + 2, ++index].Value = edu.GradDate?.ToString("dd/MM/yyyy");
                            worksheet.Cells[eduCounter + 2, ++index].Value = edu.Specialize?.ArName;
                            worksheet.Cells[eduCounter + 2, ++index].Value = edu.Grade;

                            if (!String.IsNullOrEmpty(edu.Attachment)) worksheet.Cells[eduCounter + 2, ++index].Value = Url.Action("GetFile", "Home", new { Area = "Control", path = edu.Attachment }, protocol: Request.Scheme);
                            else worksheet.Cells[eduCounter + 2, ++index].Value = "";

                            eduCounter++;
                            //if (eduCounter > 1) counter++;//There more than one education 
                            itCount++;
                        }
                    }
                }
                else
                {
                    index = index + 8;
                }


                if (itCount == 0) itCount = 1;
                var tmpIndex1 = index;
                for(var jj=0; jj < itCount; jj++)
                {
                    index = tmpIndex1;

                    ////Fill Other Data
                    //FillOtherMemberSheet(ref worksheet, member, i + 2, ref index);
                    worksheet.Cells[i + 2, ++index].Value = member.MembersTraining?.Count;
                    worksheet.Cells[i + 2, ++index].Value = member.MembersTraining?.Sum(a => a.Hours);
                    worksheet.Cells[i + 2, ++index].Value = "";//For Training Attachments

                    worksheet.Cells[i + 2, ++index].Value = member.MembersExpert?.Count;

                    var ExpYears = 0;

                    foreach (MembersExpert exp in member.MembersExpert)
                    {
                        if (exp.StartDate != null && exp.EndDate != null)
                        {
                            DateTime startDate = exp.StartDate ?? DateTime.MinValue;
                            DateTime endDate = (exp.EndDate != null ? exp.EndDate : DateTime.Now.Date) ?? DateTime.Now.Date;


                            TimeSpan difference = endDate.Subtract(startDate);
                            int differenceInYears = (int)(difference.TotalDays / 365.25);
                            ExpYears += differenceInYears;
                        }

                    }


                    worksheet.Cells[i + 2, ++index].Value = ExpYears;
                    worksheet.Cells[i + 2, ++index].Value = ""; //For Expert Attachments
                                                                //worksheet.Cells[i + 2, ++index].Value = (member.NeedTraining == true ? "نعم" : "لا");

                    var activeJobsCount = member.JobsApplications.Where(a => a.Deleted == false && a.Status == 5 && (a.EndDate == null || a.EndDate >= DateTime.Now) &&
                    a.StartDate <= DateTime.Now).Count();
                    var allEmployedCount = member.JobsApplications.Where(a => a.Deleted == false && a.Status == 5).Count();

                    worksheet.Cells[i + 2, ++index].Value = (activeJobsCount > 0 ? "مستفيد حالي" : (allEmployedCount > 0 ? "مستفيد سابق" : "غير مستفيد"));

                    var apps = member.JobsApplications.Where(a => a.Deleted == false && a.Status == 5)
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
                        else ++index;
                    }
                    else
                    {
                        index = index + 3;
                    }
                    i++;
                    //counter = i;
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
            var fileName = "jobseekers.xlsx";

            // Return the file as a FileResult
            return File(package.GetAsByteArray(), contentType, fileName);
        }
        // GET: Control/Members
        public async Task<IActionResult> Index(string? keyword, int? gender, int? city,int? social,int? health, int? property, int? housenature, int? income,int? familyCountFrom,int? familyCountTo,int? fromAge,int? toAge,int? active,string? cityarea,string? interest,int? educationtype, int PageNumber = 1)
        {
            int PageSize = 20;
            ViewBag.Cities = _context.City.OrderBy(a=> a.Name).ToList();
            ViewBag.SocialStatus = _context.LookupSocialStatus.OrderBy(a => a.Name).ToList();

            DateTime today = DateTime.Today;
            Nullable<DateTime> ageFromThreshold = null;
            Nullable<DateTime> ageToThreshold = null;
            if(fromAge!=null) ageFromThreshold = today.AddYears((0 - toAge.GetValueOrDefault())); //-20
            if(toAge!=null) ageToThreshold = today.AddYears((0 - fromAge.GetValueOrDefault())); //-30
           

            var dataContext = _context.Members.Where(p => p.Deleted == false
            && (keyword == null || (p.FirstName + " " + p.FatherName + " " + p.GrandName + " " + p.FamilyName).Contains(keyword) || p.Email.Contains(keyword) || p.Mobile.Contains(keyword) || p.IdNum.Contains(keyword))
            && (gender == null || p.Gender == gender)
            && (city == null || p.CityId == city)
            && (social == null || p.SocialId == social)
            && (health == null || p.HealthStatus == health) 
            && (property == null || p.PropertyType == property)
            && (housenature == null || p.HouseNature == housenature)
            && (income == null || p.IncomeExist == (income==1 ? true : false))
            && (familyCountFrom == null || p.FamilyMembersCount >= familyCountFrom)
            && (familyCountTo == null || p.FamilyMembersCount <= familyCountTo)       
            && (ageFromThreshold == null || p.BirthDate >= ageFromThreshold)
            && (ageToThreshold == null || p.BirthDate <= ageToThreshold)
            && (active ==null || p.Active == (active==1 ? true : false))
            && (cityarea == null || p.City.District.ToLower() == cityarea.ToLower())
            && (interest ==null || p.Interest == interest)
            && (educationtype ==null || p.MembersEducation.Where(a=> a.EducationLevelType == educationtype).Any())
            ).AsQueryable();
                
            var members = dataContext.Include(m => m.City).Include(m => m.Area).Include(m => m.SocialStatus)
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize);

            //To Get total pages for front end paginate
            int dbPages = _context.Members
            .Where(p => p.Deleted == false
            && (keyword == null || (p.FirstName + " " + p.FatherName + " " + p.GrandName + " " + p.FamilyName).Contains(keyword) || p.Email.Contains(keyword) || p.Mobile.Contains(keyword) || p.IdNum.Contains(keyword))
            && (gender == null || p.Gender == gender)
            && (city == null || p.CityId == city)
            && (social == null || p.SocialId == social)
            && (health == null || p.HealthStatus == health)
            && (property == null || p.PropertyType == property)
            && (housenature == null || p.HouseNature == housenature)
            && (income == null || p.IncomeExist == (income == 1 ? true : false))
            && (familyCountFrom == null || p.FamilyMembersCount >= familyCountFrom)
            && (familyCountTo == null || p.FamilyMembersCount <= familyCountTo)
            && (ageFromThreshold == null || p.BirthDate >= ageFromThreshold)
            && (ageToThreshold == null || p.BirthDate <= ageToThreshold)
            && (active == null || p.Active == (active == 1 ? true : false))
            && (cityarea == null || p.City.District.ToLower() == cityarea.ToLower())
            && (interest == null || p.Interest == interest)
            && (educationtype == null || p.MembersEducation.Where(a => a.EducationLevelType == educationtype).Any())
            )
            .Count();

            float paging = (float)dbPages / PageSize;
            double TotalPages = Math.Round(paging);

            var totalSeekers = dataContext.Count();
            var maleCount = dataContext.Where(a => a.Gender == 1).Count();
            var femaleCount = dataContext.Where(a => a.Gender == 2).Count();
            var completedCount = dataContext.Where(a => a.Completed == true).Count();
            var healthCount = dataContext.Where(a => a.HealthStatus == 1).Count();
            var sickCount = dataContext.Where(a => a.HealthStatus == 2).Count();
            var handiCount = dataContext.Where(a => a.HealthStatus == 3).Count();

            ViewBag.MaleCount = maleCount;
            ViewBag.TotalSeekers = totalSeekers;
            ViewBag.FemaleCount = femaleCount;
            ViewBag.ProfileCompleted = completedCount;
            ViewBag.HealthyCount = healthCount;
            ViewBag.SickCount = sickCount;
            ViewBag.HandiCount = handiCount;

            ViewBag.Gender = gender;
            ViewBag.Keyword = keyword;
            ViewBag.City = city;
            ViewBag.Social = social;
            ViewBag.Property = property;
            ViewBag.Housenature = housenature;
            ViewBag.Health = health;
            ViewBag.Income = income;

            ViewBag.FamilyCountFrom = familyCountFrom;
            ViewBag.FamilyCountTo = familyCountTo;
            ViewBag.FromAge = fromAge;
            ViewBag.ToAge = toAge;
            ViewBag.Active = active;
            ViewBag.Area = cityarea;
            ViewBag.Interest = interest;
            ViewBag.EducationType = educationtype;

            ViewBag.EducationLookup = _context.LookupEducation.Where(a => a.Deleted == false && a.Type ==0).OrderBy(a => a.Id).ThenBy(a => a.ArName).ToList();

            ViewBag.PagesCount = TotalPages;
            ViewBag.DBPages = dbPages;
            ViewBag.CurrentPage = PageNumber;

            return View(await members.ToListAsync());
        }


        public async Task<IActionResult> DownloadPDF(int? id)
        {
            if (id == null || _context.Members == null)
            {
                return NotFound();
            }

            ViewBag.MembersFamily = _context.MembersFamily.Where(a => a.MemberId == id && a.Deleted == false).OrderBy(a => a.Id).ToList();
            ViewBag.MembersEducation = _context.MembersEducation.Where(a => a.MemberId == id && a.Deleted == false)
                .Include(a => a.Education).Include(a => a.Specialize).Include(a => a.Country).OrderBy(a => a.Id).ToList();
            ViewBag.MembersExpert = _context.MembersExpert.Where(a => a.MemberId == id && a.Deleted == false).OrderBy(a => a.Id).ToList();
            ViewBag.MembersTraining = _context.MembersTraining.Where(a => a.MemberId == id && a.Deleted == false).OrderBy(a => a.Id).ToList();

            var members = await _context.Members
                .Include(m => m.City)
                .Include(m => m.SocialStatus)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (members == null)
            {
                return NotFound();
            }

            var pdf = new ViewAsPdf("DownloadPDF");
            return pdf;
        }

        // GET: Control/Members/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Members == null)
            {
                return NotFound();
            }

            ViewBag.MembersFamily = _context.MembersFamily.Where(a => a.MemberId == id && a.Deleted == false).OrderBy(a => a.Id).ToList();
            ViewBag.MembersEducation = _context.MembersEducation.Where(a => a.MemberId == id && a.Deleted == false)
                .Include(a=> a.Education).Include(a=> a.Specialize).Include(a=> a.Country).OrderBy(a => a.Id).ToList();
            ViewBag.MembersExpert = _context.MembersExpert.Where(a => a.MemberId == id && a.Deleted == false).OrderBy(a => a.Id).ToList();
            ViewBag.MembersTraining = _context.MembersTraining.Where(a => a.MemberId == id && a.Deleted == false).OrderBy(a => a.Id).ToList();

            var members = await _context.Members
                .Include(m => m.City)
                .Include(m => m.SocialStatus)
                .Include(m => m.JobsApplications)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (members == null)
            {
                return NotFound();
            }
            var activeJobsCount = members.JobsApplications.Where(a => a.Deleted == false && a.Status == 5 && (a.EndDate == null || a.EndDate >= DateTime.Now) &&
                a.StartDate <= DateTime.Now).Count();
            var allEmployedCount = members.JobsApplications.Where(a => a.Deleted == false && a.Status == 5).Count();

            ViewBag.WorkStatus =  (activeJobsCount > 0 ? "Current Beneficiary" : (allEmployedCount > 0 ? "Previous Beneficiary" : "Not Beneficiary"));

            return View(members);
        }

        // GET: Control/Members/Create
        public IActionResult Create()
        {
            ViewData["CityId"] =_context.City.ToList();
            ViewData["SocialId"] = _context.LookupSocialStatus.ToList();
            return View();
        }

        // POST: Control/Members/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,FatherName,GrandName,FamilyName,IdNum,FatherIdNum,MotherIdNum,PartnerIdNum,Gender,SocialId,BirthDate,FamilyMembersCount,CityId,AreaId,District,Street,NearTo,Mobile,Mobile2,Tel,Email,Password,ConfirmPassword,HealthStatus,DisabilityType,DisabilitySize,SickNature,HealthAtt1,PropertyType,HouseSize,HouseNature,Photo,BreedWinnder,IncomeExist,IncomeSource,IncomeValue,IncomeIdNumber,AttachTitle1,AttachTitle2,AttachTitle3,Attach1,Attach2,Attach3,BreedWinnderType,HouseType")] Members members, string[] Interest)
        {
            ViewData["CityId"] = _context.City.ToList();
            ViewData["SocialId"] = _context.LookupSocialStatus.ToList();
            if (ModelState.IsValid)
            {
                if (members.Email != null && MembersExistByEmail(members.Email,null))
                {
                    TempData["error"] = _localizer["Member Email Exisits"].Value;
                    return View(members);
                    //return RedirectToAction("Create");
                }
                if (members.Mobile != null && MembersExistByMobile(members.Mobile, null))
                {
                    TempData["error"] = _localizer["Member Mobile Exists"].Value;
                    return View(members);
                    //return RedirectToAction("Create");
                }
                if (MembersExistByIdNum(members.IdNum, null))
                {
                    TempData["error"] = _localizer["Member Id Exists"].Value;
                    return View(members);
                    //return RedirectToAction("Create");
                }

                members.Active = true;
                members.Password = Encryption.Encrypt(members.Password, true);
                members.ConfirmPassword = Encryption.Encrypt(members.ConfirmPassword, true);
                members.AddDate = DateTime.Now;
                members.Completed = false;

                members.FirstName = Functions.RemoveHtml(members.FirstName);
                members.FatherIdNum = Functions.RemoveHtml(members.FatherIdNum);
                members.GrandName = Functions.RemoveHtml(members.GrandName);
                members.FamilyName = Functions.RemoveHtml(members.FamilyName);
                members.Street = Functions.RemoveHtml(members.Street);
                members.NearTo = Functions.RemoveHtml(members.NearTo);
                members.Mobile = Functions.RemoveHtml(members.Mobile);
                members.Mobile2 = Functions.RemoveHtml(members.Mobile2);
                members.Email = Functions.RemoveHtml(members.Email);
                members.Tel = Functions.RemoveHtml(members.Tel);

                if (Interest != null)
                {
                    members.Interest = string.Join(",", Interest);
                }

                if (HttpContext.Request.Form.Files.Count > 0)
                {
                    var HealthAtt1 = ImagesUplaod.UploadFile(HttpContext, "files/file/MembersFiles/", _environment.WebRootPath, "HealthAtt1");
                    if (HealthAtt1 != "") members.HealthAtt1 = HealthAtt1;

                    var Attach1 = ImagesUplaod.UploadFile(HttpContext, "files/file/MembersFiles/", _environment.WebRootPath, "Attach1");
                    if (Attach1 !="") members.Attach1 = Attach1;

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
                TempData["success"] = _localizer["Item Added"].Value;

                _context.Add(members);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            return View(members);
        }

        // GET: Control/Members/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Members == null)
            {
                return NotFound();
            }

            var members = await _context.Members.FindAsync(id);
            if (members == null)
            {
                return NotFound();
            }
            ViewData["CityId"] = _context.City.ToList();
            ViewData["SocialId"] = _context.LookupSocialStatus.ToList();
            if (members.CityId != null) ViewData["Villages"] = _context.Villages.Where(a => a.CityId == members.CityId).ToList();
            return View(members);
        }

        // POST: Control/Members/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,FatherName,GrandName,FamilyName,IdNum,FatherIdNum,MotherIdNum,PartnerIdNum,Gender,SocialId,BirthDate,FamilyMembersCount,CityId,AreaId,District,Street,NearTo,Mobile,Mobile2,Tel,Email,Password,ConfirmPassword,HealthStatus,DisabilityType,DisabilitySize,SickNature,HealthAtt1,PropertyType,HouseSize,HouseNature,Photo,BreedWinnder,IncomeExist,IncomeSource,IncomeValue,IncomeIdNumber,AttachTitle1,AttachTitle2,AttachTitle3,Attach1,Attach2,Attach3,BreedWinnderType,Interest,HouseType")] Members members, string[] Interest)
        {
            if (id != members.Id)
            {
                return NotFound();
            }
            ViewData["CityId"] = _context.City.ToList();
            ViewData["SocialId"] = _context.LookupSocialStatus.ToList();
            if (members.CityId != null) ViewData["Villages"] = _context.Villages.Where(a => a.CityId == members.CityId).ToList();

            if (ModelState.IsValid)
            {
                if (members.Email != null && MembersExistByEmail(members.Email, id))
                {
                    TempData["error"] = _localizer["Member Email Exists"].Value;
                    return View(members);
                    //return RedirectToAction("Create");
                }
                if (members.Mobile!=null && MembersExistByMobile(members.Mobile, id))
                {
                    TempData["error"] = _localizer["Member Mobile Exists"].Value;
                    return View(members);
                    //return RedirectToAction("Create");
                }
                if (MembersExistByIdNum(members.IdNum, id))
                {
                    TempData["error"] = _localizer["Member Id Exists"].Value;
                    return View(members);
                    //return RedirectToAction("Create");
                }

                try
                {
                    _context.Members.Attach(members);
                    _context.Entry(members).State = EntityState.Modified;
                    _context.Entry(members).Property(a => a.AddDate).IsModified = false;
                    _context.Entry(members).Property(a => a.LastLogin).IsModified = false;
                    _context.Entry(members).Property(a => a.Active).IsModified = false;
                    _context.Entry(members).Property(a => a.Photo).IsModified = false;

                    members.FirstName = Functions.RemoveHtml(members.FirstName);
                    members.FatherIdNum = Functions.RemoveHtml(members.FatherIdNum);
                    members.GrandName = Functions.RemoveHtml(members.GrandName);
                    members.FamilyName = Functions.RemoveHtml(members.FamilyName);
                    members.Street = Functions.RemoveHtml(members.Street);
                    members.NearTo = Functions.RemoveHtml(members.NearTo);
                    members.Mobile = Functions.RemoveHtml(members.Mobile);
                    members.Mobile2 = Functions.RemoveHtml(members.Mobile2);
                    members.Email = Functions.RemoveHtml(members.Email);
                    members.Tel = Functions.RemoveHtml(members.Tel);

                    if (members.Password != "" && members.Password != "******")
                    {
                        members.Password = Encryption.Encrypt(members.Password, true);
                        members.Password = Encryption.Encrypt(members.ConfirmPassword, true);
                    }
                    else
                    {
                        _context.Entry(members).Property(a => a.Password).IsModified = false;
                        _context.Entry(members).Property(a => a.ConfirmPassword).IsModified = false;
                    }

                    if (Interest != null)
                    {
                        members.Interest = string.Join(",", Interest);
                    }

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
                   
                    TempData["success"] = _localizer["Item Updated"].Value;
                    //_context.Update(members);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MembersExists(members.Id))
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
            
            return View(members);
        }

        // GET: Control/Members/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Members == null)
            {
                return NotFound();
            }

            var members = await _context.Members
                .Include(m => m.City)
                .Include(m => m.SocialStatus)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (members == null)
            {
                return NotFound();
            }

            return View(members);
        }

        // POST: Control/Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Members == null)
            {
                return Problem("Entity set 'DataContext.Members'  is null.");
            }
            var members = await _context.Members.FindAsync(id);
            if (members != null)
            {
                members.Deleted = true;
                _context.Members.Update(members);
               // _context.Members.Remove(members);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MembersExists(int id)
        {
          return (_context.Members?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private bool MembersExistByEmail(string email, int? id)
        {
            return (_context.Members?.Any(e => e.Email == email && e.Deleted == false
            && (id == null || e.Id != id)))
            .GetValueOrDefault();
        }

        private bool MembersExistByMobile(string mobile, int? id)
        {
            return (_context.Members?.Any(e => e.Mobile == mobile && e.Deleted==false
            && (id == null || e.Id != id)))
            .GetValueOrDefault();
        }

        private bool MembersExistByIdNum(string idNum, int? id)
        {
            return (_context.Members?.Any(e => e.IdNum == idNum && e.Deleted == false
            && (id == null || e.Id != id)))
            .GetValueOrDefault();
        }

        public async Task<IActionResult> Activate(int? id)
        {
            if (id == null || _context.Members == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
            }
            var members = await _context.Members
                .Include(m => m.City)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (members == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
            }
                
            members.Active = true;
            _context.Update(members);
            await _context.SaveChangesAsync();
            TempData["success"] = _localizer["Item Activated"].Value;
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Deactivate(int? id)
        {
            if (id == null || _context.Members == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
            }
            var members = await _context.Members
                .Include(m => m.City)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (members == null)
            {
                TempData["error"] = _localizer["Item Not Found"].Value;
            }

            members.Active = false;
            _context.Update(members);
            await _context.SaveChangesAsync();
            TempData["success"] = _localizer["Item Deactivated"].Value;
            return RedirectToAction("Index");
        }
    }
}
