using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using PEF.Models;

namespace PEF.Models
{
    public class DataContext : IdentityDbContext<IdentityUser>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        //System Shared Controller Models
        public DbSet<Language> Languages { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<Villages> Villages { get; set; }
        public DbSet<ContactUs> ContactUs { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Permissions> Permissions { get; set; }
        public DbSet<GroupPermissions> GroupPermissions { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<PageCategory> PagesCategories { get; set; }
        public DbSet<CategoryTypes> Category_Types { get; set; }
        public DbSet<Files> Files { get; set; }
        public DbSet<FilesList> FilesList { get; set; }
        public DbSet<FilesType> FileType { get; set; }
        public DbSet<HtmlTemplate> HtmlTemplates { get; set; }
        public DbSet<HtmlTemplatesType> HtmlTemplatesTypes { get; set; }
        public DbSet<AdminLog> AdminLogs { get; set; }
        public DbSet<AdminLogFor> AdminLogFor { get; set; }
        //public DbSet<AdminLogAction> AdminLogAction { get; set; }
        public DbSet<MenuLocation> MenuLocations { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TagRel> TagsRel { get; set; }
        public DbSet<PEF.Models.Author> Author { get; set; } = default!;

        //////////////////// Lookup Tables //////////////////////
        ///
        public DbSet<LookupEducation> LookupEducation { get; set; }
        public DbSet<LookupSpecialize> LookupSpecialize { get; set; }
        public DbSet<LookupJobsFields> LookupJobsFields { get; set; }
        public DbSet<LookupProjectsProgs> LookupProjectsProgs { get; set; }
        public DbSet<LookupSocialStatus> LookupSocialStatus { get; set; }
        public DbSet<LookupCurrencies> LookupCurrencies { get; set; }
        public DbSet<LookupWorkSectors> LookupWorkSectors { get; set; }
        public DbSet<LookupCountries> LookupCountries { get; set; }
        public DbSet<PEF.Models.LookupEmployersSectors> LookupEmployersSectors { get; set; } = default!;
        public DbSet<Lookups> Lookups { get; set; }


        ///////////////////////////// Context for Job Seekers ///////////////////////
        public DbSet<Members> Members { get; set; }
        public DbSet<MembersEducation> MembersEducation { get; set; }
        public DbSet<MembersExpert> MembersExpert { get; set; }
        public DbSet<MembersFamily> MembersFamily { get; set; }
        public DbSet<MembersTraining> MembersTraining { get; set; }
        public DbSet<PEF.Models.Projects> Projects { get; set; } = default!;

        public DbSet<PEF.Models.JobsStatusTracking> JobsStatusTracking { get; set; } = default!;
        public DbSet<PEF.Models.Employers> Employers { get; set; } = default!;
        public DbSet<PEF.Models.Jobs> Jobs { get; set; } = default!;
        public DbSet<PEF.Models.Donors> Donors { get; set; } = default!;
        public DbSet<PEF.Models.JobsFields> JobsFields { get; set; } = default!;

        public DbSet<PEF.Models.ProjectsEmployers> ProjectsEmployers { get; set; } = default!;
        public DbSet<PEF.Models.JobsApplications> JobsApplications { get; set; } = default!;
        public DbSet<PEF.Models.JobsAppsAttachments> JobsAppsAttachments { get; set; } = default!;
        public DbSet<PEF.Models.JobsCities> JobsCities { get; set; } = default!;
        public DbSet<PEF.Models.UsersProjects> UsersProjects { get; set; } = default!;


        //Forms Fields 
        public DbSet<PEF.Models.Forms> Forms { get; set; } = default!;
        public DbSet<PEF.Models.FormsFields> FormsFields { get; set; } = default!;
        public DbSet<PEF.Models.FormsFieldsOptions> FormsFieldsOptions { get; set; } = default!;
        public DbSet<PEF.Models.FormsFieldsTypes> FormsFieldsTypes { get; set; } = default!;
        public DbSet<PEF.Models.FormsEntries> FormsEntries { get; set; } = default!;
        public DbSet<PEF.Models.FormsEntriesFields> FormsEntriesFields { get; set; } = default!;
        public DbSet<PEF.Models.Visits> Visits { get; set; } = default!;
        
    }
}
