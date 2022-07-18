using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SmartLearning.Core.Entities.Common;
using SmartLearning.SharedKernel;

namespace SmartLearning.Core.Entities.ClassAggregate
{
  [Index(nameof(Name), IsUnique = true)]
  public class Class : BaseEntity
  {
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [Required]
    [StringLength(20), MinLength(3)]
    public string Name { get; set; }

    [Required]
    [Display(Name = "Board")]
    public long BoardId { get; set; }
    public Board Board { get; set; }

    [Display(Name = "Standard")]
    [Required]
    public string StandardId { get; set; }
    public Standard Standard { get; set; }

    [Required]
    [Display(Name = "Subject")]
    public long SubjectId { get; set; }
    public Subject Subject { get; set; }

    public bool IsRegistrationAllowed { get; set; } = true;



    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();


    public static string GenerateGroupName(string Board, string Standard, string Subject)
    {
      return string.Join("-", Board, Standard, Subject);
    }
  }
}
