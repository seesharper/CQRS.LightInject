using CQRS.Execution;
using LightInject;

namespace CQRS.LightInject
{
    /// <summary>
    /// An <see cref="ICommandHandlerScopeFactory"/> implementation that uses LightInject to create a scope.
    /// </summary>
    public class CommandHandlerScopeFactory : ICommandHandlerScopeFactory
    {
        private readonly IServiceFactory _factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandlerScopeFactory"/> class.
        /// </summary>
        /// <param name="factory">The <see cref="IServiceFactory"/> used to create a new <see cref="ICommandHandlerScope"/>.</param>
        public CommandHandlerScopeFactory(IServiceFactory factory) => _factory = factory;

        /// <inheritdoc/>
        public ICommandHandlerScope CreateScope() => new CommandHandlerScope(_factory.BeginScope());
    }
}