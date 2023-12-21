using CQRS.Execution;
using LightInject;

namespace CQRS.LightInject
{
    /// <summary>
    /// An <see cref="IQueryHandlerScopeFactory"/> implementation that uses LightInject to create a scope.
    /// </summary>
    public class QueryHandlerScopeFactory : IQueryHandlerScopeFactory
    {
        private readonly IServiceFactory _factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryHandlerScopeFactory"/> class.
        /// </summary>
        /// <param name="factory">The <see cref="IServiceFactory"/> used to create a new <see cref="ICommandHandlerScope"/>.</param>
        public QueryHandlerScopeFactory(IServiceFactory factory) => _factory = factory;

        /// <inheritdoc/>
        public IQueryHandlerScope CreateScope() => new QueryHandlerScope(_factory.BeginScope());
    }
}