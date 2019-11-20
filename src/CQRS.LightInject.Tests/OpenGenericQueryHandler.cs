using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;

namespace CQRS.LightInject.Tests
{
    public class OpenGenericQueryHandler<TResult> : IQueryHandler<OpenGenericQuery<TResult>, TResult> where TResult : BaseClass
    {
        public Task<TResult> HandleAsync(OpenGenericQuery<TResult> query, CancellationToken cancellationToken = default)
        {
            query.WasHandled = true;
            return Task.FromResult(default(TResult));
        }
    }

    public class OpenGenericQueryResult
    {
    }

    public class OpenGenericQuery<TResult> : IQuery<TResult> where TResult : BaseClass
    {
        public bool WasHandled { get; set; }
    }
}
public class BaseClass
{
}

public class Derived : BaseClass
{
}