using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using FluentAssertions;
using LightInject;
using Xunit;

namespace CQRS.LightInject.Tests
{
    public class InterceptorTests
    {
        [Fact]
        public async Task ShouldInterceptCommandHandler()
        {
            var container = new ServiceContainer();
            container.RegisterCommandHandlers();
            container.Register<IFoo, Foo>();
            bool invoked = false;

            container.RegisterCommandInterceptor<SampleCommand>(async (command, handler, token) =>
            {
                invoked = true;
                await handler.HandleAsync(command, token);
            }
            );

            var command = new SampleCommand();
            using (var scope = container.BeginScope())
            {
                await container.GetInstance<ICommandHandler<SampleCommand>>().HandleAsync(command);
                invoked.Should().BeTrue();
                command.WasHandled.Should().BeTrue();
            }
        }

        [Fact]
        public async Task ShouldInterceptCommandHandlerWithDependency()
        {
            var container = new ServiceContainer();
            container.RegisterCommandHandlers();
            container.Register<IFoo, Foo>();
            bool invoked = false;
            IFoo passedFoo = null;
            container.RegisterCommandInterceptor<SampleCommand, IFoo>(async (command, handler, foo, token) =>
            {
                invoked = true;
                passedFoo = foo;
                await handler.HandleAsync(command, token);
            }
            );

            var command = new SampleCommand();
            using (var scope = container.BeginScope())
            {
                await container.GetInstance<ICommandHandler<SampleCommand>>().HandleAsync(command);
                invoked.Should().BeTrue();
                command.WasHandled.Should().BeTrue();
                passedFoo.Should().BeOfType<Foo>();
            }
        }


        [Fact]
        public async Task ShouldInterceptCommandWithMultipleDependencies()
        {
            var container = new ServiceContainer();
            container.RegisterCommandHandlers();
            container.Register<IFoo, Foo>();
            container.Register<IBar, Bar>();

            bool invoked = false;
            IFoo passedFoo = null;
            IBar passedBar = null;

            container.RegisterCommandInterceptor<SampleCommand, (IBar bar, IFoo foo)>(async (command, handler, dependencies, token) =>
            {
                passedFoo = dependencies.foo;
                passedBar = dependencies.bar;
                invoked = true;
                await handler.HandleAsync(command, token);
            });

            var command = new SampleCommand();
            using (var scope = container.BeginScope())
            {
                await container.GetInstance<ICommandHandler<SampleCommand>>().HandleAsync(command);
                invoked.Should().BeTrue();
                command.WasHandled.Should().BeTrue();
                passedFoo.Should().BeOfType<Foo>();
                passedBar.Should().BeOfType<Bar>();
            }
        }

        [Fact]
        public async Task ShouldInterceptQueryHandler()
        {
            var container = new ServiceContainer();
            container.RegisterQueryHandlers();
            container.Register<IFoo, Foo>();
            bool invoked = false;

            container.RegisterQueryInterceptor<SampleQuery, SampleQueryResult>(async (command, handler, token) =>
            {
                invoked = true;
                return await handler.HandleAsync(command, token);
            }
            );

            var query = new SampleQuery();
            using (var scope = container.BeginScope())
            {
                await container.GetInstance<IQueryHandler<SampleQuery, SampleQueryResult>>().HandleAsync(query);
                invoked.Should().BeTrue();
                query.WasHandled.Should().BeTrue();
            }
        }

        [Fact]
        public async Task ShouldInterceptQueryHandlerWithDependency()
        {
            var container = new ServiceContainer();
            container.RegisterQueryHandlers();
            container.Register<IFoo, Foo>();
            bool invoked = false;
            IFoo passedFoo = null;

            container.RegisterQueryInterceptor<SampleQuery, SampleQueryResult, IFoo>(async (command, handler, foo, token) =>
            {
                passedFoo = foo;
                invoked = true;
                return await handler.HandleAsync(command, token);
            }
            );

            var query = new SampleQuery();
            using (var scope = container.BeginScope())
            {
                await container.GetInstance<IQueryHandler<SampleQuery, SampleQueryResult>>().HandleAsync(query);
                invoked.Should().BeTrue();
                query.WasHandled.Should().BeTrue();
                passedFoo.Should().BeOfType<Foo>();
            }
        }

        [Fact]
        public async Task ShouldInterceptQueryHandlerWithMultipleDependencies()
        {
            var container = new ServiceContainer();
            container.RegisterQueryHandlers();
            container.Register<IFoo, Foo>();
            container.Register<IBar, Bar>();
            bool invoked = false;
            IFoo passedFoo = null;
            IBar passedBar = null;

            container.RegisterQueryInterceptor<SampleQuery, SampleQueryResult, (IFoo foo, IBar bar)>(async (command, handler, dependencies, token) =>
            {
                passedFoo = dependencies.foo;
                passedBar = dependencies.bar;
                invoked = true;
                return await handler.HandleAsync(command, token);
            }
            );

            var query = new SampleQuery();
            using (var scope = container.BeginScope())
            {
                await container.GetInstance<IQueryHandler<SampleQuery, SampleQueryResult>>().HandleAsync(query);
                invoked.Should().BeTrue();
                query.WasHandled.Should().BeTrue();
                passedFoo.Should().BeOfType<Foo>();
                passedBar.Should().BeOfType<Bar>();
            }
        }
    }
}