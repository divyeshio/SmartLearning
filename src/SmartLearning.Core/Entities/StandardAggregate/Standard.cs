using System.ComponentModel.DataAnnotations;
using Ardalis.GuardClauses;
using SmartLearning.SharedKernel;
using SmartLearning.SharedKernel.Interfaces;

namespace SmartLearning.Core.Entities.StandardAggregate;

public class Standard : EntityBase, IAggregateRoot
{

    [Required]
    [Display(Name = "Standard")]
    [Range(1, 12, ErrorMessage = "Please Enter upto 2 digits only")]
    public int Level { get; set; }

    public string DisplayName { get; set; }

    public Standard(int level)
    {
        Level = Guard.Against.InvalidInput(level, nameof(level), l => l > 0 && l <= 12);
        DisplayName = Level.ToString();
    }

    public void UpdateLevel(int newLevel)
    {
        Level = Guard.Against.InvalidInput(newLevel, nameof(newLevel), l => l > 0 && l <= 12);
        DisplayName = Level.ToString();
    }

}

