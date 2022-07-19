using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartLearning.Core.Entities.UsersAggregate;

namespace SmartLearning.Web.DTO.AccountViewModels
{
  public class ExternalLoginConfirmationViewModel
  {
    [Required]
    [Display(Name = "First Name")]
    [StringLength(10, MinimumLength = 3)]
    public string FirstName { get; set; }
    [Required]
    [Display(Name = "Last Name")]
    [StringLength(15, MinimumLength = 3)]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    public AccountTypeEnum AccountType { get; set; }

    [Display(Name = "Subject")]
    public long? SubjectId { get; set; }
    [Required]
    [Display(Name = "Board")]
    public int BoardId { get; set; }
    [Required]
    [Display(Name = "Standard")]
    public int StandardId { get; set; }

    public SelectList Boards { get; set; }
    public SelectList Standards { get; set; }
    public SelectList Subjects { get; set; }
  }
}
