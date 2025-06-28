using DirectoryProject.SharedService.SharedKernel;
using FluentValidation;

namespace DirectoryProject.SharedService.Core.Validation;

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptionsConditions<T, TElement> MustBeValueObject<T, TElement, TValueObject>(
        this IRuleBuilder<T, TElement> ruleBuilder,
        Func<TElement, Result<TValueObject>> factoryMethod)
    {
        return ruleBuilder.Custom((value, context) =>
        {
            Result<TValueObject> result = factoryMethod(value);

            if (result.IsSuccess)
                return;

            // add first error from Create method
            context.AddFailure(result.Errors![0].Serialize());
        });
    }

    public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule,
        Error error)
    {
        return rule.WithMessage(error.Serialize());
    }
}
