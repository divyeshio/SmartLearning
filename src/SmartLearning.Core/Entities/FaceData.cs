using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartLearning.Models
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
        return Array.ConvertAll(IFaceEncoding.Split(';'), Double.Parse);
      }
      set
      {
        IFaceEncoding = String.Join(";", value.Select(p => p.ToString()).ToArray());
      }
    }

    [Required]
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }

  }
}
