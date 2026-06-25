using MADOC.Domain.Core.Validation.ListDependencies;
using MADOC.Domain.Core.Validation.Lists;

namespace MADOC.Domain.Core.Documents.ListConfigurations;

public static class StudentApplicationListConfiguration
{
    private const string KeyPrefix = "student_application";

    public static readonly DocumentListCatalog ListCatalog = CreateListCatalog();

    public static readonly ListDependencySchema DependencySchema = CreateDependencySchema();

    public static class ApplicantCategory
    {
        public static readonly ListOption Student = CreateOption("applicant_category.student", "Студент");

        public static readonly ListOption GroupLeader = CreateOption("applicant_category.group_leader", "Староста");

        public static readonly ListOption Graduate = CreateOption("applicant_category.graduate", "Выпускник");
    }

    public static class ApplicationType
    {
        public static readonly ListOption Certificate = CreateOption("type.certificate", "Заявление на справку");

        public static readonly ListOption AcademicLeave = CreateOption("type.academic_leave", "Заявление на академический отпуск");

        public static readonly ListOption FinancialAid = CreateOption("type.financial_aid", "Заявление на материальную помощь");

        public static readonly ListOption Dormitory = CreateOption("type.dormitory", "Заявление на общежитие");

        public static readonly ListOption GroupCertificate = CreateOption("type.group_certificate", "Заявление на групповую справку");

        public static readonly ListOption Dictation = CreateOption("type.dictation", "Заявление на проведение диктанта");

        public static readonly ListOption ArchiveCertificate = CreateOption("type.archive_certificate", "Заявление на архивную справку");
    }

    public static class ApplicationSubtype
    {
        public static readonly ListOption Study = CreateOption("subtype.study", "Об обучении");

        public static readonly ListOption MilitaryOffice = CreateOption("subtype.military_office", "Для военкомата");

        public static readonly ListOption SocialProtection = CreateOption("subtype.social_protection", "Для социальной защиты");

        public static readonly ListOption MedicalReason = CreateOption("subtype.medical_reason", "По медицинским причинам");

        public static readonly ListOption MilitaryDraft = CreateOption("subtype.military_draft", "По причине призыва в армию");

        public static readonly ListOption FamilyReason = CreateOption("subtype.family_reason", "По семейным обстоятельствам");

        public static readonly ListOption Social = CreateOption("subtype.social", "Социальная");

        public static readonly ListOption OneTime = CreateOption("subtype.one_time", "Единовременная");

        public static readonly ListOption Schedule = CreateOption("subtype.schedule", "По расписанию");

        public static readonly ListOption ValidReason = CreateOption("subtype.valid_reason", "По уважительной причине");

        public static readonly ListOption StudyPeriod = CreateOption("subtype.study_period", "О периоде обучения");
    }

    public static class ReceiveMethod
    {
        public static readonly ListOption Electronic = CreateOption("receive_method.electronic", "В электронном виде");

        public static readonly ListOption Paper = CreateOption("receive_method.paper", "В бумажном виде");

        public static readonly ListOption Curator = CreateOption("receive_method.curator", "Через куратора");
    }

    private static DocumentListCatalog CreateListCatalog()
    {
        var catalog = new DocumentListCatalog();

        catalog.AddList(
            nameof(StudentApplicationDocument.ApplicantCategory),
            ApplicantCategory.Student,
            ApplicantCategory.GroupLeader,
            ApplicantCategory.Graduate);

        catalog.AddList(
            nameof(StudentApplicationDocument.ApplicationType),
            ApplicationType.Certificate,
            ApplicationType.AcademicLeave,
            ApplicationType.FinancialAid,
            ApplicationType.Dormitory,
            ApplicationType.GroupCertificate,
            ApplicationType.Dictation,
            ApplicationType.ArchiveCertificate);

        catalog.AddList(
            nameof(StudentApplicationDocument.ApplicationSubtype),
            ApplicationSubtype.Study,
            ApplicationSubtype.MilitaryOffice,
            ApplicationSubtype.SocialProtection,
            ApplicationSubtype.MedicalReason,
            ApplicationSubtype.MilitaryDraft,
            ApplicationSubtype.FamilyReason,
            ApplicationSubtype.Social,
            ApplicationSubtype.OneTime,
            ApplicationSubtype.Schedule,
            ApplicationSubtype.ValidReason,
            ApplicationSubtype.StudyPeriod);

        catalog.AddList(
            nameof(StudentApplicationDocument.ReceiveMethod),
            ReceiveMethod.Electronic,
            ReceiveMethod.Paper,
            ReceiveMethod.Curator);

        return catalog;
    }

    private static ListDependencySchema CreateDependencySchema()
    {
        var schema = new ListDependencySchema();

        schema.AddRule(
            nameof(StudentApplicationDocument.ApplicationType),
            nameof(StudentApplicationDocument.ApplicantCategory),
            ApplicantCategory.Student,
            ApplicationType.Certificate,
            ApplicationType.AcademicLeave,
            ApplicationType.FinancialAid,
            ApplicationType.Dormitory);

        schema.AddRule(
            nameof(StudentApplicationDocument.ApplicationType),
            nameof(StudentApplicationDocument.ApplicantCategory),
            ApplicantCategory.GroupLeader,
            ApplicationType.GroupCertificate,
            ApplicationType.Dictation);

        schema.AddRule(
            nameof(StudentApplicationDocument.ApplicationType),
            nameof(StudentApplicationDocument.ApplicantCategory),
            ApplicantCategory.Graduate,
            ApplicationType.ArchiveCertificate);

        schema.AddRule(
            nameof(StudentApplicationDocument.ApplicationSubtype),
            nameof(StudentApplicationDocument.ApplicationType),
            ApplicationType.Certificate,
            ApplicationSubtype.Study,
            ApplicationSubtype.MilitaryOffice,
            ApplicationSubtype.SocialProtection);

        schema.AddRule(
            nameof(StudentApplicationDocument.ApplicationSubtype),
            nameof(StudentApplicationDocument.ApplicationType),
            ApplicationType.AcademicLeave,
            ApplicationSubtype.MedicalReason,
            ApplicationSubtype.MilitaryDraft,
            ApplicationSubtype.FamilyReason);

        schema.AddRule(
            nameof(StudentApplicationDocument.ApplicationSubtype),
            nameof(StudentApplicationDocument.ApplicationType),
            ApplicationType.FinancialAid,
            ApplicationSubtype.Social,
            ApplicationSubtype.OneTime);

        schema.AddRule(
            nameof(StudentApplicationDocument.ApplicationSubtype),
            nameof(StudentApplicationDocument.ApplicationType),
            ApplicationType.Dormitory,
            ApplicationSubtype.FamilyReason,
            ApplicationSubtype.ValidReason);

        schema.AddRule(
            nameof(StudentApplicationDocument.ApplicationSubtype),
            nameof(StudentApplicationDocument.ApplicationType),
            ApplicationType.GroupCertificate,
            ApplicationSubtype.Study);

        schema.AddRule(
            nameof(StudentApplicationDocument.ApplicationSubtype),
            nameof(StudentApplicationDocument.ApplicationType),
            ApplicationType.Dictation,
            ApplicationSubtype.Schedule,
            ApplicationSubtype.ValidReason);

        schema.AddRule(
            nameof(StudentApplicationDocument.ApplicationSubtype),
            nameof(StudentApplicationDocument.ApplicationType),
            ApplicationType.ArchiveCertificate,
            ApplicationSubtype.StudyPeriod);

        schema.AddRule(
            nameof(StudentApplicationDocument.ReceiveMethod),
            nameof(StudentApplicationDocument.ApplicationSubtype),
            ApplicationSubtype.Study,
            ReceiveMethod.Electronic,
            ReceiveMethod.Paper);

        schema.AddRule(
            nameof(StudentApplicationDocument.ReceiveMethod),
            nameof(StudentApplicationDocument.ApplicationSubtype),
            ApplicationSubtype.MilitaryOffice,
            ReceiveMethod.Paper);

        schema.AddRule(
            nameof(StudentApplicationDocument.ReceiveMethod),
            nameof(StudentApplicationDocument.ApplicationSubtype),
            ApplicationSubtype.SocialProtection,
            ReceiveMethod.Electronic,
            ReceiveMethod.Paper);

        schema.AddRule(
            nameof(StudentApplicationDocument.ReceiveMethod),
            nameof(StudentApplicationDocument.ApplicationSubtype),
            ApplicationSubtype.MedicalReason,
            ReceiveMethod.Paper);

        schema.AddRule(
            nameof(StudentApplicationDocument.ReceiveMethod),
            nameof(StudentApplicationDocument.ApplicationSubtype),
            ApplicationSubtype.MilitaryDraft,
            ReceiveMethod.Paper);

        schema.AddRule(
            nameof(StudentApplicationDocument.ReceiveMethod),
            nameof(StudentApplicationDocument.ApplicationSubtype),
            ApplicationSubtype.FamilyReason,
            ReceiveMethod.Paper,
            ReceiveMethod.Curator);

        schema.AddRule(
            nameof(StudentApplicationDocument.ReceiveMethod),
            nameof(StudentApplicationDocument.ApplicationSubtype),
            ApplicationSubtype.Social,
            ReceiveMethod.Paper,
            ReceiveMethod.Curator);

        schema.AddRule(
            nameof(StudentApplicationDocument.ReceiveMethod),
            nameof(StudentApplicationDocument.ApplicationSubtype),
            ApplicationSubtype.OneTime,
            ReceiveMethod.Paper,
            ReceiveMethod.Curator);

        schema.AddRule(
            nameof(StudentApplicationDocument.ReceiveMethod),
            nameof(StudentApplicationDocument.ApplicationSubtype),
            ApplicationSubtype.Schedule,
            ReceiveMethod.Electronic);

        schema.AddRule(
            nameof(StudentApplicationDocument.ReceiveMethod),
            nameof(StudentApplicationDocument.ApplicationSubtype),
            ApplicationSubtype.ValidReason,
            ReceiveMethod.Electronic,
            ReceiveMethod.Curator);

        schema.AddRule(
            nameof(StudentApplicationDocument.ReceiveMethod),
            nameof(StudentApplicationDocument.ApplicationSubtype),
            ApplicationSubtype.StudyPeriod,
            ReceiveMethod.Electronic,
            ReceiveMethod.Paper);

        return schema;
    }

    private static ListOption CreateOption(string keyPart, string displayName)
    {
        return new ListOption(new ListOptionKey($"{KeyPrefix}.{keyPart}"), displayName);
    }
}