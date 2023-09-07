using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PEF.Models
{
    public class MembersTraining
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Job Seeker")]
        public int MemberId { get; set; }

        public virtual Members Member {get; set;}

        [Required]
        [DisplayName("Training Name")]
        [StringLength(250)]
        public string? Name { get; set; }

        [Required]
        [DisplayName("Training Hours")]        
        public int Hours { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [DisplayName("Start Date")]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [DisplayName("End Date")]
        public DateTime? EndDate { get; set; }

        [Required]
        [StringLength(250)]
        [DisplayName("From where you got the training?")]
        public string? TrainingSupplier { get; set; } = String.Empty;

        [Required]        
        [DisplayName("Training Tasks")]
        public string? TrainingTasks { get; set; } = String.Empty;

        [DisplayName("Attachments")]
        [StringLength(250)]
        public string Attachment { get; set; } = String.Empty;

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}")]
        public DateTime? AddTime { get; set; }

        public Boolean Deleted { get; set; }
    }
}
