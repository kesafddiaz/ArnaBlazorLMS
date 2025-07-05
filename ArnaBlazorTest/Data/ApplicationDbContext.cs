using Microsoft.EntityFrameworkCore;
using ArnaBlazorTest.Models;
using BCrypt.Net;

namespace ArnaBlazorTest.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserRole> UserRoles { get; set; } = null!;
    public DbSet<Assignment> Assignments { get; set; } = null!;
    public DbSet<Question> Questions { get; set; } = null!;
    public DbSet<AssignmentProgress> AssignmentProgresses { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configurations
        modelBuilder.Entity<User>()
            .HasOne(u => u.UserRole)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Manager)
            .WithMany(m => m.Subordinates)
            .HasForeignKey(u => u.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Assignment configurations
        modelBuilder.Entity<Assignment>()
            .HasMany(a => a.Questions)
            .WithOne(q => q.Assignment)
            .HasForeignKey(q => q.AssignmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // AssignmentProgress configurations
        modelBuilder.Entity<AssignmentProgress>()
            .HasOne(ap => ap.User)
            .WithMany(u => u.AssignmentProgresses)
            .HasForeignKey(ap => ap.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AssignmentProgress>()
            .HasOne(ap => ap.Assignment)
            .WithMany(a => a.AssignmentProgresses)
            .HasForeignKey(ap => ap.AssignmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed initial roles
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole { Id = 1, Name = "Learner" },
            new UserRole { Id = 2, Name = "Manager" }
        );
    }

    public async Task SeedTestDataAsync()
    {
        // Check if test users already exist
        if (!Users.Any(u => u.Username == "testlearner"))
        {
            var learnerPasswordHash = BCrypt.Net.BCrypt.HashPassword("password123");
            var managerPasswordHash = BCrypt.Net.BCrypt.HashPassword("password123");

            var testLearner = new User
            {
                Username = "testlearner",
                PasswordHash = learnerPasswordHash,
                Email = "learner@test.com",
                RoleId = 1, // Learner
                CreatedAt = DateTime.UtcNow
            };

            var testManager = new User
            {
                Username = "testmanager",
                PasswordHash = managerPasswordHash,
                Email = "manager@test.com",
                RoleId = 2, // Manager
                CreatedAt = DateTime.UtcNow
            };

            Users.Add(testLearner);
            Users.Add(testManager);
            await SaveChangesAsync();
            
            // Log the creation
            Console.WriteLine($"Created test users: testlearner and testmanager");
        }
        else
        {
            Console.WriteLine($"Test users already exist in database");
        }

        // Check if test assignments already exist
        if (!Assignments.Any())
        {
            var assignment1 = new Assignment
            {
                Title = "Introduction to C# Programming",
                Description = "Learn the basics of C# programming language including variables, data types, and control structures.",
                MaterialUrl = "https://docs.microsoft.com/en-us/dotnet/csharp/",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Questions = new List<Question>
                {
                    new Question
                    {
                        QuestionText = "What is the correct way to declare a variable in C#?",
                        OptionsJson = "[\"var x = 5;\", \"int x = 5;\", \"string x = 5;\", \"x = 5;\"]",
                        CorrectAnswer = "int x = 5;",
                        AssignmentId = 1
                    },
                    new Question
                    {
                        QuestionText = "Which keyword is used to create a class in C#?",
                        OptionsJson = "[\"class\", \"struct\", \"interface\", \"object\"]",
                        CorrectAnswer = "class",
                        AssignmentId = 1
                    },
                    new Question
                    {
                        QuestionText = "What is the default access modifier for class members in C#?",
                        OptionsJson = "[\"public\", \"private\", \"protected\", \"internal\"]",
                        CorrectAnswer = "private",
                        AssignmentId = 1
                    },
                    new Question
                    {
                        QuestionText = "Which of the following is a value type in C#?",
                        OptionsJson = "[\"string\", \"int\", \"object\", \"array\"]",
                        CorrectAnswer = "int",
                        AssignmentId = 1
                    },
                    new Question
                    {
                        QuestionText = "What is the purpose of the 'using' statement in C#?",
                        OptionsJson = "[\"To import namespaces\", \"To declare variables\", \"To create loops\", \"To define classes\"]",
                        CorrectAnswer = "To import namespaces",
                        AssignmentId = 1
                    }
                }
            };

            var assignment2 = new Assignment
            {
                Title = "ASP.NET Core Fundamentals",
                Description = "Explore the fundamentals of ASP.NET Core framework for building web applications.",
                MaterialUrl = "https://docs.microsoft.com/en-us/aspnet/core/",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Questions = new List<Question>
                {
                    new Question
                    {
                        QuestionText = "What is the main purpose of ASP.NET Core?",
                        OptionsJson = "[\"Desktop applications\", \"Web applications\", \"Mobile applications\", \"Game development\"]",
                        CorrectAnswer = "Web applications",
                        AssignmentId = 2
                    },
                    new Question
                    {
                        QuestionText = "Which method is used to configure services in ASP.NET Core?",
                        OptionsJson = "[\"ConfigureServices()\", \"Configure()\", \"UseServices()\", \"AddServices()\"]",
                        CorrectAnswer = "ConfigureServices()",
                        AssignmentId = 2
                    },
                    new Question
                    {
                        QuestionText = "What is the default port for ASP.NET Core development server?",
                        OptionsJson = "[\"3000\", \"5000\", \"8080\", \"80\"]",
                        CorrectAnswer = "5000",
                        AssignmentId = 2
                    },
                    new Question
                    {
                        QuestionText = "What is the purpose of middleware in ASP.NET Core?",
                        OptionsJson = "[\"To handle HTTP requests\", \"To create databases\", \"To compile code\", \"To manage users\"]",
                        CorrectAnswer = "To handle HTTP requests",
                        AssignmentId = 2
                    },
                    new Question
                    {
                        QuestionText = "Which of the following is a built-in dependency injection container in ASP.NET Core?",
                        OptionsJson = "[\"Unity\", \"Autofac\", \"Microsoft.Extensions.DependencyInjection\", \"Ninject\"]",
                        CorrectAnswer = "Microsoft.Extensions.DependencyInjection",
                        AssignmentId = 2
                    }
                }
            };

            var assignment3 = new Assignment
            {
                Title = "Entity Framework Core",
                Description = "Learn how to work with databases using Entity Framework Core ORM.",
                MaterialUrl = "https://docs.microsoft.com/en-us/ef/core/",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Questions = new List<Question>
                {
                    new Question
                    {
                        QuestionText = "What is Entity Framework Core?",
                        OptionsJson = "[\"A web framework\", \"An ORM\", \"A database\", \"A programming language\"]",
                        CorrectAnswer = "An ORM",
                        AssignmentId = 3
                    },
                    new Question
                    {
                        QuestionText = "Which method is used to add a new entity to the database?",
                        OptionsJson = "[\"Add()\", \"Insert()\", \"Create()\", \"Save()\"]",
                        CorrectAnswer = "Add()",
                        AssignmentId = 3
                    },
                    new Question
                    {
                        QuestionText = "What is the purpose of DbContext in Entity Framework?",
                        OptionsJson = "[\"To create web pages\", \"To manage database connections\", \"To handle HTTP requests\", \"To compile code\"]",
                        CorrectAnswer = "To manage database connections",
                        AssignmentId = 3
                    },
                    new Question
                    {
                        QuestionText = "What is the purpose of migrations in Entity Framework?",
                        OptionsJson = "[\"To create new databases\", \"To update database schema\", \"To delete tables\", \"To backup data\"]",
                        CorrectAnswer = "To update database schema",
                        AssignmentId = 3
                    },
                    new Question
                    {
                        QuestionText = "Which method is used to save changes to the database?",
                        OptionsJson = "[\"Save()\", \"SaveChanges()\", \"Commit()\", \"Update()\"]",
                        CorrectAnswer = "SaveChanges()",
                        AssignmentId = 3
                    }
                }
            };

            Assignments.Add(assignment1);
            Assignments.Add(assignment2);
            Assignments.Add(assignment3);
            await SaveChangesAsync();
            
            Console.WriteLine($"Created sample assignments with questions");
        }
        else
        {
            Console.WriteLine($"Sample assignments already exist in database");
        }
    }
} 