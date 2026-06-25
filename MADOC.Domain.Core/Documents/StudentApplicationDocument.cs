using System.ComponentModel.DataAnnotations;
using MADOC.Domain.Core.Documents.ListConfigurations;
using MADOC.Domain.Core.Ranges;
using MADOC.Domain.Core.Validation.Attributes;
using MADOC.Domain.Core.Validation.Enums;
using MADOC.Domain.Core.Validation.ListDependencies;
using MADOC.Domain.Core.Validation.Lists;

namespace MADOC.Domain.Core.Documents;

public class StudentApplicationDocument : BaseDocument, IListConfigurationProvider
{
    public DocumentListCatalog GetListCatalog()
    {
        return StudentApplicationListConfiguration.ListCatalog;
    }

    public ListDependencySchema GetListDependencySchema()
    {
        return StudentApplicationListConfiguration.DependencySchema;
    }

    [Display(Name = "ФИО студента")]
    [Required]
    [StringConstraint(MaxLength = 150, Alphabet = AllowedAlphabet.CyrillicOnly, AllowSpecialChars = false)]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Учебная группа")]
    [Required]
    [StringConstraint(MaxLength = 15)]
    public string GroupName { get; set; } = string.Empty;

    [Display(Name = "Возраст")]
    [Required]
    [NumberConstraint(MinValue = 15, MaxValue = 100, AllowFloats = false)]
    public int? Age { get; set; }

    [Display(Name = "Дата рождения")]
    [Required]
    [DateConstraint(MinDate = "01-01-1960", MaxDate = "31-12-2035")]
    public DateOnly? BirthDate { get; set; }

    [Display(Name = "Период отсутствия")]
    [DateRangeConstraint]
    public DateRange? AbsencePeriod { get; set; }

    [Display(Name = "Нужна бумажная копия")]
    [Required]
    [BooleanConstraint]
    public bool? NeedPaperCopy { get; set; }

    [Display(Name = "Комментарий")]
    [StringConstraint(MaxLength = 1000, IsMultiline = true)]
    public string Comment { get; set; } = string.Empty;

    [Display(Name = "Категория заявителя")]
    [Required]
    [ListConstraint]
    public ListOptionKey? ApplicantCategory { get; set; }

    [Display(Name = "Тип заявления")]
    [Required]
    [ListConstraint]
    [ListDependency(nameof(ApplicantCategory))]
    public ListOptionKey? ApplicationType { get; set; }

    [Display(Name = "Подтип заявления")]
    [Required]
    [ListConstraint]
    [ListDependency(nameof(ApplicationType))]
    public ListOptionKey? ApplicationSubtype { get; set; }

    [Display(Name = "Способ получения результата")]
    [Required]
    [ListConstraint]
    [ListDependency(nameof(ApplicationSubtype))]
    public ListOptionKey? ReceiveMethod { get; set; }

    [Display(Name = "Дата и время подачи заявления")]
    public DateTime SubmittedAt
    {
        get
        {
            return CreationDate;
        }
    }

    [Display(Name = "Количество дней отсутствия")]
    public int AbsenceDays
    {
        get
        {
            if (AbsencePeriod is null)
            {
                return 0;
            }

            return AbsencePeriod.To.DayNumber - AbsencePeriod.From.DayNumber + 1;
        }
    }

    [Display(Name = "Длительное отсутствие")]
    public bool IsLongAbsence
    {
        get
        {
            return AbsenceDays > 14;
        }
    }

    [Display(Name = "Требуется бумажная обработка")]
    public bool RequiresPaperProcessing
    {
        get
        {
            if (NeedPaperCopy == true)
            {
                return true;
            }

            if (ReceiveMethod is null)
            {
                return false;
            }

            return ReceiveMethod.Value == StudentApplicationListConfiguration.ReceiveMethod.Paper.Key;
        }
    }

    [Display(Name = "Краткое описание заявления")]
    public string ApplicationSummary
    {
        get
        {
            var applicationTypeName = GetListOptionDisplayName(nameof(ApplicationType), ApplicationType);

            var applicationSubtypeName = GetListOptionDisplayName(nameof(ApplicationSubtype), ApplicationSubtype);

            if (string.IsNullOrWhiteSpace(applicationTypeName) &&
                string.IsNullOrWhiteSpace(applicationSubtypeName))
            {
                return string.Empty;
            }

            if (string.IsNullOrWhiteSpace(applicationTypeName))
            {
                return applicationSubtypeName;
            }

            if (string.IsNullOrWhiteSpace(applicationSubtypeName))
            {
                return applicationTypeName;
            }

            return $"{applicationTypeName}: {applicationSubtypeName}";
        }
    }

    private string GetListOptionDisplayName(string fieldName, ListOptionKey? key)
    {
        if (key is null)
        {
            return string.Empty;
        }

        var list = GetListCatalog().GetList(fieldName);

        if (!list.ContainsKey(key.Value))
        {
            return key.Value.ToString();
        }

        return list.GetOption(key.Value).DisplayName;
    }
}
