using PEF.Resources.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PEF.Models
{
    public class Employers
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]        
        [StringLength(250, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(ValidationResources))]        
        [Display(Name = "Name")]
        public string Name { get; set; } = String.Empty;

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
        [StringLength(250, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(ValidationResources))]
        [Display(Name = "Arabic Name")]
        public string ArName { get; set; } = String.Empty;

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
        [StringLength(250, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(ValidationResources))]
        [Display(Name = "Shortcut")]
        public string Shortcut { get; set; } = String.Empty;

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
        [Display(Name = "Type")]
        public int Type { get; set; } = 1; //1: Employer, 2: Association


        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
        [Display(Name = "Sector")]
        [ForeignKey("EmployerSector")]
        public int Sector { get; set; }  //1: Private Sector, 2: Public Sector, 3: NGO, 4:International Associations
        //5: Public Normal co. 6: Public limited co., 7: Limited Respon Co., 8: Private Joint Co., 9: Public Joint Co.
        public virtual LookupEmployersSectors? EmployerSector { get; set; }

        [Display(Name = "Other Sectors"),MaxLength(250)]
        public string? SectorOther {get;set;}

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
        [Display(Name = "Economic Sectors")]
        [ForeignKey("WorkSector")]
        public int? WorkSectorId { get; set; } //Work Sector like (Commercial, Services, health, learning, industrial)
        public virtual LookupWorkSectors? WorkSector { get; set; }

        [Display(Name = "Other Economic Sectors"), MaxLength(250)]
        public string? WorkSectorOther { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
        [Display(Name = "Email (Username)")]
        [RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$", ErrorMessageResourceName = "EmailRegulareExpression", ErrorMessageResourceType = typeof(ValidationResources))]
        [StringLength(250, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(ValidationResources))]
        public string? Email { get; set; } = String.Empty;

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
        [StringLength(250, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(ValidationResources))]
        [Display(Name = "Password")]
        public string Password { get; set; } = String.Empty;

        [Display(Name = "Confirm Password")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
        [Compare("Password",ErrorMessageResourceName = "ConfirmNotMatch", ErrorMessageResourceType = typeof(ValidationResources))]
        
        public string ConfirmPassword { get; set; } = String.Empty;

        [Display(Name = "Establish Year")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
        public int EstablishYear { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
        [Display(Name = "Registration Number")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(ValidationResources))]
        public string RegNumber { get; set; } = String.Empty; //Registation ID

        
        [Display(Name = "Operation Number")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(ValidationResources))]
        public string? OperationNumber { get; set; } = String.Empty;  //مشتغل مرخص

        /// <summary>
        /// Contct Details
        /// </summary>
        /// 


        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
        [Display(Name = "Province")]
        public int? CityId { get; set; }
        [Display(Name = "Province")]
        public virtual City? City { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
        [Display(Name = "City / Area")]        
        [ForeignKey("Area")]
        public int AreaId { get; set; } 
        public virtual Villages? Area { get; set; } 
        
        [Display(Name = "Neighborhood/Village")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(ValidationResources))]
        public string? Village { get; set; } = String.Empty;

        
        [Display(Name = "Mobile")]
        [RegularExpression(@"^05\d{8}$", ErrorMessageResourceName = "MobileRegularExpression", ErrorMessageResourceType = typeof(ValidationResources))]
        //[RegularExpression(@"^05\d{8}$", ErrorMessage = "Invalid Mobile Number (05XX-XXX-XXX)"), StringLength(10, MinimumLength = 10)]
        public string? Mobile { get; set; } = String.Empty;

        [Display(Name = "Tel")]
        [RegularExpression(@"^0\d{8}$", ErrorMessageResourceName = "TelRegularExpression", ErrorMessageResourceType = typeof(ValidationResources))]
        //[RegularExpression(@"^0\d{8}$", ErrorMessage = "Invalid Telephone Number (05XX-XXX-XXX)"), StringLength(9   , MinimumLength = 9)]
        public string? Tel { get; set; } = String.Empty;

        [Display(Name = "Detailed Address")]
        [StringLength(500, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(ValidationResources))]
        public string? Address { get; set; } = String.Empty;


        //Contact Person

        [Display(Name = "Contact Name")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(ValidationResources))]
        public string? ContactName { get; set; } = String.Empty;

        [Display(Name = "Job Title")]
        [StringLength(250, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(ValidationResources))]
        public string? contactJobTitle { get; set; } = String.Empty;  //المسمى الوظيفي

        [Display(Name = "Mobile")]
        [RegularExpression(@"^05\d{8}$", ErrorMessageResourceName = "MobileRegularExpression", ErrorMessageResourceType = typeof(ValidationResources))]
        //[RegularExpression(@"^05\d{8}$", ErrorMessage = "Invalid Mobile Number (05XX-XXX-XXX)"), StringLength(10, MinimumLength = 10)]
        public string? ContactMobile { get; set; } = String.Empty;

        [Display(Name = "Mobile 2")]
        [RegularExpression(@"^05\d{8}$", ErrorMessageResourceName = "MobileRegularExpression", ErrorMessageResourceType = typeof(ValidationResources))]
        //[RegularExpression(@"^05\d{8}$", ErrorMessage = "Invalid Mobile Number (05XX-XXX-XXX)"), StringLength(10, MinimumLength = 10)]        
        public string? ContactMobile2 { get; set; } = String.Empty;

        [Display(Name = "Contact Email")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(ValidationResources))]
        [RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$", ErrorMessageResourceName = "EmailRegulareExpression", ErrorMessageResourceType = typeof(ValidationResources))]
        public string? ContactEmail { get; set; } = String.Empty;


        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Arabic Description")]
        public string? ArDescription { get; set; }

        [Display(Name = "Extra Details")]
        public string? ExtraData { get; set; }

        [Display(Name = "Arabic Extra Details")]
        public string? ArExtraData { get; set; }

        [Display(Name = "Safety Procedures")]
        public string? SafetyProcedures { get; set; }


        [Display(Name = "Arabic Safety Procedures")]
        public string? ArSafetyProcedures { get; set; }


        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
        [Display(Name = "Permanent Male Employees Count")]
        public int? PermanentEmpCount { get; set; } = 0;

        
        [Display(Name = "Permanent Female Employees Count")]
        public int? PermanentFemaleEmpCount { get; set; } = 0;

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
        [Display(Name = "Part-Time Male Employees Count")]
        public int? ParttimeEmpCount { get; set; } = 0;

        
        [Display(Name = "Part-Time Femail Employees Count")]
        public int? ParttimeFemaleEmpCount { get; set; } = 0;

        
        [Display(Name = "Full-Time Male Employees Count")]
        public int? FulltimeEmpCount { get; set; } = 0;

        
        [Display(Name = "Full-Time Female Employees Count")]
        public int? FulltimeFemaleEmpCount { get; set; } = 0;

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
        [Display(Name = "Week work days")]
        public int? WeekWorkDays { get; set; } = 0;

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
        [Display(Name = "Day work hours")]
        public int? DayWorkHours { get; set; } = 0;


        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
        [Display(Name = "Does the employer have work injuries insurance?")]
        public Boolean InjuredInsurance { get; set; } = false;

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
        [Display(Name = "Does the operator have health insurance for the employees?")]
        public Boolean HealthInsurance { get; set; } = false; 
        
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
        [Display(Name = "Host Job Trainees?")]
        public Boolean AcceptTrainers { get; set; } = false;

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
        [Display(Name = "Number of branches")]
        public int Branches { get; set; } = 1;

        
        [Display(Name = "Branches Location")]        
        public string? BranchesLocation { get; set; }

        [Display(Name = "Financial Report")]
        [StringLength(250, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(ValidationResources))]
        public string? FinancialReport { get; set; }

        
        [Display(Name = "Administrative Report")]
        [StringLength(250, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(ValidationResources))]
        public string? AnnualReport { get; set; }

        
        [Display(Name = "Registration Certificate")]
        [StringLength(250, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(ValidationResources))]
        public string? RegistrationDocument { get; set; }

        [Display(Name = "Commercial Register")]
        [StringLength(250, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(ValidationResources))]
        public string? CommercialRegister { get; set; }

        [Display(Name = "Other Attachments")]
        [StringLength(250, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(ValidationResources))]
        public string? OtherAttachments { get; set; }

        [Display(Name = "Logo")]
        [StringLength(250, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(ValidationResources))]
        public string? Logo { get; set; }

        [Display(Name = "Add Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime? AddDate { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}")]
        public DateTime? LastLogin { get; set; }

        public int? ActivationCode { get; set; } = null;

        public Boolean Active { get; set; } = false;

        public Boolean Deleted { get; set; } = false;

        public virtual ICollection<Jobs>? Jobs { get; set; } 
        public virtual ICollection<ProjectsEmployers>? ProjectEmployers { get; set; }
        public virtual ICollection<JobsStatusTracking>? JobsStatusTracking { get; set; }
        public virtual ICollection<FormsEntries>? FormsEntries { get; set; }
    }
}
