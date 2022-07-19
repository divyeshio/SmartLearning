using Ardalis.EFCore.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartLearning.Core.Entities;
using SmartLearning.Core.Entities.BoardAggregate;
using SmartLearning.Core.Entities.ClassAggregate;
using SmartLearning.Core.Entities.Common;
using SmartLearning.Core.Entities.LiveClassAggregate;
using SmartLearning.Core.Entities.TestAggregate;
using SmartLearning.Core.Entities.UsersAggregate;
using SmartLearning.SharedKernel;

namespace SmartLearning.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IDomainEventDispatcher? _dispatcher;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDomainEventDispatcher? dispatcher)
            : base(options)
        {
            _dispatcher = dispatcher;
        }

        public DbSet<Board> Boards => Set<Board>();
        public DbSet<Book> Books { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<ReferenceBook> ReferenceBooks { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<ClassProposal> ClassProposals { get; set; }
        public DbSet<UserClass> UserClass { get; set; }
        public DbSet<SamplePaper> SamplePapers { get; set; }
        public DbSet<Standard> Standards { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestQuestion> TestQuestions { get; set; }
        public DbSet<TestAttempt> TestAttempts { get; set; }
        public DbSet<TestResult> TestResults { get; set; }
        public DbSet<LiveClass> LiveClasses { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<FaceData> Faces { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>().HasOne(b => b.Board).WithMany(b => b.Users).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<ApplicationUser>().HasOne(b => b.Standard).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<ApplicationUser>().HasOne(b => b.Subject).WithMany().OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<SamplePaper>().HasOne(s => s.UploadedBy).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Test>().HasOne(s => s.CreatedBy).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<TestAttempt>().HasOne(s => s.Student).WithMany().OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Note>().HasOne(s => s.UploadedBy).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Message>().HasOne(b => b.FromUser).WithMany(u => u.Messages).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ApplicationUser>().Property(b => b.isEnabled).HasDefaultValue(true);
            modelBuilder.Entity<ApplicationUser>().HasOne(u => u.FaceData).WithOne(f => f.User).HasForeignKey<FaceData>(f => f.UserId);


            modelBuilder.Entity<ApplicationUser>().HasMany(u => u.Classes).WithMany(g => g.Users)
                .UsingEntity<UserClass>(ug => ug
                                          .HasOne(ug => ug.Class)
                                          .WithMany()
                                          .HasForeignKey("ClassId").OnDelete(DeleteBehavior.NoAction),
                                        ug => ug
                                          .HasOne(ug => ug.User)
                                          .WithMany()
                                          .HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade))
                                       .ToTable("UserClass")
                                       .HasKey(ug => new { ug.ClassId, ug.UserId });

           

            modelBuilder.Entity<ApplicationUser>(entity => entity.ToTable(name: "Users"));
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

}
