using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PEF.Models;
using PEF.Helpers;
using System.Data;
using System.Reflection;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;

namespace PEF.Areas.Control.Controllers
{
    
    [Area("Control")]
    public class UsersController : Controller
    {
        private readonly DataContext _context;
        private readonly SignInManager<User> _signInManager;

        public UsersController(DataContext context)
        {
            _context = context;
        }
        [Authorize]        
        [PEF.AuthorizedAction]
        //[Authorize(Roles = "Admin")]
        // GET: Control/Users
        public async Task<IActionResult> Index()
        {
            var projects = _context.Projects.Where(a => a.Deleted == false).ToList();
            var dataContext = _context.Users.Where(a => a.Deleted == 0).Include(u => u.Group).Include(u => u.Language);

            ViewBag.Projects = projects;
            return View(await dataContext.ToListAsync());
        }

        [Authorize]
        public async Task<IActionResult> addProjectUser([Bind("Id", "UserId", "ProjectId")] UsersProjects projects)
        {
            if (ModelState.IsValid)
            {
                if (!_context.UsersProjects.Where(a => a.UserId == projects.UserId && a.ProjectId == projects.ProjectId).Any())
                {
                    _context.Add(projects);
                    await _context.SaveChangesAsync();
                    return Json(new
                    {
                        result = true,
                        msg = "Project have been assigned to user!"
                    });
                }
                else
                {
                    return Json(new
                    {
                        result = false,
                        msg = "Project already assigned to user!"
                    });
                }

            }
            else
            {
                return Json(new
                {
                    result = false,
                    msg = "Failed to add project!"
                }); ; ;
            }

        }
        [Authorize]
        public async Task<IActionResult> removeProjectUser(int item)
        {
            UsersProjects result = await _context.UsersProjects.Where(a => a.Id == item).FirstOrDefaultAsync();
            if (result != null)
            {
                _context.UsersProjects.Remove(result);
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


        [Authorize]
        public IActionResult loadProjects(int user)
        {
            var projects = _context.UsersProjects.Where(a => a.UserId == user && a.Project.Deleted == false)
                .Include(a => a.User)
                .Include(a => a.Project)
                .OrderByDescending(a => a.Project.Name).ToList();

            return Json(new
            {
                result = true,
                projects = projects
            });
        }

        [Authorize]
        //[PEF.AuthorizedAction]
        //[Authorize]
        public async Task<IActionResult> Profile()
        {
            //Check if user is Employer/Donor/Trainer
            if(HttpContext.Session.GetString("type")=="Trainer" || HttpContext.Session.GetString("type") == "Donor" || HttpContext.Session.GetString("type") == "Employer")
            {
                if(HttpContext.Session.GetString("type") == "Employer")
                {
                    return RedirectToAction("Details", "Employers", new { id = HttpContext.Session.GetString("id")});
                }
                else if (HttpContext.Session.GetString("type") == "Trainer")
                {
                    return RedirectToAction("Details", "Trainer", new { id = HttpContext.Session.GetString("id") });
                }
                else if (HttpContext.Session.GetString("type") == "Donor")
                {
                    return RedirectToAction("Details", "Donor", new { id = HttpContext.Session.GetString("id") });
                }
            }

            if (HttpContext.Session.GetString("id") != "")
            {
                var user = await _context.Users.FindAsync(int.Parse(HttpContext.Session.GetString("id") ?? "1"));
                return View(user);
            }
            else
            {
                TempData["error"] = "User not found...";
                return RedirectToAction("Index");
            }            
        }

        [Authorize]
        //[PEF.AuthorizedAction]
        [HttpPost("Profile")]
        public async Task<IActionResult> Profile(string fullname,string nickname,string password, string confirm_password, string email)
        {
            if (HttpContext.Session.GetString("id") != "")
            {
                var user = await _context.Users.FindAsync(int.Parse(HttpContext.Session.GetString("id") ?? "1"));
                user.Nickname = nickname;
                if(password!="******" && password != "")
                {
                    user.Password = Encryption.Encrypt(password, true);
                }
                
                user.Email = email;

                await _context.SaveChangesAsync();
                TempData["success"] = "Profile updated successfully...";
                return View(user);
                //}
                //else
                //{
                //    TempData["error"] = "Cannot edit profile! Validation Error...";
                //}

                return View("profile");
            }
            else
            {
                TempData["error"] = "User not found...";
                return RedirectToAction("Index");
            }
        }

        //string ReturnUrl
        public IActionResult UserLogin(string ReturnUrl)
        {
            ViewData["error"] = TempData["error"];
            ViewData["redirectUrl"] = System.Net.WebUtility.UrlDecode(ReturnUrl);
            return View("Login");
        }

        //string ReturnUrl
        public IActionResult Login(string ReturnUrl)
        {
            ViewData["error"] = TempData["error"];
            ViewData["redirectUrl"] = System.Net.WebUtility.UrlDecode(ReturnUrl);
            return View();
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login(string username, string password, string redirectUrl)
        {
            //if (ModelState.IsValid)
            //{
            //    var user = _context.Users.Include(a => a.Group).FirstOrDefault(u => u.Email == username);
            //    if (user != null)
            //    {
            //        var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, false, lockoutOnFailure: false);

            //        if (result.Succeeded)
            //        {
            //            return RedirectToAction("Index", "Home");
            //        }
            //        else
            //        {
            //            ViewBag.Msg = "Wrong username or password!";
            //            //ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            //        }
            //    }
            //    else
            //    {
            //        ViewBag.Msg = "Wrong username or password!!!";
            //    }

            //}

            //return View("Login");
            ViewData["redirectUrl"] = System.Net.WebUtility.UrlDecode(redirectUrl);

            var user = _context.Users.Include(a => a.Group).FirstOrDefault(u => u.Email == username);
            if (user != null) //Login Success
            {
                if (Encryption.Decrypt(user.Password, true) == password)
                {
                    //ViewBag.Msg = "Login success...";
                    var claims = new List<Claim>();

                    claims.Add(new Claim(ClaimTypes.NameIdentifier, username));
                    claims.Add(new Claim(ClaimTypes.Name, user.Fullname));
                    claims.Add(new Claim(ClaimTypes.Sid, user.Id.ToString()));
                    claims.Add(new Claim(ClaimTypes.Email, user.Email));
                    claims.Add(new Claim(ClaimTypes.GroupSid, user.GroupId.ToString()));
                    claims.Add(new Claim("username", username));
                    claims.Add(new Claim("nickname", user.Nickname));
                    claims.Add(new Claim("lastLogin", user.LastLogin.ToString()));
                    claims.Add(new Claim("id", user.Id.ToString()));

                    HttpContext.Session.SetString("id", user.Id.ToString());
                    HttpContext.Session.SetInt32("group_id", (int)user.GroupId);
                    HttpContext.Session.SetString("username", user.LoginName);
                    HttpContext.Session.SetString("email", user.Email);
                    HttpContext.Session.SetString("lastLogin", user.LastLogin.ToString());
                    HttpContext.Session.SetString("nickname", user.Nickname);
                    HttpContext.Session.SetString("Name", user.Fullname);
                    HttpContext.Session.SetString("ArName", user.Fullname);

                    HttpContext.Session.SetString("type", "CMS_User");

                    if (username == "admin@intertech.ps") HttpContext.Session.SetString("is_super_admin", "True");
                    else {
                        HttpContext.Session.SetString("is_super_admin", "False");

                        //Set the permissions
                        //List<Permissions> perms = _context.GroupPermissions.Where(a => a.GroupId == user.GroupId)
                        List<Permissions> perms = _context.GroupPermissions.Where(a => a.GroupId == user.GroupId)
                        .Include(a => a.Permissions)
                        .Select(a=> new Permissions
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
                        }));
                        
                    }
                    

                    if (user.GroupId == 1)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                    }
                    //

                    user.LastLogin = DateTime.Now;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrinciple = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrinciple);

                    if (redirectUrl == null || redirectUrl == "") redirectUrl = "/Control";
                    if (redirectUrl.Contains("GetFile")) redirectUrl = "/Control";  //Prevent Loop redirection in case of GetFile downloaded

                    return Redirect(System.Net.WebUtility.UrlDecode(redirectUrl));
                }
                else
                {
                    ViewBag.Msg = "Wrong password...";
                }
            }
            else
            {//Employers Login
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

                            claims.Add(new Claim("lastLogin", employer.LastLogin?.ToString()));
                            claims.Add(new Claim("id", employer.Id.ToString()));

                            HttpContext.Session.SetString("id", employer.Id.ToString());
                            HttpContext.Session.SetInt32("group_id", group_id); //Static ID for group
                            HttpContext.Session.SetString("username", employer.Email);
                            HttpContext.Session.SetString("email", employer.Email);
                            HttpContext.Session.SetString("lastLogin", employer.LastLogin?.ToString());
                            HttpContext.Session.SetString("nickname", employer.Name);
                            HttpContext.Session.SetString("Name", employer.Name);
                            HttpContext.Session.SetString("ArName", employer.ArName);

                            HttpContext.Session.SetString("is_super_admin", "False");
                            HttpContext.Session.SetString("type", "Employer");
                            claims.Add(new Claim(ClaimTypes.Role, "Employer"));

                            List<GroupPermissions> perms = _context.GroupPermissions.Where(a => a.GroupId == group_id).Include(a => a.Permissions).ToList();
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
                            ViewBag.Msg = "Wrong password...";
                        }
                    }
                    else
                    {
                        ViewBag.Msg = "الحساب غير مفعل!!! يرجى الانتظار بينما يتم تحديث الحساب من قبل الاداراة..";
                        ViewBag.Msg = "Account not activated! Wait until your account being activated...";
                    }

                }
                else
                {
                    ViewBag.Msg = "Wrong username or password...";
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

                
        public async Task<IActionResult> Logout()
        {
            string type = HttpContext.Session.GetString("type");
            
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if(type == "CMS_User")
            {
                return Redirect("/");
            }
            else
            {
                return RedirectToAction("Index", "Home", new {area=""});
            }
            
        }

        [Authorize]
        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult Forget()
        {
            return View();
        }

        [Authorize]
        [PEF.AuthorizedAction]
        //[Authorize(Roles = "Admin")]
        // GET: Control/Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                TempData["error"] = "User not found...";
                return RedirectToAction("Index");
            }

            var user = await _context.Users
                .Include(u => u.Group)
                .Include(u => u.Language)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                TempData["error"] = "User not found...";
                return RedirectToAction("Index");
            }

            return View(user);
        }
        [Authorize]
        [PEF.AuthorizedAction]
        //[Authorize(Roles = "Admin")]
        // GET: Control/Users/Create
        public IActionResult Create()
        {
            ViewData["GroupId"] = new SelectList(_context.Groups.Where(a => a.Deleted == 0), "Id", "Name");
            ViewData["LangId"] = new SelectList(_context.Languages.Where(a => a.Deleted==0), "Id", "Name");
            return View();
        }

        // POST: Control/Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [PEF.AuthorizedAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Fullname,Nickname,GroupId,LoginName,Password,Email,LangId,LastLogin,AddDate,Active,Deleted")] User user)
        {
            if (ModelState.IsValid)
            {
                if (HttpContext.Session.GetString("id") != "")
                {
                    user.AddedBy = int.Parse(HttpContext.Session.GetString("id") ?? "1");
                }
                else user.AddedBy = null;
               
                user.Password = Encryption.Encrypt(user.Password, true);
                user.AddDate = DateTime.Now;

                user.LoginName = Functions.RemoveHtml(user.LoginName);
                user.Nickname = Functions.RemoveHtml(user.Nickname);
                user.Fullname = Functions.RemoveHtml(user.Fullname);
                user.Email = Functions.RemoveHtml(user.Email);

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GroupId"] = new SelectList(_context.Groups.Where(a => a.Deleted == 0), "Id", "Name", user.GroupId);
            ViewData["LangId"] = new SelectList(_context.Languages.Where(a => a.Deleted == 0), "Id", "Name", user.LangId);
            return View(user);
        }
        [Authorize]
        [PEF.AuthorizedAction]
        //[Authorize(Roles = "Admin")]
        // GET: Control/Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Users == null)
            {
                TempData["error"] = "User not found...";
                return RedirectToAction("Index");
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                TempData["error"] = "User not found...";
                return RedirectToAction("Index");
            }
            ViewData["GroupId"] = new SelectList(_context.Groups.Where(a => a.Deleted == 0), "Id", "Name", user.GroupId);
            ViewData["LangId"] = new SelectList(_context.Languages.Where(a => a.Deleted == 0), "Id", "Name", user.LangId);
            return View(user);
        }

        // POST: Control/Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        
        [Authorize]
        [PEF.AuthorizedAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Fullname,Nickname,GroupId,LoginName,Password,Email,LangId")] User user)
        {
            if (id != user.Id)
            {
                TempData["error"] = "User not found...";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Users.Attach(user);
                    _context.Entry(user).State = EntityState.Modified;
                    _context.Entry(user).Property(e => e.AddedBy).IsModified = false;
                    _context.Entry(user).Property(e => e.AddDate).IsModified = false;
                    //User userToUpdate = _context.Users.Find(id);
                    //userToUpdate.Fullname = user.Fullname;
                    //userToUpdate.Nickname = user.Nickname;
                    //userToUpdate.GroupId = user.GroupId;
                    //userToUpdate.LoginName = user.LoginName;
                    //userToUpdate.Email = user.Email;
                    //userToUpdate.LangId = user.LangId;

                    if (user.Password!="" && user.Password != "******")
                    {
                        user.Password = Encryption.Encrypt(user.Password, true);
                    }
                    else
                    {
                        _context.Entry(user).Property(a => a.Password).IsModified = false;
                    }

                    user.LoginName = Functions.RemoveHtml(user.LoginName);
                    user.Nickname = Functions.RemoveHtml(user.Nickname);
                    user.Fullname = Functions.RemoveHtml(user.Fullname);
                    user.Email = Functions.RemoveHtml(user.Email);

                    //_context.Update(user);
                    await _context.SaveChangesAsync();
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        TempData["error"] = "User not found...";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GroupId"] = new SelectList(_context.Groups.Where(a => a.Deleted == 0), "Id", "Name", user.GroupId);
            ViewData["LangId"] = new SelectList(_context.Languages.Where(a => a.Deleted == 0), "Id", "Name", user.LangId);
            return View(user);
        }
        [Authorize]
        [PEF.AuthorizedAction]
        //[Authorize(Roles = "Admin")]
        // GET: Control/Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Group)
                .Include(u => u.Language)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        [Authorize]
        [PEF.AuthorizedAction]
        //[Authorize(Roles = "Admin")]
        // POST: Control/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'DataContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.Deleted = 1;
                _context.Update(user);
                // _context.Users.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
          return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        
    }

   
}
