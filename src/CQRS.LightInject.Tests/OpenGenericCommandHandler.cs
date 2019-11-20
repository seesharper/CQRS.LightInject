using CQRS.Command.Abstractions;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace CQRS.LightInject.Tests
{
    public class OpenGenericCommandHandler<TCommand> : ICommandHandler<TCommand> where TCommand : BaseCommand
    {
        public Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            command.WasHandled = true;
            return Task.CompletedTask;
        }
    }

    public class BaseCommand
    {
        public bool WasHandled { get; set; }
    }

    public class DerivedCommand : BaseCommand
    {

    }
}