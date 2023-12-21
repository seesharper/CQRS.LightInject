using CQRS.Execution;
using CQRS.Query.Abstractions;
using LightInject;

namespace CQRS.LightInject
{
    /// <summary>
    /// An <see cref="IQueryHandlerScope"/> implementation that wraps the <see cref="Scope"/> created by LightInject.
    /// </summary>
    public class QueryHandlerScope : IQueryHandlerScope
    {
        private readonly Scope _scope;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryHandlerScope"/> class.
        /// </summary>
        /// <param name="scope">The <see cref="Scope"/> being wrapped by this <see cref="QueryHandlerScope"/>.</param>
        public QueryHandlerScope(Scope scope) => _scope = scope;

        /// <inheritdoc/>
        public IQueryExecutor CreateQueryExecutor() => _scope.GetInstance<IQueryExecutor>();

        /// <inheritdoc/>
        public void Dispose() => _scope.Dispose();


    }
}