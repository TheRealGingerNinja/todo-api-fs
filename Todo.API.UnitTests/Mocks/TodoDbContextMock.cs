using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Todo.API.Data;
using Todo.API.Data.Entities;

namespace Todo.API.UnitTests.Mocks
{
    public class TodoDbContextMock
    {
        public readonly Mock<TodoDbContext> todoDbContextMock;

        private int modified = 0;

        public TodoDbContextMock()
        {
            todoDbContextMock = new Mock<TodoDbContext>();
        }

        public TodoDbContextMock Setup<TEntity>(List<TEntity> data)
            where TEntity : class, new()
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var type = typeof(TEntity);

            if (type == typeof(User))
            {
                SetupUsers(data as List<User>);
            }

            if(type == typeof(Role))
            {
                SetupRoles(data as List<Role>);
            }

            if(type == typeof(TodoItem))
            {
                SetupTodoItems(data as List<TodoItem>);
            }

            return this;
        }

        public TodoDbContext GetMock()
        {
            todoDbContextMock.Setup(m => m.SaveChanges()).Returns(modified).Verifiable();
            todoDbContextMock.Setup(m => m.SaveChangesAsync(new CancellationToken())).Returns(() => Task.Run(() => modified)).Verifiable();

            return todoDbContextMock.Object;
        }

        private void SetupUsers(List<User> data)
        {
            var dbSetMock = DbSetMock.GetDbSetMock(data, entity => entity.Id, nameof(User.Id));
            todoDbContextMock.Setup(dbSet => dbSet.Set<User>()).Returns(dbSetMock);
            todoDbContextMock.Setup(dbSet => dbSet.Users).Returns(dbSetMock);
        }

        private void SetupRoles(List<Role> data)
        {
            var dbSetMock = DbSetMock.GetDbSetMock(data, entity => entity.Id, nameof(Role.Id));
            todoDbContextMock.Setup(dbSet => dbSet.Set<Role>()).Returns(dbSetMock);
            todoDbContextMock.Setup(dbSet => dbSet.Roles).Returns(dbSetMock);
        }

        private void SetupTodoItems(List<TodoItem> data)
        {
            var dbSetMock = DbSetMock.GetDbSetMock(data, entity => entity.Id, nameof(TodoItem.Id), () => modified++);
            todoDbContextMock.Setup(dbSet => dbSet.Set<TodoItem>()).Returns(dbSetMock);
            todoDbContextMock.Setup(dbSet => dbSet.TodoItems).Returns(dbSetMock);
        }
    }
}
