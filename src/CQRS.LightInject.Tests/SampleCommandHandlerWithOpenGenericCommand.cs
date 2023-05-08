using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;

namespace CQRS.Execution.Tests;

public record SampleOpenGenericCommand<T>()
{
    public bool WasHandled { get; set; }
}

public class SampleCommandHandlerWithOpenGenericCommand<T> : ICommandHandler<SampleOpenGenericCommand<T>>
{
    public SampleCommandHandlerWithOpenGenericCommand()
    {
    }

    public Task HandleAsync(SampleOpenGenericCommand<T> command, CancellationToken cancellationToken = default)
    {
        command.WasHandled = true;
        return Task.CompletedTask;
    }
}