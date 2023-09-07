using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PEF.Models
{
    public class MembersExpert
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Job Seeker")]
        [ForeignKey("Member")]
        public int? MemberId { get; set; }

        public virtual Members? Member { get; set; }

        [Required]
        [StringLength(250)]
        [DisplayName("Job Title")]
        public string JobTitle { get; set; } = String.Empty;

        [Required]
        [StringLength(250)]
        [DisplayName("Work Location (Association / Company / Government Department)")]
        public string JobLocation { get; set; } = String.Empty;

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [DisplayName("Start Date")]
        public DateTime? StartDate { get; set; } = null;

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [DisplayName("End Date")]
        public DateTime? EndDate { get; set; } = null;

        [DisplayName("Expert Years")]
        public int Years { get; set; } = 0;

        [Required]
        [DisplayName("Experiment Description")]
        public string? ExpDescription { get; set; } = String.Empty;

        //[Required]
        //public int Period { get; set; } = 0;  //Period will be in months

        [DisplayName("Expertise Certificate")]
        [StringLength(250)]
        public string Attachment { get; set; } =  String.Empty;

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}")]
        public DateTime? AddTime { get; set; }

        public Boolean Deleted { get; set; }
    }
}
