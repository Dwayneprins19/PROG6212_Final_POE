using System.ComponentModel.DataAnnotations;

namespace CMCSProject.Models
{
	public class Claim
	{
		public int Id { get; set; }	

		[Required]
		[Display(Name ="Lecturer Name")]
		public string LecturerName { get; set; }

		[Required]
		[Range(1, 200, ErrorMessage = "Hours worked must be between 1 and 200")]
		[Display(Name ="Hours Worked")]
		public decimal HoursWorked { get; set; }
		
		[Required]
		[Range(50, 1000, ErrorMessage = "Hourly rate must be bewteen 50 and 1000.")]
		[Display(Name ="Hourly Rate (R)")]
		public decimal HourlyRate { get; set; }

		[Display(Name = "Total Amount (R)")]
		[DataType(DataType.Currency)]
		public decimal TotalAmount => HoursWorked * HourlyRate; 

		[Display(Name = "Status")]
		public string Status { get; set; } = "Pending";

		[Display(Name ="Uploaded Document")]
		public string UploadedDocument { get; set; }

		[Display(Name = "Additional Notes")]
		[MaxLength(250)]
		public string Notes { get; set; }

		public DateTime DateSubmitted { get; set; } = DateTime.Now;
	}
}
