using Xunit;
using Microsoft.EntityFrameworkCore;
using ArnaBlazorTest.Services;
using ArnaBlazorTest.Models;
using ArnaBlazorTest.Data;
using System.Collections.Generic;
using System;

namespace ArnaBlazorTest.Tests
{
    public class QuizServiceTests
    {
        [Fact]
        public async void ScoringLogic_CorrectAnswers_ReturnsExpectedScore()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var context = new ApplicationDbContext(options);

            // Add assignment and questions
            var assignment = new Assignment { Id = 1, Title = "Test Assignment" };
            context.Assignments.Add(assignment);
            var questions = new List<Question>();
            for (int i = 1; i <= 5; i++)
            {
                questions.Add(new Question
                {
                    Id = i,
                    AssignmentId = 1,
                    QuestionText = $"Q{i}",
                    Options = new List<string> { "A", "B", "C", "D" },
                    CorrectAnswer = "A"
                });
            }
            context.Questions.AddRange(questions);
            context.SaveChanges();

            var quizRepository = new QuizRepository(context);
            var quizService = new QuizService(quizRepository);
            var userId = 42;
            var assignmentId = 1;
            // 3 correct, 2 incorrect
            var answers = new Dictionary<int, string>
            {
                { 1, "A" }, // correct
                { 2, "A" }, // correct
                { 3, "A" }, // correct
                { 4, "B" }, // incorrect
                { 5, "C" }  // incorrect
            };

            // Act
            var (score, message) = await quizService.SubmitQuizAsync(userId, assignmentId, answers);

            // Assert
            Assert.Equal(60, score); // 3 correct * 20 points
            Assert.Equal("Quiz submitted successfully", message);
        }

        [Fact]
        public async void ScoringLogic_AllCorrectAnswers_Returns100Score()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var context = new ApplicationDbContext(options);

            var assignment = new Assignment { Id = 1, Title = "Test Assignment" };
            context.Assignments.Add(assignment);
            var questions = new List<Question>();
            for (int i = 1; i <= 5; i++)
            {
                questions.Add(new Question
                {
                    Id = i,
                    AssignmentId = 1,
                    QuestionText = $"Q{i}",
                    Options = new List<string> { "A", "B", "C", "D" },
                    CorrectAnswer = "A"
                });
            }
            context.Questions.AddRange(questions);
            context.SaveChanges();

            var quizRepository = new QuizRepository(context);
            var quizService = new QuizService(quizRepository);
            var userId = 42;
            var assignmentId = 1;
            // All correct answers
            var answers = new Dictionary<int, string>
            {
                { 1, "A" }, // correct
                { 2, "A" }, // correct
                { 3, "A" }, // correct
                { 4, "A" }, // correct
                { 5, "A" }  // correct
            };

            // Act
            var (score, message) = await quizService.SubmitQuizAsync(userId, assignmentId, answers);

            // Assert
            Assert.Equal(100, score); // 5 correct * 20 points = 100
            Assert.Equal("Quiz submitted successfully", message);
        }

        [Fact]
        public async void ScoringLogic_AllIncorrectAnswers_Returns0Score()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var context = new ApplicationDbContext(options);

            var assignment = new Assignment { Id = 1, Title = "Test Assignment" };
            context.Assignments.Add(assignment);
            var questions = new List<Question>();
            for (int i = 1; i <= 5; i++)
            {
                questions.Add(new Question
                {
                    Id = i,
                    AssignmentId = 1,
                    QuestionText = $"Q{i}",
                    Options = new List<string> { "A", "B", "C", "D" },
                    CorrectAnswer = "A"
                });
            }
            context.Questions.AddRange(questions);
            context.SaveChanges();

            var quizRepository = new QuizRepository(context);
            var quizService = new QuizService(quizRepository);
            var userId = 42;
            var assignmentId = 1;
            // All incorrect answers
            var answers = new Dictionary<int, string>
            {
                { 1, "B" }, // incorrect
                { 2, "C" }, // incorrect
                { 3, "D" }, // incorrect
                { 4, "B" }, // incorrect
                { 5, "C" }  // incorrect
            };

            // Act
            var (score, message) = await quizService.SubmitQuizAsync(userId, assignmentId, answers);

            // Assert
            Assert.Equal(0, score); // 0 correct * 20 points = 0
            Assert.Equal("Quiz submitted successfully", message);
        }

        [Fact]
        public async void ScoringLogic_OneCorrectAnswer_Returns20Score()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var context = new ApplicationDbContext(options);

            var assignment = new Assignment { Id = 1, Title = "Test Assignment" };
            context.Assignments.Add(assignment);
            var questions = new List<Question>();
            for (int i = 1; i <= 5; i++)
            {
                questions.Add(new Question
                {
                    Id = i,
                    AssignmentId = 1,
                    QuestionText = $"Q{i}",
                    Options = new List<string> { "A", "B", "C", "D" },
                    CorrectAnswer = "A"
                });
            }
            context.Questions.AddRange(questions);
            context.SaveChanges();

            var quizRepository = new QuizRepository(context);
            var quizService = new QuizService(quizRepository);
            var userId = 42;
            var assignmentId = 1;
            // Only one correct answer
            var answers = new Dictionary<int, string>
            {
                { 1, "A" }, // correct
                { 2, "B" }, // incorrect
                { 3, "C" }, // incorrect
                { 4, "D" }, // incorrect
                { 5, "B" }  // incorrect
            };

            // Act
            var (score, message) = await quizService.SubmitQuizAsync(userId, assignmentId, answers);

            // Assert
            Assert.Equal(20, score); // 1 correct * 20 points = 20
            Assert.Equal("Quiz submitted successfully", message);
        }

        [Fact]
        public async void ScoringLogic_CaseInsensitiveAnswers_ReturnsCorrectScore()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var context = new ApplicationDbContext(options);

            var assignment = new Assignment { Id = 1, Title = "Test Assignment" };
            context.Assignments.Add(assignment);
            var questions = new List<Question>();
            for (int i = 1; i <= 5; i++)
            {
                questions.Add(new Question
                {
                    Id = i,
                    AssignmentId = 1,
                    QuestionText = $"Q{i}",
                    Options = new List<string> { "A", "B", "C", "D" },
                    CorrectAnswer = "A"
                });
            }
            context.Questions.AddRange(questions);
            context.SaveChanges();

            var quizRepository = new QuizRepository(context);
            var quizService = new QuizService(quizRepository);
            var userId = 42;
            var assignmentId = 1;
            // Mixed case answers
            var answers = new Dictionary<int, string>
            {
                { 1, "a" }, // correct (lowercase)
                { 2, "A" }, // correct (uppercase)
                { 3, "b" }, // incorrect
                { 4, "A" }, // correct (uppercase)
                { 5, "c" }  // incorrect
            };

            // Act
            var (score, message) = await quizService.SubmitQuizAsync(userId, assignmentId, answers);

            // Assert
            Assert.Equal(60, score); // 3 correct * 20 points = 60
            Assert.Equal("Quiz submitted successfully", message);
        }
    }
}
