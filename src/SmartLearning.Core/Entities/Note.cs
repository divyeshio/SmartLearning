using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using SmartLearning.Helpers;

namespace SmartLearning.Models
{
  public class Note
  {
    [Key]
    [Required]
    public long Id { get; set; }

    [Display(Name = "Chapter")]
    [Required(ErrorMessage = "Please Select A Chapter")]
    public long ChapterId { get; set; }
    public Chapter Chapter { get; set; }

    [NotMapped, AllowedFileExtensions(new string[] { ".pdf" })]
    public IFormFile NoteFile { get; set; }

    [Required]
    public string FileName { get; set; }

    [Required]
    [Display(Name = "Url")]
    public string NoteUrl { get; set; }

    public string UploadedById { get; set; }
    public ApplicationUser UploadedBy { get; set; }
  }
}
