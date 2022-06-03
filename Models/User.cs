using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserAssignmentRedone.Models
{
	public class User
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		
		[Display(Name = "National ID Number")]
		[Column(TypeName = "nvarchar(10)")]
		[RegularExpression(@"\d{10}", ErrorMessage = "Please enter a valid 10-digit number.")]
		public string NationalId { get; set; }

		[Column(TypeName="nvarchar(20)")]
		[RegularExpression(@"^[a-z,A-Z]+[a-z,A-Z,0-9_-]{5,20}$", ErrorMessage = "Please enter a unique username between 6 and 20 characters.")]
		public string Username { get; set; }
		
		[Column(TypeName = "nvarchar(20)")]
		[MinLength(3)]
		[RegularExpression(@"^[a-z,A-Z,.'-]{3,20}$", ErrorMessage = "Please enter a name containing at least 3 letters.")]
		public string FirstName { get; set; }
		
		[Column(TypeName = "nvarchar(20)")]
		[MinLength(3)]
		[RegularExpression(@"^[a-z, A-Z ,.'-]{3,20}$", ErrorMessage = "Please enter a name containing at least 3 letters.")]
		public string LastName { get; set; }

		
		[DataType(DataType.Date)]
		
		public DateTime BirthDate { get; set; }


		[RegularExpression(@"0+9+\d{9}", ErrorMessage = "Please enter a phone number using following format: 09XXXXXXXXX.")]
		public string PhoneNumber { get; set; }
		
		
		public User()
		{
		}
	}
}

