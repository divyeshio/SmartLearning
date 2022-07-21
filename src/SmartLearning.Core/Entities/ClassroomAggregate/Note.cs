using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using SmartLearning.Core.Entities.UsersAggregate;
using SmartLearning.Core.Helpers;
using SmartLearning.SharedKernel;

namespace SmartLearning.Core.Entities.ClassroomAggregate;

public class Note : EntityBase
{
  [Display(Name = "Chapter")]
  [Required(ErrorMessage = "Please Select A Chapter")]
  public int ChapterId { get; set; }
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
