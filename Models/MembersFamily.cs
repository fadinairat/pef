using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PEF.Models
{
    public class MembersFamily
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Member")]
        [DisplayName("Job Seeker")]
        public int? MemberId { get; set; }

        public virtual Members? Member { get; set; }

        [Required]
        [StringLength(250)]
        [DisplayName("Full Name")]
        public string FullName { get; set; } = String.Empty;

        [Required]        
        [DisplayName("Gender")]
        public int Gender { get; set; }

        [Required]
        [DisplayName("Relative Relation")]
        public int Relation { get; set; } // 1: Son, 2: Daughter, 3: Wife, 4: Husband, 5: Brother, 6: Sister, 7:Father, 8: Mother, 9:Other

        [Required]
        [DisplayName("Date of Birth")]
        public DateTime? BirthDate { get; set; }

        [Required]
        [DisplayName("ID Number")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Invalid ID number!"), StringLength(9, MinimumLength = 9)]
        public string? IdNum { get; set; }

        [Required]
        [DisplayName("Education Level")]
        [ForeignKey("Education")]
        public int? EducationId { get; set; }
        public virtual LookupEducation? Education { get; set; }

        [Required]
        [DisplayName("Health Status")]
        public int HealthStatus { get; set; } = 0; //0: Healthy , 1: Have Disease, 2:Handicapped

        
        [StringLength(250)]
        [DisplayName("Disease")]
        public string Disease { get; set; } = String.Empty;

        [Required]
        [DisplayName("Does he/she have a job?")]
        public Boolean IsWork { get; set; } = false;

        [Required]
        [StringLength(250)]
        [DisplayName("Job")]
        public string Job { get; set; } = String.Empty;


        [DisplayName("Creation Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}")]
        public DateTime AddDate { get; set; } = DateTime.Now;

        public Boolean Deleted { get; set; } = false;
    }
}
