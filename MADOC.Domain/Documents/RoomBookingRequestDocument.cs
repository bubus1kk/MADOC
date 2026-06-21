using MADOC.Domain.Documents.ListConfigurations;
using MADOC.Domain.Validation.Attributes;
using MADOC.Domain.Validation.Enums;
using MADOC.Domain.Validation.ListDependencies;
using MADOC.Domain.Validation.Lists;
using System.ComponentModel.DataAnnotations;

namespace MADOC.Domain.Documents
{
    public class RoomBookingRequestDocument : BaseDocument, IListConfigurationProvider
    {
        public DocumentListCatalog GetListCatalog()
        {
            return RoomBookingRequestConfiguration.ListCatalog;
        }

        public ListDependencySchema GetListDependencySchema()
        {
            return RoomBookingRequestConfiguration.DependencySchema;
        }

        [Display(Name = "ФИО организатора")]
        [Required]
        [StringConstraint(MaxLength = 150, Alphabet = AllowedAlphabet.CyrillicOnly, AllowSpecialChars = false)]
        public string OrganizerFullName { get; set; } = string.Empty;

        [Display(Name = "Группа или подразделение")]
        [Required]
        [StringConstraint(MaxLength = 50)]
        public string OrganizerUnit { get; set; } = string.Empty;

        [Display(Name = "Количество участников")]
        [Required]
        [NumberConstraint(MinValue = 1, MaxValue = 300, AllowFloats = false)]
        public int? ParticipantsCount { get; set; }

        [Display(Name = "Дата мероприятия")]
        [Required]
        [DateConstraint(MinDate = "01-01-2024", MaxDate = "31-12-2035")]
        public DateOnly? EventDate { get; set; }

        [Display(Name = "Время начала")]
        [Required]
        [TimeConstraint(MinTime = "08:00", MaxTime = "20:00")]
        public TimeOnly? StartTime { get; set; }

        [Display(Name = "Время окончания")]
        [Required]
        [TimeConstraint(MinTime = "08:30", MaxTime = "21:00")]
        public TimeOnly? EndTime { get; set; }

        [Display(Name = "Описание мероприятия")]
        [StringConstraint(MaxLength = 1000, IsMultiline = true)]
        public string EventDescription { get; set; } = string.Empty;

        [Display(Name = "Формат мероприятия")]
        [Required]
        [ListConstraint]
        public ListOptionKey? EventFormat { get; set; }

        [Display(Name = "Корпус")]
        [Required]
        [ListConstraint]
        [ListDependency(nameof(EventFormat))]
        public ListOptionKey? Building { get; set; }

        [Display(Name = "Тип аудитории")]
        [Required]
        [ListConstraint]
        [ListDependency(nameof(Building))]
        public ListOptionKey? RoomType { get; set; }

        [Display(Name = "Аудитория")]
        [Required]
        [ListConstraint]
        [ListDependency(nameof(Building), nameof(RoomType))]
        public ListOptionKey? Room { get; set; }

        [Display(Name = "Комплект оборудования")]
        [Required]
        [ListConstraint]
        [ListDependency(nameof(Room))]
        public ListOptionKey? EquipmentSet { get; set; }

        [Display(Name = "Длительность мероприятия в часах")]
        public double DurationHours
        {
            get
            {
                if (StartTime is null || EndTime is null)
                {
                    return 0;
                }

                var duration = EndTime.Value.ToTimeSpan() - StartTime.Value.ToTimeSpan();

                if (duration.TotalHours < 0)
                {
                    return 0;
                }

                return duration.TotalHours;
            }
        }

        [Display(Name = "Крупное мероприятие")]
        public bool IsLargeEvent
        {
            get
            {
                return ParticipantsCount > 100;
            }
        }

        [Display(Name = "Требуется оборудование")]
        public bool RequiresEquipment
        {
            get
            {
                if (EquipmentSet is null)
                {
                    return false;
                }

                return EquipmentSet.Value != RoomBookingRequestConfiguration.EquipmentSet.NotRequired.Key;
            }
        }

        [Display(Name = "Краткое описание бронирования")]
        public string BookingSummary
        {
            get
            {
                var eventFormatName = GetListOptionDisplayName(nameof(EventFormat), EventFormat);

                var roomName = GetListOptionDisplayName(nameof(Room), Room);

                if (string.IsNullOrWhiteSpace(eventFormatName) && string.IsNullOrWhiteSpace(roomName))
                {
                    return string.Empty;
                }

                if (string.IsNullOrWhiteSpace(eventFormatName))
                {
                    return roomName;
                }

                if (string.IsNullOrWhiteSpace(roomName))
                {
                    return eventFormatName;
                }

                return $"{eventFormatName}: {roomName}";
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
