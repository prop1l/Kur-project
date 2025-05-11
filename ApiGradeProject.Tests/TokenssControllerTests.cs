using System;
using System.Collections.Generic;
using System.Linq;
using ApiGradeProject.Controllers;
using ApiGradeProject.Database;
using ApiGradeProject.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tests.ControllerTests
{
    public class TokenssControllerTests
    {
        private readonly PostgresContext _context;
        private readonly TokenssController _controller;

        public TokenssControllerTests()
        {
            var options = new DbContextOptionsBuilder<PostgresContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_Tokenss_{Guid.NewGuid()}")
                .Options;

            _context = new PostgresContext(options);
            _controller = new TokenssController(_context);

            _context.Tokensses.RemoveRange(_context.Tokensses);
            _context.Users.RemoveRange(_context.Users);
            _context.SaveChanges();
        }

        [Fact]
        public void GetAll_ReturnsAllTokenss()
        {
            var user = new User { Email = "tokenuser@example.com", Login = "tokenuser", Password = "pass" };
            _context.Users.Add(user);
            _context.Tokensses.AddRange(
                new Tokenss { TokenValue = "abc123", UsersId = user.UserId, ExpiresAt = DateTime.UtcNow.AddDays(1) },
                new Tokenss { TokenValue = "def456", UsersId = user.UserId, ExpiresAt = DateTime.UtcNow.AddDays(1) }
            );
            _context.SaveChanges();

            var result = _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var tokenssList = Assert.IsAssignableFrom<List<Tokenss>>(okResult.Value);
            Assert.Equal(2, tokenssList.Count);
        }

        [Fact]
        public void GetById_ReturnsToken_WhenExists()
        {
            var user = new User { Email = "tokenuser@example.com", Login = "tokenuser", Password = "pass" };
            _context.Users.Add(user);
            var token = new Tokenss { TokenId = 1, TokenValue = "abc123", UsersId = user.UserId, ExpiresAt = DateTime.UtcNow.AddDays(1) };
            _context.Tokensses.Add(token);
            _context.SaveChanges();

            var result = _controller.GetById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedToken = Assert.IsType<Tokenss>(okResult.Value);
            Assert.Equal("abc123", returnedToken.TokenValue);
        }

        [Fact]
        public void GetById_ReturnsNotFound_WhenDoesNotExist()
        {
            var result = _controller.GetById(999);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void Delete_RemovesToken_WhenExists()
        {
            var user = new User { Email = "tokenuser@example.com", Login = "tokenuser", Password = "pass" };
            _context.Users.Add(user);
            var token = new Tokenss { TokenId = 1, TokenValue = "abc123", UsersId = user.UserId, ExpiresAt = DateTime.UtcNow.AddDays(1) };
            _context.Tokensses.Add(token);
            _context.SaveChanges();

            var result = _controller.Delete(1);

            Assert.IsType<NoContentResult>(result);
            Assert.False(_context.Tokensses.Any(t => t.TokenId == 1));
        }

        [Fact]
        public void Delete_ReturnsNotFound_WhenTokenDoesNotExist()
        {
            var result = _controller.Delete(999);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void DeleteAll_RemovesAllTokens()
        {
            var user = new User { Email = "tokenuser@example.com", Login = "tokenuser", Password = "pass" };
            _context.Users.Add(user);
            _context.Tokensses.AddRange(
                new Tokenss { TokenValue = "abc123", UsersId = user.UserId, ExpiresAt = DateTime.UtcNow.AddDays(1) },
                new Tokenss { TokenValue = "def456", UsersId = user.UserId, ExpiresAt = DateTime.UtcNow.AddDays(1) }
            );
            _context.SaveChanges();

            var result = _controller.DeleteAll();

            Assert.IsType<NoContentResult>(result);
            Assert.False(_context.Tokensses.Any());
        }

        [Fact]
        public void DeleteAll_ReturnsNotFound_WhenNoTokens()
        {
            var result = _controller.DeleteAll();

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}