using System.ComponentModel.DataAnnotations;

namespace MADOC.Tests.Validation;

public static class ValidationTest
{
    public static List<ValidationResult> ValidateObject(object model)
    {
        var results = new List<ValidationResult>();// создание списка для хранения результатов валидации

        var context = new ValidationContext(model);// создание контекста валидации для объекта модели

        Validator.TryValidateObject(
            model,
            context,
            results,
            validateAllProperties: true);// проверка всех свойств объекта

        return results;
    }
}