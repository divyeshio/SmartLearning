using SmartLearning.Core.Entities.UsersAggregate;

namespace SmartLearning.Web.DTO
{
  public class AdminDashboardViewModel
  {
    public ApplicationUser User { get; set; }

    public int AdminCount { get; set; }
    public int StudentsCount { get; set; }
    public int FacultyCount { get; set; }
    public int ClassesCount { get; set; }
    public int TestCount { get; set; }
    public int NotesCount { get; set; }
    public int SamplePapersCount { get; set; }
    public int ReferenceBookCount { get; set; }
    public int BoardsCount { get; set; }
    public int StandardsCount { get; set; }
    public int SubjectsCount { get; set; }
  }
}
