using System;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Execution;
using CQRS.Query.Abstractions;
using AwesomeAssertions;
using LightInject;
using Xunit;

namespace CQRS.LightInject.Tests;

public class ScopedHandlerTests
{

    [Fact]
    public async Task ShouldExecuteScopedCommandHandler()
    {
        var container = new ServiceContainer();
        container.RegisterScoped<IFoo, Foo>();
        container.RegisterCommandHandlers();
        using (var scope = container.BeginScope())
        {
            var commandExecutor = container.GetInstance<ICommandExecutor>();
            var command = new SampleCommand();
            await commandExecutor.ExecuteScopedAsync(command);
            command.WasHandled.Should().BeTrue();
        }
    }

    [Fact]
    public async Task ShouldExecuteScopedQueryHandler()
    {
        var container = new ServiceContainer();
        container.RegisterScoped<IFoo, Foo>();
        container.RegisterQueryHandlers();
        using (var scope = container.BeginScope())
        {
            var queryExecutor = container.GetInstance<IQueryExecutor>();
            var query = new SampleQuery();
            await queryExecutor.ExecuteScopedAsync(query);
            query.WasHandled.Should().BeTrue();
        }
    }

    [Fact]
    public async Task ShouldHandleExecutingUnknownScopedCommand()
    {
        var container = new ServiceContainer();
        container.RegisterScoped<IFoo, Foo>();
        container.RegisterCommandHandlers();
        using (var scope = container.BeginScope())
        {
            var commandExecutor = container.GetInstance<ICommandExecutor>();
            var command = new UnknownCommand();
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await commandExecutor.ExecuteScopedAsync(command));
        }
    }

    public class UnknownCommand
    {
    }
}