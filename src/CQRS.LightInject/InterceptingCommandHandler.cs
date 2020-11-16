namespace CQRS.LightInject
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRS.Command.Abstractions;

    /// <summary>
    /// A <see cref="ICommandHandler{TCommand}"/> decorator used to intercept command handlers.
    /// </summary>
    /// <typeparam name="TCommand">The type of command to handle.</typeparam>
    public class InterceptingCommandHandler<TCommand> : ICommandHandler<TCommand>
    {
        private readonly ICommandHandler<TCommand> handler;
        private readonly Func<TCommand, ICommandHandler<TCommand>, CancellationToken, Task> implementation;

        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptingCommandHandler{TCommand}"/> class.
        /// </summary>
        /// <param name="handler">The handler being decorated.</param>
        /// <param name="implementation">The function representing the implementation of this handler.</param>
        public InterceptingCommandHandler(ICommandHandler<TCommand> handler, Func<TCommand, ICommandHandler<TCommand>, CancellationToken, Task> implementation)
        {
            this.handler = handler;
            this.implementation = implementation;
        }

        /// <inheritdoc/>
        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
            => await implementation(command, handler, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// A <see cref="ICommandHandler{TCommand}"/> decorator used to intercept command handlers.
    /// </summary>
    /// <typeparam name="TCommand">The type of command to handle.</typeparam>
    /// <typeparam name="TDependency">The dependency to be passed to the implementation function.</typeparam>
    public class InterceptingCommandHandler<TCommand, TDependency> : ICommandHandler<TCommand>
    {
        private readonly ICommandHandler<TCommand> handler;
        private readonly TDependency dependency;
        private readonly Func<TCommand, ICommandHandler<TCommand>, TDependency, CancellationToken, Task> implementation;

        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptingCommandHandler{TCommand, TDependency}"/> class.
        /// </summary>
        /// <param name="handler">The handler being decorated.</param>
        /// <param name="dependency">The dependency to be passed to the implementation function.</param>
        /// <param name="implementation">The function representing the implementation of this handler.</param>
        public InterceptingCommandHandler(ICommandHandler<TCommand> handler, TDependency dependency, Func<TCommand, ICommandHandler<TCommand>, TDependency, CancellationToken, Task> implementation)
        {
            this.handler = handler;
            this.dependency = dependency;
            this.implementation = implementation;
        }

        /// <inheritdoc/>
        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
            => await implementation(command, handler, dependency, cancellationToken).ConfigureAwait(false);
    }
}
