using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ValidationFilter : IAsyncActionFilter
{
  private readonly IServiceProvider _serviceProvider;

  public ValidationFilter(IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider;
  }

  public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
  {
    foreach (var arg in context.ActionArguments.Values)
    {
      if (arg is null) continue;

      var validatorType = typeof(IValidator<>).MakeGenericType(arg.GetType());
      var validator = _serviceProvider.GetService(validatorType) as IValidator;

      if (validator is null) continue;

      var validationContext = new ValidationContext<object>(arg);
      var result = await validator.ValidateAsync(validationContext);

      if (!result.IsValid)
      {
        foreach (var error in result.Errors)
        {
          context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }

        context.Result = new BadRequestObjectResult(context.ModelState);
        return;
      }
    }

    await next();
  }
}
