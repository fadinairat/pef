using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PEF.Models
{
    public class Donors
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Donor Name")]
        [StringLength(250)]
        public string Name { get; set; } = String.Empty;

        [Required]
        [StringLength(250)]
        [DisplayName("Donor Arabic Name")]
        public string ArName { get; set; } = String.Empty;

        [DisplayName("Description")]
        public string? Description { get; set; } = String.Empty;

        [DisplayName("Arabic Description")]
        public string? ArDescription { get; set; } = String.Empty;


        [DisplayName("Logo")]
        [StringLength(250)]
        public string? Logo { get; set; } = String.Empty;

        [Required]
        [DisplayName("Email")]
        [StringLength(250)]
        public string? Email { get; set; } = String.Empty;

        
        [StringLength(250)]
        [DisplayName("Password")]
        public string? Password { get; set; } = String.Empty;

        [DisplayName("Confirm Password")]
        //[Required(ErrorMessage = "Confirm password is mandatory field")]
        [Compare("Password", ErrorMessage = "Password and confirm password must be identical!")]
        public string? ConfirmPassword { get; set; } = String.Empty;


        [DisplayName("Add Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime? AddDate { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}")]
        public DateTime? LastLogin { get; set; }

        [Required]
        [DisplayName("Added By")]
        [ForeignKey("User")]
        public int AddedBy { get; set; }

        public virtual User? User { get; set; }

        public Boolean Active { get; set; } = false;
        public Boolean Deleted { get; set; } = false;

        public virtual ICollection<Projects>? Projects { get; set; }
    }
}
