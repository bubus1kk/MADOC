using MADOC.Domain.Documents;
using MADOC.Domain.Validation.ListDependencies;
using MADOC.Domain.Validation.Lists;

namespace MADOC.Domain.Documents.ListConfigurations;

public static class CertificateRequestListConfiguration
{
    private const string KeyPrefix = "certificate_request";

    public static readonly DocumentListCatalog ListCatalog = CreateListCatalog();

    public static readonly ListDependencySchema DependencySchema = CreateDependencySchema();

    public static class CertificateType
    {
        public static readonly ListOption Study = CreateOption("type.study","Справка об обучении");

        public static readonly ListOption StudyPeriod = CreateOption("type.study_period","Справка о периоде обучения");

        public static readonly ListOption MilitaryOffice = CreateOption("type.military_office","Справка для военкомата");

        public static readonly ListOption Scholarship = CreateOption("type.scholarship","Справка о стипендии");
    }

    public static class CertificatePurpose
    {
        public static readonly ListOption AnyPlace = CreateOption("purpose.any_place","По месту требования");

        public static readonly ListOption SocialProtection = CreateOption("purpose.social_protection","Для социальной защиты");

        public static readonly ListOption Employer = CreateOption("purpose.employer","Для работодателя");

        public static readonly ListOption Archive = CreateOption("purpose.archive","Для архива");

        public static readonly ListOption Transfer = CreateOption("purpose.transfer","Для перевода");

        public static readonly ListOption Recovery = CreateOption("purpose.recovery","Для восстановления");

        public static readonly ListOption InitialMilitaryRegistration = CreateOption("purpose.initial_military_registration",
            "Первичная постановка");

        public static readonly ListOption DataClarification = CreateOption("purpose.data_clarification","Уточнение данных");

        public static readonly ListOption Bank = CreateOption("purpose.bank","Для банка");
    }

    public static class CertificateFormat
    {
        public static readonly ListOption Electronic = CreateOption("format.electronic","Электронная");

        public static readonly ListOption Paper = CreateOption("format.paper","Бумажная");

        public static readonly ListOption PaperWithStamp = CreateOption("format.paper_with_stamp","Бумажная с печатью");
    }

    public static class ReceivePlace
    {
        public static readonly ListOption PersonalAccount = CreateOption("receive_place.personal_account","Личный кабинет");

        public static readonly ListOption Email = CreateOption("receive_place.email","Электронная почта");

        public static readonly ListOption EducationOffice = CreateOption("receive_place.education_office","Учебная часть");

        public static readonly ListOption Chancellery = CreateOption("receive_place.chancellery","Канцелярия");
    }

    private static DocumentListCatalog CreateListCatalog()
    {
        var catalog = new DocumentListCatalog();

        catalog.AddList(
            nameof(CertificateRequestDocument.CertificateType),
            CertificateType.Study,
            CertificateType.StudyPeriod,
            CertificateType.MilitaryOffice,
            CertificateType.Scholarship);

        catalog.AddList(
            nameof(CertificateRequestDocument.CertificatePurpose),
            CertificatePurpose.AnyPlace,
            CertificatePurpose.SocialProtection,
            CertificatePurpose.Employer,
            CertificatePurpose.Archive,
            CertificatePurpose.Transfer,
            CertificatePurpose.Recovery,
            CertificatePurpose.InitialMilitaryRegistration,
            CertificatePurpose.DataClarification,
            CertificatePurpose.Bank);

        catalog.AddList(
            nameof(CertificateRequestDocument.CertificateFormat),
            CertificateFormat.Electronic,
            CertificateFormat.Paper,
            CertificateFormat.PaperWithStamp);

        catalog.AddList(
            nameof(CertificateRequestDocument.ReceivePlace),
            ReceivePlace.PersonalAccount,
            ReceivePlace.Email,
            ReceivePlace.EducationOffice,
            ReceivePlace.Chancellery);

        return catalog;
    }

    private static ListDependencySchema CreateDependencySchema()
    {
        var schema = new ListDependencySchema();

        schema.AddRule(
            nameof(CertificateRequestDocument.CertificatePurpose),
            nameof(CertificateRequestDocument.CertificateType),
            CertificateType.Study,
            CertificatePurpose.AnyPlace,
            CertificatePurpose.SocialProtection,
            CertificatePurpose.Employer);

        schema.AddRule(
            nameof(CertificateRequestDocument.CertificatePurpose),
            nameof(CertificateRequestDocument.CertificateType),
            CertificateType.StudyPeriod,
            CertificatePurpose.Archive,
            CertificatePurpose.Transfer,
            CertificatePurpose.Recovery);

        schema.AddRule(
            nameof(CertificateRequestDocument.CertificatePurpose),
            nameof(CertificateRequestDocument.CertificateType),
            CertificateType.MilitaryOffice,
            CertificatePurpose.InitialMilitaryRegistration,
            CertificatePurpose.DataClarification);

        schema.AddRule(
            nameof(CertificateRequestDocument.CertificatePurpose),
            nameof(CertificateRequestDocument.CertificateType),
            CertificateType.Scholarship,
            CertificatePurpose.Bank,
            CertificatePurpose.SocialProtection);

        schema.AddRule(
            nameof(CertificateRequestDocument.CertificateFormat),
            nameof(CertificateRequestDocument.CertificatePurpose),
            CertificatePurpose.AnyPlace,
            CertificateFormat.Electronic,
            CertificateFormat.Paper);

        schema.AddRule(
            nameof(CertificateRequestDocument.CertificateFormat),
            nameof(CertificateRequestDocument.CertificatePurpose),
            CertificatePurpose.SocialProtection,
            CertificateFormat.PaperWithStamp);

        schema.AddRule(
            nameof(CertificateRequestDocument.CertificateFormat),
            nameof(CertificateRequestDocument.CertificatePurpose),
            CertificatePurpose.Employer,
            CertificateFormat.Electronic,
            CertificateFormat.PaperWithStamp);

        schema.AddRule(
            nameof(CertificateRequestDocument.CertificateFormat),
            nameof(CertificateRequestDocument.CertificatePurpose),
            CertificatePurpose.Archive,
            CertificateFormat.PaperWithStamp);

        schema.AddRule(
            nameof(CertificateRequestDocument.CertificateFormat),
            nameof(CertificateRequestDocument.CertificatePurpose),
            CertificatePurpose.Transfer,
            CertificateFormat.PaperWithStamp);

        schema.AddRule(
            nameof(CertificateRequestDocument.CertificateFormat),
            nameof(CertificateRequestDocument.CertificatePurpose),
            CertificatePurpose.Recovery,
            CertificateFormat.PaperWithStamp);

        schema.AddRule(
            nameof(CertificateRequestDocument.CertificateFormat),
            nameof(CertificateRequestDocument.CertificatePurpose),
            CertificatePurpose.InitialMilitaryRegistration,
            CertificateFormat.PaperWithStamp);

        schema.AddRule(
            nameof(CertificateRequestDocument.CertificateFormat),
            nameof(CertificateRequestDocument.CertificatePurpose),
            CertificatePurpose.DataClarification,
            CertificateFormat.PaperWithStamp);

        schema.AddRule(
            nameof(CertificateRequestDocument.CertificateFormat),
            nameof(CertificateRequestDocument.CertificatePurpose),
            CertificatePurpose.Bank,
            CertificateFormat.PaperWithStamp);

        schema.AddRule(
            nameof(CertificateRequestDocument.ReceivePlace),
            nameof(CertificateRequestDocument.CertificateFormat),
            CertificateFormat.Electronic,
            ReceivePlace.PersonalAccount,
            ReceivePlace.Email);

        schema.AddRule(
            nameof(CertificateRequestDocument.ReceivePlace),
            nameof(CertificateRequestDocument.CertificateFormat),
            CertificateFormat.Paper,
            ReceivePlace.EducationOffice);

        schema.AddRule(
            nameof(CertificateRequestDocument.ReceivePlace),
            nameof(CertificateRequestDocument.CertificateFormat),
            CertificateFormat.PaperWithStamp,
            ReceivePlace.EducationOffice,
            ReceivePlace.Chancellery);

        return schema;
    }

    private static ListOption CreateOption(string keyPart, string displayName)
    {
        return new ListOption(
            new ListOptionKey($"{KeyPrefix}.{keyPart}"),
            displayName);
    }
}