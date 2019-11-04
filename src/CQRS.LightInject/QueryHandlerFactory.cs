namespace CQRS.LightInject
{
    using System;
    using CQRS.Execution;
    using global::LightInject;

    /// <summary>
    /// An <see cref="IQueryHandlerFactory"/> implementation that uses <see cref="IServiceFactory"/> to create command handlers.
    /// </summary>
    public class QueryHandlerFactory : IQueryHandlerFactory
    {
        private readonly IServiceFactory serviceFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryHandlerFactory"/> class.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceFactory"/> that is responsible for creating query handlers.</param>
        public QueryHandlerFactory(IServiceFactory serviceProvider)
        {
            this.serviceFactory = serviceProvider;
        }

        /// <inheritdoc/>
        public object GetQueryHandler(Type queryHandlerType)
        {
            return serviceFactory.GetInstance(queryHandlerType);
        }
    }
}