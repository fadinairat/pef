using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Permissions;
using PEF.Areas.Control.Controllers;

namespace PEF.Models
{
    public class Jobs
    {
      
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Employer")]
        [Display(Name="Employer/Association")]
        public int EmployerId { get; set; }   //The Association/Employer who add the adv
        public virtual Employers? Employer { get; set; }


        [Display(Name = "Title")]
        [StringLength(500)]
        public string? Name { get; set; } = String.Empty;

        [Required]
        [Display(Name = "Arabic Title")]
        [StringLength(500)]
        public string ArName { get; set; } = String.Empty;

        [ForeignKey("Project")]
        [Display(Name = "Job Project")]
        public int? ProjectId { get; set; }

        public virtual Projects? Project { get; set; }

        [Required]
        [Display(Name = "Vacancy Type")]
        public int Type { get; set; } = 1;   //1: Employment, 2: Paid Training, 3: Not-Paid Training, 4: Support small projects

        [Required]
        [Display(Name = "Beneficiary Count")]
        public int BeneficiaryCount { get; set; } = 999999;


        [Display(Name = "Description")]
        public string? Description { get; set; } = String.Empty;

        [Required]
        [Display(Name = "Arabic Description")]
        public string ArDescription { get; set; } = String.Empty;

        [Required]
        [Display(Name = "Salary Type")]
        public int? SalaryType { get; set; } = 1;   //0: Daily, 1: Monthly


        [Display(Name = "Salary")]
        public int? Salary { get; set; } = null;


        [Display(Name = "Salary Currency")]
        [ForeignKey("Currency")]
        public int? CurrencyId { get; set; } = null;
        public virtual LookupCurrencies? Currency { get; set; }

        [Display(Name = "Job Featured")]
        public Boolean Featured { get; set; } = false;

        //[Required]
        //[Display(Name = "Type")]
        //[ForeignKey("Type")]
        //public int TypeId { get; set; } = 1;   //1: Paid Employment, 2: Paid Training, 3: Free Training , 4: Small Business
        //public virtual LookupJobTypes? Type {get;set;}

        [Required]
        [Display(Name = "The Nature of Work")]
        public int? WorkNature { get; set; } = 1;   //0: Part time, 1: Full Time

        [Display(Name = "City")]
        public int? CityId { get; set; } 

        public virtual City? City { get; set; }

        
        [Display(Name = "Job Tasks")]
        public string? Tasks { get; set; } = String.Empty;

        [Required]
        [Display(Name = "Arabic Job Tasks")]
        public string? ArTasks { get; set; } = String.Empty;

       
        [Display(Name = "Conditions")]
        public string? Conditions { get; set; } = String.Empty;
        
        [Display(Name = "Arabic Conditions")]
        public string? ArConditions { get; set; } = String.Empty;

        [Display(Name = "Contacts")]
        public string? Contacts { get; set; } = String.Empty;

        [Display(Name = "Arabic Contacts")]
        public string? ArContacts { get; set; } = String.Empty;

        /// <summary>
        /// The Job Selection Creteria
        /// </summary>

        [Display(Name = "Gender")]
        public int? SelGender { get; set; } = null; //Default is Both

        [Display(Name = "Social Status")]
        [ForeignKey("SocialStatus")]
        public int? SelSocialId { get; set; } = null;
        public virtual LookupSocialStatus? SocialStatus { get; set; } = null;

        [Display(Name = "From Age")]
        public int? SelFromAge { get; set; } = null;


        [Display(Name = "To Age")]
        public int? SelToAge { get; set; } = null;


        [Display(Name = "District (WestBank/Gaza/Both)")]
        public string? SelDistrict { get; set; } = "Both";   //WestBank/Gaza/Both

                
        [ForeignKey("Job City")]
        [Display(Name = "Governorate")]
        public int? SelCityId { get; set; }
        public virtual City? SelCity { get; set; }


        [ForeignKey("Village")]
        [Display(Name = "Village/District")]
        public int? SelVillageId { get; set; }
        public virtual Villages? Village { get; set; }


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [Display(Name = "Add Date")]
        public DateTime AddDate { get; set; } = DateTime.Now;


        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [Display(Name = "Advertisment Start Date")]
        public DateTime? StartDate { get; set; } = DateTime.Now;


        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [Display(Name = "Advertisment Expiry Date")]
        public DateTime? ExpireDate { get; set; } = DateTime.Now;


        [Display(Name = "Approved")]
        public Boolean Approved { get; set; } = false;
        public Boolean Deleted { get; set; } = false;

        //public virtual ICollection<JobsStatusTracking>? JobsStatusTracking { get; set; }
        public virtual ICollection<JobsApplications>? JobsApplications { get; set; }
        public virtual ICollection<JobsFields>? Fields { get; set; }
        public virtual ICollection<FormsEntries>? Entries { get; set; }
        public virtual ICollection<JobsCities>? JobsCities { get; set; }
    }
}
