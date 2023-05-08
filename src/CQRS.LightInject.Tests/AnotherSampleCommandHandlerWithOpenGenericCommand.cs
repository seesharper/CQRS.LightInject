using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;

namespace CQRS.Execution.Tests;

public record AnotherSampleOpenGenericCommand<T>()
{
    public bool WasHandled { get; set; }
}

public class AnotherSampleCommandHandlerWithOpenGenericCommand<T> : ICommandHandler<AnotherSampleOpenGenericCommand<T>>
{
    public AnotherSampleCommandHandlerWithOpenGenericCommand()
    {
    }

    public Task HandleAsync(AnotherSampleOpenGenericCommand<T> command, CancellationToken cancellationToken = default)
    {
        command.WasHandled = true;
        return Task.CompletedTask;
    }
}