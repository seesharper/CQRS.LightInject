# CQRS.LightInject

This repo contains the integrations for implementing the CQRS pattern using LightInject.

Before we begin lets just do a quick recap the definitions 

### ICommandHandler

Represents code that alters the state of the system

### IQueryHandler

Represents code that performs a query that has zero side-effects.

## Register Handlers

The following code will register all command and query handlers from the calling assembly.

```c#
serviceRegistry.RegisterCommandHandlers();
serviceRegistry.RegisterQueryHandlers();
```

 We can also register handlers from another assembly like this.

```c#
serviceRegistry.RegisterCommandHandlers(typeof(SomeType).Assembly);
serviceRegistry.RegisterQueryHandlers(typeof(SomeType).Assembly);
```

## Executing Handlers

The easiest way of executing handlers is to inject `IQueryExecutor` and/or `ICommandExecutor` into where we want to execute handlers.

```c#
public class CustomersController
{
	public CustomersController(IQueryExecutor queryExecutor, ICommandExecutor commandExecutor)
  {		 
  	this.queryExecutor = queryExecutor;
    this.commandExecutor = commandExecutor;  	
  }
}
```

## Intercepting handlers

By intercepting handlers we mean that we can provide a function to be executed as part of the execution chain.

```c#
container.RegisterCommandInterceptor<SampleCommand>(async (command, handler, token) =>
{
  // Do something before the handler   
  await handler.HandleAsync(command, token);
  // Do something after the handler
}
);
```

 The same thing for query handlers. 

```c#
container.RegisterQueryInterceptor<SampleQuery, SampleQueryResult>(async (command, handler, token) =>
{
	// Do something before the handler   
 	var result = await handler.HandleAsync(command, token);
  // Do something after the handler
}
);
```

### Dependencies

We can also inject dependencies into the function like this

```c#
container.Register<IFoo,Foo>();
container.RegisterQueryInterceptor<SampleQuery, SampleQueryResult, IFoo>(async (command, handler, foo, token) =>
{    
    return await handler.HandleAsync(command, token);
}
);
```

 Injecting multiple dependencies can be done using a tuple.

```c#
container.Register<IFoo, Foo>();
container.Register<IBar, Bar>();
container.RegisterQueryInterceptor<SampleQuery, SampleQueryResult, (IFoo foo, IBar bar)>(async (command, handler, dependencies, token) =>
{    
    return await handler.HandleAsync(command, token);
}
);
```

