namespace CQRS.LightInject
{
    using CQRS.Command.Abstractions;
    using CQRS.Execution;
    using global::LightInject;

    /// <summary>
    /// An <see cref="ICommandHandlerFactory"/> implementation that uses <see cref="IServiceFactory"/> to create command handlers.
    /// </summary>
    public class CommandHandlerFactory : ICommandHandlerFactory
    {
        private readonly IServiceFactory serviceFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandlerFactory"/> class.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceFactory"/> that is responsible for creating command handlers.</param>
        public CommandHandlerFactory(IServiceFactory serviceProvider)
        {
            this.serviceFactory = serviceProvider;
        }

        /// <inheritdoc/>
        public ICommandHandler<TCommand> CreateCommandHandler<TCommand>()
        {
            return serviceFactory.GetInstance<ICommandHandler<TCommand>>();
        }
    }
}
