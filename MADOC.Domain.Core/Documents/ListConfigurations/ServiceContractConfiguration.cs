using MADOC.Domain.Core.Validation.ListDependencies;
using MADOC.Domain.Core.Validation.Lists;

namespace MADOC.Domain.Core.Documents.ListConfigurations;

public static class ServiceContractConfiguration
{
    private const string KeyPrefix = "service_contract";

    public static readonly DocumentListCatalog ListCatalog = CreateListCatalog();

    public static readonly ListDependencySchema DependencySchema = CreateDependencySchema();

    public static class ContractorType
    {
        public static readonly ListOption Individual = CreateOption(
            "contractor_type.individual",
            "Физическое лицо");

        public static readonly ListOption LegalEntity = CreateOption(
            "contractor_type.legal_entity",
            "Юридическое лицо");

        public static readonly ListOption GovernmentOrganization = CreateOption(
            "contractor_type.government_organization",
            "Государственная организация");

        public static readonly ListOption EducationalOrganization = CreateOption(
            "contractor_type.educational_organization",
            "Образовательная организация");
    }

    public static class ContractType
    {
        public static readonly ListOption ServiceContract = CreateOption(
            "contract_type.service_contract",
            "Договор оказания услуг");

        public static readonly ListOption CivilLawContract = CreateOption(
            "contract_type.civil_law_contract",
            "Гражданско-правовой договор");

        public static readonly ListOption SupplyContract = CreateOption(
            "contract_type.supply_contract",
            "Договор поставки");

        public static readonly ListOption LeaseContract = CreateOption(
            "contract_type.lease_contract",
            "Договор аренды");

        public static readonly ListOption CooperationAgreement = CreateOption(
            "contract_type.cooperation_agreement",
            "Соглашение о сотрудничестве");

        public static readonly ListOption InternshipContract = CreateOption(
            "contract_type.internship_contract",
            "Договор практики");
    }

    public static class PaymentSchedule
    {
        public static readonly ListOption OneTimePayment = CreateOption(
            "payment_schedule.one_time_payment",
            "Единовременная оплата");

        public static readonly ListOption MonthlyPayment = CreateOption(
            "payment_schedule.monthly_payment",
            "Ежемесячная оплата");

        public static readonly ListOption StagedPayment = CreateOption(
            "payment_schedule.staged_payment",
            "Поэтапная оплата");

        public static readonly ListOption InvoicePayment = CreateOption(
            "payment_schedule.invoice_payment",
            "Оплата по счету");

        public static readonly ListOption NoPayment = CreateOption(
            "payment_schedule.no_payment",
            "Без оплаты");
    }

    public static class SigningMethod
    {
        public static readonly ListOption PaperSigning = CreateOption(
            "signing_method.paper_signing",
            "Бумажное подписание");

        public static readonly ListOption ElectronicSignature = CreateOption(
            "signing_method.electronic_signature",
            "Электронная подпись");
    }

    private static DocumentListCatalog CreateListCatalog()
    {
        var catalog = new DocumentListCatalog();

        catalog.AddList(
            nameof(ServiceContractDocument.ContractorType),
            ContractorType.Individual,
            ContractorType.LegalEntity,
            ContractorType.GovernmentOrganization,
            ContractorType.EducationalOrganization);

        catalog.AddList(
            nameof(ServiceContractDocument.ContractType),
            ContractType.ServiceContract,
            ContractType.CivilLawContract,
            ContractType.SupplyContract,
            ContractType.LeaseContract,
            ContractType.CooperationAgreement,
            ContractType.InternshipContract);

        catalog.AddList(
            nameof(ServiceContractDocument.PaymentSchedule),
            PaymentSchedule.OneTimePayment,
            PaymentSchedule.MonthlyPayment,
            PaymentSchedule.StagedPayment,
            PaymentSchedule.InvoicePayment,
            PaymentSchedule.NoPayment);

        catalog.AddList(
            nameof(ServiceContractDocument.SigningMethod),
            SigningMethod.PaperSigning,
            SigningMethod.ElectronicSignature);

        return catalog;
    }

    private static ListDependencySchema CreateDependencySchema()
    {
        var schema = new ListDependencySchema();

        schema.AddRule(
            nameof(ServiceContractDocument.ContractType),
            nameof(ServiceContractDocument.ContractorType),
            ContractorType.Individual,
            ContractType.ServiceContract,
            ContractType.CivilLawContract,
            ContractType.LeaseContract);

        schema.AddRule(
            nameof(ServiceContractDocument.ContractType),
            nameof(ServiceContractDocument.ContractorType),
            ContractorType.LegalEntity,
            ContractType.ServiceContract,
            ContractType.SupplyContract,
            ContractType.LeaseContract,
            ContractType.CooperationAgreement);

        schema.AddRule(
            nameof(ServiceContractDocument.ContractType),
            nameof(ServiceContractDocument.ContractorType),
            ContractorType.GovernmentOrganization,
            ContractType.ServiceContract,
            ContractType.SupplyContract,
            ContractType.CooperationAgreement);

        schema.AddRule(
            nameof(ServiceContractDocument.ContractType),
            nameof(ServiceContractDocument.ContractorType),
            ContractorType.EducationalOrganization,
            ContractType.CooperationAgreement,
            ContractType.InternshipContract);

        schema.AddRule(
            nameof(ServiceContractDocument.PaymentSchedule),
            nameof(ServiceContractDocument.ContractType),
            ContractType.ServiceContract,
            PaymentSchedule.OneTimePayment,
            PaymentSchedule.MonthlyPayment,
            PaymentSchedule.StagedPayment);

        schema.AddRule(
            nameof(ServiceContractDocument.PaymentSchedule),
            nameof(ServiceContractDocument.ContractType),
            ContractType.CivilLawContract,
            PaymentSchedule.OneTimePayment,
            PaymentSchedule.MonthlyPayment);

        schema.AddRule(
            nameof(ServiceContractDocument.PaymentSchedule),
            nameof(ServiceContractDocument.ContractType),
            ContractType.SupplyContract,
            PaymentSchedule.OneTimePayment,
            PaymentSchedule.InvoicePayment);

        schema.AddRule(
            nameof(ServiceContractDocument.PaymentSchedule),
            nameof(ServiceContractDocument.ContractType),
            ContractType.LeaseContract,
            PaymentSchedule.MonthlyPayment,
            PaymentSchedule.StagedPayment);

        schema.AddRule(
            nameof(ServiceContractDocument.PaymentSchedule),
            nameof(ServiceContractDocument.ContractType),
            ContractType.CooperationAgreement,
            PaymentSchedule.NoPayment);

        schema.AddRule(
            nameof(ServiceContractDocument.PaymentSchedule),
            nameof(ServiceContractDocument.ContractType),
            ContractType.InternshipContract,
            PaymentSchedule.NoPayment);

        schema.AddRule(
            nameof(ServiceContractDocument.SigningMethod),
            nameof(ServiceContractDocument.PaymentSchedule),
            PaymentSchedule.OneTimePayment,
            SigningMethod.PaperSigning,
            SigningMethod.ElectronicSignature);

        schema.AddRule(
            nameof(ServiceContractDocument.SigningMethod),
            nameof(ServiceContractDocument.PaymentSchedule),
            PaymentSchedule.MonthlyPayment,
            SigningMethod.PaperSigning,
            SigningMethod.ElectronicSignature);

        schema.AddRule(
            nameof(ServiceContractDocument.SigningMethod),
            nameof(ServiceContractDocument.PaymentSchedule),
            PaymentSchedule.StagedPayment,
            SigningMethod.PaperSigning,
            SigningMethod.ElectronicSignature);

        schema.AddRule(
            nameof(ServiceContractDocument.SigningMethod),
            nameof(ServiceContractDocument.PaymentSchedule),
            PaymentSchedule.InvoicePayment,
            SigningMethod.PaperSigning,
            SigningMethod.ElectronicSignature);

        schema.AddRule(
            nameof(ServiceContractDocument.SigningMethod),
            nameof(ServiceContractDocument.PaymentSchedule),
            PaymentSchedule.NoPayment,
            SigningMethod.PaperSigning,
            SigningMethod.ElectronicSignature);

        return schema;
    }

    private static ListOption CreateOption(string keyPart, string displayName)
    {
        return new ListOption(
            new ListOptionKey($"{KeyPrefix}.{keyPart}"),
            displayName);
    }
}
