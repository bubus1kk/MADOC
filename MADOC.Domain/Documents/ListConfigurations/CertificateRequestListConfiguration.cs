using MADOC.Domain.Validation.ListDependencies;
using MADOC.Domain.Validation.Lists;

namespace MADOC.Domain.Documents.ListConfigurations
{
    public static class CertificateRequestListConfiguration
    {
        public static readonly DocumentListCatalog ListCatalog = CreateListCatalog();

        public static readonly ListDependencySchema DependencySchema = CreateDependencySchema();

        public static class CertificateType
        {
            public static readonly ListOptionKey Study = new("certificate.type.study");
            public static readonly ListOptionKey StudyPeriod = new("certificate.type.study_period");
            public static readonly ListOptionKey MilitaryOffice = new("certificate.type.military_office");
            public static readonly ListOptionKey Scholarship = new("certificate.type.scholarship");
        }

        public static class CertificatePurpose
        {
            public static readonly ListOptionKey AnyPlace = new("certificate.purpose.any_place");
            public static readonly ListOptionKey SocialProtection = new("certificate.purpose.social_protection");
            public static readonly ListOptionKey Employer = new("certificate.purpose.employer");
            public static readonly ListOptionKey Archive = new("certificate.purpose.archive");
            public static readonly ListOptionKey Transfer = new("certificate.purpose.transfer");
            public static readonly ListOptionKey Recovery = new("certificate.purpose.recovery");
            public static readonly ListOptionKey InitialMilitaryRegistration = new("certificate.purpose.initial_military_registration");
            public static readonly ListOptionKey DataClarification = new("certificate.purpose.data_clarification");
            public static readonly ListOptionKey Bank = new("certificate.purpose.bank");
        }

        public static class CertificateFormat
        {
            public static readonly ListOptionKey Electronic = new("certificate.format.electronic");
            public static readonly ListOptionKey Paper = new("certificate.format.paper");
            public static readonly ListOptionKey PaperWithStamp = new("certificate.format.paper_with_stamp");
        }

        public static class ReceivePlace
        {
            public static readonly ListOptionKey PersonalAccount = new("certificate.receive_place.personal_account");
            public static readonly ListOptionKey Email = new("certificate.receive_place.email");
            public static readonly ListOptionKey EducationOffice = new("certificate.receive_place.education_office");
            public static readonly ListOptionKey Chancellery = new("certificate.receive_place.chancellery");
        }

        private static DocumentListCatalog CreateListCatalog()
        {
            var catalog = new DocumentListCatalog();

            catalog.AddList(
                nameof(CertificateRequestDocument.CertificateType),
                new ListOption(CertificateType.Study, "Справка об обучении"),
                new ListOption(CertificateType.StudyPeriod, "Справка о периоде обучения"),
                new ListOption(CertificateType.MilitaryOffice, "Справка для военкомата"),
                new ListOption(CertificateType.Scholarship, "Справка о стипендии"));

            catalog.AddList(
                nameof(CertificateRequestDocument.CertificatePurpose),
                new ListOption(CertificatePurpose.AnyPlace, "По месту требования"),
                new ListOption(CertificatePurpose.SocialProtection, "Для социальной защиты"),
                new ListOption(CertificatePurpose.Employer, "Для работодателя"),
                new ListOption(CertificatePurpose.Archive, "Для архива"),
                new ListOption(CertificatePurpose.Transfer, "Для перевода"),
                new ListOption(CertificatePurpose.Recovery, "Для восстановления"),
                new ListOption(CertificatePurpose.InitialMilitaryRegistration, "Первичная постановка"),
                new ListOption(CertificatePurpose.DataClarification, "Уточнение данных"),
                new ListOption(CertificatePurpose.Bank, "Для банка"));

            catalog.AddList(
                nameof(CertificateRequestDocument.CertificateFormat),
                new ListOption(CertificateFormat.Electronic, "Электронная"),
                new ListOption(CertificateFormat.Paper, "Бумажная"),
                new ListOption(CertificateFormat.PaperWithStamp, "Бумажная с печатью"));

            catalog.AddList(
                nameof(CertificateRequestDocument.ReceivePlace),
                new ListOption(ReceivePlace.PersonalAccount, "Личный кабинет"),
                new ListOption(ReceivePlace.Email, "Электронная почта"),
                new ListOption(ReceivePlace.EducationOffice, "Учебная часть"),
                new ListOption(ReceivePlace.Chancellery, "Канцелярия"));

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
                CertificateFormat.Electronic,
                CertificateFormat.Paper,
                CertificateFormat.PaperWithStamp);

            schema.AddRule(
                nameof(CertificateRequestDocument.CertificateFormat),
                nameof(CertificateRequestDocument.CertificatePurpose),
                CertificatePurpose.Employer,
                CertificateFormat.Electronic,
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
    }
}