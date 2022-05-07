using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SmartLearning.Models
{
  public class ApplicationUser : IdentityUser
  {
    public bool? AdminApproved { get; set; }
    [Required]
    [Display(Name = "First Name")]
    [StringLength(10, MinimumLength = 3)]
    public string? FirstName { get; set; }
    [Required]
    [Display(Name = "Last Name")]
    [StringLength(15, MinimumLength = 3)]
    public string? LastName { get; set; }
    [NotMapped]
    public string FullName
    {
      get { return FirstName + ' ' + LastName; }
      set { FullName = value; }
    }
    public string? Avatar { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime DoJ { get; } = DateTime.Today.Date;

    public bool isEnabled { get; set; } = true;

    [Required]
    [EnumDataType(typeof(AccountTypeEnum))]
    [Column(TypeName = "nvarchar(24)")]
    [Display(Name = "Account Type")]
    public AccountTypeEnum AccountType { get; set; }
    public string? StandardId { get; set; }
    public Standard? Standard { get; set; }
    public long? BoardId { get; set; }
    public Board? Board { get; set; }
    public long? SubjectId { get; set; }
    public Subject? Subject { get; set; }

    public long? FaceDataId { get; set; }
    public FaceData? FaceData { get; set; }
    public ICollection<Class> Classes { get; } = new List<Class>();
    public ICollection<TestAttempt>? TestAttempts { get; set; }
    public ICollection<Message> Messages { get; } = new List<Message>();
  }
}
