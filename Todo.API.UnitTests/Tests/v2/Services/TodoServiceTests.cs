using System.Linq;
using Todo.API.Data;
using Todo.API.Data.Entities;
using Todo.API.Services;
using Xunit;

namespace Todo.API.UnitTests.Tests.v2.Services
{
    public class TodoServiceTests : TestBase
    {
        private readonly TodoDbContext context;

        private readonly TodoService service;

        private readonly User user;

        public TodoServiceTests()
        {
            context = new Mocks.TodoDbContextMock()
                .Setup(_users)
                .Setup(_todoItems)
                .GetMock();

            service = new TodoService(context);

            user = context.Users.First();
        }

        [Fact]
        public async void CanCreateTodo()
        {
            var todoItem = await service.Create("Test", "Test", user);

            Assert.NotNull(todoItem);

            Assert.True(todoItem.Id > 0);

            await service.Delete(todoItem.Id);
        }

        [Fact]
        public async void CanGetAllTodosByUserId()
        {
            await service.Create("Test", "Test", user);

            await service.Create("Test1", "Test1", user);

            var todos = service.GetAll(user.Id);

            Assert.Equal(3, todos.Count);
        }

        [Fact]
        public async void CanGetTodoById()
        {
            var todo1 = await service.Create("Test", "Test", user);

            var todo2 = await service.Create("Test1", "Test1", user);

            var getTodo = await service.GetById(todo2.Id);

            Assert.Equal(getTodo, todo2);
        }

        [Fact]
        public async void CanUpdateExistingTodo()
        {
            var todo = await service.Create("Test", "Test", user);

            todo.Title = "Test Updated";

            todo.IsComplete = true;

            var result = await service.Update(todo);

            var updated = await service.GetById(todo.Id);

            Assert.Equal(1, result);
            
            Assert.Equal("Test Updated", updated.Title);
            
            Assert.True(updated.IsComplete);
        }

    }
}
