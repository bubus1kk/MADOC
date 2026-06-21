//using MADOC.Domain.Ranges;
//using MADOC.Domain.Validation.Attributes;
//using MADOC.Domain.Validation.Enums;
//using MADOC.Domain.Validation.ListDependencies;
//using System.ComponentModel.DataAnnotations;

//namespace MADOC.Domain.Documents
//{
//    public class CertificateRequestDocument : BaseDocument, IListDependencySchemaProvider
//    {
//        private static readonly ListDependencySchema DependencySchema = CreateDependencySchema();

//        [Display(Name = "ФИО заявителя")]
//        [Required]
//        [StringConstraint(
//            MaxLength = 150,
//            Alphabet = AllowedAlphabet.CyrillicOnly,
//            AllowSpecialChars = false)]
//        public string RequesterFullName { get; set; } = string.Empty;

//        [Display(Name = "Учебная группа")]
//        [Required]
//        [StringConstraint(MaxLength = 30)]
//        public string RequesterGroup { get; set; } = string.Empty;

//        [Display(Name = "Количество экземпляров")]
//        [Required]
//        [NumberConstraint(
//            MinValue = 1,
//            MaxValue = 10,
//            AllowFloats = false)]
//        public int? CopiesCount { get; set; }

//        [Display(Name = "Желаемая дата получения")]
//        [Required]
//        [DateConstraint(
//            MinDate = "01-01-2024",
//            MaxDate = "31-12-2035")]
//        public DateOnly? DesiredReceiveDate { get; set; }

//        [Display(Name = "Желаемое время получения")]
//        [Required]
//        [TimeConstraint(
//            MinTime = "08:00",
//            MaxTime = "18:00")]
//        public TimeOnly? DesiredReceiveTime { get; set; }

//        [Display(Name = "Организация-получатель")]
//        [StringConstraint(MaxLength = 200)]
//        public string OrganizationName { get; set; } = string.Empty;

//        [Display(Name = "Тип справки")]
//        [Required]
//        [ListConstraint(
//            "Справка об обучении",
//            "Справка о периоде обучения",
//            "Справка для военкомата",
//            "Справка о стипендии")]
//        public string CertificateType { get; set; } = string.Empty;

//        [Display(Name = "Назначение справки")]
//        [Required]
//        [ListConstraint(
//            "По месту требования",
//            "Для социальной защиты",
//            "Для работодателя",
//            "Для архива",
//            "Для перевода",
//            "Для восстановления",
//            "Первичная постановка",
//            "Уточнение данных",
//            "Для банка")]
//        [ListDependency(nameof(CertificateType))]
//        public string CertificatePurpose { get; set; } = string.Empty;

//        [Display(Name = "Формат справки")]
//        [Required]
//        [ListConstraint(
//            "Электронная",
//            "Бумажная",
//            "Бумажная с печатью")]
//        [ListDependency(nameof(CertificatePurpose))]
//        public string CertificateFormat { get; set; } = string.Empty;

//        [Display(Name = "Место получения")]
//        [Required]
//        [ListConstraint(
//            "Личный кабинет",
//            "Электронная почта",
//            "Учебная часть",
//            "Канцелярия")]
//        [ListDependency(nameof(CertificateFormat))]
//        public string ReceivePlace { get; set; } = string.Empty;

//        [Display(Name = "Нужна печать")]
//        public bool NeedStamp
//        {
//            get
//            {
//                return CertificateFormat == "Бумажная с печатью";
//            }
//        }

//        [Display(Name = "Электронная справка")]
//        public bool IsElectronicCertificate
//        {
//            get
//            {
//                return CertificateFormat == "Электронная";
//            }
//        }

//        [Display(Name = "Бумажная справка")]
//        public bool IsPaperCertificate
//        {
//            get
//            {
//                if (CertificateFormat == "Бумажная")
//                {
//                    return true;
//                }

//                return CertificateFormat == "Бумажная с печатью";
//            }
//        }

//        [Display(Name = "Требуется посещение учреждения")]
//        public bool RequiresOfficeVisit
//        {
//            get
//            {
//                if (ReceivePlace == "Учебная часть")
//                {
//                    return true;
//                }

//                return ReceivePlace == "Канцелярия";
//            }
//        }

//        [Display(Name = "Период обработки заявки")]
//        public DateTimeRange? ProcessingPeriod
//        {
//            get
//            {
//                if (DesiredReceiveDate is null || DesiredReceiveTime is null)
//                {
//                    return null;
//                }

//                var receiveDateTime = DesiredReceiveDate.Value.ToDateTime(
//                    DesiredReceiveTime.Value);

//                return new DateTimeRange(
//                    CreationDate,
//                    receiveDateTime);
//            }
//        }

//        [Display(Name = "Количество дней обработки")]
//        public int ProcessingDays
//        {
//            get
//            {
//                if (ProcessingPeriod is null)
//                {
//                    return 0;
//                }

//                return (ProcessingPeriod.To.Date - ProcessingPeriod.From.Date).Days + 1;
//            }
//        }

//        [Display(Name = "Краткое описание справки")]
//        public string CertificateSummary
//        {
//            get
//            {
//                if (string.IsNullOrWhiteSpace(CertificateType) &&
//                    string.IsNullOrWhiteSpace(CertificatePurpose))
//                {
//                    return string.Empty;
//                }

//                return $"{CertificateType}: {CertificatePurpose}";
//            }
//        }

//        public ListDependencySchema GetListDependencySchema()
//        {
//            return DependencySchema;
//        }

//        private static ListDependencySchema CreateDependencySchema()
//        {
//            var schema = new ListDependencySchema();

//            schema.AddRule(
//                nameof(CertificatePurpose),
//                nameof(CertificateType),
//                "Справка об обучении",
//                "По месту требования",
//                "Для социальной защиты",
//                "Для работодателя");

//            schema.AddRule(
//                nameof(CertificatePurpose),
//                nameof(CertificateType),
//                "Справка о периоде обучения",
//                "Для архива",
//                "Для перевода",
//                "Для восстановления");

//            schema.AddRule(
//                nameof(CertificatePurpose),
//                nameof(CertificateType),
//                "Справка для военкомата",
//                "Первичная постановка",
//                "Уточнение данных");

//            schema.AddRule(
//                nameof(CertificatePurpose),
//                nameof(CertificateType),
//                "Справка о стипендии",
//                "Для банка",
//                "Для социальной защиты");

//            schema.AddRule(
//                nameof(CertificateFormat),
//                nameof(CertificatePurpose),
//                "По месту требования",
//                "Электронная",
//                "Бумажная");

//            schema.AddRule(
//                nameof(CertificateFormat),
//                nameof(CertificatePurpose),
//                "Для социальной защиты",
//                "Бумажная с печатью");

//            schema.AddRule(
//                nameof(CertificateFormat),
//                nameof(CertificatePurpose),
//                "Для работодателя",
//                "Электронная",
//                "Бумажная с печатью");

//            schema.AddRule(
//                nameof(CertificateFormat),
//                nameof(CertificatePurpose),
//                "Для архива",
//                "Бумажная с печатью");

//            schema.AddRule(
//                nameof(CertificateFormat),
//                nameof(CertificatePurpose),
//                "Для перевода",
//                "Бумажная с печатью");

//            schema.AddRule(
//                nameof(CertificateFormat),
//                nameof(CertificatePurpose),
//                "Для восстановления",
//                "Бумажная с печатью");

//            schema.AddRule(
//                nameof(CertificateFormat),
//                nameof(CertificatePurpose),
//                "Первичная постановка",
//                "Бумажная с печатью");

//            schema.AddRule(
//                nameof(CertificateFormat),
//                nameof(CertificatePurpose),
//                "Уточнение данных",
//                "Бумажная с печатью");

//            schema.AddRule(
//                nameof(CertificateFormat),
//                nameof(CertificatePurpose),
//                "Для банка",
//                "Бумажная с печатью");

//            schema.AddRule(
//                nameof(ReceivePlace),
//                nameof(CertificateFormat),
//                "Электронная",
//                "Личный кабинет",
//                "Электронная почта");

//            schema.AddRule(
//                nameof(ReceivePlace),
//                nameof(CertificateFormat),
//                "Бумажная",
//                "Учебная часть");

//            schema.AddRule(
//                nameof(ReceivePlace),
//                nameof(CertificateFormat),
//                "Бумажная с печатью",
//                "Учебная часть",
//                "Канцелярия");

//            return schema;
//        }
//    }
//}
