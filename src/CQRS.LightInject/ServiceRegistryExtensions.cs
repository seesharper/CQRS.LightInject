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
            return RegisterCommandHandlers(serviceRegistry, Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Adds all command handlers found in the given <paramref name="assembly"/> to the <paramref name="serviceRegistry"/> as scoped services.
        /// </summary>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/>.</param>
        /// <param name="assembly">The assembly from which to add command handlers. </param>
        /// <returns><see cref="IServiceRegistry"/>.</returns>
        public static IServiceRegistry RegisterCommandHandlers(this IServiceRegistry serviceRegistry, Assembly assembly)
        {
            var commandHanderDescriptions = assembly.GetCommandHandlerDescriptors();

            foreach (var commandHanderDescription in commandHanderDescriptions)
            {
                if (commandHanderDescription.HandlerType.ContainsGenericParameters)
                {
                    // If we have an open generic handler type, we register this as the base interface
                    // and let the generic argument mapper in LightInject handle this.
                    serviceRegistry.RegisterScoped(typeof(ICommandHandler<>), commandHanderDescription.ImplementingType, commandHanderDescription.ImplementingType.FullName);
                }
                else
                {
                    serviceRegistry.RegisterScoped(commandHanderDescription.HandlerType, commandHanderDescription.ImplementingType);
                }
            }

            serviceRegistry.RegisterScoped<ICommandHandlerFactory>(sp => new CommandHandlerFactory(sp));
            serviceRegistry.RegisterScoped<ICommandExecutor, CommandExecutor>();

            return serviceRegistry;
        }

        /// <summary>
        /// Adds all query handlers found in the calling assembly to the <paramref name="serviceRegistry"/> as scoped services.
        /// </summary>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/>.</param>
        /// <returns><see cref="IServiceRegistry"/>.</returns>
        public static IServiceRegistry RegisterQueryHandlers(this IServiceRegistry serviceRegistry)
        {
            return RegisterQueryHandlers(serviceRegistry, Assembly.GetCallingAssembly());
        }

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

            return serviceRegistry;
        }
    }
}
