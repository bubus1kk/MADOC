using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MADOC.Domain.Validation.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class BooleanConstraintAttribute : ValidationAttribute
{
    public string? DependsOnField { get; set; }
    public bool ForbiddenStateIfDependentIs { get; set; }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success;

        if (value is not bool currentValue)
            return new ValidationResult("Значение должно быть логическим типом bool.");

        if (!currentValue)// Если текущее значение false, то нет ограничений, независимо от зависимого поля
            return ValidationResult.Success;

        if (string.IsNullOrWhiteSpace(DependsOnField))// Если не указано зависимое поле, то просто проверяем, что значение не true
            return ValidationResult.Success;

        var objectType = validationContext.ObjectInstance.GetType();// Получаем информацию о зависимом поле

        var dependentProperty = objectType.GetProperty(DependsOnField,BindingFlags.Instance | BindingFlags.Public);// Ищем его среди публичных свойств объекта

        if (dependentProperty is null)
            return new ValidationResult($"Зависимое поле '{DependsOnField}' не найдено.");

        var dependentValue = dependentProperty.GetValue(validationContext.ObjectInstance);

        if (dependentValue is not bool dependentBoolValue)
            return new ValidationResult($"Зависимое поле '{DependsOnField}' должно быть типа bool.");

        if (dependentBoolValue == ForbiddenStateIfDependentIs)// Если зависимое поле находится в запрещённом состоянии, то текущее значение не может быть true
        {
            return new ValidationResult(
                $"Значение не может быть true, когда поле '{DependsOnField}' равно {ForbiddenStateIfDependentIs}.");
        }

        return ValidationResult.Success;
    }
}