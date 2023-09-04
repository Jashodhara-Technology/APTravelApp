using System.ComponentModel.DataAnnotations;

namespace APTravelApp.Models
{
	public class Customer
	{
		public int FormId { get; set; }
		public int Id { get; set; }
		[Required, StringLength(100)]
		public string Name { get; set; }

        [Required, StringLength(150)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } 

        //[Required(ErrorMessage = "You must provide a phone number")]
        [Display(Name = "Phone No.")]
        [DataType(DataType.PhoneNumber)]
        //[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        [RegularExpression(@"(^[0-9]{10}$)|(^\+[0-9]{2}\s+[0-9]{2}[0-9]{8}$)|(^[0-9]{3}-[0-9]{4}-[0-9]{4}$)", ErrorMessage = "Not a valid phone number")]
        [StringLength(15, MinimumLength = 10)]
        public string PhoneNo { get; set; }
        [Required]
        public string Message { get; set; }


	}
}
