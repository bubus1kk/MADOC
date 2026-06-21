using System.ComponentModel.DataAnnotations;
using MADOC.Domain.Documents.ListConfigurations;
using MADOC.Domain.Ranges;
using MADOC.Domain.Validation.Attributes;
using MADOC.Domain.Validation.Enums;
using MADOC.Domain.Validation.ListDependencies;
using MADOC.Domain.Validation.Lists;

namespace MADOC.Domain.Documents;

public class CertificateRequestDocument : BaseDocument, IListConfigurationProvider
{
    public DocumentListCatalog GetListCatalog()
    {
        return CertificateRequestListConfiguration.ListCatalog;
    }

    public ListDependencySchema GetListDependencySchema()
    {
        return CertificateRequestListConfiguration.DependencySchema;
    }

    [Display(Name = "ФИО заявителя")]
    [Required]
    [StringConstraint(MaxLength = 150,Alphabet = AllowedAlphabet.CyrillicOnly, AllowSpecialChars = false)]
    public string RequesterFullName { get; set; } = string.Empty;

    [Display(Name = "Учебная группа")]
    [Required]
    [StringConstraint(MaxLength = 30)]
    public string RequesterGroup { get; set; } = string.Empty;

    [Display(Name = "Количество экземпляров")]
    [Required]
    [NumberConstraint(MinValue = 1,MaxValue = 10,AllowFloats = false)]
    public int? CopiesCount { get; set; }

    [Display(Name = "Желаемая дата получения")]
    [Required]
    [DateConstraint(MinDate = "01-01-2024",MaxDate = "31-12-2035")]
    public DateOnly? DesiredReceiveDate { get; set; }

    [Display(Name = "Желаемое время получения")]
    [Required]
    [TimeConstraint(MinTime = "08:00",MaxTime = "18:00")]
    public TimeOnly? DesiredReceiveTime { get; set; }

    [Display(Name = "Организация-получатель")]
    [StringConstraint(MaxLength = 200)]
    public string OrganizationName { get; set; } = string.Empty;

    [Display(Name = "Тип справки")]
    [Required]
    [ListConstraint]
    public ListOptionKey? CertificateType { get; set; }

    [Display(Name = "Назначение справки")]
    [Required]
    [ListConstraint]
    [ListDependency(nameof(CertificateType))]
    public ListOptionKey? CertificatePurpose { get; set; }

    [Display(Name = "Формат справки")]
    [Required]
    [ListConstraint]
    [ListDependency(nameof(CertificatePurpose))]
    public ListOptionKey? CertificateFormat { get; set; }

    [Display(Name = "Место получения")]
    [Required]
    [ListConstraint]
    [ListDependency(nameof(CertificateFormat))]
    public ListOptionKey? ReceivePlace { get; set; }

    [Display(Name = "Нужна печать")]
    public bool NeedStamp
    {
        get
        {
            if (CertificateFormat is null)
            {
                return false;
            }

            return CertificateFormat.Value == CertificateRequestListConfiguration.CertificateFormat.PaperWithStamp.Key;
        }
    }

    [Display(Name = "Электронная справка")]
    public bool IsElectronicCertificate
    {
        get
        {
            if (CertificateFormat is null)
            {
                return false;
            }

            return CertificateFormat.Value == CertificateRequestListConfiguration.CertificateFormat.Electronic.Key;
        }
    }

    [Display(Name = "Бумажная справка")]
    public bool IsPaperCertificate
    {
        get
        {
            if (CertificateFormat is null)
            {
                return false;
            }

            if (CertificateFormat.Value == CertificateRequestListConfiguration.CertificateFormat.Paper.Key)
            {
                return true;
            }

            return CertificateFormat.Value == CertificateRequestListConfiguration.CertificateFormat.PaperWithStamp.Key;
        }
    }

    [Display(Name = "Требуется посещение учреждения")]
    public bool RequiresOfficeVisit
    {
        get
        {
            if (ReceivePlace is null)
            {
                return false;
            }

            if (ReceivePlace.Value == CertificateRequestListConfiguration.ReceivePlace.EducationOffice.Key)
            {
                return true;
            }

            return ReceivePlace.Value == CertificateRequestListConfiguration.ReceivePlace.Chancellery.Key;
        }
    }

    [Display(Name = "Период обработки заявки")]
    public DateTimeRange? ProcessingPeriod
    {
        get
        {
            if (DesiredReceiveDate is null || DesiredReceiveTime is null)
            {
                return null;
            }

            var receiveDateTime = DesiredReceiveDate.Value.ToDateTime(
                DesiredReceiveTime.Value);

            return new DateTimeRange(
                CreationDate,
                receiveDateTime);
        }
    }

    [Display(Name = "Количество дней обработки")]
    public int ProcessingDays
    {
        get
        {
            if (ProcessingPeriod is null)
            {
                return 0;
            }

            return (ProcessingPeriod.To.Date - ProcessingPeriod.From.Date).Days + 1;
        }
    }

    [Display(Name = "Краткое описание справки")]
    public string CertificateSummary
    {
        get
        {
            var certificateTypeName = GetListOptionDisplayName(
                nameof(CertificateType),
                CertificateType);

            var certificatePurposeName = GetListOptionDisplayName(
                nameof(CertificatePurpose),
                CertificatePurpose);

            if (string.IsNullOrWhiteSpace(certificateTypeName) &&
                string.IsNullOrWhiteSpace(certificatePurposeName))
            {
                return string.Empty;
            }

            if (string.IsNullOrWhiteSpace(certificateTypeName))
            {
                return certificatePurposeName;
            }

            if (string.IsNullOrWhiteSpace(certificatePurposeName))
            {
                return certificateTypeName;
            }

            return $"{certificateTypeName}: {certificatePurposeName}";
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
