using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartLearning.Core.Entities.TestAggregate;

namespace SmartLearning.Web.DTO
{
  public class TestQuestionViewModel
  {
    public int Id { get; set; }

    [BindNever]
    [Display(Name = "Chapter")]
    public long ChapterId { get; set; }



    [Required(ErrorMessage = "Question is required")]

    public string Question { get; set; }

    [Required(ErrorMessage = "Option A is required")]
    public string OptionA { get; set; }
    [Required(ErrorMessage = "Option B is required")]
    public string OptionB { get; set; }
    [Required(ErrorMessage = "Option C is required")]
    public string OptionC { get; set; }
    [Required(ErrorMessage = "Option D is required")]
    public string OptionD { get; set; }


    [Required(ErrorMessage = "Correct Answer is required")]
    [EnumDataType(typeof(AnswerOptions))]
    public AnswerOptions Answer { get; set; }

    public SelectList Chapters { get; set; }
    public SelectList Boards { get; set; }
    public SelectList Standards { get; set; }
    public SelectList Subjects { get; set; }

  }
}
