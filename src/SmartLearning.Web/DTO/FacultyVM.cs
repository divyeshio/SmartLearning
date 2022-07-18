using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SmartLearning.Web.DTO
{
  public class FacultyVM
  {
    [Required]
    [Key]
    public string Id { get; set; }
    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [Required]
    public bool isEnabled { get; set; }


    [Required]
    [Display(Name = "Subject")]
    public long SubjectId { get; set; }
    [Required]
    [Display(Name = "Board")]
    public long BoardId { get; set; }
    [Required]
    [Display(Name = "Standard")]
    public string StandardId { get; set; }

    public SelectList? Boards { get; set; }
    public SelectList? Standards { get; set; }
    public SelectList? Subjects { get; set; }
  }
}
