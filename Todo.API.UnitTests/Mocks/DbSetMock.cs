using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Todo.API.UnitTests.Mocks
{
    public static class DbSetMock
    {
        private static Mock<DbSet<TEntity>> GetDbSetMock<TEntity>(IQueryable<TEntity> data)
            where TEntity : class, new()
        {
            var mock = new Mock<DbSet<TEntity>>();

            mock.As<IQueryable<TEntity>>()
                .Setup(m => m.Expression)
                .Returns(data.Expression);

            mock.As<IQueryable<TEntity>>()
                .Setup(m => m.ElementType)
                .Returns(data.ElementType);

            mock.As<IQueryable<TEntity>>()
                .Setup(m => m.GetEnumerator())
                .Returns(data.GetEnumerator());

            mock.As<IQueryable<TEntity>>()
                .Setup(m => m.Provider)
                .Returns(data.Provider);

            mock.As<IAsyncEnumerable<TEntity>>()
                .Setup(m => m.GetAsyncEnumerator(new CancellationToken()))
                .Returns(new AsyncEnumerator<TEntity>(data.GetEnumerator()));

            mock.Setup(m => m.Attach(It.IsAny<TEntity>()))
                .Returns((EntityEntry<TEntity> entityEntry) => entityEntry);

            return mock;
        }

        public static DbSet<TEntity> GetDbSetMock<TEntity>(List<TEntity> data, Func<TEntity, int> keyExp, string keyName, Func<int> modified = null)
           where TEntity : class, new()
        {
            var mock = GetDbSetMock(data.AsQueryable());

            mock.Setup(m => m.Find(It.IsAny<object[]>()))
                .Returns<object[]>(ids => data.FirstOrDefault(d => keyExp(d) == (int)ids[0]));

            mock.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                .Returns<object[]>(async (ids) => await Task.FromResult(data.FirstOrDefault(d => keyExp(d) == (int)ids[0])));

            mock.Setup(m => m.Add(It.IsAny<TEntity>()))
                .Callback((TEntity entity) =>
                {
                    var propInfo = entity.GetType().GetProperty(keyName);

                    var value = data.Any() ? data.Max(keyExp) + 1 : 1;

                    propInfo.SetValue(entity, value, null);

                    data.Add(entity);
                });

            mock.Setup(m => m.AddRange(It.IsAny<IEnumerable<TEntity>>()))
                .Callback((IEnumerable<TEntity> entities) =>
                {
                    foreach (var entity in entities)
                    {
                        var propInfo = entity.GetType().GetProperty(keyName);

                        var value = data.Any() ? data.Max(keyExp) + 1 : 1;

                        propInfo.SetValue(entity, value, null);

                        data.Add(entity);
                    }
                });

            mock.Setup(m => m.Update(It.IsAny<TEntity>()))
                .Callback((TEntity entity) =>
                {
                    var index = data.FindIndex(e => keyExp(e) == keyExp(entity));

                    data[index] = entity;
                    
                    modified();
                });

            mock.Setup(m => m.Remove(It.IsAny<TEntity>()))
                .Callback((TEntity entity) => data.ToList().Remove(entity));

            return mock.Object;
        }

        public static DbSet<TEntity> GetDbSetMock<TEntity>(List<TEntity> data, Func<TEntity, long> keyExp, string keyName)
           where TEntity : class, new()
        {
            var mock = GetDbSetMock(data.AsQueryable());

            mock.Setup(m => m.Find(It.IsAny<object[]>()))
                .Returns<object[]>(ids => data.FirstOrDefault(d => keyExp(d) == (long)ids[0]));

            mock.Setup(m => m.FindAsync(It.IsAny<object[]>()))
               .Returns<object[]>(async (ids) => await Task.FromResult(data.FirstOrDefault(d => keyExp(d) == (long)ids[0])));

            mock.Setup(m => m.Add(It.IsAny<TEntity>()))
                .Callback((TEntity entity) =>
                {
                    var propInfo = entity.GetType().GetProperty(keyName);

                    var value = data.Any() ? data.Max(keyExp) + 1 : 1;

                    propInfo.SetValue(entity, value, null);

                    data.Add(entity);
                });

            mock.Setup(m => m.AddRange(It.IsAny<IEnumerable<TEntity>>()))
                .Callback((IEnumerable<TEntity> entities) =>
                {
                    foreach (var entity in entities)
                    {
                        var propInfo = entity.GetType().GetProperty(keyName);

                        var value = data.Any() ? data.Max(keyExp) + 1 : 1;

                        propInfo.SetValue(entity, value, null);

                        data.Add(entity);
                    }
                });

            mock.Setup(m => m.Update(It.IsAny<TEntity>()))
                .Callback((TEntity entity) =>
                {
                    var index = data.FindIndex(e => keyExp(e) == keyExp(entity));

                    data[index] = entity;
                });

            mock.Setup(m => m.Remove(It.IsAny<TEntity>()))
                .Callback((TEntity entity) => data.ToList().Remove(entity));

            return mock.Object;
        }
    }
}
