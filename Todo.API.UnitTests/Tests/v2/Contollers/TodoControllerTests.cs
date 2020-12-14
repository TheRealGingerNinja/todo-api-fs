using Microsoft.AspNetCore.Mvc;
using Todo.API.Controllers;
using Todo.API.Models.Todo;
using Xunit;

namespace Todo.API.UnitTests.Tests.v2.Contollers
{
    public class TodoControllerTests : TestBase
    {
        private readonly TodoController controller;

        public TodoControllerTests()
        {
            controller = new TodoController(_userService, _todoService);
        }

        [Fact]
        public async void Create_Fails_When_Not_Authenticated()
        {
            controller.ControllerContext = GetControllerContext();

            var todoDto = new TodoDto()
            {
                Title = "Test Todo",
                Description = "Test Todo Desc"
            };

            var result = await controller.Create(todoDto);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void Create_Success_When_Authenticated()
        {
            controller.ControllerContext = GetControllerContext(Mocks.TokenMock.USER_98_TOKEN);

            var todoDto = new TodoDto()
            {
                Title = "Test Todo",
                Description = "Test Todo"
            };

            var result = await controller.Create(todoDto);

            Assert.IsType<CreatedAtRouteResult>(result);

            var actualResult = result as CreatedAtRouteResult;

            var expected = "Todos_GetById";

            Assert.Equal(actualResult.RouteName, expected);
        }

        [Fact]
        public async void GetById_Fails_CanOnlyGetOwnTodos()
        {
            controller.ControllerContext = GetControllerContext(Mocks.TokenMock.USER_99_TOKEN);

            var result = await controller.GetById(1);

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async void GetById_Return_Ok()
        {
            controller.ControllerContext = GetControllerContext(Mocks.TokenMock.USER_98_TOKEN);

            var result = await controller.GetById(1);

            Assert.IsType<OkObjectResult>(result);

            var actual = (OkObjectResult)result;
            var todo = actual.Value as TodoDto;
            var expectedTodId = 1;
            var expectedUserId = 98;

            Assert.NotNull(todo);
            Assert.Equal(todo.Id, expectedTodId);
            Assert.Equal(todo.User.Id, expectedUserId);
        }
    }
}
