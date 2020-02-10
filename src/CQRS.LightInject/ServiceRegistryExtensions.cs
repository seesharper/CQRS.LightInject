namespace CQRS.LightInject
{
    using System.Reflection;
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
        public static IServiceRegistry RegisterCommandHandlers(this IServiceRegistry serviceRegistry)
        {
            return RegisterCommandHandlers(serviceRegistry, Assembly.GetCallingAssembly(), new PerScopeLifetime());
        }

        /// <summary>
        /// Adds all command handlers found in the calling assembly to the <paramref name="serviceRegistry"/> as scoped services.
        /// </summary>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/>.</param>
        /// <param name="lifetime">Lifetime of <see cref="ICommandHandler{TCommand}"/></param>
        /// <returns><see cref="IServiceRegistry"/>.</returns>
        public static IServiceRegistry RegisterCommandHandlers(this IServiceRegistry serviceRegistry, ILifetime lifetime)
        {
            return RegisterCommandHandlers(serviceRegistry, Assembly.GetCallingAssembly(), lifetime);
        }

        /// <summary>
        /// Adds all command handlers found in the given <paramref name="assembly"/> to the <paramref name="serviceRegistry"/> as scoped services.
        /// </summary>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/>.</param>
        /// <param name="assembly">The assembly from which to add command handlers. </param>
        /// <returns><see cref="IServiceRegistry"/>.</returns>
        public static IServiceRegistry RegisterCommandHandlers(this IServiceRegistry serviceRegistry, Assembly assembly)
        {
            return RegisterCommandHandlers(serviceRegistry, assembly, new PerScopeLifetime());
        }


        /// <summary>
        /// Adds all command handlers found in the given <paramref name="assembly"/> to the <paramref name="serviceRegistry"/> as scoped services.
        /// </summary>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/>.</param>
        /// <param name="assembly">The assembly from which to add command handlers. </param>
        /// <param name="lifetime">Lifetime of <see cref="ICommandHandler{TCommand}"/></param>
        /// <returns><see cref="IServiceRegistry"/>.</returns>
        public static IServiceRegistry RegisterCommandHandlers(this IServiceRegistry serviceRegistry, Assembly assembly, ILifetime lifetime)
        {
            var commandHanderDescriptions = assembly.GetCommandHandlerDescriptors();

            foreach (var commandHanderDescription in commandHanderDescriptions)
            {
                if (commandHanderDescription.HandlerType.ContainsGenericParameters)
                {
                    // If we have an open generic handler type, we register this as the base interface
                    // and let the generic argument mapper in LightInject handle this.
                    serviceRegistry.Register(typeof(ICommandHandler<>), commandHanderDescription.ImplementingType, commandHanderDescription.ImplementingType.FullName, lifetime);
                }
                else
                {
                    serviceRegistry.Register(commandHanderDescription.HandlerType, commandHanderDescription.ImplementingType, lifetime);
                }
            }

            serviceRegistry.Register<ICommandHandlerFactory>(sp => new CommandHandlerFactory(sp), lifetime);
            serviceRegistry.Register<ICommandExecutor, CommandExecutor>(lifetime);

            return serviceRegistry;
        }

        /// <summary>
        /// Adds all query handlers found in the calling assembly to the <paramref name="serviceRegistry"/> as scoped services.
        /// </summary>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/>.</param>
        /// <returns><see cref="IServiceRegistry"/>.</returns>
        public static IServiceRegistry RegisterQueryHandlers(this IServiceRegistry serviceRegistry)
        {
            return RegisterQueryHandlers(serviceRegistry, Assembly.GetCallingAssembly(), new PerScopeLifetime());
        }

        /// <summary>
        /// Adds all query handlers found in the calling assembly to the <paramref name="serviceRegistry"/> as scoped services.
        /// </summary>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/>.</param>
        /// <param name="lifetime">Lifetime of <see cref="IQueryHandler{TQuery,TResult}"/></param>
        /// <returns><see cref="IServiceRegistry"/>.</returns>
        public static IServiceRegistry RegisterQueryHandlers(this IServiceRegistry serviceRegistry, ILifetime lifetime)
        {
            return RegisterQueryHandlers(serviceRegistry, Assembly.GetCallingAssembly(), lifetime);
        }

        /// <summary>
        /// Adds all query handlers found in the given <paramref name="assembly"/> to the <paramref name="serviceRegistry"/> as scoped services.
        /// </summary>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/>.</param>
        /// <param name="assembly">The assembly from which to add query handlers. </param>
        /// <returns><see cref="IServiceRegistry"/>.</returns>
        public static IServiceRegistry RegisterQueryHandlers(this IServiceRegistry serviceRegistry, Assembly assembly)
        {
            return RegisterQueryHandlers(serviceRegistry, assembly, new PerScopeLifetime());
        }

        /// <summary>
        /// Adds all query handlers found in the given <paramref name="assembly"/> to the <paramref name="serviceRegistry"/> as scoped services.
        /// </summary>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/>.</param>
        /// <param name="assembly">The assembly from which to add query handlers. </param>
        /// <param name="lifetime">Lifetime of <see cref="IQueryHandler{TQuery,TResult}"/></param>
        /// <returns><see cref="IServiceRegistry"/>.</returns>
        public static IServiceRegistry RegisterQueryHandlers(this IServiceRegistry serviceRegistry, Assembly assembly, ILifetime lifetime)
        {
            var queryHandlerDescriptions = assembly.GetQueryHandlerHandlerDescriptors();

            foreach (var queryHandlerDescription in queryHandlerDescriptions)
            {
                if (queryHandlerDescription.HandlerType.ContainsGenericParameters)
                {
                    // If we have an open generic handler type, we register this as the base interface
                    // and let the generic argument mapper in LightInject handle this.
                    serviceRegistry.Register(typeof(IQueryHandler<,>), queryHandlerDescription.ImplementingType, queryHandlerDescription.ImplementingType.FullName, lifetime);
                }
                else
                {
                    serviceRegistry.Register(queryHandlerDescription.HandlerType, queryHandlerDescription.ImplementingType, lifetime);
                }
            }

            serviceRegistry.Register<IQueryHandlerFactory>(sp => new QueryHandlerFactory(sp), lifetime);
            serviceRegistry.Register<IQueryExecutor, QueryExecutor>(lifetime);

            return serviceRegistry;
        }
    }
}
