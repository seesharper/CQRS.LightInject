namespace CQRS.LightInject
{
    using System.Reflection;
    using CQRS.Command.Abstractions;
    using CQRS.Execution;
    using CQRS.Query.Abstractions;
    using global::LightInject;

    /// <summary>
    /// Extends the <see cref="IServiceCollection"/> with methods for registering command and query handlers.
    /// </summary>
    public static class ServiceRegistryExtensions
    {
        /// <summary>
        /// Adds all command handlers found in the calling assembly to the <paramref name="serviceCollection"/> as scoped services.
        /// </summary>
        /// <param name="serviceCollection">The target <see cref="IServiceCollection"/>.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        public static IServiceRegistry RegisterCommandHandlers(this IServiceRegistry serviceRegistry)
        {
            return RegisterCommandHandlers(serviceRegistry, Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Adds all command handlers found in the given <paramref name="assembly"/> to the <paramref name="serviceRegistry"/> as scoped services.
        /// </summary>
        /// <param name="serviceRegistry">The target <see cref="IServiceCollection"/>.</param>
        /// <param name="assembly">The assembly from which to add command handlers. </param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        public static IServiceRegistry RegisterCommandHandlers(this IServiceRegistry serviceRegistry, Assembly assembly)
        {
            var commandHanderDescriptions = assembly.GetCommandHandlerDescriptors();

            foreach (var commandHanderDescription in commandHanderDescriptions)
            {
                serviceRegistry.RegisterScoped(commandHanderDescription.HandlerType, commandHanderDescription.ImplementingType);
            }

            serviceRegistry.RegisterScoped<ICommandHandlerFactory>(sp => new CommandHandlerFactory(sp));
            serviceRegistry.RegisterScoped<ICommandExecutor, CommandExecutor>();

            return serviceRegistry;
        }

        /// <summary>
        /// Adds all query handlers found in the calling assembly to the <paramref name="serviceRegistry"/> as scoped services.
        /// </summary>
        /// <param name="serviceRegistry">The target <see cref="IServiceCollection"/>.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        public static IServiceRegistry RegisterQueryHandlers(this IServiceRegistry serviceRegistry)
        {
            return RegisterQueryHandlers(serviceRegistry, Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Adds all query handlers found in the given <paramref name="assembly"/> to the <paramref name="serviceRegistry"/> as scoped services.
        /// </summary>
        /// <param name="serviceRegistry">The target <see cref="IServiceCollection"/>.</param>
        /// <param name="assembly">The assembly from which to add query handlers. </param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        public static IServiceRegistry RegisterQueryHandlers(this IServiceRegistry serviceRegistry, Assembly assembly)
        {
            var queryHandlerDescriptions = assembly.GetQueryHandlerHandlerDescriptors();

            foreach (var queryHandlerDescription in queryHandlerDescriptions)
            {
                serviceRegistry.RegisterScoped(queryHandlerDescription.HandlerType, queryHandlerDescription.ImplementingType);
            }

            serviceRegistry.RegisterScoped<IQueryHandlerFactory>(sp => new QueryHandlerFactory(sp));
            serviceRegistry.RegisterScoped<IQueryExecutor, QueryExecutor>();

            return serviceRegistry;
        }
    }
}
