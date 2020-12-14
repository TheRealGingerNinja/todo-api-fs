using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.API.Data.Entities;
using Todo.API.Services;

namespace Todo.API.UnitTests.Mocks.Services
{
    public static class TodoServiceMock
    {
        public static ITodoService Setup(List<TodoItem> data)
        {
            var mock = new Mock<ITodoService>();

            static long keyExp(TodoItem entity) => entity.Id;

            mock.Setup(m => m.GetAll(It.IsAny<long>()))
                .Returns((int userId) => data.Where(e => e.User.Id == userId).ToList());

            mock.Setup(m => m.GetById(It.IsAny<int>()))
                .Returns((int id) => Task.FromResult(data.FirstOrDefault(e => keyExp(e) == id)));

            mock.Setup(m => m.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<User>()))
                .Returns((string title, string description, User user) =>
                {
                    var todoItem = new TodoItem()
                    {
                        Title = title,
                        Description = description,
                        IsComplete = false,
                        LastUpdate = DateTime.UtcNow,
                        User = user
                    };

                    data.Add(todoItem);

                    return Task.FromResult(todoItem);
                });

            mock.Setup(m => m.Update(It.IsAny<TodoItem>()))
                .Callback((TodoItem model) =>
                {
                    long id = keyExp(model);

                    var entity = data.FirstOrDefault(e => keyExp(e) == id);

                    if (entity == null)
                    {
                        var message = string.Format("The {0} model with id {1} does not exist.", typeof(TodoItem).Name, id);

                        throw new ArgumentNullException(message);
                    }

                    var index = data.IndexOf(entity);

                    data[index] = model;
                })
                .Returns((TodoItem entity) => Task.FromResult(entity.Id));

            mock.Setup(m => m.Delete(It.IsAny<int>()))
                .Returns((int id) =>
                {
                    var entity = data.FirstOrDefault(e => keyExp(e) == id);

                    if (entity != null)
                    {
                        data.Remove(entity);
                    }

                    return Task.FromResult(entity);
                });

            return mock.Object;
        }
    }
}
