using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartLearning.Models;

namespace SmartLearning.Core.Entities.Common
{
  public class FaceData
  {
    [Key]
    public long Id { get; set; }

    [Required]
    public string IFaceEncoding { get; set; }
    [NotMapped]
    public double[] FaceEncoding
    {
      get
      {
        return Array.ConvertAll(IFaceEncoding.Split(';'), double.Parse);
      }
      set
      {
        IFaceEncoding = string.Join(";", value.Select(p => p.ToString()).ToArray());
      }
    }

    [Required]
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }

  }
}
