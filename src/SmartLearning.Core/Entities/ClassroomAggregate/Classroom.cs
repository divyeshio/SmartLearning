using System.ComponentModel.DataAnnotations;
using Ardalis.GuardClauses;
using SmartLearning.Core.Entities.BoardAggregate;
using SmartLearning.Core.Entities.Common;
using SmartLearning.Core.Entities.StandardAggregate;
using SmartLearning.Core.Entities.SubjectAggregate;
using SmartLearning.Core.Entities.UsersAggregate;
using SmartLearning.SharedKernel;
using SmartLearning.SharedKernel.Interfaces;

namespace SmartLearning.Core.Entities.ClassroomAggregate;

public class Classroom : EntityBase, IAggregateRoot
{
    [Required]
    [StringLength(20), MinLength(3)]
    public string Name { get; set; }

    [Required]
    [Display(Name = "Board")]
    public int BoardId { get; set; }
    public Board Board { get; set; }

    [Display(Name = "Standard")]
    [Required]
    public int StandardId { get; set; }
    public Standard Standard { get; set; }

    [Required]
    [Display(Name = "Subject")]
    public int SubjectId { get; set; }
    public Subject Subject { get; set; }

    public bool IsRegistrationAllowed { get; set; } = true;


    public Classroom()
    {

    }

    public Classroom(Board board, Standard standard, Subject subject)
    {
        Board = Guard.Against.Default(board, nameof(board));
        Standard = Guard.Against.Default(standard, nameof(standard));
        Subject = Guard.Against.Default(subject, nameof(subject));
        BoardId = board.Id;
        StandardId = standard.Id;
        SubjectId = subject.Id;
        Name = string.Join("-", board.AbbrName, standard.DisplayName, subject.Name);
    }

    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();


    public static string GenerateGroupName(string board, string standard, string subject)
    {
        return string.Join("-", board, standard, subject);
    }
}
