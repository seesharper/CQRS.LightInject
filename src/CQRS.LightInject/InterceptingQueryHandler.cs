namespace CQRS.LightInject
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRS.Query.Abstractions;

    /// <summary>
    /// A <see cref="IQueryHandler{TQuery,TResult}"/> decorator used to intercept query handlers.
    /// </summary>
    /// <typeparam name="TQuery">The type of query to handle.</typeparam>
    /// <typeparam name="TResult"> The type of result to be returned.</typeparam>
    public class InterceptingQueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> handler;

        private readonly Func<TQuery, IQueryHandler<TQuery, TResult>, CancellationToken, Task<TResult>> implementation;

        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptingQueryHandler{TQuery,TResult}"/> class.
        /// </summary>
        /// <param name="handler">The handler to be decorated.</param>
        /// <param name="implementation">The function representing the implementation of this handler.</param>
        public InterceptingQueryHandler(IQueryHandler<TQuery, TResult> handler, Func<TQuery, IQueryHandler<TQuery, TResult>, CancellationToken, Task<TResult>> implementation)
        {
            this.handler = handler;
            this.implementation = implementation;
        }

        /// <inheritdoc/>
        public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
            => await implementation(query, handler, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// A <see cref="IQueryHandler{TQuery,TResult}"/> decorator used to intercept query handlers.
    /// </summary>
    /// <typeparam name="TQuery">The type of query to handle.</typeparam>
    /// <typeparam name="TResult"> The type of result to be returned.</typeparam>
    /// <typeparam name="TDependency">The type of dependency to be passed to the implementation function.</typeparam>
    public class InterceptingQueryHandler<TQuery, TResult, TDependency> : IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> handler;
        private readonly TDependency dependency;

        private readonly Func<TQuery, IQueryHandler<TQuery, TResult>, TDependency, CancellationToken, Task<TResult>> handlerFunction;

        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptingQueryHandler{TQuery,TResult, TDependency}"/> class.
        /// </summary>
        /// <param name="handler">The handler to be decorated.</param>
        /// <param name="dependency">The dependency to be passed to the <paramref name="handlerFunction"/>.</param>
        /// <param name="handlerFunction">The function representing the implementation of this handler.</param>
        public InterceptingQueryHandler(IQueryHandler<TQuery, TResult> handler, TDependency dependency, Func<TQuery, IQueryHandler<TQuery, TResult>, TDependency, CancellationToken, Task<TResult>> handlerFunction)
        {
            this.handler = handler;
            this.dependency = dependency;
            this.handlerFunction = handlerFunction;
        }

        /// <inheritdoc/>
        public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
            => await handlerFunction(query, handler, dependency, cancellationToken).ConfigureAwait(false);
    }
}