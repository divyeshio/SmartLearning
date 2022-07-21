
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartLearning.Core.Helpers;

namespace SmartLearning.Web.DTO
{
    public class SamplePaperViewModel
    {
        [Required]
        public int Id { get; set; }
        public SelectList Boards { get; set; }
        public SelectList Standards { get; set; }
        public SelectList Subjects { get; set; }
        public SelectList Classes { get; set; }

        [AllowedFileExtensions(new string[] { ".pdf" })]
        public IFormFile File { get; set; }

        [Required]
        [Range(2000, 3000)]
        public int Year { get; set; }

        public int ClassId { get; set; }
    }
}
