using CleanArchitecture.Application.Abstractions.Messaging;
using Microsoft.Extensions.Logging;
using MediatR;

namespace CleanArchitecture.Application.Abstractions.Behaviors;

public class LoggingBehavior<TRequest, TResponse>
: IPipelineBehavior<TRequest, TResponse>
where TRequest : IBaseCommand
{

    private readonly ILogger<TRequest> _logger;

    public LoggingBehavior(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var name = request.GetType().Name;
        try
        {
            _logger.LogInformation($"Executing command request: {name} ");
            var result = await next();
            _logger.LogInformation($"Succesfuly Executed command request: {name} ");
            return result;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"Error Executing command request: {name} ");
            throw;
        }
    }

}

