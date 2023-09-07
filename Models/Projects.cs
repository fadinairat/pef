using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PEF.Models
{
    public class Projects
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Program Type")]
        [ForeignKey("Program")]
        public int ProgramType { get; set; } = 1;
        public virtual LookupProjectsProgs? Program { get; set; }

        [Required]
        [Display(Name = "Project Type")]
        public int ProjectType { get; set; } = 1; // 1: Grants, 2: Loans

 
        [Display(Name = "Project Name")]
        [StringLength(250)]
        public string? Name { get; set; } = String.Empty;

        [Required]
        [Display(Name = "Arabic Name")]
        [StringLength(250)]
        public string ArName { get; set; } = String.Empty;

        [ForeignKey("Form")]
        [Display(Name = "Job Application Form")]
        public int? FormId {get;set;}
        public virtual Forms? Form { get; set; }
        
        
        [Display(Name = "Internal Page Form")]
        public int? InternalFormId {get;set;}

        [Display(Name = "Donor")]
        [ForeignKey("Donor")]
        public int? DonorId { get; set; }
        public virtual Donors? Donor { get; set; }

        
        [Display(Name = "Description")]
        public string? Description { get; set; } = String.Empty;

        [Display(Name = "Arabic Description")]
        public string? ArDescription { get; set; } = String.Empty;

        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Budget")]
        public decimal? Budget { get; set; } = null;


        [Required]
        [Display(Name = "Currency")]
        [ForeignKey("Currency")]
        public int CurrencyId { get; set; }

        public virtual LookupCurrencies? Currency { get; set; }

        [Display(Name = "Execution Partners")]
        public string? Partners { get; set; } = String.Empty;

        [Display(Name = "Arabic Execution Partners")]
        public string? ArPartners { get; set; } = String.Empty;

        [Display(Name = "Project Activities")]
        public string? ProjectActivities { get; set; } = String.Empty;

        [Display(Name = "Project Arabic Activites")]
        public string? ArProjectActivities { get; set; } = String.Empty;

        [Display(Name = "Target Group")]
        public string? TargetGroup { get; set; } = String.Empty;

        [Display(Name = "Arabic Target Group")]
        public string? ArTargetGroup { get; set; } = String.Empty;

        [Required]
        [Display(Name = "Beneficiary Count")]
        public int BeneficiaryNumbers { get; set; }

        [Display(Name = "Execution Location")]
        [ForeignKey("City")]
        public int? Location { get; set; }

        public virtual City? City { get; set; } = null;


        [Display(Name = "District (WestBank/Gaza/Both)")]
        public string? District { get; set; } = "Both";   //WestBank/Gaza/Both

        [Display(Name = "Target Locations")]
        public string? TargetLocations { get; set; } = null;

        [Display(Name = "Duration")]
        public float Duration { get; set; }

        [Display(Name = "Duration Unit")]
        public int DurationUnit { get; set; } = 1;   //Days/Months/Years


        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Project Coordinator")]
        public string? ProjectCoordinator { get; set; } = String.Empty;

        [Display(Name = "Arabic Project Coordinator")]
        public string? ArProjectCoordinator { get; set; } = String.Empty;

        [Display(Name = "Outputs")]
        public string? Outputs { get; set; } = String.Empty;

        [Display(Name = "Arabic Outputs")]
        public string? ArOutputs { get; set; } = String.Empty;

        [Display(Name = "Conditions")]
        public string? Conditions { get; set; } = String.Empty;


        [Display(Name = "Arabic Conditions")]
        public string? ArConditions { get; set; } = String.Empty;

        [Display(Name = "Project Image")]
        public string? ProjectImage { get; set; } = String.Empty;

        [Display(Name = "Standards")]
        public string? Standards { get; set; } = String.Empty;

        [Display(Name = "Arabic Standards")]
        public string? ArStandards { get; set; } = String.Empty;

        [Display(Name = "Notes")]
        public string? Notes { get; set; } = String.Empty;
        [Display(Name = "Arabic Notes")]
        public string? ArNotes { get; set; } = String.Empty;


        [Required]
        [Display(Name = "Added By")]
        [ForeignKey("User")]
        public int AddedBy { get; set; }

        public virtual User? User { get; set; }



        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}")]
        public DateTime AddedTime { get; set; } = DateTime.Now;

        [ForeignKey("EdUser")]
        public int? EditedBy { get; set; }

        //public virtual User EdUser { get; set; } = new User();


        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}")]
        public DateTime? LastEdit { get; set; }


        public Boolean Active { get; set; } = true;

        public Boolean Deleted { get; set; } = false;

        public virtual ICollection<Jobs>? Jobs { get; set; }
        public virtual ICollection<ProjectsEmployers>? ProjectEmployers {get; set;}
        public virtual ICollection<UsersProjects>? UsersProjects { get; set; }


    }
}
