using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using SmartLearning.Core.Entities.BoardAggregate;
using SmartLearning.Core.Entities.ClassroomAggregate;
using SmartLearning.Core.Entities.Common;
using SmartLearning.Core.Entities.StandardAggregate;
using SmartLearning.Core.Entities.SubjectAggregate;
using SmartLearning.Core.Entities.TestAggregate;
using SmartLearning.SharedKernel.Interfaces;

namespace SmartLearning.Core.Entities.UsersAggregate
{
  public class ApplicationUser : IdentityUser, IAggregateRoot
  {
   /* [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();*/

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
    public string? Avatar { get; set; } = "default.jpg";

    [Required]
    [DataType(DataType.Date)]
    public DateTime DoJ { get; } = DateTime.Today.Date;

    public bool isEnabled { get; set; } = true;

    [Required]
    [EnumDataType(typeof(AccountTypeEnum))]
    [Column(TypeName = "nvarchar(24)")]
    [Display(Name = "Account Type")]
    public AccountTypeEnum AccountType { get; set; }
    public int? StandardId { get; set; }
    public Standard? Standard { get; set; }
    public int? BoardId { get; set; }
    public Board? Board { get; set; }
    public int? SubjectId { get; set; }
    public Subject? Subject { get; set; }

    public int? FaceDataId { get; set; }
    public FaceData? FaceData { get; set; }
    public ICollection<Classroom> Classes { get; } = new List<Classroom>();
    public ICollection<TestAttempt>? TestAttempts { get; set; }
    public ICollection<Message> Messages { get; } = new List<Message>();
  }
}
