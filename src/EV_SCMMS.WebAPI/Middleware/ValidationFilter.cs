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

      foreach (var property in arg.GetType().GetProperties())
      {
        var value = property.GetValue(arg);
        if (value is null) continue;

        var subValidatorType = typeof(IValidator<>).MakeGenericType(value.GetType());
        var subValidator = _serviceProvider.GetService(subValidatorType) as IValidator;
        if (subValidator != null)
        {
          var subResult = await subValidator.ValidateAsync(new ValidationContext<object>(value));
          result.Errors.AddRange(subResult.Errors);
        }
      }

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
