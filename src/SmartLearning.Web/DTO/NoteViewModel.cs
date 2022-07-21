using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartLearning.Core.Helpers;

namespace SmartLearning.Web.DTO
{
    public class NoteViewModel
    {
        [Required]
        public int Id { get; set; }
        public SelectList Chapters { get; set; }
        public SelectList Boards { get; set; }
        public SelectList Standards { get; set; }
        public SelectList Subjects { get; set; }

        [Display(Name = "Chapter")]
        [Required(ErrorMessage = "Please Select A Chapter")]
        public int ChapterId { get; set; }

        [AllowedFileExtensions(new string[] { ".pdf" })]
        public IFormFile NoteFile { get; set; }
    }
}
