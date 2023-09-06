using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using PEF.Helpers;
using PEF.Models;

namespace PEF.Controllers
{
    //[MOJustice.AuthorizedMembers]
    public class MembersController : Controller
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;

        public MembersController(DataContext context, IConfiguration config, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {
            _context = context;
            _config = config;
            _environment = environment;
        }

        public IActionResult removeExpert(int item)
        {
            try
            {
                string userId = User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault();
                if (userId != null)
                {
                    Members member = _context.Members.Where(a => a.Id == int.Parse(userId) && a.Deleted == false && a.Active == true).FirstOrDefault();
                    var expert = _context.MembersExpert.Where(a => a.Id == item && a.MemberId == int.Parse(userId)).FirstOrDefault();
                    if (expert != null)
                    {
                        expert.Deleted = true;
                    }
                    _context.MembersExpert.Update(expert);
                    _context.SaveChanges();

                    return Json(new
                    {
                        Results = true,
                        Msg = "تم ازالة الخبرة"
                    });
                }
                else
                {
                    return Json(new
                    {
                        Results = false,
                        Msg = "Wrong Authentication!"
                    });
                }
            }
            catch (Exception)
            {
                return Json(new
                {
                    Results = false,
                    Msg = "Something wrong!"
                });
                throw;
            }
            

        }

        public IActionResult removeTraining(int item)
        {
            try
            {
                string userId = User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault();
                if (userId != null)
                {
                    Members member = _context.Members.Where(a => a.Id == int.Parse(userId) && a.Deleted == false && a.Active == true).FirstOrDefault();
                    var training = _context.MembersTraining.Where(a => a.Id == item && a.MemberId == int.Parse(userId)).FirstOrDefault();
                    if (training != null)
                    {
                        training.Deleted = true;
                    }
                    _context.MembersTraining.Update(training);
                    _context.SaveChanges();

                    return Json(new
                    {
                        Results = true,
                        Msg = "تم ازالة التدريب"
                    });
                }
                else
                {
                    return Json(new
                    {
                        Results = false,
                        Msg = "Wrong Authentication!"
                    });
                }
            }
            catch (Exception)
            {
                return Json(new
                {
                    Results = false,
                    Msg = "Something wrong!"
                });
                throw;
            }
            
        }
               

        public IActionResult removeFamily(int item)
        {
            try
            {
                string userId = User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault();
                if (userId != null)
                {
                    Members member = _context.Members.Where(a => a.Id == int.Parse(userId) && a.Deleted == false && a.Active == true).FirstOrDefault();
                    var family = _context.MembersFamily.Where(a => a.Id == item && a.MemberId == int.Parse(userId)).FirstOrDefault();
                    if (family != null)
                    {
                        family.Deleted = true;
                    }
                    _context.MembersFamily.Update(family);
                    _context.SaveChanges();

                    return Json(new
                    {
                        Results = true,
                        Msg = "تم ازالة فرد العائلة"
                    });
                }
                else
                {
                    return Json(new
                    {
                        Results = false,
                        Msg = "Wrong Authentication!"
                    });
                }
            }
            catch (Exception)
            {
                return Json(new
                {
                    Results = false,
                    Msg = "Something wrong!"
                });
                throw;
            }
            

        }


        public IActionResult removeEducation(int item)
        {
            try
            {
                string userId = User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault();
                if (userId != null)
                {
                    Members member = _context.Members.Where(a => a.Id == int.Parse(userId) && a.Deleted == false && a.Active == true).FirstOrDefault();
                    var education = _context.MembersEducation.Where(a => a.Id == item && a.MemberId == int.Parse(userId)).FirstOrDefault();
                    if (education != null)
                    {
                        education.Deleted = true;
                    }
                    _context.MembersEducation.Update(education);
                    _context.SaveChanges();

                    return Json(new
                    {
                        Results = true,
                        Msg = "تم ازالة فرد السجل التعليمي"
                    });
                }
                else
                {
                    return Json(new
                    {
                        Results = false,
                        Msg = "Wrong Authentication!"
                    });
                }
            }
            catch (Exception)
            {
                return Json(new
                {
                    Results = false,
                    Msg = "Something wrong!"
                });
                throw;
            }
            
                
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details()
        {
            try
            {
                string userId = User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault();
                if (userId != null)
                {
                    ViewBag.MembersFamily = _context.MembersFamily.Where(a => a.MemberId == int.Parse(userId) && a.Deleted == false).OrderBy(a => a.Id).ToList();
                    ViewBag.MembersEducation = _context.MembersEducation.Where(a => a.MemberId == int.Parse(userId) && a.Deleted == false)
                        .Include(a => a.Education).Include(a => a.Specialize).OrderBy(a => a.Id).ToList();
                    ViewBag.MembersExpert = _context.MembersExpert.Where(a => a.MemberId == int.Parse(userId) && a.Deleted == false).OrderBy(a => a.Id).ToList();
                    ViewBag.MembersTraining = _context.MembersTraining.Where(a => a.MemberId == int.Parse(userId) && a.Deleted == false).OrderBy(a => a.Id).ToList();

                    Members member = _context.Members.Where(a => a.Id == int.Parse(userId) && a.Deleted == false && a.Active == true)
                    .Include(m => m.City)
                    .Include(m => m.SocialStatus)
                    .Include(m=> m.Area)
                    .FirstOrDefault();

                    if (member == null)
                    {
                        TempData["error"] = "لم يتم العثور على بياناتك الشخصية! سجل الدخول مرة أخرى.";
                        return RedirectToAction("Index", "Home");
                    }
                    ViewData["CityId"] = _context.City.ToList();
                    ViewData["SocialId"] = _context.LookupSocialStatus.ToList();

                    return View(member);
                }
                else return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
                throw;
            }
           
        }

        public IActionResult Profile()
        {
            try
            {
                string userId = User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault();
                if (userId != null)
                {
                    Members member = _context.Members.Where(a => a.Id == int.Parse(userId) && a.Deleted == false && a.Active == true).FirstOrDefault();
                    ViewData["CityId"] = _context.City.ToList();
                    ViewData["SocialId"] = _context.LookupSocialStatus.ToList();
                    if(member.CityId !=null) ViewData["Villages"] = _context.Villages.Where(a => a.CityId == member.CityId).ToList();

                    if (member == null)
                    {
                        TempData["error"] = "لم يتم العثور على بياناتك الشخصية! سجل الدخول مرة أخرى.";
                        return RedirectToAction("Index", "Home");
                    }
                    return View(member);
                }
                else return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
                throw;
            }
            
        }

        [HttpGet]
        public IActionResult ProfDetails()
        {         
            if(!String.IsNullOrEmpty(HttpContext.Session.GetString("mem_id")))
            {
                var Id = int.Parse(HttpContext.Session.GetString("mem_id"));
                Members member = _context.Members.Where(a => a.Id == Id && a.Active==true && a.Deleted == false).FirstOrDefault();

                if (member == null)
                {
                    TempData["error"] = "لم يتم العثور على بياناتك الشخصية! سجل الدخول مرة أخرى.";
                    return RedirectToAction("Index", "Home");
                }

                var experts = _context.MembersExpert.Where(a => a.MemberId == Id && a.Deleted == false).ToList();
                var training = _context.MembersTraining.Where(a => a.MemberId == Id && a.Deleted == false).ToList();
                var family = _context.MembersFamily.Where(a => a.MemberId == Id && a.Deleted == false).ToList();
                var education = _context.MembersEducation.Where(a => a.MemberId == Id && a.Deleted == false)
                .Include(a=> a.Education)
                .Include(a=> a.Specialize)
                .Include(a => a.Country)
                .ToList();

                ViewBag.Experts = experts;
                ViewBag.Training = training;
                ViewBag.Family = family;
                ViewBag.Education = education;

                ViewBag.haveExperts = false;
                ViewBag.haveTraining = false;
                ViewBag.haveFamily = false;
                ViewBag.haveEducation = false;

                ViewBag.EducationLookup = _context.LookupEducation.Where(a => a.Deleted == false).OrderBy(a=> a.Type).ThenBy(a=> a.Id).ThenBy(a => a.ArName).ToList();
                ViewBag.Countries = _context.LookupCountries.Where(a => a.Deleted == false).OrderBy(a => a.ArName).ToList();
                ViewBag.Specializations = _context.LookupSpecialize.Where(a => a.Deleted == false).OrderBy(a => a.ArName).ToList();

                if (experts.Count() > 0) ViewBag.haveExperts = true;
                if (training.Count() > 0) ViewBag.haveTraining = true;
                if (family.Count() > 0) ViewBag.haveFamily = true;
                if (education.Count() > 0) ViewBag.haveEducation = true;
                Console.Write("Expert:"+ViewBag.haveExperts);
                return View(member);
            }   
            else{
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]        
        public async Task<IActionResult> SaveProfDetails()
        {
            try
            {
                var expItemsCount = int.Parse(HttpContext.Request.Form["expItemsCount"]);
                var trainItemsCount = int.Parse(HttpContext.Request.Form["trainItemsCount"]);
                var familyItemsCount = int.Parse(HttpContext.Request.Form["famItemsCount"]);
                var edItemsCount = int.Parse(HttpContext.Request.Form["edItemsCount"]);


                if (!String.IsNullOrEmpty(HttpContext.Session.GetString("mem_id")))
                {
                    var Id = int.Parse(HttpContext.Session.GetString("mem_id"));
                    Members member = _context.Members.Where(a => a.Id == Id && a.Deleted == false).FirstOrDefault();


                    if (member != null)
                    {
                        //Insert or update the experts
                        member.MembersExpert = new List<MembersExpert>();
                        for (int i = 1; i <= expItemsCount; i++)
                        {
                            var jobTitle = HttpContext.Request.Form["JobTitle" + i];
                            var jobLocation = HttpContext.Request.Form["JobLocation" + i];

                            var expDescription = HttpContext.Request.Form["ExpDescription" + i];
                            var jobFile = "";
                            if (HttpContext.Request.Form.Files.Count > 0)
                            {
                                jobFile = ImagesUplaod.UploadFile(HttpContext, "files/file/MembersFiles/", _environment.WebRootPath, "JobFile" + i);
                            }
                            if (!String.IsNullOrEmpty(jobTitle) && !String.IsNullOrEmpty(jobLocation))
                            {
                                MembersExpert expert = new MembersExpert { MemberId = Id, JobTitle = jobTitle, JobLocation = jobLocation, ExpDescription = expDescription, Attachment = jobFile, AddTime = DateTime.Now };

                                if (!String.IsNullOrEmpty(HttpContext.Request.Form["StartDate" + i])) expert.StartDate = DateTime.Parse(HttpContext.Request.Form["StartDate" + i]);
                                if (!String.IsNullOrEmpty(HttpContext.Request.Form["EndDate" + i])) expert.EndDate = DateTime.Parse(HttpContext.Request.Form["EndDate" + i]);
                                member.MembersExpert.Add(expert);
                            }
                        }

                        //Members Training Add/Edit
                        member.MembersTraining = new List<MembersTraining>();
                        for (int i = 1; i <= trainItemsCount; i++)
                        {
                            var trainTitle = HttpContext.Request.Form["TrainTitle" + i];
                            var trainHours = 0;
                            if (!string.IsNullOrEmpty(HttpContext.Request.Form["TrainHours" + i])) trainHours = int.Parse(HttpContext.Request.Form["TrainHours" + i]);

                            var trainSupplier = HttpContext.Request.Form["TrainSupplier" + i];
                            var trainTask = HttpContext.Request.Form["TrainTask" + i];
                            var trainFile = "";
                            if (HttpContext.Request.Form.Files.Count > 0)
                            {
                                trainFile = ImagesUplaod.UploadFile(HttpContext, "files/file/MembersFiles/", _environment.WebRootPath, "trainFile" + i);
                            }

                            if (!String.IsNullOrEmpty(trainTitle))
                            {
                                MembersTraining training = new MembersTraining { MemberId = Id, Name = trainTitle, Hours = trainHours, TrainingSupplier = trainSupplier, TrainingTasks = trainTask, Attachment = trainFile, AddTime = DateTime.Now };

                                if (!String.IsNullOrEmpty(HttpContext.Request.Form["TrainStartDate" + i])) training.StartDate = DateTime.Parse(HttpContext.Request.Form["TrainStartDate" + i]);
                                if (!String.IsNullOrEmpty(HttpContext.Request.Form["TrainEndDate" + i])) training.EndDate = DateTime.Parse(HttpContext.Request.Form["TrainEndDate" + i]);
                                member.MembersTraining.Add(training);
                            }
                        }

                        //Members Training Add/Edit
                        member.MembersFamily = new List<MembersFamily>();
                        for (int i = 1; i <= familyItemsCount; i++)
                        {
                            var fullname = HttpContext.Request.Form["FamFullName" + i];
                            var gender = 0;
                            if (!string.IsNullOrEmpty(HttpContext.Request.Form["FamGender" + i])) gender = int.Parse(HttpContext.Request.Form["FamGender" + i]);
                            var relation = 1;
                            if (!string.IsNullOrEmpty(HttpContext.Request.Form["FamRelation" + i])) relation = int.Parse(HttpContext.Request.Form["FamRelation" + i]);


                            var idNum = HttpContext.Request.Form["IdNum" + i];
                            var edLevel = 1;
                            if (!string.IsNullOrEmpty(HttpContext.Request.Form["EdLevel" + i])) edLevel = int.Parse(HttpContext.Request.Form["EdLevel" + i]);

                            Boolean isWork = false;
                            if (HttpContext.Request.Form["IsWork" + i] == "1") isWork = true;

                            var job = HttpContext.Request.Form["Job" + i];
                            var healthStatus = 1;
                            if(!string.IsNullOrEmpty(HttpContext.Request.Form["HealthStatus" + i])) healthStatus = int.Parse(HttpContext.Request.Form["HealthStatus" + i]);

                            var disease = "";
                            if (healthStatus == 2) disease = HttpContext.Request.Form["Disease" + i];

                            if (!String.IsNullOrEmpty(fullname) && !String.IsNullOrEmpty(gender.ToString()) && !String.IsNullOrEmpty(relation.ToString()))
                            {
                                MembersFamily family = new MembersFamily { MemberId = Id, FullName = fullname, Gender = gender, Relation = relation, IdNum = idNum, EducationId = edLevel, HealthStatus = healthStatus, Disease = disease, Job = job, IsWork = isWork, AddDate = DateTime.Now };

                                if (!String.IsNullOrEmpty(HttpContext.Request.Form["BirthDate" + i])) family.BirthDate = DateTime.Parse(HttpContext.Request.Form["BirthDate" + i]);

                                member.MembersFamily.Add(family);
                            }
                        }

                        //Members Education Add/Edit
                        member.MembersEducation = new List<MembersEducation>();
                        for (int i = 1; i <= edItemsCount; i++)
                        {
                            var levelType = int.Parse(HttpContext.Request.Form["EdLevelType" + i]);
                            var education = int.Parse(HttpContext.Request.Form["EdEducation" + i]);
                            var university = HttpContext.Request.Form["EdUniversity" + i];
                            Nullable<int> country = null;
                            string? countryName = "";
                            Nullable<int> specialize = null;
                            if (!String.IsNullOrEmpty(HttpContext.Request.Form["EdCountry" + i])) countryName =HttpContext.Request.Form["EdCountry" + i];

                            var grade = HttpContext.Request.Form["EdGrade" + i];
                            if (!String.IsNullOrEmpty(HttpContext.Request.Form["EdSpecialize" + i])) specialize = int.Parse(HttpContext.Request.Form["EdSpecialize" + i]);

                            var edFile = "";
                            if (HttpContext.Request.Form.Files.Count > 0)
                            {
                                edFile = ImagesUplaod.UploadFile(HttpContext, "files/file/MembersFiles/", _environment.WebRootPath, "EdFile" + i);
                            }


                            if (!String.IsNullOrEmpty(levelType.ToString()) && !String.IsNullOrEmpty(education.ToString()))
                            {
                                if (levelType == 1)
                                {
                                    country = null;
                                    countryName = "";
                                    university = "";
                                    grade = "";
                                    specialize = null;
                                }
                                MembersEducation educationObj = new MembersEducation { MemberId = Id, EducationLevelType = levelType, EducationId = education, University = university, CountryId = country,CountryName= countryName, Grade = grade, SpecializeId = specialize, Attachment = edFile, AddTime = DateTime.Now };

                                if (!String.IsNullOrEmpty(HttpContext.Request.Form["EdGradeDate" + i])) educationObj.GradDate = DateTime.Parse(HttpContext.Request.Form["EdGradeDate" + i]);

                                member.MembersEducation.Add(educationObj);
                            }
                        }

                        await _context.SaveChangesAsync();

                        TempData["success"] = "تم تعديل ملفك الشخصي بنجاح,,, ملفك الشخصي الآن مكتمل، بامكانك البدء بالتقدم لفرص التشغيل.";
                        return RedirectToAction("ProfDetails", "Members");
                    }
                    else
                    {
                        TempData["error"] = "لا يمكن تحديث البيانات!";

                    }
                }
                else
                {
                    TempData["error"] = "الرجاء تسجيل الدخول!";
                    return RedirectToAction("Index", "Home");
                }
                
            }
            catch (Exception)
            {                
                throw;
            }
            return View("ProfDetails");
            //Console.Write("The Job Title:" + HttpContext.Request.Form["JobTitle1"].ToString());
            //TempData["success"] = "تم تعديل ملفك الشخصي بنجاح,,, ملفك الشخصي الأن مكتمل، بامكانك البدء بالتقدم للوظائف.";
            //ViewBag.Msg = "تم تعديل ملفك الشخصي بنجاح,,, ملفك الشخصي الأن مكتمل، بامكانك البدء بالتقدم للوظائف."+"<bR>The Job Title:" + HttpContext.Request.Form["JobTitle1"].ToString();
            
            //return RedirectToAction("Index", "Jobs");            
        }

        public async Task<IActionResult> UpdateProfile([Bind("Id,FirstName,FatherName,GrandName,FamilyName,IdNum,FatherIdNum,MotherIdNum,PartnerIdNum,Gender,SocialId,BirthDate,FamilyMembersCount,CityId,AreaId,Area,District,Street,NearTo,Mobile,Mobile2,Tel,Email,Password,ConfirmPassword,HealthStatus,DisabilityType,DisabilitySize,HealthAtt1,PropertyType,HouseSize,HouseNature,Photo,BreedWinnder,IncomeExist,IncomeSource,IncomeValue,IncomeIdNumber,AttachTitle1,AttachTitle2,AttachTitle3,Attach1,Attach2,Attach3,BreedWinnderType,HouseType")] Members members, string[] Interests)
        {
            //
            string userId = User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault();
            if (userId != null && userId != "")
            {
                //if (string.IsNullOrEmpty(members.Email)  || string.IsNullOrEmpty(members.Mobile))
                //{
                //    TempData["erorr"] = "البريد الالكتروني ورقم الموبايل الزامي!";
                //    return RedirectToAction("Profile");
                //}

                //if (MembersExistByEmail(members.Email, int.Parse(userId)))
                //{
                //    TempData["error"] = "البريد الالكتروني مستخدم مسبقا من قبل مستخدم اخر...";
                //    return RedirectToAction("Profile");
                //}
                if (MembersExistByMobile(members.Mobile, int.Parse(userId)))
                {
                    TempData["error"] = "رقم الهاتف الخلوي مستخدم مسبقا من قبل مستخدم آخر...";
                    return RedirectToAction("Profile");
                }
                if (MembersExistByIdNum(members.IdNum, int.Parse(userId)))
                {
                    TempData["error"] = "رقم الهوية مستخدم مسبقا من قبل مستخدم آخر...";
                    return RedirectToAction("Profile");
                }

                ModelState.Remove("Password");
                ModelState.Remove("ConfirmPassword");
                ModelState.Remove("FirstName");
                ModelState.Remove("FatherName");
                ModelState.Remove("GrandName");
                ModelState.Remove("FamilyName");
                ModelState.Remove("IdNum");
                ModelState.Remove("CityId");
                ModelState.Remove("Email");
                if (ModelState.IsValid)
                {
                    members.Id = int.Parse(userId);
                    _context.Members.Attach(members);

                    _context.Entry(members).State = EntityState.Modified;                                       
                    _context.Entry(members).Property(e => e.AddDate).IsModified = false;
                    //_context.Entry(members).Property(e => e.AddedBy).IsModified = false;
                    _context.Entry(members).Property(e => e.LastLogin).IsModified = false;
                    _context.Entry(members).Property(e => e.Active).IsModified = false;

                    _context.Entry(members).Property(e => e.FirstName).IsModified = false;
                    _context.Entry(members).Property(e => e.FatherName).IsModified = false;
                    _context.Entry(members).Property(e => e.FamilyName).IsModified = false;
                    _context.Entry(members).Property(e => e.GrandName).IsModified = false;

                    _context.Entry(members).Property(e => e.Gender).IsModified = false;
                    _context.Entry(members).Property(e => e.SocialId).IsModified = false;
                    _context.Entry(members).Property(e => e.IdNum).IsModified = false;
                    _context.Entry(members).Property(e => e.BirthDate).IsModified = false;

                    _context.Entry(members).Property(e => e.CityId).IsModified = false;
                    _context.Entry(members).Property(e => e.AreaId).IsModified = false;
                    _context.Entry(members).Property(e => e.Email).IsModified = false;

                    //_context.Entry(members).Property(e => e.Attach1).IsModified = false;
                    //_context.Entry(members).Property(e => e.Attach2).IsModified = false;
                    //_context.Entry(members).Property(e => e.Attach3).IsModified = false;
                    //_context.Entry(members).Property(e => e.HealthAtt1).IsModified = false;
                    //members.RegisteredToMOE = false;

                    if (!String.IsNullOrEmpty(members.Password) && members.Password != "******")
                    {
                        members.Password = Encryption.Encrypt(members.Password, true);
                        members.Password = Encryption.Encrypt(members.ConfirmPassword, true);
                    }
                    else
                    {
                        _context.Entry(members).Property(a => a.Password).IsModified = false;
                        _context.Entry(members).Property(a => a.ConfirmPassword).IsModified = false;
                    }

                    if (Interests != null)
                    {
                        members.Interest = string.Join(",", Interests);
                    }

                    if (HttpContext.Request.Form.Files.Count > 0)
                    {
                        var HealthAtt1 = ImagesUplaod.UploadFile(HttpContext, "files/file/MembersFiles/", _environment.WebRootPath, "HealthAtt1");
                        if (HealthAtt1 != "") members.HealthAtt1 = HealthAtt1;
                        else
                            _context.Entry(members).Property(e => e.HealthAtt1).IsModified = false;

                        var Attach1 = ImagesUplaod.UploadFile(HttpContext, "files/file/MembersFiles/", _environment.WebRootPath, "Attach1");
                        if (Attach1 != "") members.Attach1 = Attach1;
                        else _context.Entry(members).Property(e => e.Attach1).IsModified = false;

                        var Attach2 = ImagesUplaod.UploadFile(HttpContext, "files/file/MembersFiles/", _environment.WebRootPath, "Attach2");
                        if (Attach2 != "") members.Attach2 = Attach2;
                        else _context.Entry(members).Property(e => e.Attach2).IsModified = false;

                        var Attach3 = ImagesUplaod.UploadFile(HttpContext, "files/file/MembersFiles/", _environment.WebRootPath, "Attach3");
                        if (Attach3 != "") members.Attach3 = Attach3;
                        else _context.Entry(members).Property(e => e.Attach3).IsModified = false;

                        var ImageUrl = ImagesUplaod.UploadSingleImage(HttpContext, "files/image/MembersImg/", _environment.WebRootPath, "Photo");
                        if (ImageUrl.Item1.ToString() != "") members.Photo = ImageUrl.Item1;
                        else _context.Entry(members).Property(e => e.Photo).IsModified = false;
                    }
                    else
                    {
                        _context.Entry(members).Property(e => e.Attach1).IsModified = false;
                        _context.Entry(members).Property(e => e.Attach2).IsModified = false;
                        _context.Entry(members).Property(e => e.Attach3).IsModified = false;
                        _context.Entry(members).Property(e => e.HealthAtt1).IsModified = false;
                        _context.Entry(members).Property(e => e.Photo).IsModified = false;
                    }

                    await _context.SaveChangesAsync();
                    _context.Entry(members).State = EntityState.Detached;  // Detach the entity from the context to enable modifying to same object in database

                    //to not retrieve a cached copy of members
                    var memberDetails = _context.Members.AsNoTracking().FirstOrDefault(a => a.Deleted == false && a.Id == members.Id);
                    if (!String.IsNullOrEmpty(memberDetails.FirstName) && !String.IsNullOrEmpty(memberDetails.FatherName) && !String.IsNullOrEmpty(memberDetails.GrandName) && !String.IsNullOrEmpty(memberDetails.FamilyName) && !String.IsNullOrEmpty(memberDetails.SocialId.ToString()) && !String.IsNullOrEmpty(memberDetails.IdNum.ToString()) && !String.IsNullOrEmpty(memberDetails.CityId.ToString()) && !String.IsNullOrEmpty(memberDetails.BirthDate.ToString()) && !String.IsNullOrEmpty(memberDetails.Gender.ToString()) && !String.IsNullOrEmpty(memberDetails.Email) && !String.IsNullOrEmpty(memberDetails.Mobile))
                    {
                        memberDetails.Completed = true;
                        _context.Update(memberDetails);
                        await _context.SaveChangesAsync();
                        HttpContext.Session.SetString("completed", "True");
                        Console.Write(HttpContext.Session.GetString("completed"));
                    }

                    TempData["success"] = "تم تعديل ملفك الشخصي بنجاح...";
                    return RedirectToAction("Profile");
                }
                else
                {
                    Members member = _context.Members.Where(a => a.Id == int.Parse(userId) && a.Deleted == false && a.Active == true).FirstOrDefault();
                    ViewData["CityId"] = _context.City.ToList();
                    ViewData["SocialId"] = _context.LookupSocialStatus.ToList();
                    
                    if (members.CityId != null) ViewData["Villages"] = _context.Villages.Where(a => a.CityId == members.CityId).ToList();
                    TempData["error"] = "لم يتم تعديل البيانات! تأكد من البيانات المدخلة!";
                    //if (member.CityId != null) ViewData["Villages"] = _context.Villages.Where(a => a.CityId == member.CityId).ToList();
                    return View("profile", member);
                    //return RedirectToAction("Profile", "Members");
                }
            }
            else
            {
                TempData["error"] = "You don't have permissions to access this page...";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet("Change")]
        public IActionResult Change()
        {
            Console.Write("Change");
            return View();
        }

        [HttpPost("Change")]
        public async Task<IActionResult> Change(string oldpassword, string newpassword, string confirmpassword)
        {
            string userId = User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault();
            if (userId != null && userId != "")
            {
                //var userId = User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).FirstOrDefault();
                var user = await _context.Members.FindAsync(int.Parse(userId.ToString()));
                if (user != null)
                {
                    if (Encryption.Decrypt(user.Password, true) == oldpassword)
                    {
                        if (newpassword == confirmpassword)
                        {
                            user.Password = Encryption.Encrypt(newpassword, true);
                            user.ConfirmPassword = Encryption.Encrypt(newpassword, true);
                            _context.Members.Update(user);
                            await _context.SaveChangesAsync();
                            TempData["success"] = "تم تغيير كلمة المرور بنجاح...";
                        }
                        else
                        {
                            TempData["error"] = "كلمة المرور غير متطابقة...";
                        }
                        return View();
                    }
                    else
                    {
                        TempData["error"] = "خطأ في كلمة المرور القديمة...";
                        return View();
                    }
                }
                else
                {
                    TempData["error"] = "المستخدم غير موجود...";
                    return RedirectToAction("Change", "Members");
                }

            }
            else
            {
                TempData["error"] = "المستخدم غير موجود...";
                return RedirectToAction("Index", "Home");
            }
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
            Boolean member_exist = (_context.Members?.Any(e => e.Email == email && (id == null || e.Id != id))).GetValueOrDefault();
            return (member_exist || employer_exist || donor_exist);
            //return (_context.Members?.Any(e => e.Email == email && e.Deleted == false
            //&& (id == null || e.Id != id)))
            //.GetValueOrDefault();
        }

        private bool MembersExistByMobile(string mobile, int? id)
        {
            //Make sure that there is no user in the system use the same email address
            Boolean employer_exist = (_context.Employers?.Any(e => e.Mobile == mobile && e.Deleted == false)).GetValueOrDefault();
            Boolean member_exist = (_context.Members?.Any(e => e.Mobile == mobile && (id == null || e.Id != id) && e.Deleted==false)).GetValueOrDefault();

            return (employer_exist || member_exist);
        }

        private bool MembersExistByIdNum(string IdNum, int? id)
        {
            //Make sure that there is no user in the system use the same email address
           
            Boolean member_exist = (_context.Members?.Any(e => e.IdNum == IdNum && (id == null || e.Id != id && e.Deleted == false))).GetValueOrDefault();

            return member_exist;
        }
    }
}
