using System.ComponentModel.DataAnnotations;
using SmartLearning.Core.Helpers;

namespace SmartLearning.Web.DTO.ManageViewModels
{
  public class IndexViewModel
  {
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }

    [Phone]
    [DataType(DataType.PhoneNumber)]
    [Display(Name = "Phone number")]
    public string PhoneNumber { get; set; }

    [AllowedFileExtensions(new string[] { ".jpg", ".jpeg", ".gif", ".png" })]
    [MaxFileSize(2097152)]
    public IFormFile AvatarFile { get; set; }
    public string Avatar { get; set; }

    public string AccountType { get; set; }

    public string Standard { get; set; }
    public string Subject { get; set; }
    public string Board { get; set; }

  }
}
