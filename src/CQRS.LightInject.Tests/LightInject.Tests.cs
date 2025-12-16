using System.Linq;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Execution.Tests;
using CQRS.Query.Abstractions;
using AwesomeAssertions;
using LightInject;
using Xunit;

namespace CQRS.LightInject.Tests
{
    public class CommandExecutorTests
    {
        [Fact]
        public async Task ShouldExecuteCommandHandler()
        {
            var container = new ServiceContainer();
            container.Register<IFoo, Foo>();
            container.RegisterCommandHandlers();
            using (var scope = container.BeginScope())
            {
                var commandExecutor = scope.GetInstance<ICommandExecutor>();
                var command = new SampleCommand();
                await commandExecutor.ExecuteAsync(command);
                command.WasHandled.Should().BeTrue();
            }
        }

        [Fact]
        public async Task ShouldExecuteQueryHandler()
        {
            var container = new ServiceContainer();
            container.RegisterQueryHandlers();
            container.Register<IFoo, Foo>();
            using (var scope = container.BeginScope())
            {
                var queryExecutor = scope.GetInstance<IQueryExecutor>();
                var query = new SampleQuery();
                var result = await queryExecutor.ExecuteAsync(query);

                query.WasHandled.Should().BeTrue();
            }
        }

        [Fact]
        public void ShouldNotAddQueryExecutorTwice()
        {
            var container = new ServiceContainer();
            container.RegisterQueryHandlers();
            container.RegisterQueryHandlers();

            container.AvailableServices.Count(sr => sr.ServiceType == typeof(IQueryExecutor)).Should().Be(1);
        }

        [Fact]
        public void ShouldNotAddCommandExecutorTwice()
        {
            var container = new ServiceContainer();
            container.RegisterCommandHandlers();
            container.RegisterCommandHandlers();

            container.AvailableServices.Count(sr => sr.ServiceType == typeof(ICommandExecutor)).Should().Be(1);
        }

        [Fact]
        public async Task ShouldExecuteOpenGenericQueryHandler()
        {
            var container = new ServiceContainer();
            container.RegisterQueryHandlers();
            container.Register<IFoo, Foo>();
            using (var scope = container.BeginScope())
            {
                var queryExecutor = scope.GetInstance<IQueryExecutor>();
                var handler = container.GetInstance<IQueryHandler<OpenGenericQuery<Derived>, Derived>>();
                var query = new OpenGenericQuery<Derived>();
                var result = await queryExecutor.ExecuteAsync(query);
                query.WasHandled.Should().BeTrue();
            }
        }

        [Fact]
        public async Task ShouldExecuteOpenGenericCommandHandler()
        {
            var container = new ServiceContainer();
            container.RegisterCommandHandlers();
            container.Register<IFoo, Foo>();
            using (var scope = container.BeginScope())
            {
                var commandExecutor = scope.GetInstance<ICommandExecutor>();
                var command = new DerivedCommand();
                await commandExecutor.ExecuteAsync(command);
                command.WasHandled.Should().BeTrue();
            }
        }

        [Fact]
        public async Task ShouldExecuteCommandHandlerWithOpenGenericCommand()
        {
            var container = new ServiceContainer();
            container.RegisterCommandHandlers();
            container.Register<IFoo, Foo>();
            using (var scope = container.BeginScope())
            {
                var commandExecutor = scope.GetInstance<ICommandExecutor>();
                var sampleCommand = new SampleOpenGenericCommand<string>();
                await commandExecutor.ExecuteAsync(sampleCommand);
                sampleCommand.WasHandled.Should().BeTrue();
                var anotherSampleCommand = new AnotherSampleOpenGenericCommand<string>();
                await commandExecutor.ExecuteAsync(anotherSampleCommand);
                anotherSampleCommand.WasHandled.Should().BeTrue();
            }
        }
    }
}
