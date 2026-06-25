using System.ComponentModel.DataAnnotations;
using MADOC.Domain.Core.Documents.ListConfigurations;
using MADOC.Domain.Core.Ranges;
using MADOC.Domain.Core.Validation.Attributes;
using MADOC.Domain.Core.Validation.Enums;
using MADOC.Domain.Core.Validation.ListDependencies;
using MADOC.Domain.Core.Validation.Lists;

namespace MADOC.Domain.Core.Documents;

public class ServiceContractDocument : BaseDocument, IListConfigurationProvider
{
    public DocumentListCatalog GetListCatalog()
    {
        return ServiceContractConfiguration.ListCatalog;
    }

    public ListDependencySchema GetListDependencySchema()
    {
        return ServiceContractConfiguration.DependencySchema;
    }

    [Display(Name = "ФИО ответственного")]
    [Required]
    [StringConstraint(
        MaxLength = 150,
        Alphabet = AllowedAlphabet.CyrillicOnly,
        AllowSpecialChars = false)]
    public string ResponsibleFullName { get; set; } = string.Empty;

    [Display(Name = "Наименование контрагента")]
    [Required]
    [StringConstraint(MaxLength = 200)]
    public string ContractorName { get; set; } = string.Empty;

    [Display(Name = "Предмет договора")]
    [Required]
    [StringConstraint(
        MaxLength = 1000,
        IsMultiline = true)]
    public string ContractSubject { get; set; } = string.Empty;

    [Display(Name = "Сумма договора")]
    [NumberConstraint(
        MinValue = 0,
        MaxValue = 10000000,
        AllowFloats = true)]
    public double? ContractAmount { get; set; }

    [Display(Name = "Период действия договора")]
    [Required]
    [DateRangeConstraint]
    public DateRange? ContractPeriod { get; set; }

    [Display(Name = "Тип контрагента")]
    [Required]
    [ListConstraint]
    public ListOptionKey? ContractorType { get; set; }

    [Display(Name = "Тип договора")]
    [Required]
    [ListConstraint]
    [ListDependency(nameof(ContractorType))]
    public ListOptionKey? ContractType { get; set; }

    [Display(Name = "График оплаты")]
    [Required]
    [ListConstraint]
    [ListDependency(nameof(ContractType))]
    public ListOptionKey? PaymentSchedule { get; set; }

    [Display(Name = "Способ подписания")]
    [Required]
    [ListConstraint]
    [ListDependency(nameof(PaymentSchedule))]
    public ListOptionKey? SigningMethod { get; set; }

    [Display(Name = "Без оплаты")]
    public bool IsFreeContract
    {
        get
        {
            if (PaymentSchedule is null)
            {
                return false;
            }

            return PaymentSchedule.Value == ServiceContractConfiguration.PaymentSchedule.NoPayment.Key;
        }
    }

    [Display(Name = "Электронное подписание")]
    public bool UsesElectronicSignature
    {
        get
        {
            if (SigningMethod is null)
            {
                return false;
            }

            return SigningMethod.Value == ServiceContractConfiguration.SigningMethod.ElectronicSignature.Key;
        }
    }

    [Display(Name = "Длительность договора в днях")]
    public int ContractDurationDays
    {
        get
        {
            if (ContractPeriod is null)
            {
                return 0;
            }

            return ContractPeriod.To.DayNumber - ContractPeriod.From.DayNumber + 1;
        }
    }

    [Display(Name = "Краткое описание договора")]
    public string ContractSummary
    {
        get
        {
            var contractTypeName = GetListOptionDisplayName(
                nameof(ContractType),
                ContractType);

            var contractorTypeName = GetListOptionDisplayName(
                nameof(ContractorType),
                ContractorType);

            if (string.IsNullOrWhiteSpace(contractTypeName) &&
                string.IsNullOrWhiteSpace(contractorTypeName))
            {
                return string.Empty;
            }

            if (string.IsNullOrWhiteSpace(contractTypeName))
            {
                return contractorTypeName;
            }

            if (string.IsNullOrWhiteSpace(contractorTypeName))
            {
                return contractTypeName;
            }

            return $"{contractTypeName}: {contractorTypeName}";
        }
    }

    private string GetListOptionDisplayName(
        string fieldName,
        ListOptionKey? key)
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
