using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiGradeProject.Controllers;
using ApiGradeProject.Database;
using ApiGradeProject.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tests.ControllerTests
{
    public class GroupsControllerTests
    {
        private readonly PostgresContext _context;
        private readonly GroupsController _controller;

        public GroupsControllerTests()
        {
            var options = new DbContextOptionsBuilder<PostgresContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_Groups_{Guid.NewGuid()}")
                .Options;

            _context = new PostgresContext(options);
            _controller = new GroupsController(_context);

            _context.Groups.RemoveRange(_context.Groups);
            _context.Specialities.RemoveRange(_context.Specialities);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAll_ReturnsAllGroups()
        {
            var speciality = new Speciality { SpecName = "Информатика" };
            _context.Specialities.Add(speciality);
            await _context.SaveChangesAsync();

            _context.Groups.AddRange(
                new Group { GroupId = 1, Speciality = speciality },
                new Group { GroupId = 2, Speciality = speciality }
            );
            await _context.SaveChangesAsync();

            var result = _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var groups = Assert.IsAssignableFrom<List<Group>>(okResult.Value);
            Assert.Equal(2, groups.Count);
        }

        [Fact]
        public async Task GetById_ReturnsGroup_WhenExists()
        {
            var speciality = new Speciality { SpecName = "Математика" };
            _context.Specialities.Add(speciality);
            await _context.SaveChangesAsync();

            var group = new Group { GroupId = 1, Speciality = speciality };
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            var result = _controller.GetById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedGroup = Assert.IsType<Group>(okResult.Value);
            Assert.Equal(1, returnedGroup.GroupId);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenDoesNotExist()
        {
            var result = _controller.GetById(999);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Create_AddsNewGroup()
        {
            var speciality = new Speciality { SpecName = "Физика" };
            _context.Specialities.Add(speciality);
            await _context.SaveChangesAsync();

            var newGroup = new Group
            {
                GroupId = 5,
                SpecialityId = speciality.SpecialityId
            };

            var result = _controller.Create(newGroup);

            var createdAtAction = Assert.IsType<CreatedAtActionResult>(result);
            var createdGroup = Assert.IsType<Group>(createdAtAction.Value);
            Assert.Equal(5, createdGroup.GroupId);
            Assert.True(_context.Groups.Any(g => g.GroupId == 5));
        }

        [Fact]
        public async Task Delete_RemovesGroup_WhenExists()
        {
            var group = new Group
            {
                GroupId = 1,
                Speciality = new Speciality { SpecName = "Литература" }
            };
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            var result = _controller.Delete(1);

            Assert.IsType<NoContentResult>(result);
            Assert.False(_context.Groups.Any(g => g.GroupId == 1));
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenGroupDoesNotExist()
        {
            var result = _controller.Delete(999);
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}