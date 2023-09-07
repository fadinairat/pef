using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PEF.Models
{
    public class MembersEducation
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Job Seeker")]
        [ForeignKey("Member")]
        public int MemberId { get; set; }

        public virtual Members Member { get; set; } = new Members();

        [Required]
        [DisplayName("Education Level Type")]
        public int EducationLevelType { get; set; } // 1: Worker, 2: Profession, 3: Graduated



        [Required]
        [DisplayName("Education Level")]
        [ForeignKey("Education")]
        public int EducationId { get; set; }

        public virtual LookupEducation? Education { get; set; }

        [StringLength(250)]
        [DisplayName("University / College")]
        public string? University { get; set; } = String.Empty;

        [DisplayName("Country")]
        public string? CountryName { get; set; }

        [DisplayName("Country")]
        [ForeignKey("Country")]
        public int? CountryId { get; set; }
        public virtual LookupCountries? Country { get; set; }

        [DataType(DataType.Date)]        
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [DisplayName("Graduation Date")]
        public DateTime? GradDate { get; set; }



        [DisplayName("Graduate Grade")]
        [StringLength(50)]
        public string? Grade { get; set; } = String.Empty;


        [DisplayName("Specialization")]
        [ForeignKey("Specialize")]
        public int? SpecializeId { get; set; }

        public virtual LookupSpecialize? Specialize { get; set; }

        [DisplayName("Certificate Document")]
        [StringLength(250)]
        public string? Attachment { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}")]
        public DateTime? AddTime { get; set; }

        public Boolean? Deleted { get; set; } = false;
                
    }
}
