using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using SmartLearning.Helpers;
using SmartLearning.Models;

namespace SmartLearning.Core.Entities.ClassAggregate
{
  public class SamplePaper
  {
    [Key]
    [Required]
    public long Id { get; set; }

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
    public string ClassId { get; set; }
    public Class Class { get; set; }

    public string UploadedById { get; set; }
    public ApplicationUser UploadedBy { get; set; }
  }
}
