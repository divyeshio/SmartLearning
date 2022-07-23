using Ardalis.GuardClauses;
using SmartLearning.Core.Entities.BoardAggregate;
using SmartLearning.Core.Entities.BoardAggregate.Specifications;
using SmartLearning.Core.Entities.ClassroomAggregate;
using SmartLearning.Core.Entities.StandardAggregate;
using SmartLearning.Core.Entities.SubjectAggregate;
using SmartLearning.Core.Interfaces;
using SmartLearning.SharedKernel.Interfaces;

namespace SmartLearning.Core.Services;
public class ClassroomService : IClassroomService
{
    private readonly IReadRepository<Board> _boardRepository;
    private readonly IReadRepository<Standard> _standardRepository;
    private readonly IReadRepository<Subject> _subjectRepository;
    private readonly IRepository<Classroom> _classroomRepository;

    public ClassroomService(IReadRepository<Subject> subjectRepository, IReadRepository<Standard> standardRepository, IReadRepository<Board> boardRepository, IRepository<Classroom> classroomRepository)
    {
        _subjectRepository = subjectRepository;
        _standardRepository = standardRepository;
        _boardRepository = boardRepository;
        _classroomRepository = classroomRepository;
    }

    public async Task<Classroom> CreateClassroomAsync(int boardId, int standardId, int subjectId)
    {
        var board = await _boardRepository.GetByIdAsync(boardId);
        var standard = await _standardRepository.GetByIdAsync(standardId);
        var subject = await _subjectRepository.GetByIdAsync(subjectId);

        Guard.Against.Default(board, nameof(board));
        Guard.Against.Default(standard, nameof(standard));
        Guard.Against.Default(subject, nameof(subject));

        var newClassroom = await _classroomRepository.AddAsync(new Classroom(board, standard, subject));
        return newClassroom;
    }

    public async Task<Classroom?> GetByIdAsync(int id)
    {
        return await _classroomRepository.GetByIdAsync(id);
    }

    public async Task<Classroom> UpdateClassroom(Classroom existingClassroom, int boardId, int standardId, int subjectId)
    {
        Guard.Against.Default(existingClassroom, nameof(existingClassroom));

        var checkDetails = await CheckClassroomDetails(boardId, standardId, subjectId);
        if (checkDetails == false)
            throw new InvalidDataException();

        existingClassroom.SubjectId = subjectId;
        existingClassroom.BoardId = boardId;
        existingClassroom.StandardId = standardId;

        await _classroomRepository.UpdateAsync(existingClassroom);
        return existingClassroom;
    }

    private async Task<bool> CheckClassroomDetails(int boardId, int standardId, int subjectId)
    {
        var boardCheck = await _boardRepository.AnyAsync(new CheckIfBoardExistsSpec(boardId));
        var standardCheck = await _standardRepository.AnyAsync(new CheckIfStandardExistsSpec(standardId));
        var subjectCheck = await _subjectRepository.AnyAsync(new CheckIfSubjectExistsSpec(subjectId));

        return boardCheck && standardCheck && subjectCheck;
    }

}
