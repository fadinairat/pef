using PEF.Resources.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PEF.Models
{
	public class Members
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
		[StringLength(50)]
		[Display(Name = "First Name")]
		public string FirstName { get; set; } = String.Empty;

		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
		[StringLength(50)]
		[Display(Name = "Father Name")]
		public string FatherName { get; set; } = String.Empty;

		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
		[StringLength(50)]
		[Display(Name = "Grand Father Name")]
		public string GrandName { get; set; } = String.Empty;

		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
		[StringLength(50)]
		[Display(Name = "Family Name")]
		public string FamilyName { get; set; } = String.Empty;

		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
		[Display(Name = "ID Number")]
		[RegularExpression(@"^[0-9]*$", ErrorMessageResourceName = "IdRegularExpression", ErrorMessageResourceType = typeof(ValidationResources)), StringLength(9, MinimumLength = 9)]
		public string IdNum { get; set; } = String.Empty;

        
        [Display(Name = "Father ID Number")]
        [RegularExpression(@"^[0-9]*$", ErrorMessageResourceName = "IdRegularExpression", ErrorMessageResourceType = typeof(ValidationResources)), StringLength(9, MinimumLength = 9)]
        public string? FatherIdNum { get; set; } = String.Empty;

        
        [Display(Name = "Mother ID Number")]
        [RegularExpression(@"^[0-9]*$", ErrorMessageResourceName = "IdRegularExpression", ErrorMessageResourceType = typeof(ValidationResources)), StringLength(9, MinimumLength = 9)]
        public string? MotherIdNum { get; set; } = String.Empty;

        [Display(Name = "Husband/Wife Id Number")]
        [RegularExpression(@"^[0-9]*$", ErrorMessageResourceName = "IdRegularExpression", ErrorMessageResourceType = typeof(ValidationResources)), StringLength(9, MinimumLength = 9)]
        public string? PartnerIdNum { get; set; } = String.Empty;

		
		/// New Field
		[Display(Name = "Interest Type"),MaxLength(250)]
		public string? Interest { get; set; } //1: Employment, 2: Paid Training, 3: Not-Paid Training, 4: Support small projects

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
		[Display(Name = "Gender")]
		public int? Gender { get; set; } = 1;  //1:male, 2:Female


		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
		[Display(Name = "Social Status")]
		[ForeignKey("SocialStatus")]
		public int? SocialId { get; set; } = 0;

		public virtual LookupSocialStatus? SocialStatus { get; set; }

		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
		[Display(Name = "Date of Birth")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
		public DateTime BirthDate { get; set; }

		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
		[Display(Name = "Family members count")]
		public int? FamilyMembersCount { get; set; } = 0;

		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
		[ForeignKey("City")]
		[Display(Name = "Governorate")]
		public int? CityId { get; set; }
		public virtual City? City { get; set; }

		[Display(Name = "Area/City")]
		[ForeignKey("Area")]
		public int? AreaId { get; set; } = null;
		public virtual Villages? Area {get;set;} = null;

		[Display(Name = "District")]
		[StringLength(250)]
		public string? District { get; set; } = String.Empty;

		[Display(Name = "Street")]
		[StringLength(250)]
		public string? Street { get; set; } = String.Empty;

		[Display(Name = "Near To")]
		[StringLength(250)]
		public string? NearTo { get; set; } = String.Empty;

		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]		
		[Display(Name = "Mobile Number")]
        [RegularExpression(@"^05\d{8}$", ErrorMessageResourceName = "MobileRegularExpression", ErrorMessageResourceType = typeof(ValidationResources)), StringLength(10, MinimumLength = 10)]
        public string? Mobile { get; set; } = String.Empty;
				
		[Display(Name = "Mobile Number 2")]
        [RegularExpression(@"^05\d{8}$", ErrorMessageResourceName = "MobileRegularExpression", ErrorMessageResourceType = typeof(ValidationResources)), StringLength(10, MinimumLength = 10)]
        public string? Mobile2 { get; set; } = String.Empty;

		
		[Display(Name = "Telephone")]
        [RegularExpression(@"^0\d{8}$", ErrorMessageResourceName = "MobileRegularExpression", ErrorMessageResourceType = typeof(ValidationResources)), StringLength(9, MinimumLength = 9)]
        public string? Tel { get; set; } = String.Empty;

		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
		[StringLength(50)]
        [Display(Name = "Email Address")]
        public string? Email { get; set; } = String.Empty;

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
        [StringLength(250)]
        [Display(Name = "Password")]
        public string Password { get; set; } = String.Empty;

        
        [Display(Name = "Confirm Password")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
        [Compare("Password", ErrorMessageResourceName = "ConfirmNotMatch", ErrorMessageResourceType = typeof(ValidationResources))]
        public string ConfirmPassword { get; set; } = String.Empty;

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
        [Display(Name = "Health Status")]
		public int? HealthStatus { get; set; } = 1; //1: Health, 2:Sick, 3: Handicapped

		[Display(Name = "Disability Type")]
		public int? DisabilityType { get; set; } = null;    //1: MOVE, 2: Listen, 3: See, 4: Mental, 5: Speech

		[Display(Name = "Disability Size")]
		public int? DisabilitySize { get; set; } = 1;  //1: Partial, 2: Totally

		[Display(Name = "Sick Nature")]
		public int? SickNature { get; set; } = 0;  //1: for Heart Disease, 2: Pressure, 3: Sukari, 4: Romatizm

		[Display(Name = "Health Attachment")]
		[StringLength(250)]
		public string? HealthAtt1 { get; set; } = String.Empty;

        [Display(Name = "HealthAtt2")]
        [StringLength(250)]
        public string? HealthAtt2 { get; set; } = String.Empty;

        [Display(Name = "HealthAtt3")]
        [StringLength(250)]
        public string? HealthAtt3 { get; set; } = String.Empty;

		
        [Display(Name = "Property Type")]
		public int? PropertyType { get; set; } = 0; //0: Rent and 1:Owner, 3: for Use

		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
		[Display(Name = "House Nature")]
		public int? HouseNature { get; set; } = 0;  //0: Flat, 1: House, 2: Other

		[Display(Name = "House Type")]
		public int? HouseType { get; set; } //0:Batoon, 1: Zenko, 2:Tent, 3: Caravan

		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
		[Display(Name = "House Size")]
		public int? HouseSize { get; set; }

		[Display(Name = "Profile Picture")]
		[StringLength(250)]
		public string? Photo { get; set; } = String.Empty;

		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
		[Display(Name = "Is there income source?")]
		public Boolean IncomeExist { get; set; } = false;

		[Display(Name = "The name of breadwinner")] //اسم معيل الاسرة
		public string? BreedWinnder { get; set; } = String.Empty;

		[Display(Name = "Breadwinner Type")]
		public int? BreedWinnderType { get; set; } // 0: Himself, 1: Husband/Wife, 2: Brother/Sister, 3: Son, 4: Father, 5: Mother, 6: Other


		[Display(Name = "Source of Income")]
		public string? IncomeSource { get; set; } = String.Empty;

        [Display(Name = "ID Number")]
        [RegularExpression(@"^[0-9]*$", ErrorMessageResourceName = "IdRegularExpression", ErrorMessageResourceType = typeof(ValidationResources)), StringLength(9, MinimumLength = 9)]
        public string? IncomeIdNumber { get; set; } = String.Empty;

        [Display(Name = "Income Value")]
        public int? IncomeValue { get; set; } = 0;

		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
		[Display(Name = "Family members count less than 18")]
		public int? MembersLess18 { get; set; } = 0;

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
        [Display(Name = "Family members count age more than 18 (Live in family)")]
        public int? MembersMore18 { get; set; } = 0;


        [Display(Name = "Attachment(1) Title")]
        [StringLength(250)]
        public string? AttachTitle1 { get; set; } = String.Empty;

        [Display(Name = "Attachment 1")]
		[StringLength(250)]
		public string? Attach1 { get; set; } = String.Empty;

        [Display(Name = "Attachment(2) Title")]
        [StringLength(250)]
        public string? AttachTitle2 { get; set; } = String.Empty;

        [Display(Name = "Attachment 2")]
        [StringLength(250)]
        public string? Attach2 { get; set; } = String.Empty;

        [Display(Name = "Attachment(3) Title")]
        [StringLength(250)]
        public string? AttachTitle3 { get; set; } = String.Empty;

        [Display(Name = "Attachment 3")]
        [StringLength(250)]
        public string? Attach3 { get; set; } = String.Empty;

		//Training Needs
        public Boolean NeedTraining { get; set; } = false;

        [Display(Name = "Training Name")]
        [StringLength(500)]
        public string? TrainingName { get; set; } = String.Empty;

        public int? VerifyCode { get; set; } = null;
        public int? ForgetCode { get; set; } = null;
		public DateTime? ForgetTime { get; set; } = null;

        [Display(Name = "Registration Date")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
		public DateTime? AddDate { get; set; } = DateTime.Now;


		[DataType(DataType.DateTime)]
		[Display(Name = "Last Login")]
		[DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}")]
		public DateTime? LastLogin { get; set; }

		public Boolean Active { get; set; } = false;

		public Boolean Deleted { get; set; } = false;

		public Boolean Completed { get; set; } = false; //For profile completed, to enable the jobs access

        public virtual ICollection<MembersEducation>? MembersEducation { get; set; }
        public virtual ICollection<MembersTraining>? MembersTraining { get; set; }
        public virtual ICollection<MembersFamily>? MembersFamily { get; set; }
        public virtual ICollection<MembersExpert>? MembersExpert { get; set; }
        public virtual ICollection<JobsApplications>? JobsApplications { get; set; }
        public virtual ICollection<JobsAppsAttachments>? JobsAppsAttachments { get; set; }
        public virtual ICollection<FormsEntries>? FormsEntries { get; set; }

    }
}
