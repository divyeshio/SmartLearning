using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using SmartLearning.Helpers;
using SmartLearning.SharedKernel;

namespace SmartLearning.Core.Entities.ClassAggregate
{
  public class Note : BaseEntity
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
