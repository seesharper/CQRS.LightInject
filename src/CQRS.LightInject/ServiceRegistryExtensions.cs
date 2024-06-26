namespace CQRS.LightInject
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRS.Command.Abstractions;
    using CQRS.Execution;
    using CQRS.Query.Abstractions;
    using global::LightInject;

    /// <summary>
    /// Extends the <see cref="IServiceRegistry"/> with methods for registering command and query handlers.
    /// </summary>
    public static class ServiceRegistryExtensions
    {
        /// <summary>
        /// Adds all command handlers found in the calling assembly to the <paramref name="serviceRegistry"/> as scoped services.
        /// </summary>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/>.</param>
        /// <returns><see cref="IServiceRegistry"/>.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static IServiceRegistry RegisterCommandHandlers(this IServiceRegistry serviceRegistry)
            => RegisterCommandHandlers(serviceRegistry, Assembly.GetCallingAssembly());

        /// <summary>
        /// Adds all command handlers found in the given <paramref name="assembly"/> to the <paramref name="serviceRegistry"/> as scoped services.
        /// </summary>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/>.</param>
        /// <param name="assembly">The assembly from which to add command handlers. </param>
        /// <returns><see cref="IServiceRegistry"/>.</returns>
        public static IServiceRegistry RegisterCommandHandlers(this IServiceRegistry serviceRegistry, Assembly assembly)
        {
            var commandHandlerDescriptions = assembly.GetCommandHandlerDescriptors();

            foreach (var commandHandlerDescription in commandHandlerDescriptions)
            {
                if (commandHandlerDescription.HandlerType.ContainsGenericParameters)
                {
                    // If we have an open generic handler type, we register this as the base interface
                    // and let the generic argument mapper in LightInject handle this.
                    serviceRegistry.RegisterScoped(typeof(ICommandHandler<>), commandHandlerDescription.ImplementingType, commandHandlerDescription.ImplementingType.FullName);
                }
                else
                {
                    serviceRegistry.RegisterScoped(commandHandlerDescription.HandlerType, commandHandlerDescription.ImplementingType);
                }
            }

            serviceRegistry.RegisterScoped<ICommandHandlerFactory>(sp => new CommandHandlerFactory(sp));
            serviceRegistry.RegisterScoped<ICommandExecutor, CommandExecutor>();
            serviceRegistry.RegisterScoped(typeof(ICommandHandler<>), typeof(ScopedCommandHandler<>));
            serviceRegistry.RegisterScoped<ICommandHandlerScopeFactory>(sf => new CommandHandlerScopeFactory(sf));
            return serviceRegistry;
        }

        /// <summary>
        /// Adds all query handlers found in the calling assembly to the <paramref name="serviceRegistry"/> as scoped services.
        /// </summary>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/>.</param>
        /// <returns><see cref="IServiceRegistry"/>.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static IServiceRegistry RegisterQueryHandlers(this IServiceRegistry serviceRegistry)
            => RegisterQueryHandlers(serviceRegistry, Assembly.GetCallingAssembly());

        /// <summary>
        /// Adds all query handlers found in the given <paramref name="assembly"/> to the <paramref name="serviceRegistry"/> as scoped services.
        /// </summary>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/>.</param>
        /// <param name="assembly">The assembly from which to add query handlers. </param>
        /// <returns><see cref="IServiceRegistry"/>.</returns>
        public static IServiceRegistry RegisterQueryHandlers(this IServiceRegistry serviceRegistry, Assembly assembly)
        {
            var queryHandlerDescriptions = assembly.GetQueryHandlerHandlerDescriptors();

            foreach (var queryHandlerDescription in queryHandlerDescriptions)
            {
                if (queryHandlerDescription.HandlerType.ContainsGenericParameters)
                {
                    // If we have an open generic handler type, we register this as the base interface
                    // and let the generic argument mapper in LightInject handle this.
                    serviceRegistry.RegisterScoped(typeof(IQueryHandler<,>), queryHandlerDescription.ImplementingType, queryHandlerDescription.ImplementingType.FullName);
                }
                else
                {
                    serviceRegistry.RegisterScoped(queryHandlerDescription.HandlerType, queryHandlerDescription.ImplementingType);
                }
            }

            serviceRegistry.RegisterScoped<IQueryHandlerFactory>(sp => new QueryHandlerFactory(sp));
            serviceRegistry.RegisterScoped<IQueryExecutor, QueryExecutor>();
            serviceRegistry.RegisterScoped(typeof(IQueryHandler<,>), typeof(ScopedQueryHandler<,>));
            serviceRegistry.RegisterScoped<IQueryHandlerScopeFactory>(sf => new QueryHandlerScopeFactory(sf));
            return serviceRegistry;
        }

        /// <summary>
        /// Registers an function-based `ICommandHandler{TCommand}` decorator used to intercept command handlers.
        /// </summary>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/>.</param>
        /// <param name="implementation">A function representing the implementation of this handler.</param>
        /// <typeparam name="TCommand">The type of command being handeled.</typeparam>
        /// <returns><see cref="IServiceRegistry"/>.</returns>
        public static IServiceRegistry RegisterCommandInterceptor<TCommand>(this IServiceRegistry serviceRegistry, Func<TCommand, ICommandHandler<TCommand>, CancellationToken, Task> implementation)
            => serviceRegistry.Decorate<ICommandHandler<TCommand>>((factory, handler) => new InterceptingCommandHandler<TCommand>(handler, implementation));

        /// <summary>
        /// Registers an function-based `ICommandHandler{TCommand}` decorator used to intercept command handlers.
        /// </summary>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/>.</param>
        /// <param name="implementation">A function representing the implementation of this handler.</param>
        /// <typeparam name="TCommand">The type of command being handeled.</typeparam>
        /// <typeparam name="TDependency">The dependency beging passed to the <paramref name="implementation"/> function.</typeparam>
        /// <returns><see cref="IServiceRegistry"/>.</returns>
        public static IServiceRegistry RegisterCommandInterceptor<TCommand, TDependency>(this IServiceRegistry serviceRegistry, Func<TCommand, ICommandHandler<TCommand>, TDependency, CancellationToken, Task> implementation)
        {
            if (!typeof(TDependency).IsAbstract)
            {
                serviceRegistry.Register<TDependency>();
            }

            return serviceRegistry.Decorate<ICommandHandler<TCommand>>((factory, handler) => new InterceptingCommandHandler<TCommand, TDependency>(handler, factory.GetInstance<TDependency>(), implementation));
        }

        /// <summary>
        /// Registers an function-based `IQueryHandler{TQuery, TResult}` decorator used to intercept query handlers.
        /// </summary>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/>.</param>
        /// <param name="implementation">A function representing the implementation of this handler.</param>
        /// <typeparam name="TQuery">The type of query being handeled.</typeparam>
        /// <typeparam name="TResult">The type of result to be returned.</typeparam>
        /// <returns><see cref="IServiceRegistry"/>.</returns>
        public static IServiceRegistry RegisterQueryInterceptor<TQuery, TResult>(this IServiceRegistry serviceRegistry, Func<TQuery, IQueryHandler<TQuery, TResult>, CancellationToken, Task<TResult>> implementation)
            where TQuery : IQuery<TResult>
            => serviceRegistry.Decorate<IQueryHandler<TQuery, TResult>>((factory, handler) => new InterceptingQueryHandler<TQuery, TResult>(handler, implementation));

        /// <summary>
        /// Registers an function-based `IQueryHandler{TQuery, TResult}` decorator used to intercept query handlers.
        /// </summary>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/>.</param>
        /// <param name="implementation">A function representing the implementation of this handler.</param>
        /// <typeparam name="TQuery">The type of query being handeled.</typeparam>
        /// <typeparam name="TResult">The type of result to be returned.</typeparam>
        /// <typeparam name="TDependency">The dependency beging passed to the <paramref name="implementation"/> function.</typeparam>
        /// <returns><see cref="IServiceRegistry"/>.</returns>
        public static IServiceRegistry RegisterQueryInterceptor<TQuery, TResult, TDependency>(this IServiceRegistry serviceRegistry, Func<TQuery, IQueryHandler<TQuery, TResult>, TDependency, CancellationToken, Task<TResult>> implementation)
           where TQuery : IQuery<TResult>
        {
            if (!typeof(TDependency).IsAbstract)
            {
                serviceRegistry.Register<TDependency>();
            }

            return serviceRegistry.Decorate<IQueryHandler<TQuery, TResult>>((factory, handler) => new InterceptingQueryHandler<TQuery, TResult, TDependency>(handler, factory.GetInstance<TDependency>(), implementation));
        }
    }
}
