using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PEF.Models
{
	public class Comments
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[StringLength(100)]
		public string Name { get; set; } = String.Empty;

        [Required, StringLength(100)]
		public string Email { get; set; } = String.Empty;

        [Required, StringLength(100)]
        public string Mobile { get; set; } = String.Empty;

        [Required, StringLength(100)]
		public string Location { get; set; } = String.Empty;

		[Required]
		[StringLength(100)]
		public string Subject { get; set; } = String.Empty;

		[Required]
		[Column(TypeName = "nvarchar(max)")]
		public string Body { get; set; } = String.Empty;

		public Boolean? Published { get; set; } = false;

		public Boolean? Reviewed { get; set; } = false;

		[DataType(DataType.DateTime)]
		[DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}")]
		public DateTime AddTime { get; set; }

		public Boolean Deleted { get; set; } = false;

	}
}
