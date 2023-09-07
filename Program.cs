using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using PEF.Models;
using PEF.Services;
//using PEF.Services.EmailServices;
using PEF.Settings;

//using Serilog;
//using System.Globalization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using System.Globalization;
using Microsoft.AspNetCore.Localization.Routing;

var myAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();


//The AddJsonOptions to enable the DB object serialization to allow max length
builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

//to Enable componenets and there views
builder.Services.AddControllersWithViews()
.AddRazorOptions(options =>
{
    options.ViewLocationFormats.Add("/{0}.cshtml");
});

builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddTransient<IMailService, MailService>();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

builder.Services.AddScoped<IEmailService, EmailService>();


builder.Services.AddDbContext<DataContext>(options =>
{
    //options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));

    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

});




builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/Control/Users/Login";
    options.AccessDeniedPath = "/Control/Users/AccessDenied";

    options.Events = new CookieAuthenticationEvents()
    {
        //OnSigningIn = async context =>
        //{
        //    //Add Role Claim Identity before login is done
        //    var principal = context.Principal;
        //    if(principal.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
        //    {
        //        if(principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value == "fadi@intertech.ps")
        //        {
        //            var claimsIdentity = principal.Identity as ClaimsIdentity;
        //            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
        //        }
        //    }
        //    await Task.CompletedTask;
        //},
        //OnSignedIn = async conext =>
        //{
        //    await Task.CompletedTask;
        //},
        //OnValidatePrincipal = async context  =>
        //{
        //    await Task.CompletedTask;
        //}
    };
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins, builder =>
    {
        builder.WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});


////////////////  Configure the localization for website translation //////////////
builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("ar-AE");

    var cultures = new CultureInfo[]
    {
        new CultureInfo("ar-AE"),
        new CultureInfo("en")
    };

    options.SupportedCultures = cultures;
    options.SupportedUICultures = cultures;

    // Add supported cultures for the Area
    options.RequestCultureProviders.Insert(0, new RouteDataRequestCultureProvider
    {
        RouteDataStringKey = "lang",
        UIRouteDataStringKey = "lang"
    });
});
//AddDataAnnotationsLocalization very important to use the localization with models (for example the name of field)
//
builder.Services.AddControllersWithViews()
        .AddViewLocalization().AddDataAnnotationsLocalization();

//options =>
//{
//    options.DataAnnotationLocalizerProvider = (type, factory) =>
//        factory.Create(typeof(PEF.Models.CustomValidationResources));
//}



///////////// End of Localization Settings ///////////

//Write the error log of the application to log file stored on the server, you will find the configuration in the "appsettings.json" file
//var _logger = new LoggerConfiguration()
//.ReadFrom.Configuration(builder.Configuration).Enrich.FromLogContext()     //This code to read config of serilog from appsettings
////.MinimumLevel.Error()
////.WriteTo.File("D:\\.Net Core Sites\\.NetCoreLogging\\ESooq\\WebLog-.log", rollingInterval: RollingInterval.Day)
//.CreateLogger();

//builder.Logging.AddSerilog(_logger);


var app = builder.Build();
//app.UsePathBase("/api");

// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}


if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}
else
{
	app.UseExceptionHandler("/Home/500");
}

app.Use(async (context, next) =>
{
	await next();
    if (context.Response.StatusCode == 404)
    {
        context.Request.Path = "/Home/404";
        await next();
    }
    else if (context.Response.StatusCode == 500)
    {
        context.Request.Path = "/Home/500";
        await next();
    }
    else if (context.Response.StatusCode == 503)
    {
        context.Request.Path = "/Home/500";
    }
});

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/ErrorPage");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}
//else
//{
//    app.UseDeveloperExceptionPage();
//}
////if (app.Environment.IsDevelopment())
////{
////    app.UseDeveloperExceptionPage();
////    //app.UseDatabaseErrorPage();
////}
////else
////{
////    app.UseExceptionHandler("/Home/ErrorPage");
////}

//app.Use(async (context, next) =>
//{
//    if (context.Response.StatusCode == 404)
//    {
//        context.Request.Path = "/Home/NotFound";
//        await next();
//    }
//    else if (context.Response.StatusCode == 500)
//    {
//        context.Request.Path = "/Home/ErrorPage";
//        await next();
//    }
//	else if (context.Response.StatusCode == 503)
//	{
//		context.Request.Path = "/Home/ErrorPage";
//		await next();
//	}
//	await next();
//});


//Enable the Localication Middleware
app.UseRequestLocalization();


//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors(myAllowSpecificOrigins);

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();   //To enable using of HttpContext.Session
app.UseEndpoints(endpoints =>
{

    //endpoints.MapControllerRoute(
    //    name: "GetFile",
    //    pattern: "GetFile/{path}",
    //    defaults: new { controller = "Home", action = "GetFile", path = "" });

    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}"
      );
    

    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}/{Title?}");


    //endpoints.MapControllerRoute(
    //    name: "default",
    //    pattern: "{lang=ar}/{controller=Home}/{action=Index}"
    //);
});
app.MapRazorPages();

app.Run();