using Microsoft.AspNetCore.Mvc;
using PEF.Models;
using PEF.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;


namespace PEF.Controllers
{
    
    public class PagesController : Controller
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;

        public PagesController(DataContext context, IConfiguration config, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {
            _context = context;           
            _config = config;
            _environment = environment;
        }

        // Get: Pages/Details/ID
        public async Task<IActionResult> Details(int? id, string title)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var pageDetails = _context.Pages.Where(a => a.PageId == id && a.Deleted == false && a.Active == true && a.Publish == true).Include(a=> a.Form).Include(a=> a.HtmlTemplate).FirstOrDefault();

            if (pageDetails == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            pageDetails.Views = pageDetails.Views + 1;
            _context.Update(pageDetails);
            await _context.SaveChangesAsync();

            String route = "<a href='"+Url.Action("Index", "Home")+ "' class='rtText' >الرئيسية &raquo;</a>";
            PageCategory cat = _context.PagesCategories
                .Include(a => a.Category)
                .Where(a => a.PageId == id)
                .FirstOrDefault();
            if(cat != null && cat.Category.ShowInPath== true)
            {
                route += " <a href='" + Url.Action("Details", "Categories", new {id= cat.CategoryId, title=cat.Category.ArName }) + "' class='rtText' >" + cat.Category.ArName+" &raquo;</a>";
            }

            ViewData["PageTags"] = _context.TagsRel.Where(a => a.PageId == id && a.Deleted == 0).Include(a => a.Tag).ToList();

            if (pageDetails.TemplateId != null && pageDetails.TemplateId != 0 && pageDetails.HtmlTemplate.FilePath.ToString() != "")
            {
                ViewBag.Template = pageDetails.HtmlTemplate.FilePath.ToString().Trim();
            }
            else ViewBag.Template = "~/Views/Shared/Templates/_defaultPage.cshtml";

            ViewBag.FormFields = _context.FormsFields.Where(a => a.Deleted == false && a.FormId == pageDetails.FormId)
            .Include(a => a.Options)
            .OrderBy(a => a.Priority).ThenBy(a => a.Id).ToList();

            ViewBag.Route = route;
            ViewBag.OgImage = pageDetails.Thumb;

            return View(pageDetails);
        }

        [HttpPost]
        public async Task<IActionResult> SaveJobApp(int FormId, int PageId)
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
                MemberId = null,
                FormId = FormId,
                JobId = null,
                Type = "Page",
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
            return RedirectToAction("Details", new { id = PageId });
                
        }

        [HttpPost]
        public async Task<IActionResult> ContactUs([FromForm] ContactUs model)
        {
            if (ModelState.IsValid)
            {
                if (!await GoogleRecaptcha.IsReCaptchaPassedAsync(Request.Form["g-recaptcha-response"],
                    _config["GoogleReCaptcha:secret"]))
                {
                    ModelState.AddModelError(string.Empty, "You failed the CAPTCHA");
                    return Redirect(HttpContext.Request.Headers["Referer"]);
                }
                model.SystemDate = DateTime.Now;

                _context.Add(model);
                await _context.SaveChangesAsync();
                //_uow.GetRepository<ContactUs>().Add(model);
                //_uow.SaveChanges();
                TempData["success"] = "تم ارسال رسالتك بنجاح... سوف يتم المتابعة معك بأقرب وقت.";

                return Redirect(HttpContext.Request.Headers["Referer"]);
            }

            return Redirect(HttpContext.Request.Headers["Referer"]);
        }
    }
}
