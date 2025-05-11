using ApiGradeProject.Controllers;
using ApiGradeProject.Database;
using ApiGradeProject.Database.Models;
using ApiGradeProject.Script;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tests.ControllerTests
{
    public class UsersControllerTests
    {
        private readonly PostgresContext _context;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            var options = new DbContextOptionsBuilder<PostgresContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            _context = new PostgresContext(options);
            _controller = new UsersController(_context);

            _context.Users.RemoveRange(_context.Users);
            _context.Tokensses.RemoveRange(_context.Tokensses);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAll_ReturnsAllUsers()
        {
            _context.Users.AddRange(
                new User { Login = "user1", Email = "u1@example.com", Password = "pass1" },
                new User { Login = "user2", Email = "u2@example.com", Password = "pass2" });
            await _context.SaveChangesAsync();

            var result = _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var users = Assert.IsAssignableFrom<List<User>>(okResult.Value);
            Assert.Equal(2, users.Count);
        }

        [Fact]
        public async Task GetById_ReturnsUser_WhenExists()
        {
            var user = new User { UserId = 1, Login = "testuser", Email = "t@example.com", Password = "pass" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = _controller.GetById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<User>(okResult.Value);
            Assert.Equal("testuser", returnedUser.Login);
        }

        [Fact]
        public async Task Create_AddsNewUser()
        {
            var newUser = new User { Login = "newuser", Email = "n@example.com", Password = "password123" };

            var result = _controller.Create(newUser);

            var createdAtAction = Assert.IsType<CreatedAtActionResult>(result);
            var createdUser = Assert.IsType<User>(createdAtAction.Value);
            Assert.Equal("newuser", createdUser.Login);
            Assert.True(_context.Users.Any(u => u.Email == "n@example.com"));
        }

        [Fact]
        public async Task Delete_RemovesUser()
        {
            var user = new User { UserId = 1, Login = "deleteuser", Email = "d@example.com", Password = "pass" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = _controller.Delete(1);

            Assert.IsType<NoContentResult>(result);
            Assert.False(_context.Users.Any(u => u.UserId == 1));
        }

        [Fact]
        public async Task Authenticate_ReturnsOk_WhenValid()
        {
            var password = "password";
            var hashed = PasswordHash.HashPassword(password);
            var user = new User
            {
                Login = "auth",
                Email = "a@example.com",
                Password = hashed
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = _controller.Authenticate("a@example.com", password);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task VerifyToken_ConfirmsEmail()
        {
            var user = new User
            {
                Login = "verify",
                Email = "v@example.com",
                Password = "pass"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = new Tokenss
            {
                TokenValue = "123456",
                UsersId = user.UserId,
                ExpiresAt = DateTime.UtcNow.AddDays(1)
            };
            _context.Tokensses.Add(token);
            await _context.SaveChangesAsync();

            var result = _controller.VerifyToken(user.UserId, "123456");

            Assert.IsType<OkObjectResult>(result);
            var updatedUser = await _context.Users.FindAsync(user.UserId);
            Assert.True(updatedUser.IsEmailConfirmed);
        }
    }
}