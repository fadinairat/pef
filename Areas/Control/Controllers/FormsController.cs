using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Packaging;
using PEF.Migrations;
using PEF.Models;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using Microsoft.AspNetCore.Html;
using PEF.Helpers;
using AutoMapper.Execution;
using Microsoft.Extensions.Localization;

namespace PEF.Areas.Control.Controllers
{
    [Area("Control")]
    [Authorize]
    [PEF.AuthorizedAction]
    public class FormsController : Controller
    {
        private readonly DataContext _context;
        private readonly string currentCulture;
        private readonly IStringLocalizer<HomeController> _localizer;

        public FormsController(DataContext context,IStringLocalizer<HomeController> localizer)
        {
            _localizer = localizer;
            currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            _context = context;
        }

        // GET: FormsController
        public async Task<IActionResult> Index(string? keyword, int? stype, int? slang, Boolean? sType, int PageNumber = 1)
        {
			Boolean IsSuper = false;
			if (HttpContext.Session.GetString("is_super_admin") == "True") IsSuper = true;
			Nullable<int> UserId = null;
			UserId = int.Parse(HttpContext.Session.GetString("id"));

			int PageSize = 20;
			int dbPages = 1;

			List<Forms> formsList = null;
			
			if (!IsSuper && UserId != null)
            {
				formsList = await _context.Forms.Where(a => a.Deleted == false
			    && (keyword == null || (a.Title.Contains(keyword) || a.ArTitle.Contains(keyword)))
			    && (slang == null || a.LangId == slang)
			    && (sType == null || a.IsJobForm == sType)
			    )
			    .Include(a => a.Language)
			    .Include(a => a.User)
			    .Include(a => a.FormsEntries)
			    .Include(a => a.Projects)
			    //.Skip((PageNumber - 1) * PageSize)
			    //.Take(PageSize)
			    .OrderByDescending(a => a.Id)
			    .ToListAsync();

				//List of Allowed Projects
				var projectsList = _context.UsersProjects.Where(a => a.UserId == UserId).Include(a => a.Project).ToList();

				var formsToRemove = new List<Forms>();
				foreach (var form in formsList) // Loop to all forms
                {
                    bool found = false;
                    if (form.Projects != null) 
                    {
					    foreach (var prj in form.Projects) // Loop to all projects assigned to form
					    {
                            foreach(var userPrj in projectsList) // Loop to all allowed projects to current user
                            {
                                if (userPrj.ProjectId == prj.Id)
                                {
								    found = true;
                                    break;
							    }
                            }
					    }
				    }
                    
                    foreach(UsersProjects userPrj in projectsList)
                    {
                        if(userPrj.Project?.InternalFormId!=null && userPrj.Project.InternalFormId == form.Id)
                        {
                            found = true;
                            break;
                        }
                    }
                    
                    //if Not Exist remove
                    if (!found) formsToRemove.Add(form);
                }
                foreach(var form in formsToRemove)
                {
                    formsList.Remove(form);
                }
			}
            else //Super admin user
            { 
				formsList = await _context.Forms.Where(a => a.Deleted == false
				&& (keyword == null || (a.Title.Contains(keyword) || a.ArTitle.Contains(keyword)))
				&& (slang == null || a.LangId == slang)
				&& (sType == null || a.IsJobForm == sType)
				)
				.Include(a => a.Language)
				.Include(a => a.User)
				.Include(a => a.FormsEntries)
				.Include(a => a.Projects)
				.Skip((PageNumber - 1) * PageSize)
				.Take(PageSize)
				.OrderByDescending(a => a.Id)
				.ToListAsync();

				dbPages = _context.Forms.Where(a => a.Deleted == false
			    && (keyword == null || (a.Title.Contains(keyword) || a.ArTitle.Contains(keyword)))
			    && (slang == null || a.LangId == slang)
			    && (sType == null || a.IsJobForm == sType)
			    )
			    .Count();
			}

            float paging = (float) dbPages / PageSize;
            double TotalPages = Math.Round(paging);

            ViewBag.keyword = keyword;
            ViewBag.Language = slang;
            ViewBag.PagesCount = TotalPages;
            ViewBag.CurrentPage = PageNumber;

            ViewBag.Languages = _context.Languages.Where(a => a.Deleted == 0).ToList();
            return View(formsList);
        }

        // GET: FormsController/Questions/Id
        public async Task<IActionResult> Fields(int id)
        {           
            var Fields = await _context.FormsFields.Where(a => a.Deleted == false && a.FormId == id)
                .OrderBy(a => a.Priority)
                .ThenBy(a => a.Id)
                .ToListAsync();

            var form = await _context.Forms.FindAsync(id);
           
            if (form == null)
            {
                TempData["error"] = _localizer["Not Found"];
                return RedirectToAction("Fields");
            }

            ViewBag.Form = form;

            string fields_list = "";
            if(Fields.Count() >0)
            {
                
                foreach (FormsFields item in Fields)
                {
                    string required = "false";
                    string multiple = "false";
                    string other = "false";
                    string toggle = "false";
                    string inline = "false";
                    string values = "";

                    fields_list += "{";

                    var Options = _context.FormsFieldsOptions.Where(a => a.FieldId == item.Id).ToList();
                    if (Options != null)
                    {
                        int OpCount = 0;
                        foreach(FormsFieldsOptions op in Options)
                        {
                            string selected = "false";
                            if (op.Selected) selected = "true";
                            if (OpCount != 0) values += ",";

                            values += "{";
                            values += "label: " + JsonConvert.SerializeObject(op.Label) + ",";
                            values += "value: " + JsonConvert.SerializeObject(op.Value) + ",";
                            values += "selected: " + selected + "";
                            values += "}";
                            OpCount++;
                        }
                    }
                    if (values !="") fields_list += "values :[" + values + "],";
                    //fields_list += "values: '',";
                    fields_list += "field_id: '" + item.Id + "',";
                    fields_list += "name: "+ JsonConvert.SerializeObject(item.Title) + ",";
                    fields_list += "label: "+ JsonConvert.SerializeObject(item.Label) + ",";
                    fields_list += "placeholder: "+ JsonConvert.SerializeObject(item.PlaceHolder) + ",";
                    fields_list += "helptxt: "+ JsonConvert.SerializeObject(item.PlaceHolder) + ",";
                    fields_list += "type: '"+item.Type+"',";
                    fields_list += "subtype: '"+item.SubType+"',";
                    fields_list += "description: " + JsonConvert.SerializeObject(item.Description) + ",";
                    //"name:"+$ff->fi_min_answer_number;
                    if (item.Required) required = "true";
                    else required = "false";
                    fields_list += "required: "+required+",";
                    fields_list += "value: "+ JsonConvert.SerializeObject(item.DefaultValue) + ",";
                    fields_list += "min: '"+item.MinAnsNumber+"',";
                    fields_list += "max: '"+item.MaxAnsNumber+"',";
                    fields_list += "maxlength: '"+item.MaxLength+"',";
                    fields_list += "step: '" + item.Step + "',";
                    fields_list += "rows: '"+item.Rows+"',";
                    if (item.AllowMultiple) multiple = "true";
                    else multiple = "false";
                    fields_list += "multiple: "+multiple+",";
                    if (item.EnableOther) other = "true";
                    else other = "false";
                    fields_list += "other: "+other+",";
                    if (item.Toggle) toggle = "true";
                    else toggle = "false";
                    fields_list += "fi_toggle: "+toggle+",";

                    if (item.Inline) inline = "true";
                    else inline = "false";
                    fields_list += "inline: "+inline+",";
                    fields_list += "className: '"+item.Class+"'";
                    fields_list += "},";
                }                
            }
            ViewBag.Fields = fields_list;
            return View(Fields);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Fields(int? id,string all_fields)
        {
            if(id == null)
            {
                TempData["error"] = _localizer["Not Found"];
                return RedirectToAction("Fields");

            }

            var form = await _context.Forms.FindAsync(id);
            if (form == null)
            {
                TempData["error"] = _localizer["Not Found"];
                return RedirectToAction("Fields");
            }
         
            List<FormsFields> Fields = new List<FormsFields>();
            if (all_fields != null)
            {
                TempData["success"] = _localizer["Data Saved"];
                dynamic dynJson = JsonConvert.DeserializeObject(all_fields);
                int i = 1;

                //Check for deleted items and remove from DB
                List<FormsFields> currentFields = _context.FormsFields.Where(a => a.FormId == id && a.Deleted == false).ToList();
                foreach(FormsFields formField in currentFields)
                {
                    Boolean found = false;
                    foreach(var item in dynJson)
                    {
                        if(item.name == formField.Title)
                        {
                            //Item exist in form
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        _context.FormsFields.Remove(formField);//Remove the field as it is not exist with submitted fields
                    }                   
                }

                foreach (var item in dynJson)
                {
                    //try
                    //{
                    Random rnd = new Random();
                    string title = "";
                    string label = "";
                        

                    Boolean required = false;
                    Boolean allowmultiple = false;
                    Boolean inline = false;
                    Boolean other = false;

                    if (item.required == "true") required = true;
                    if (item.multiple == "true") allowmultiple = true;
                    if (item.inline == "true") inline = true;
                    if (item.other == "true") other = true;
                    int minLength = 0;
                    int maxLength = 500;
                    int rows = 3;

                    double step = 1.0;

                    Nullable<int> minAnsNumber = null;
                    Nullable<int> maxAnsNumber = null;
                        

                    if (item.min !=null) minAnsNumber = item.min;
                    if (item.max != null) maxAnsNumber = item.max;

                    if (item.step != null) step = item.step;

                    if (item.maxlength != null) maxLength = item.maxlength;
                    if (item.rows != null) rows = item.rows;

                    string tName = item.name;
                    string tLabel = item.label;
                    if(!string.IsNullOrEmpty(tName)) title = Regex.Replace(tName, "<.*?>", ""); //To remove all html from the title and label
                    if(!string.IsNullOrEmpty(tLabel)) label = Regex.Replace(tLabel, "<.*?>", "");//To remove all html from the title and label

                    if (string.IsNullOrEmpty(title)) title = item.type+"-"+ rnd.Next(1000000,9999999)+"-"+i;
                    if (string.IsNullOrEmpty(label)) label = item.type;

                    var Field = new FormsFields
                    {
                            FormId = form.Id,
                            Title = title,
                            ArTitle = title,
                            Label = label,
                            ArLabel = label,
                            PlaceHolder = item.placeholder,
                            ArPlaceHolder = item.placeholder,
                            Type = item.type,
                            SubType = item.subtype,
                            Description = item.description,
                            MinAnsNumber = minAnsNumber,
                            MaxAnsNumber = maxAnsNumber,
                            Step = step,
                            Required = required,
                            Priority = i,
                            DefaultValue = item.value,
                            MinLength = minLength,
                            MaxLength = maxLength,
                            Rows = rows,
                            AllowMultiple = allowmultiple,
                            EnableOther = other,
                            Toggle = false,
                            Inline = inline,
                            Class = item.className,
                            Style = "",
                            AddedBy = int.Parse(HttpContext.Session.GetString("id") ?? "1"),
                            AddedTime = DateTime.Now,
                            Active = true,
                            Deleted = false
                        };

                        int op_priority = 1;
                        Field.Options = new List<FormsFieldsOptions>();
                        //To insert the options if have
                        if (item.values != null)
                        {
                            //IEnumerable<FormsFieldsOptions> op_list = _context.FormsFieldsOptions.Where(u => u.FieldId == Field.Id);
                            //_context.FormsFieldsOptions.RemoveRange(op_list);
                            foreach (var option in item.values)
                            {
                                Boolean opSelected = false;
                                if (option.selected == "true") opSelected = true;

                                var FieldOption = new FormsFieldsOptions
                                {                 
                                    FieldId = Field.Id,
                                    Label = option.label,
                                    ArLabel = option.label,
                                    Value = option.value,
                                    ArValue = option.value,
                                    Selected = opSelected,
                                    Priority = op_priority,
                                    AddedBy = int.Parse(HttpContext.Session.GetString("id") ?? "1"),
                                    AddedTime = DateTime.Now,
                                    Active = true,
                                    Deleted = false
                                };
                                //_context.FormsFieldsOptions.Add(FieldOption);
                                Field.Options.Add(FieldOption);
                                op_priority++;
                            }
                        }
                        //else
                        //{
                        //    IEnumerable<FormsFieldsOptions> op_list = _context.FormsFieldsOptions.Where(u => u.FieldId == Field.Id);
                        //    _context.FormsFieldsOptions.RemoveRange(op_list);
                        //}

                        //Check if field already exist and being update
                        if(!_context.FormsFields.Where(a=> a.Title == title).Any())
                        {
                            //New Item will be added
                            Fields.Add(Field);
                        }
                        else
                        {
                            //Edit existing Item
                            FormsFields EditField = _context.FormsFields.Where(a => a.Title == Field.Title).FirstOrDefault();
                            if (EditField != null)
                            {
                                //Edit all fields according to sumitted data
                                EditField.Label = Field.Label;
                                EditField.ArLabel = Field.ArLabel;
                                EditField.PlaceHolder = Field.PlaceHolder;
                                EditField.ArPlaceHolder = Field.ArPlaceHolder;
                                EditField.Type = Field.Type;
                                EditField.SubType = Field.SubType;
                                EditField.MinAnsNumber = Field.MinAnsNumber;
                                EditField.MaxAnsNumber = Field.MaxAnsNumber;
                                EditField.Step = Field.Step;
                                EditField.Required = Field.Required;
                                EditField.Priority = Field.Priority;
                                EditField.DefaultValue = Field.DefaultValue;
                                EditField.MinLength = Field.MinLength;
                                EditField.MaxLength = Field.MaxLength;
                                EditField.Rows = Field.Rows;
                                EditField.AllowMultiple = Field.AllowMultiple;
                                EditField.EnableOther = Field.EnableOther;
                                EditField.Toggle = Field.Toggle;
                                EditField.Inline = Field.Inline;
                                EditField.Class = Field.Class;
                                EditField.Description = Field.Description;

                                //Remove all previous options
                                _context.FormsFieldsOptions.RemoveRange(_context.FormsFieldsOptions.Where(a => a.FieldId == EditField.Id));
                            //Insert all newely submitted options

                                if (Field.Options != null)
                                {
                                    EditField.Options = new List<FormsFieldsOptions>();
                                    EditField.Options.AddRange(Field.Options);
                                }
                                _context.FormsFields.Update(EditField);
                            }
                            
                        }
                       // Console.WriteLine(Field);
                        i++;

                    //}
                    //catch(Exception ex)
                    //{
                    //    Console.Write(ex.Message);
                    //}
                    
                }
                if (Fields.Count() > 0) { //Some fields will be inserted
                    _context.FormsFields.RemoveRange(_context.FormsFields.Where(a => a.FormId == id && (a.Type=="paragraph" || a.Type == "header")));//Remove Old Fields
                    _context.FormsFields.AddRange(Fields);
                }
                await _context.SaveChangesAsync();
                TempData["success"] = _localizer["Field Inserted"];
                return RedirectToAction("Index");
            }
            else
            {
                //Remove All fields
                _context.FormsFields.RemoveRange(_context.FormsFields.Where(a => a.FormId == id));
                TempData["error"] = _localizer["Empty Fields"];
            }            

            ViewBag.Form = form;
            ViewBag.Fields = "";
            return View();
        }

        public ActionResult Preview(int id)
        {
            var form = _context.Forms.Where(a => a.Deleted == false && a.Id == id && a.IsJobForm == false).FirstOrDefault();
            if (form == null) return RedirectToAction("Index");

            ViewBag.Form = form;
            ViewBag.FormFields = _context.FormsFields.Where(a => a.Deleted == false && a.FormId == id)
            .Include(a => a.Options)
            .OrderBy(a => a.Priority).ThenBy(a => a.Id).ToList();

            return View(form);
        }

        // GET: FormsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        public IActionResult ResultDetails(int resId)
        {
            List<FormsEntriesFields> entries = _context.FormsEntriesFields.Where(a => a.Deleted == false && a.EntryId == resId).Include(a => a.Field)
                .OrderBy(a => a.Field.Priority).ThenBy(a => a.Id)
                .ToList();
            if (entries == null)
            {
                return Json(new
                {
                    result = false,
                    msg = _localizer["No Results"]
                });
            }
            else
            {
                return Json(new
                {
                    result = true,
                    answers = entries
                });
            }
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

        public IActionResult ExportToExcel(int formId)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var package = new ExcelPackage();

            var worksheet = package.Workbook.Worksheets.Add("Sheet1");

            int employer_id = 0;
            if (HttpContext.Session.GetString("type") == "Employer")
            {
                employer_id = int.Parse(HttpContext.Session.GetString("id"));
            }

            var form = _context.Forms.Where(a => a.Deleted == false && a.Id == formId)
                .Include(a=> a.FormsFields)
                .FirstOrDefault();
            if(form == null)
            {
                TempData["error"] = _localizer["Not Found"];
                return RedirectToAction("Results");
            }

            //This row to export all model data            
            int index = 1;
            // Set the header row
            worksheet.Cells[1, index].Value = "#";

            if (form.Type == 0) { //Public Form
               
            }
            else if(form.Type == 1) { //Job Application Form                
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
                worksheet.Cells[1, ++index].Value = "الحالة العملية حسب بيانات الصندوق الفلسطيني للتشغيل";
                worksheet.Cells[1, ++index].Value = "تاريخ بداية اخر استفادة";
                worksheet.Column(index).Style.Numberformat.Format = "dd/MM/yyyy"; // You can use any desired date format
                worksheet.Cells[1, ++index].Value = "تاريخ نهاية اخر استفادة";
                worksheet.Column(index).Style.Numberformat.Format = "dd/MM/yyyy"; // You can use any desired date format
                worksheet.Cells[1, ++index].Value = "عدد الأشهر";

                worksheet.Cells[1, ++index].Value = "اسم المشغل";
                worksheet.Cells[1, ++index].Value = "المشروع";
                worksheet.Cells[1, ++index].Value = "الوظيفة";
                worksheet.Cells[1, ++index].Value = "الحالة";
            }
            else if(form.Type == 2) { //Project/Employer Form
                
                //worksheet.Cells[1, ++index].Value = "الاسم بالعربي";
                //worksheet.Cells[1, ++index].Value = "الاسم بالانجليزي";
                //worksheet.Cells[1, ++index].Value = "الاختصار";
                //worksheet.Cells[1, ++index].Value = "نوع الحساب";
                //worksheet.Cells[1, ++index].Value = "نوع المؤسسة";
                //worksheet.Cells[1, ++index].Value = "القطاع";
                //worksheet.Cells[1, ++index].Value = "سنة التأسيس";
                //worksheet.Cells[1, ++index].Value = "رقم التسجيل";
                //worksheet.Cells[1, ++index].Value = "رقم المشتغل المرخص";
                //worksheet.Cells[1, ++index].Value = "المحافظة";
                //worksheet.Cells[1, ++index].Value = "المنطقة";
                //worksheet.Cells[1, ++index].Value = "العنوان";

                //worksheet.Cells[1, ++index].Value = "الهاتف";
                //worksheet.Cells[1, ++index].Value = "رقم الجوال";
                //worksheet.Cells[1, ++index].Value = "رقم الجوال 2";
                //worksheet.Cells[1, ++index].Value = "البريد الالكتروني";

                //worksheet.Cells[1, ++index].Value = "اسم  شخص التواصل";
                //worksheet.Cells[1, ++index].Value = "المسمى الوظيفي";
                //worksheet.Cells[1, ++index].Value = "رقم الجوال";
                //worksheet.Cells[1, ++index].Value = "رقم الجوال 2";
                //worksheet.Cells[1, ++index].Value = "البريد الالكتروني";
                //worksheet.Cells[1, ++index].Value = "مجال العمل بالعربي";
                //worksheet.Cells[1, ++index].Value = "بيانات اضافية بالعربي";
                //worksheet.Cells[1, ++index].Value = "عدد الموظفين الدائمين - ذكور";
                //worksheet.Cells[1, ++index].Value = "عدد الموظفين الدائمين - اناث";
                //worksheet.Cells[1, ++index].Value = "عدد الموظفين بدوام كامل - ذكور";
                //worksheet.Cells[1, ++index].Value = "عدد الموظفين بدوام كامل - اناث";
                //worksheet.Cells[1, ++index].Value = "عدد الموظفين بدوام جزئي - ذكور";
                //worksheet.Cells[1, ++index].Value = "عدد الموظفين بدوام جزئي - اناث";
                //worksheet.Cells[1, ++index].Value = "عدد أيام الدوام في الأسبوع";
                //worksheet.Cells[1, ++index].Value = "ساعات العمل اليومية";
                //worksheet.Cells[1, ++index].Value = "هل يوجد لدى المشغل / تامين إصابات عمل";
                //worksheet.Cells[1, ++index].Value = "هل يوجد لدى المشغل تامين صحي للموظفين";
                //worksheet.Cells[1, ++index].Value = "هل المؤسسة/ الشركة على استعداد لاستضافة متدربين على رأس العمل ";
                //worksheet.Cells[1, ++index].Value = "عدد الفروع للمؤسسة / الشركة";
                //worksheet.Cells[1, ++index].Value = "عناوين الفروع";
                //worksheet.Cells[1, ++index].Value = "إجراءات السلامة والصحة المهنية بالعربية";
                //worksheet.Cells[1, ++index].Value = "تقارير مالية";
                //worksheet.Cells[1, ++index].Value = "تقارير ادارية";
                //worksheet.Cells[1, ++index].Value = "شهادة ترخيص المؤسسة / الشركة";
                //worksheet.Cells[1, ++index].Value = "السجل التجاري";
                //worksheet.Cells[1, ++index].Value = "مرفقات اخرى";
            }
            
            
            //if (form.IsJobForm)
            //{
                
            //}

            //Public form fields
            worksheet.Cells[1, ++index].Value = "النموذج";
            worksheet.Cells[1, ++index].Value = "وقت التقدم";

            
            if (form.FormsFields != null)
            {
                List<FormsFields> formFields = form.FormsFields.OrderBy(a => a.Priority).ThenBy(a => a.Id).ToList();
                foreach (FormsFields field in formFields)
                {
                    if (field.Type != "header" && field.Type != "button" && field.Type != "hidden" && field.Type != "paragraph" && field.Type != "")
                    {
                        var label = Functions.RemoveHtml(field.ArLabel);
                        if (field.ArLabel != null) worksheet.Cells[1, ++index].Value = label;
                        //Console.WriteLine(Functions.RemoveHtml(field.ArLabel));
                    }
                }
            }

            // Format header row
            using (var range = worksheet.Cells[1, 1, 1, index])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightSlateGray);
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                //range.Style.WrapText = true; // Wrap text if needed
                range.Style.Font.Size = 12; // Set font size                
                worksheet.Row(1).Height += 20; // Add extra height to the row
            }

        
            int counter = 0;
            // Set the data rows
            if(form.Type == 0 || form.Type == 2) { //Public Form
                var formAnswers = _context.FormsEntries.Where(a => a.Deleted == false && a.FormId == formId)
                .Include(a => a.Form)
                .Include(a => a.Entries).ThenInclude(a => a.Field)
                .OrderBy(a => a.AddedTime).ThenBy(a => a.Id)
                .ToList();
                foreach (FormsEntries ans in formAnswers)
                {
                    index = 1;
                    worksheet.Cells[counter + 2, index].Value = ans.Id;                    
                    worksheet.Cells[counter + 2, ++index].Value = ans.Form?.ArTitle;
                    worksheet.Cells[counter + 2, ++index].Value = ans.AddedTime.ToString("dd/MM/yyyy HH:mm");

                    if (ans.Entries != null)
                    {
                        List<FormsEntriesFields> entries = ans.Entries.OrderBy(a => a.Field.Priority).ThenBy(a => a.Field.Id).ToList();
                        foreach (FormsEntriesFields entr in entries)
                        {
                            if (entr.Type == "file")
                            {
                                if (!String.IsNullOrEmpty(entr.Answer)) worksheet.Cells[counter + 2, ++index].Value = Url.Action("GetFile", "Home", new { Area = "Control", path = entr.Answer }, protocol: Request.Scheme);
                                else worksheet.Cells[counter + 2, ++index].Value = "";
                            }
                            else if (entr.Answer != null) worksheet.Cells[counter + 2, ++index].Value = Regex.Replace(entr.Answer, "<.*?>", "");
                            else worksheet.Cells[counter + 2, ++index].Value = "";
                        }
                        counter++;
                    }                    
                }
            }
            else if(form.Type == 1)//Members/Jobs Applications
            {
                _context.Database.SetCommandTimeout(300);
                var formAnswers = _context.FormsEntries.Where(a => a.Deleted == false && a.FormId == formId)
                .Include(a => a.Form)
                .Include(a => a.Job).ThenInclude(a => a.Project)
                .Include(a => a.Job).ThenInclude(a => a.Employer)
                .Include(a => a.Member)
                .Include(m => m.Member.JobsApplications)
                .Include(m => m.Member.MembersEducation).ThenInclude(me => me.Country)
                .Include(m => m.Member.MembersEducation).ThenInclude(me => me.Specialize)
                .Include(m => m.Member.MembersEducation).ThenInclude(me => me.Education)
                .Include(m => m.Member.MembersTraining)
                .Include(m => m.Member.MembersExpert)
                .Include(m => m.Member.Area)
                .Include(a => a.Member.SocialStatus)
                .Include(a => a.Entries).ThenInclude(a => a.Field)
                .OrderBy(a => a.AddedTime).ThenBy(a => a.Id)
                //.Skip(0)
                //.Take(1000)
                .ToList();
                //Console.WriteLine("INSIDE THE FORM BODY");
                foreach (FormsEntries ans in formAnswers)
                {
                    index = 0;
                    if (ans.Member != null)//To Make sure that this form applied by member not a public application
                    {
                        FillMemberSheet(ref worksheet, ans.Member, (counter + 2), ref index);
                        //worksheet.Cells[counter + 2, index].Value = ans.Id;

                        //Commented Entry for member details
                        { 
                        //worksheet.Cells[counter + 2, ++index].Value = ans.Member.Id;
                        //worksheet.Cells[counter + 2, ++index].Value = ans.Member.FirstName + " " + ans.Member.FatherName + " " + ans.Member.GrandName + " " + ans.Member.FamilyName;
                        //worksheet.Cells[counter + 2, ++index].Value = ans.Member.IdNum;
                        //if (ans.Member.Gender == 2) worksheet.Cells[counter + 2, ++index].Value = "أنثى";
                        //else worksheet.Cells[counter + 2, ++index].Value = "ذكر";
                        //worksheet.Cells[counter + 2, ++index].Value = ans.Member.BirthDate.ToString("dd/MM/yyyy");
                        //worksheet.Cells[counter + 2, ++index].Value = Functions.CalculateAge(ans.Member.BirthDate).ToString();
                        //worksheet.Cells[counter + 2, ++index].Value = ans.Member.SocialStatus?.ArName;

                        //worksheet.Cells[counter + 2, ++index].Value = ans.Member.FatherIdNum;
                        //worksheet.Cells[counter + 2, ++index].Value = ans.Member.MotherIdNum;
                        //worksheet.Cells[counter + 2, ++index].Value = ans.Member.PartnerIdNum;
                        //worksheet.Cells[counter + 2, ++index].Value = ans.Member.City?.ArName;
                        //worksheet.Cells[counter + 2, ++index].Value = ans.Member.Area?.ArName;
                        //worksheet.Cells[counter + 2, ++index].Value = ans.Member.District;
                        //worksheet.Cells[counter + 2, ++index].Value = ans.Member.Street;
                        //worksheet.Cells[counter + 2, ++index].Value = ans.Member.NearTo;
                        //worksheet.Cells[counter + 2, ++index].Value = ans.Member.Tel;
                        //worksheet.Cells[counter + 2, ++index].Value = ans.Member.Mobile;
                        //worksheet.Cells[counter + 2, ++index].Value = ans.Member.Mobile2;
                        //worksheet.Cells[counter + 2, ++index].Value = ans.Member.Email;

                        //if (ans.Member.HealthStatus == 1) worksheet.Cells[counter + 2, ++index].Value = "سليم";
                        //else if (ans.Member.HealthStatus == 2) worksheet.Cells[counter + 2, ++index].Value = "مريض";
                        //else if (ans.Member.HealthStatus == 3) worksheet.Cells[counter + 2, ++index].Value = "ذوي اعاقة";
                        //else ++index;


                        //if (ans.Member.SickNature == 1) worksheet.Cells[counter + 2, ++index].Value = "مريض قلب";
                        //else if (ans.Member.SickNature == 2) worksheet.Cells[counter + 2, ++index].Value = "مريض ضغط";
                        //else if (ans.Member.SickNature == 3) worksheet.Cells[counter + 2, ++index].Value = "مريض سكري";
                        //else if (ans.Member.SickNature == 4) worksheet.Cells[counter + 2, ++index].Value = "مريض روماتزم";
                        //else ++index;

                        //if (ans.Member.DisabilityType == 1) worksheet.Cells[counter + 2, ++index].Value = "حركية";
                        //else if (ans.Member.DisabilityType == 2) worksheet.Cells[counter + 2, ++index].Value = "سمعية";
                        //else if (ans.Member.DisabilityType == 3) worksheet.Cells[counter + 2, ++index].Value = "بصرية";
                        //else if (ans.Member.DisabilityType == 4) worksheet.Cells[counter + 2, ++index].Value = "عقلية";
                        //else if (ans.Member.DisabilityType == 5) worksheet.Cells[counter + 2, ++index].Value = "نطقية";
                        //else ++index;

                        //if (ans.Member.DisabilitySize == 1) worksheet.Cells[counter + 2, ++index].Value = "جزئي";
                        //else if (ans.Member.DisabilitySize == 2) worksheet.Cells[counter + 2, ++index].Value = "كلي";
                        //else ++index;

                        //if (ans.Member.PropertyType == 0) worksheet.Cells[counter + 2, ++index].Value = "أجار";
                        //else if (ans.Member.PropertyType == 1) worksheet.Cells[counter + 2, ++index].Value = "ملك";
                        //else if (ans.Member.PropertyType == 2) worksheet.Cells[counter + 2, ++index].Value = "استخدام";
                        //else ++index;

                        //if (ans.Member.HouseNature == 0) worksheet.Cells[counter + 2, ++index].Value = "شقة";
                        //else if (ans.Member.HouseNature == 1) worksheet.Cells[counter + 2, ++index].Value = "منزل مستقل";
                        //else if (ans.Member.HouseNature == 2) worksheet.Cells[counter + 2, ++index].Value = "أخرى";
                        //else ++index;

                        //worksheet.Cells[counter + 2, ++index].Value = ans.Member.HouseSize;

                        //if (ans.Member.IncomeExist)
                        //{
                        //    worksheet.Cells[counter + 2, ++index].Value = "نعم";
                        //    worksheet.Cells[counter + 2, ++index].Value = ans.Member.BreedWinnder;
                        //    worksheet.Cells[counter + 2, ++index].Value = ans.Member.IncomeIdNumber;

                        //    if (ans.Member.BreedWinnderType == 0) worksheet.Cells[counter + 2, ++index].Value = "نفسه";
                        //    else if (ans.Member.BreedWinnderType == 1) worksheet.Cells[counter + 2, ++index].Value = "زوج / زوجة";
                        //    else if (ans.Member.BreedWinnderType == 2) worksheet.Cells[counter + 2, ++index].Value = "أخ / أخت";
                        //    else if (ans.Member.BreedWinnderType == 3) worksheet.Cells[counter + 2, ++index].Value = "ابن";
                        //    else if (ans.Member.BreedWinnderType == 4) worksheet.Cells[counter + 2, ++index].Value = "أب";
                        //    else if (ans.Member.BreedWinnderType == 5) worksheet.Cells[counter + 2, ++index].Value = "أم";
                        //    else if (ans.Member.BreedWinnderType == 6) worksheet.Cells[counter + 2, ++index].Value = "غير ذلك";
                        //    else worksheet.Cells[counter + 2, ++index].Value = "";

                        //    worksheet.Cells[counter + 2, ++index].Value = ans.Member.IncomeSource;
                        //    worksheet.Cells[counter + 2, ++index].Value = ans.Member.IncomeValue.ToString();
                        //}
                        //else
                        //{
                        //    worksheet.Cells[counter + 2, ++index].Value = "لا";
                        //    worksheet.Cells[counter + 2, ++index].Value = "";
                        //    worksheet.Cells[counter + 2, ++index].Value = "";
                        //    worksheet.Cells[counter + 2, ++index].Value = "";
                        //    worksheet.Cells[counter + 2, ++index].Value = "";
                        //    worksheet.Cells[counter + 2, ++index].Value = "";
                        //}
                        //worksheet.Cells[counter + 2, ++index].Value = ans.Member.FamilyMembersCount.ToString();

                        //if (!String.IsNullOrEmpty(ans.Member.Attach1)) worksheet.Cells[counter + 2, ++index].Value = Url.Action("GetFile", "Home", new { Area = "Control", path = ans.Member.Attach1 }, protocol: Request.Scheme);
                        //else worksheet.Cells[counter + 2, ++index].Value = "";

                        //if (!String.IsNullOrEmpty(ans.Member.Attach2)) worksheet.Cells[counter + 2, ++index].Value = Url.Action("GetFile", "Home", new { Area = "Control", path = ans.Member.Attach2 }, protocol: Request.Scheme);
                        //else worksheet.Cells[counter + 2, ++index].Value = "";
                        }

                        var educations = ans.Member.MembersEducation;
                        var eduCounter = counter;
                        int tmp = 0;
                        if (educations != null)
                        {
                            int tmpIndex = index;
                            foreach (MembersEducation edu in educations)
                            {
                                if(edu.Deleted == false)
                                {
                                    if (tmp > 0)
                                    {
                                        index = 0;
                                        FillMemberSheet(ref worksheet, ans.Member, eduCounter + 2, ref index);
                                    }
                                    else index = tmpIndex;

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
                                    //if (tmp > 1) counter++;//There more than one education 
                                    tmp++;
                                }
                            }
                        }
                        else index = index + 8;
                        //8 is the number of columns for education data

                        if (tmp == 0) tmp = 1;
                        var tmpIndex1 = index;
                        for (var jj = 0; jj < tmp; jj++)
                        {
                            index = tmpIndex1;
                            worksheet.Cells[counter + 2, ++index].Value = ans.Member.MembersTraining?.Count;
                            worksheet.Cells[counter + 2, ++index].Value = ans.Member.MembersTraining?.Sum(a => a.Hours);
                            worksheet.Cells[counter + 2, ++index].Value = "";//For Training Attachments
                            worksheet.Cells[counter + 2, ++index].Value = ans.Member.MembersExpert?.Count;

                            var ExpYears = 0;
                            foreach (MembersExpert exp in ans.Member.MembersExpert)
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
                            worksheet.Cells[counter + 2, ++index].Value = ExpYears;
                            worksheet.Cells[counter + 2, ++index].Value = ""; //For Expert Attachments

                            var activeJobsCount = ans.Member?.JobsApplications?.Where(a => a.Deleted == false && a.Status == 5 && (a.EndDate == null || a.EndDate >= DateTime.Now) &&
                        a.StartDate <= DateTime.Now).Count();
                            var allEmployedCount = ans.Member?.JobsApplications?.Where(a => a.Deleted == false && a.Status == 5).Count();

                            worksheet.Cells[counter + 2, ++index].Value = (activeJobsCount > 0 ? "مستفيد حالي" : (allEmployedCount > 0 ? "مستفيد سابق" : "غير مستفيد"));

                            var apps = ans.Member?.JobsApplications?.Where(a => a.Deleted == false && a.Status == 5)
                            .OrderByDescending(a => a.ApplyDate).FirstOrDefault();

                            if (apps != null)
                            {
                                //Start and End Date may be null, so we have to use .HasValue to avoid errors
                                if (apps.StartDate != null) worksheet.Cells[counter + 2, ++index].Value = apps.StartDate.HasValue ? apps.StartDate.Value.ToString("dd/MM/yyyy") : string.Empty;
                                else worksheet.Cells[counter + 2, ++index].Value = "";

                                if (apps.EndDate != null) worksheet.Cells[counter + 2, ++index].Value = apps.EndDate.HasValue ? apps.EndDate.Value.ToString("dd/MM/yyyy") : string.Empty;
                                else worksheet.Cells[counter + 2, ++index].Value = "";


                                if (apps.StartDate != null && apps.EndDate != null)
                                {
                                    worksheet.Cells[counter + 2, ++index].Value = Functions.CalculateMonths((DateTime)apps.StartDate, (DateTime)apps.EndDate);
                                }
                                else worksheet.Cells[counter + 2, ++index].Value = "";
                            }
                            else
                            {
                                worksheet.Cells[counter + 2, ++index].Value = "";
                                worksheet.Cells[counter + 2, ++index].Value = "";
                                worksheet.Cells[counter + 2, ++index].Value = "";
                            }

                            //Project and Employer Details
                            worksheet.Cells[counter + 2, ++index].Value = ans.Job?.Employer?.ArName;
                            worksheet.Cells[counter + 2, ++index].Value = ans.Job?.Project?.ArName;
                            worksheet.Cells[counter + 2, ++index].Value = ans.Job?.ArName;
                            var currentApp = ans.Member?.JobsApplications?.Where(a => a.Deleted == false && a.JobId == ans.JobId && a.MemberId == ans.MemberId).FirstOrDefault();
                            if (currentApp != null)
                            {
                                var statusText = "قيد الانتظار";
                                if (currentApp.Status == 0) statusText = "قيد الانتظار";
                                else if (currentApp.Status == 1) statusText = "تم التأكيد";
                                else if (currentApp.Status == 2) statusText = "قيد الدراسة";
                                else if (currentApp.Status == 3) statusText = "قائمة الانتظار";
                                else if (currentApp.Status == 4) statusText = "مقابلة";
                                else if (currentApp.Status == 5) statusText = "تم التعاقد/التوظيف";
                                else if (currentApp.Status == 6) statusText = "تم الرفض";

                                worksheet.Cells[counter + 2, ++index].Value = statusText;
                            }
                            else worksheet.Cells[counter + 2, ++index].Value = "";

                            worksheet.Cells[counter + 2, ++index].Value = ans.Form?.ArTitle;
                            worksheet.Cells[counter + 2, ++index].Value = ans.AddedTime.ToString("dd/MM/yyyy HH:mm");

                            if (ans.Entries != null)
                            {
                                List<FormsEntriesFields> entries = ans.Entries.OrderBy(a => a.Field.Priority).ThenBy(a => a.Field.Id).ToList();
                                foreach (FormsEntriesFields entr in entries)
                                {
                                    if (entr.Type == "file")
                                    {
                                        if (!String.IsNullOrEmpty(entr.Answer)) worksheet.Cells[counter + 2, ++index].Value = Url.Action("GetFile", "Home", new { Area = "Control", path = entr.Answer }, protocol: Request.Scheme);
                                        else worksheet.Cells[counter + 2, ++index].Value = "";
                                    }
                                    else if (entr.Answer != null) worksheet.Cells[counter + 2, ++index].Value = Regex.Replace(entr.Answer, "<.*?>", "");
                                    else worksheet.Cells[counter + 2, ++index].Value = "";
                                }
                                //counter++;
                            }
                            //if (tmp > 1) counter = counter + (tmp - 1);
                            //else counter++;
                            counter++;
                        }
                        
                    }
                }
            }
            else if(form.Type == 2) //Employer/Projects Forms
            {
                var formAnswers = _context.FormsEntries.Where(a => a.Deleted == false && a.FormId == formId)
                //.Include(a => a.Form)
                //.Include(a => a.Employer)
                //.Include(e => e.Employer.EmployerSector)
                //.Include(a => a.Entries).ThenInclude(a => a.Field)
                .Select(e => new
                {
                    Entry = e,
                    Form = e.Form,
                    //Employer = e.Employer,                    
                    //City = e.Employer.City != null ? e.Employer.City : null,
                    //Area = e.Employer.Area != null ? e.Employer.Area : null,
                    //WorkSector = e.Employer.WorkSector != null ? e.Employer.WorkSector : null,
                    //JobsCount = e.Employer.Jobs.Where(a => a.Deleted == false).Count(),
                    //AppsCount = e.Employer.Jobs.SelectMany(a => a.JobsApplications.Where(j => j.Deleted == false && j.Job.Deleted == false)).Count(),
                    //EmployedCount = e.Employer.Jobs.SelectMany(a => a.JobsApplications.Where(j => j.Deleted == false && j.Job.Deleted == false && (j.Status == 1 || j.Status == 5))).Count(),
                })
                .OrderBy(a => a.Entry.AddedTime).ThenBy(a => a.Entry.Id)
                .ToList();

                int i = 2;
                foreach (var ans in formAnswers)
                {
                    index = 1;
                    worksheet.Cells[i, index].Value = ans.Entry.Id;
                    //
                    /*Employers employer = ans.Employer;
                    if (employer != null)
                    {
                        /////////// Employer Main Details //////////////////
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
                        worksheet.Cells[i, ++index].Value = ans.Employer?.City?.ArName;
                        worksheet.Cells[i, ++index].Value = ans.Employer?.Area?.ArName;
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
                    }
                    else index = index + 43;
                    */


                    ///////////////// Main Form Details ////////////////
                    worksheet.Cells[counter + 2, ++index].Value = ans.Entry.Form?.ArTitle;
                    worksheet.Cells[counter + 2, ++index].Value = ans.Entry.AddedTime.ToString("dd/MM/yyyy HH:mm");

                    if (ans.Entry.Entries != null)
                    {
                        List<FormsEntriesFields> entries = ans.Entry.Entries.OrderBy(a => a.Field.Priority).ThenBy(a => a.Field.Id).ToList();
                        foreach (FormsEntriesFields entr in entries)
                        {
                            if (entr.Type == "file")
                            {
                                if (!String.IsNullOrEmpty(entr.Answer)) worksheet.Cells[counter + 2, ++index].Value = Url.Action("GetFile", "Home", new { Area = "Control", path = entr.Answer }, protocol: Request.Scheme);
                                else worksheet.Cells[counter + 2, ++index].Value = "";
                            }
                            else if (entr.Answer != null) worksheet.Cells[counter + 2, ++index].Value = Regex.Replace(entr.Answer, "<.*?>", "");
                            else worksheet.Cells[counter + 2, ++index].Value = "";
                        }
                    }
                    counter++;
                    i++;
                }
            }



        // Auto-fit the columns
        worksheet.Cells.AutoFitColumns();

        // Set the content type and file name for the file result
        var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        var fileName = form.ArTitle+".xlsx";

        // Return the file as a FileResult
        return File(package.GetAsByteArray(), contentType, fileName);
}

public ActionResult Results(int? formId,Boolean? type,int PageNumber = 1)
{           
int PageSize = 20;

var apps = _context.FormsEntries.Where(a => a.Deleted == false 
&& (type==null || a.Form.IsJobForm == type)
&& (formId==null || a.FormId == formId)
)               
.Include(a=> a.Form)
.Include(a=> a.Member)
.Include(a=> a.Employer)
.OrderByDescending(a => a.Id)
.Skip((PageNumber - 1) * PageSize)
.Take(PageSize)
.ToList();

var dbPages = _context.FormsEntries.Where(a => a.Deleted == false && a.FormId == formId
&& (type == null || a.Form.IsJobForm == type)
&& (formId == null || a.FormId == formId)
).Count();


float paging = (float) dbPages / PageSize;
double TotalPages = Math.Round(paging);
ViewBag.PagesCount = TotalPages;
ViewBag.DBPages = dbPages;
if (formId != null)
{
Forms form = _context.Forms.Where(a => a.Id == formId).FirstOrDefault();
ViewBag.Form = form;
ViewBag.FormTitle = form.Title;
}

ViewBag.CurrentPage = PageNumber;
ViewBag.TotalApps = apps.Count();

ViewBag.Type = type;
ViewBag.FormId = formId;
ViewBag.Forms = _context.Forms.Where(a => a.Deleted == false).OrderBy(a => a.Title).ToList();

return View(apps);
}

public ActionResult EntryDetails(int entryId)
{
return View();
}

// GET: FormsController/Create
public ActionResult Create()
{
ViewBag.Languages = _context.Languages.Where(a => a.Deleted == 0).ToList();
return View();
}

// POST: FormsController/Create
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create([Bind("Id,Title,ArTitle,Description,ArDescription,Direction,LangId,SubmitLabel,ArSubmitLabel,AddedBy,AddedTime,Type,IsJobForm,IsPublic")] Forms form)
{
form.IsJobForm = true;
form.IsPublic = false;

if (ModelState.IsValid)
{
form.AddedBy = int.Parse(HttpContext.Session.GetString("id") ?? "1");
form.AddedTime = DateTime.Now;

form.IsJobForm = false;
form.IsPublic = false;

//Filter Against Vulnerable content
form.Title = Functions.RemoveHtml(form.Title);
form.ArTitle = Functions.RemoveHtml(form.ArTitle);

form.Description = Functions.FilterHtml(form.Description);
form.ArDescription = Functions.FilterHtml(form.ArDescription);

await _context.AddAsync(form);
await _context.SaveChangesAsync();

TempData["success"] = "Form added successfully...";                
return RedirectToAction("Create");
}

TempData["error"] = "Cannot add form...";            
ViewBag.Languages = _context.Languages.Where(a => a.Deleted == 0).ToList();

return View(form);
}

// GET: Control/Forms/Edit/5
public async Task<IActionResult> Edit(int? id)
{
if (id == null || _context.Forms == null)
{
TempData["error"] = "Form not found...";
return RedirectToAction("Index");
}

var form = await _context.Forms.FindAsync(id);

if (form == null)
{
TempData["error"] = "Form not found...";
return RedirectToAction("Index");
}
ViewBag.Languages = _context.Languages.Where(a => a.Deleted == 0).ToList();
return View(form);
}

// POST: Control/Menus/Edit/5
// To protect from overposting attacks, enable the specific properties you want to bind to.
// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ArTitle,Description,ArDescription,Direction,LangId,SubmitLabel,ArSubmitLabel,AddedBy,AddedTime,IsJobForm,IsPublic,Type")] Forms form)
{

if (id != form.Id)
{
TempData["error"] = "Form not found...";
return RedirectToAction("Index");
}



if (ModelState.IsValid)
{
try
{
_context.Forms.Attach(form);
_context.Entry(form).State = EntityState.Modified;
_context.Entry(form).Property(p => p.AddedBy).IsModified = false;
_context.Entry(form).Property(p => p.AddedTime).IsModified = false;
_context.Entry(form).Property(p => p.IsJobForm).IsModified = false;
_context.Entry(form).Property(p => p.IsPublic).IsModified = false;
//_context.Entry(form).Property(p => p.IsJobForm).IsModified = false;
//_context.Entry(form).Property(p => p.IsPublic).IsModified = false;

//Filter Against Vulnerable content
form.Title = Functions.RemoveHtml(form.Title);
form.ArTitle = Functions.RemoveHtml(form.ArTitle);

form.Description = Functions.FilterHtml(form.Description);
form.ArDescription = Functions.FilterHtml(form.ArDescription);

//_context.Update(form);
await _context.SaveChangesAsync();
TempData["success"] = "Form edited successfully...";
}
catch (DbUpdateConcurrencyException)
{
TempData["error"] = "Form not found...";
return RedirectToAction("Index");
}
return RedirectToAction(nameof(Index));
}
ViewBag.Languages = _context.Languages.Where(a => a.Deleted == 0).ToList();
return View(form);
}

// GET: FormsController/Delete/5
public ActionResult Delete(int id)
{
return View();
}

// POST: FormsController/Delete/5
[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult Delete(int id, IFormCollection collection)
{
try
{
return RedirectToAction(nameof(Index));
}
catch
{
return View();
}
}
}
}
