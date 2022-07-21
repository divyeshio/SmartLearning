using SmartLearning.Core.Entities.ClassroomAggregate;

namespace SmartLearning.Core.Interfaces;

public interface IClassroomService
{
  Task<Classroom> CreateClassroomAsync(int boardId, int standardId, int subjectId);
  Task<Classroom?> GetByIdAsync(int id);
  Task<Classroom> UpdateClassroom(Classroom existingClassroom, int boardId, int standardId, int subjectId);
}
