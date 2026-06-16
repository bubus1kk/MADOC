using System.ComponentModel.DataAnnotations;

namespace MADOC.Tests.Domain;

public static class ValidationTestHelper
{
    public static List<ValidationResult> ValidateObject(object model)
    {
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(
            model,
            new ValidationContext(model),
            results,
            validateAllProperties: true);

        return results;
    }

    public static bool IsValid(object model)
    {
        return ValidateObject(model).Count == 0;
    }
}