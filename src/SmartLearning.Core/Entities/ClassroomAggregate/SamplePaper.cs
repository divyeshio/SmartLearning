using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using SmartLearning.Core.Entities.UsersAggregate;
using SmartLearning.Core.Helpers;
using SmartLearning.SharedKernel;

namespace SmartLearning.Core.Entities.ClassroomAggregate
{
  public class SamplePaper : EntityBase
  {
    [Required]
    [DataType(DataType.Url)]
    public string SamplePaperUrl { get; set; }
    [Required]
    public string FileName { get; set; }
    [NotMapped, AllowedFileExtensions(new string[] { ".pdf" })]
    public IFormFile File { get; set; }

    [Required]
    [Column(TypeName = "smallint")]
    [Range(1900, 3000)]
    public int Year { get; set; }

    [Required]
    public int ClassId { get; set; }
    public Classroom Class { get; set; }

    public string UploadedById { get; set; }
    public ApplicationUser UploadedBy { get; set; }
  }
}
