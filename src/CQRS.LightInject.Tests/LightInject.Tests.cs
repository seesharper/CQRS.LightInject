using System.Linq;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using FluentAssertions;
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
        public void ShouldBeSameInstanceQueryExecutor()
        {
            var container = new ServiceContainer();
            container.RegisterQueryHandlers();
            using (var scope = container.BeginScope())
            {
                var queryExecutor1 = scope.GetInstance<IQueryExecutor>();
                var queryExecutor2 = scope.GetInstance<IQueryExecutor>();
                Assert.Equal(queryExecutor1, queryExecutor2);
            }
        }

        [Fact]
        public void ShouldNotBeSameInstanceQueryExecutor()
        {
            var container = new ServiceContainer();
            container.RegisterQueryHandlers(new PerRequestLifeTime());
            using (var scope = container.BeginScope())
            {
                var queryExecutor1 = scope.GetInstance<IQueryExecutor>();
                var queryExecutor2 = scope.GetInstance<IQueryExecutor>();
                Assert.NotEqual(queryExecutor1, queryExecutor2);
            }
        }

        [Fact]
        public void ShouldBeSameInstanceQueryHandler()
        {
            var container = new ServiceContainer();
            container.RegisterQueryHandlers();
            using (var scope = container.BeginScope())
            {
                var queryHandler1 = scope.GetInstance<IQueryHandler<OpenGenericQuery<Derived>, Derived>>();
                var queryHandler2 = scope.GetInstance<IQueryHandler<OpenGenericQuery<Derived>, Derived>>();
                Assert.Equal(queryHandler1, queryHandler2);
            }
        }

        [Fact]
        public void ShouldNotBeSameInstanceQueryHandler()
        {
            var container = new ServiceContainer();
            container.RegisterQueryHandlers(new PerRequestLifeTime());
            using (var scope = container.BeginScope())
            {
                var queryHandler1 = scope.GetInstance<IQueryHandler<OpenGenericQuery<Derived>, Derived>>();
                var queryHandler2 = scope.GetInstance<IQueryHandler<OpenGenericQuery<Derived>, Derived>>();
                Assert.NotEqual(queryHandler1, queryHandler2);
            }
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
            using (var scope = container.BeginScope())
            {
                var commandExecutor = scope.GetInstance<ICommandExecutor>();
                var command = new DerivedCommand();
                await commandExecutor.ExecuteAsync(command);
                command.WasHandled.Should().BeTrue();
            }
        }

        [Fact]
        public void ShouldBeSameInstanceCommandHandler()
        {
            var container = new ServiceContainer();
            container.RegisterCommandHandlers();
            using (var scope = container.BeginScope())
            {
                var handler1 = container.GetInstance<ICommandHandler<SampleCommand>>();
                var handler2 = container.GetInstance<ICommandHandler<SampleCommand>>();
                Assert.Equal(handler1, handler2);
            }
        }

        [Fact]
        public void ShouldNotBeSameInstanceCommandHandler()
        {
            var container = new ServiceContainer();
            container.RegisterCommandHandlers(new PerRequestLifeTime());
            using (var scope = container.BeginScope())
            {
                var handler1 = container.GetInstance<ICommandHandler<SampleCommand>>();
                var handler2 = container.GetInstance<ICommandHandler<SampleCommand>>();
                Assert.NotEqual(handler1, handler2);
            }
        }

        [Fact]
        public void ShouldBeSameInstanceCommandExecutor()
        {
            var container = new ServiceContainer();
            container.RegisterCommandHandlers();
            using (var scope = container.BeginScope())
            {
                var commandExecutor1 = scope.GetInstance<ICommandExecutor>();
                var commandExecutor2 = scope.GetInstance<ICommandExecutor>();
                Assert.Equal(commandExecutor1, commandExecutor2);
            }
        }

        [Fact]
        public void ShouldNotBeSameInstanceCommandExecutor()
        {
            var container = new ServiceContainer();
            container.RegisterCommandHandlers(new PerRequestLifeTime());
            using (var scope = container.BeginScope())
            {
                var commandExecutor1 = scope.GetInstance<ICommandExecutor>();
                var commandExecutor2 = scope.GetInstance<ICommandExecutor>();
                Assert.NotEqual(commandExecutor1, commandExecutor2);
            }
        }
    }
}
