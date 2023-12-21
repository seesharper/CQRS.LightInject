using CQRS.Command.Abstractions;
using CQRS.Execution;
using LightInject;

namespace CQRS.LightInject
{
    /// <summary>
    /// An <see cref="ICommandHandlerScope"/> implementation that wraps the <see cref="Scope"/> created by LightInject.
    /// </summary>
    public class CommandHandlerScope : ICommandHandlerScope
    {
        private readonly Scope _scope;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandlerScope"/> class.
        /// </summary>
        /// <param name="scope">The <see cref="Scope"/> being wrapped by this <see cref="CommandHandlerScope"/>.</param>
        public CommandHandlerScope(Scope scope) => _scope = scope;

        /// <inheritdoc/>
        public ICommandExecutor CreateCommandExecutor() => _scope.GetInstance<ICommandExecutor>();

        /// <inheritdoc/>
        public void Dispose() => _scope.Dispose();
    }
}