using MADOC.Domain.Validation.ListDependencies;
using MADOC.Domain.Validation.Lists;

namespace MADOC.Domain.Documents.ListConfigurations;

public static class BusinessTripRequestConfiguration
{
    private const string KeyPrefix = "business_trip_request";

    public static readonly DocumentListCatalog ListCatalog = CreateListCatalog();

    public static readonly ListDependencySchema DependencySchema = CreateDependencySchema();

    public static class Department
    {
        public static readonly ListOption Administration = CreateOption("department.administration", "Администрация");

        public static readonly ListOption EducationalDepartment = CreateOption("department.educational_department", "Учебная часть");

        public static readonly ListOption ItDepartment = CreateOption("department.it_department", "IT-отдел");

        public static readonly ListOption Accounting = CreateOption("department.accounting", "Бухгалтерия");
    }

    public static class Purpose
    {
        public static readonly ListOption Meeting = CreateOption("purpose.meeting", "Совещание");

        public static readonly ListOption Conference = CreateOption("purpose.conference", "Конференция");

        public static readonly ListOption Seminar = CreateOption("purpose.seminar", "Семинар");

        public static readonly ListOption Olympiad = CreateOption("purpose.olympiad", "Олимпиада");

        public static readonly ListOption Training = CreateOption("purpose.training", "Обучение");

        public static readonly ListOption SystemImplementation = CreateOption("purpose.system_implementation", "Внедрение системы");

        public static readonly ListOption ReportMeeting = CreateOption("purpose.report_meeting", "Отчетная встреча");

        public static readonly ListOption Audit = CreateOption("purpose.audit", "Аудит");
    }

    public static class Destination
    {
        public static readonly ListOption Local = CreateOption("destination.local", "Местное");

        public static readonly ListOption Regional = CreateOption("destination.regional", "Региональное");

        public static readonly ListOption Federal = CreateOption("destination.federal", "Федеральное");

        public static readonly ListOption International = CreateOption("destination.international", "Международное");
    }

    public static class TransportType
    {
        public static readonly ListOption PublicTransport = CreateOption("transport_type.public_transport", "Общественный транспорт");

        public static readonly ListOption CompanyTransport = CreateOption("transport_type.company_transport", "Служебный транспорт");

        public static readonly ListOption Bus = CreateOption("transport_type.bus", "Автобус");

        public static readonly ListOption Train = CreateOption("transport_type.train", "Поезд");

        public static readonly ListOption Plane = CreateOption("transport_type.plane", "Самолет");
    }

    public static class Accommodation
    {
        public static readonly ListOption NotRequired = CreateOption("accommodation.not_required", "Не требуется");

        public static readonly ListOption Hotel = CreateOption("accommodation.hotel", "Гостиница");
    }

    private static DocumentListCatalog CreateListCatalog()
    {
        var catalog = new DocumentListCatalog();

        catalog.AddList(
            nameof(BusinessTripRequestDocument.Department),
            Department.Administration,
            Department.EducationalDepartment,
            Department.ItDepartment,
            Department.Accounting);

        catalog.AddList(
            nameof(BusinessTripRequestDocument.Purpose),
            Purpose.Meeting,
            Purpose.Conference,
            Purpose.Seminar,
            Purpose.Olympiad,
            Purpose.Training,
            Purpose.SystemImplementation,
            Purpose.ReportMeeting,
            Purpose.Audit);

        catalog.AddList(
            nameof(BusinessTripRequestDocument.Destination),
            Destination.Local,
            Destination.Regional,
            Destination.Federal,
            Destination.International);

        catalog.AddList(
            nameof(BusinessTripRequestDocument.TransportType),
            TransportType.PublicTransport,
            TransportType.CompanyTransport,
            TransportType.Bus,
            TransportType.Train,
            TransportType.Plane);

        catalog.AddList(
            nameof(BusinessTripRequestDocument.Accommodation),
            Accommodation.NotRequired,
            Accommodation.Hotel);

        return catalog;
    }

    private static ListDependencySchema CreateDependencySchema()
    {
        var schema = new ListDependencySchema();

        schema.AddRule(
            nameof(BusinessTripRequestDocument.Purpose),
            nameof(BusinessTripRequestDocument.Department),
            Department.Administration,
            Purpose.Meeting,
            Purpose.Conference);

        schema.AddRule(
            nameof(BusinessTripRequestDocument.Purpose),
            nameof(BusinessTripRequestDocument.Department),
            Department.EducationalDepartment,
            Purpose.Conference,
            Purpose.Seminar,
            Purpose.Olympiad,
            Purpose.Training);

        schema.AddRule(
            nameof(BusinessTripRequestDocument.Purpose),
            nameof(BusinessTripRequestDocument.Department),
            Department.ItDepartment,
            Purpose.SystemImplementation,
            Purpose.ReportMeeting,
            Purpose.Audit);

        schema.AddRule(
            nameof(BusinessTripRequestDocument.Purpose),
            nameof(BusinessTripRequestDocument.Department),
            Department.Accounting,
            Purpose.ReportMeeting,
            Purpose.Audit);

        schema.AddRule(
            nameof(BusinessTripRequestDocument.Destination),
            nameof(BusinessTripRequestDocument.Purpose),
            Purpose.Meeting,
            Destination.Local,
            Destination.Regional);

        schema.AddRule(
            nameof(BusinessTripRequestDocument.Destination),
            nameof(BusinessTripRequestDocument.Purpose),
            Purpose.Conference,
            Destination.Regional,
            Destination.Federal,
            Destination.International);

        schema.AddRule(
            nameof(BusinessTripRequestDocument.Destination),
            nameof(BusinessTripRequestDocument.Purpose),
            Purpose.Seminar,
            Destination.Regional,
            Destination.Federal);

        schema.AddRule(
            nameof(BusinessTripRequestDocument.Destination),
            nameof(BusinessTripRequestDocument.Purpose),
            Purpose.Olympiad,
            Destination.Regional,
            Destination.Federal);

        schema.AddRule(
            nameof(BusinessTripRequestDocument.Destination),
            nameof(BusinessTripRequestDocument.Purpose),
            Purpose.Training,
            Destination.Federal,
            Destination.International);

        schema.AddRule(
            nameof(BusinessTripRequestDocument.Destination),
            nameof(BusinessTripRequestDocument.Purpose),
            Purpose.SystemImplementation,
            Destination.Regional,
            Destination.Federal);

        schema.AddRule(
            nameof(BusinessTripRequestDocument.Destination),
            nameof(BusinessTripRequestDocument.Purpose),
            Purpose.ReportMeeting,
            Destination.Regional,
            Destination.Federal);

        schema.AddRule(
            nameof(BusinessTripRequestDocument.Destination),
            nameof(BusinessTripRequestDocument.Purpose),
            Purpose.Audit,
            Destination.Federal);

        schema.AddRule(
            nameof(BusinessTripRequestDocument.TransportType),
            nameof(BusinessTripRequestDocument.Destination),
            Destination.Local,
            TransportType.PublicTransport,
            TransportType.CompanyTransport);

        schema.AddRule(
            nameof(BusinessTripRequestDocument.TransportType),
            nameof(BusinessTripRequestDocument.Destination),
            Destination.Regional,
            TransportType.CompanyTransport,
            TransportType.Bus,
            TransportType.Train);

        schema.AddRule(
            nameof(BusinessTripRequestDocument.TransportType),
            nameof(BusinessTripRequestDocument.Destination),
            Destination.Federal,
            TransportType.Train,
            TransportType.Plane);

        schema.AddRule(
            nameof(BusinessTripRequestDocument.TransportType),
            nameof(BusinessTripRequestDocument.Destination),
            Destination.International,
            TransportType.Plane);

        schema.AddRule(
            nameof(BusinessTripRequestDocument.Accommodation),
            nameof(BusinessTripRequestDocument.TransportType),
            TransportType.PublicTransport,
            Accommodation.NotRequired);

        schema.AddRule(
            nameof(BusinessTripRequestDocument.Accommodation),
            nameof(BusinessTripRequestDocument.TransportType),
            TransportType.CompanyTransport,
            Accommodation.NotRequired);

        schema.AddRule(
            nameof(BusinessTripRequestDocument.Accommodation),
            nameof(BusinessTripRequestDocument.TransportType),
            TransportType.Bus,
            Accommodation.Hotel);

        schema.AddRule(
            nameof(BusinessTripRequestDocument.Accommodation),
            nameof(BusinessTripRequestDocument.TransportType),
            TransportType.Train,
            Accommodation.Hotel);

        schema.AddRule(
            nameof(BusinessTripRequestDocument.Accommodation),
            nameof(BusinessTripRequestDocument.TransportType),
            TransportType.Plane,
            Accommodation.Hotel);

        return schema;
    }

    private static ListOption CreateOption(string keyPart, string displayName)
    {
        return new ListOption(new ListOptionKey($"{KeyPrefix}.{keyPart}"), displayName);
    }
}
