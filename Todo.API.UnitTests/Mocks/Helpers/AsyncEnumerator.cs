using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Todo.API.UnitTests
{
    public class AsyncEnumerator<TEntity> : IAsyncEnumerator<TEntity>
           where TEntity : class, new()
    {
        private IEnumerator<TEntity> enumerator;

        public AsyncEnumerator(IEnumerator<TEntity> enumerator)
        {
            this.enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
        }

        public TEntity Current
        {
            get { return enumerator.Current; }
        }

        public Task<bool> MoveNext(CancellationToken cancelationToken)
        {
            return new Task<bool>(() => enumerator.MoveNext(), cancelationToken);
        }

        public ValueTask<bool> MoveNextAsync()
        {
            try
            {
                return new ValueTask<bool>(this.MoveNext(new CancellationToken()).Result);
            }
            catch (Exception exception)
            {
                return new ValueTask<bool>(Task.FromException<bool>(exception));
            }
        }

        public async ValueTask DisposeAsync()
        {
            Dispose(disposing: false);
            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                enumerator?.Dispose();
            }

            enumerator = null;
        }
    }
}
