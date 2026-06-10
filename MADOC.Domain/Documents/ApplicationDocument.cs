using System.ComponentModel.DataAnnotations;
using MADOC.Domain.Documents.Ranges;
using MADOC.Domain.Validation.Attributes;
using MADOC.Domain.Validation.Enums;

namespace MADOC.Domain.Documents;

public class ApplicationDocument : BaseDocument
{

    [Display(Name = "ФИО заявителя")]
    [Required]
    [StringConstraint(
        MaxLength = 120,
        IsMultiline = false,
        Alphabet = AllowedAlphabet.CyrillicOnly,
        AllowSpecialChars = false)]
    public string ApplicantFullName { get; set; } = string.Empty;

    [Display(Name = "Группа")]
    [Required]
    [StringConstraint(
        MaxLength = 20,
        IsMultiline = false,
        Alphabet = AllowedAlphabet.Any,
        AllowSpecialChars = true)]
    public string GroupName { get; set; } = string.Empty;

    [Display(Name = "Электронная почта")]
    [Required]
    [EmailAddress]
    [StringConstraint(
        MaxLength = 100,
        IsMultiline = false,
        Alphabet = AllowedAlphabet.Any,
        AllowSpecialChars = true)]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Номер телефона")]
    [Required]
    [Phone]
    [StringConstraint(
        MaxLength = 20,
        IsMultiline = false,
        Alphabet = AllowedAlphabet.Any,
        AllowSpecialChars = true)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Display(Name = "Категория заявителя")]
    [Required]
    [ListConstraint("Студент", "Староста", "Преподаватель", "Сотрудник")]
    public string ApplicantCategory { get; set; } = string.Empty;

    [Display(Name = "Название мероприятия")]
    [Required]
    [StringConstraint(
        MaxLength = 150,
        IsMultiline = false,
        Alphabet = AllowedAlphabet.Any,
        AllowSpecialChars = true)]
    public string EventTitle { get; set; } = string.Empty;

    [Display(Name = "Описание мероприятия")]
    [Required]
    [StringConstraint(
        MaxLength = 1000,
        IsMultiline = true,
        Alphabet = AllowedAlphabet.Any,
        AllowSpecialChars = true)]
    public string EventDescription { get; set; } = string.Empty;

    [Display(Name = "Формат мероприятия")]
    [Required]
    [ListConstraint("Очный", "Дистанционный", "Смешанный")]
    public string EventFormat { get; set; } = string.Empty;

    [Display(Name = "Место проведения")]
    [Required]
    [StringConstraint(
        MaxLength = 150,
        IsMultiline = false,
        Alphabet = AllowedAlphabet.Any,
        AllowSpecialChars = true)]
    public string EventLocation { get; set; } = string.Empty;

    [Display(Name = "Количество участников")]
    [Required]
    [NumberConstraint(
        Min = 1,
        Max = 500,
        AllowFloats = false)]
    public int? ParticipantsCount { get; set; }

    [Display(Name = "Оборудование согласовано")]
    public bool IsEquipmentApproved { get; set; }

    [Display(Name = "Требуется оборудование колледжа")]
    [BooleanConstraint(
        DependsOnField = nameof(IsEquipmentApproved),
        ForbiddenStateIfDependentIs = false)]
    public bool UsesCollegeEquipment { get; set; }

    [Display(Name = "Дата подачи заявления")]
    [Required]
    [DateConstraint(
        Min = "2026-01-01",
        Max = "2030-12-31")]
    public DateOnly? ApplicationDate { get; set; }

    [Display(Name = "Дата начала мероприятия")]
    [Required]
    [DateConstraint(
        DependsOnField = nameof(ApplicationDate),
        Dependency = DependencyRule.NotLessThan)]
    public DateOnly? EventStartDate { get; set; }

    [Display(Name = "Дата окончания мероприятия")]
    [Required]
    [DateConstraint(
        DependsOnField = nameof(EventStartDate),
        Dependency = DependencyRule.NotLessThan)]
    public DateOnly? EventEndDate { get; set; }

    [Display(Name = "Время начала мероприятия")]
    [Required]
    [TimeConstraint(
        Min = "08:00",
        Max = "20:00")]
    public TimeOnly? EventStartTime { get; set; }

    [Display(Name = "Время окончания мероприятия")]
    [Required]
    [TimeConstraint(
        Min = "08:00",
        Max = "22:00",
        DependsOnField = nameof(EventStartTime),
        Dependency = DependencyRule.NotLessThan)]
    public TimeOnly? EventEndTime { get; set; }

    [Display(Name = "Начало регистрации участников")]
    [Required]
    [DateTimeConstraint(
        Min = "2026-01-01 08:00",
        Max = "2030-12-31 23:59")]
    public DateTime? RegistrationStartMoment { get; set; }

    [Display(Name = "Окончание регистрации участников")]
    [Required]
    [DateTimeConstraint(
        DependsOnField = nameof(RegistrationStartMoment),
        Dependency = DependencyRule.NotLessThan)]
    public DateTime? RegistrationEndMoment { get; set; }

    [Display(Name = "Планируемый бюджет")]
    [Required]
    [NumberRangeConstraint(
        Min = 0,
        Max = 100000,
        AllowFloats = false)]
    public NumberRange? BudgetRange { get; set; }

    [Display(Name = "Период проведения мероприятия")]
    [Required]
    [DateRangeConstraint]
    public DateRange? EventDatePeriod { get; set; }

    [Display(Name = "Временной интервал мероприятия")]
    [Required]
    [TimeRangeConstraint]
    public TimeRange? EventTimePeriod { get; set; }

    [Display(Name = "Период подготовки мероприятия")]
    [Required]
    [DateTimeRangeConstraint]
    public DateTimeRange? PreparationPeriod { get; set; }
}