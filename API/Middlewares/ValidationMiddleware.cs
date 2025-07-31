using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

public class ValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;

    public ValidationMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        _next = next;
        _serviceProvider = serviceProvider;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method == HttpMethods.Post ||
            context.Request.Method == HttpMethods.Put)
        {
            context.Request.EnableBuffering();

            var bodyType = GetBodyType(context);
            if (bodyType != null)
            {
                var bodyStream = new StreamReader(context.Request.Body);
                var bodyText = await bodyStream.ReadToEndAsync();

                context.Request.Body.Position = 0;

                var model = JsonSerializer.Deserialize(bodyText, bodyType, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (model != null)
                {
                    var validatorType = typeof(IValidator<>).MakeGenericType(bodyType);
                    var validator = context.RequestServices.GetService(validatorType) as IValidator;

                    if (validator != null)
                    {
                        var contextValidation = new ValidationContext<object>(model);
                        var result = validator.Validate(contextValidation);

                        if (!result.IsValid)
                        {
                            context.Response.StatusCode = StatusCodes.Status400BadRequest;
                            context.Response.ContentType = "application/json";

                            var errors = result.Errors.Select(e => new
                            {
                                Campo = e.PropertyName,
                                Mensagem = e.ErrorMessage
                            });

                            var responseBody = JsonSerializer.Serialize(new
                            {
                                Erros = errors
                            });

                            await context.Response.WriteAsync(responseBody);
                            return;
                        }
                    }
                }
            }
        }

        await _next(context);
    }

    private static Type? GetBodyType(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var routeHandler = endpoint?.Metadata.GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>();
        return routeHandler?.Parameters.FirstOrDefault(p => p.ParameterType != typeof(CancellationToken))?.ParameterType;
    }
}
