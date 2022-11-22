using Ardalis.EFCore.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartLearning.Core.Entities;
using SmartLearning.Core.Entities.BoardAggregate;
using SmartLearning.Core.Entities.ClassroomAggregate;
using SmartLearning.Core.Entities.Common;
using SmartLearning.Core.Entities.LiveClassAggregate;
using SmartLearning.Core.Entities.StandardAggregate;
using SmartLearning.Core.Entities.SubjectAggregate;
using SmartLearning.Core.Entities.TestAggregate;
using SmartLearning.Core.Entities.UsersAggregate;
using SmartLearning.SharedKernel;
using SmartLearning.SharedKernel.Interfaces;

namespace SmartLearning.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
  private readonly IDomainEventDispatcher? _dispatcher;

  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDomainEventDispatcher? dispatcher)
      : base(options)
  {
    _dispatcher = dispatcher;
  }

  public DbSet<Board> Boards => Set<Board>();
  public DbSet<Book> Books => Set<Book>();
  public DbSet<Chapter> Chapters => Set<Chapter>();
  public DbSet<Message> Messages => Set<Message>();
  public DbSet<Note> Notes => Set<Note>();
  public DbSet<Quote> Quotes => Set<Quote>();
  public DbSet<ReferenceBook> ReferenceBooks => Set<ReferenceBook>();
  public DbSet<Classroom> Classes => Set<Classroom>();
  public DbSet<ClassProposal> ClassProposals => Set<ClassProposal>();
  public DbSet<UserClass> UserClass => Set<UserClass>();
  public DbSet<SamplePaper> SamplePapers => Set<SamplePaper>();
  public DbSet<Standard> Standards => Set<Standard>();
  public DbSet<Subject> Subjects => Set<Subject>();
  public DbSet<Test> Tests => Set<Test>();
  public DbSet<TestQuestion> TestQuestions => Set<TestQuestion>();
  public DbSet<TestAttempt> TestAttempts => Set<TestAttempt>();
  public DbSet<TestResult> TestResults => Set<TestResult>();
  public DbSet<LiveClass> LiveClasses => Set<LiveClass>();
  public DbSet<Post> Posts => Set<Post>();
  public DbSet<FaceData> Faces => Set<FaceData>();




  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<IdentityRole>(entity => entity.ToTable(name: "Roles"));
    modelBuilder.Entity<IdentityUserRole<string>>(entity => entity.ToTable("UserRoles"));
    modelBuilder.Entity<IdentityUserClaim<string>>(entity => entity.ToTable("UserClaims"));
    modelBuilder.Entity<IdentityUserLogin<string>>(entity => entity.ToTable("UserLogins"));
    modelBuilder.Entity<IdentityRoleClaim<string>>(entity => entity.ToTable("RoleClaims"));
    modelBuilder.Entity<IdentityUserToken<string>>(entity => entity.ToTable("UserTokens"));
    modelBuilder.ApplyAllConfigurationsFromCurrentAssembly();
  }
  public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
  {
    int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

    // ignore events if no dispatcher provided
    if (_dispatcher == null) return result;

    // dispatch events only if save was successful
    var entitiesWithEvents = ChangeTracker.Entries<EntityBase>()
        .Select(e => e.Entity)
        .Where(e => e.DomainEvents.Any())
    .ToArray();

    await _dispatcher.DispatchAndClearEvents(entitiesWithEvents);

    return result;
  }

  public override int SaveChanges()
  {
    return SaveChangesAsync().GetAwaiter().GetResult();
  }
}

