using MADOC.Domain.Core.Documents.ListConfigurations;
using MADOC.Domain.Core.Ranges;
using MADOC.Domain.Core.Validation.Attributes;
using MADOC.Domain.Core.Validation.Enums;
using MADOC.Domain.Core.Validation.ListDependencies;
using MADOC.Domain.Core.Validation.Lists;
using System.ComponentModel.DataAnnotations;

namespace MADOC.Domain.Core.Documents
{
    public class BusinessTripRequestDocument : BaseDocument, IListConfigurationProvider
    {
        public DocumentListCatalog GetListCatalog()
        {
            return BusinessTripRequestConfiguration.ListCatalog;
        }

        public ListDependencySchema GetListDependencySchema()
        {
            return BusinessTripRequestConfiguration.DependencySchema;
        }

        [Display(Name = "ФИО сотрудника")]
        [Required]
        [StringConstraint(MaxLength = 150, Alphabet = AllowedAlphabet.CyrillicOnly, AllowSpecialChars = false)]
        public string EmployeeFullName { get; set; } = string.Empty;

        [Display(Name = "Должность")]
        [Required]
        [StringConstraint(MaxLength = 100)]
        public string Position { get; set; } = string.Empty;

        [Display(Name = "Период командировки")]
        [Required]
        [DateRangeConstraint]
        public DateRange? TripPeriod { get; set; }

        [Display(Name = "Город назначения")]
        [Required]
        [StringConstraint(MaxLength = 100)]
        public string DestinationCity { get; set; } = string.Empty;

        [Display(Name = "Основание командировки")]
        [StringConstraint(MaxLength = 1000, IsMultiline = true)]
        public string Reason { get; set; } = string.Empty;

        [Display(Name = "Подразделение")]
        [Required]
        [ListConstraint]
        public ListOptionKey? Department { get; set; }

        [Display(Name = "Цель")]
        [Required]
        [ListConstraint]
        [ListDependency(nameof(Department))]
        public ListOptionKey? Purpose { get; set; }

        [Display(Name = "Направление")]
        [Required]
        [ListConstraint]
        [ListDependency(nameof(Purpose))]
        public ListOptionKey? Destination { get; set; }

        [Display(Name = "Вид транспорта")]
        [Required]
        [ListConstraint]
        [ListDependency(nameof(Destination))]
        public ListOptionKey? TransportType { get; set; }

        [Display(Name = "Проживание")]
        [Required]
        [ListConstraint]
        [ListDependency(nameof(TransportType))]
        public ListOptionKey? Accommodation { get; set; }

        [Display(Name = "Количество дней командировки")]
        public int TripDays
        {
            get
            {
                if (TripPeriod is null)
                {
                    return 0;
                }

                return TripPeriod.To.DayNumber - TripPeriod.From.DayNumber + 1;
            }
        }

        [Display(Name = "Международная командировка")]
        public bool IsInternationalTrip
        {
            get
            {
                if (Destination is null)
                {
                    return false;
                }

                return Destination.Value == BusinessTripRequestConfiguration.Destination.International.Key;
            }
        }

        [Display(Name = "Требуется проживание")]
        public bool RequiresAccommodation
        {
            get
            {
                if (Accommodation is null)
                {
                    return false;
                }

                return Accommodation.Value == BusinessTripRequestConfiguration.Accommodation.Hotel.Key;
            }
        }

        [Display(Name = "Краткое описание командировки")]
        public string TripSummary
        {
            get
            {
                var purposeName = GetListOptionDisplayName(nameof(Purpose), Purpose);

                var destinationName = GetListOptionDisplayName(nameof(Destination), Destination);

                if (string.IsNullOrWhiteSpace(purposeName) && string.IsNullOrWhiteSpace(destinationName))
                {
                    return string.Empty;
                }

                if (string.IsNullOrWhiteSpace(purposeName))
                {
                    return destinationName;
                }

                if (string.IsNullOrWhiteSpace(destinationName))
                {
                    return purposeName;
                }

                return $"{purposeName}: {destinationName}";
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
}
