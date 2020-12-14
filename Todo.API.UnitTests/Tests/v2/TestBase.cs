using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Todo.API.Data.Entities;
using Todo.API.Services;

namespace Todo.API.UnitTests.Tests.v2
{
    public class TestBase : IDisposable
    {
        protected List<User> _users;

        protected List<TodoItem> _todoItems;

        protected UserManager<User> _userManager;

        protected IUserService _userService;

        protected ITodoService _todoService;

        private bool isDisposed;

        public TestBase()
        {
            _users = new List<User>
            {
                new User { Id = 98, Email = "first.user@todos.com" },
                new User { Id = 99, Email = "second.user@todos.com" }
            };

            _todoItems = new List<TodoItem>
            {
                new TodoItem { Id = 1, Title = "Train Dexter", Description = "", IsComplete = false, LastUpdate = DateTime.Now.AddDays(-1) , User = _users[0] },
                new TodoItem { Id = 2, Title = "Train Laylay", Description = "", IsComplete = false, LastUpdate = DateTime.Now.AddDays(-1), User = _users[1] }
            };

            _userManager = Mocks.Services.UserManagerMock.Setup(_users);

            _userService = Mocks.Services.UserServiceMock.Setup(_userManager, _users);

            _todoService = Mocks.Services.TodoServiceMock.Setup(_todoItems);
        }

        public ControllerContext GetControllerContext(string token = "")
        {
            var httpContext = new DefaultHttpContext();

            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            if (!string.IsNullOrEmpty(token))
            {
                httpContext.Request.Headers["Authorization"] = $"bearer {token}";
            }

            return controllerContext;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (_todoItems != null)
            {
                _todoItems = null;
            }

            if (_users != null)
            {
                _users = null;
            }

            if (disposing)
            {
                _userManager.Dispose();
            }

            isDisposed = true;
        }
    }
}
