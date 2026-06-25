using MADOC.Domain.Core.Documents;

namespace MADOC.Application.Documents;

public sealed class DocumentTypeRegistry : IDocumentTypeRegistry
{
    private readonly IReadOnlyList<DocumentTypeDescriptor> descriptors;

    private readonly Dictionary<DocumentTypeKey, DocumentTypeDescriptor> descriptorsByKey;

    public DocumentTypeRegistry()
        : this(CreateDefaultDescriptors())
    {
    }

    public DocumentTypeRegistry(IEnumerable<DocumentTypeDescriptor> descriptors)
    {
        ArgumentNullException.ThrowIfNull(descriptors);

        var descriptorList = descriptors.ToArray();

        if (descriptorList.Length == 0)
        {
            throw new ArgumentException("Реестр типов документов не может быть пустым", nameof(descriptors));
        }

        var dictionary = new Dictionary<DocumentTypeKey, DocumentTypeDescriptor>();

        foreach (var descriptor in descriptorList)
        {
            if (!dictionary.TryAdd(descriptor.Key, descriptor))
            {
                throw new InvalidOperationException($"Тип документа с ключом {descriptor.Key} зарегистрирован несколько раз");
            }
        }

        this.descriptors = descriptorList;
        descriptorsByKey = dictionary;
    }

    public IReadOnlyList<DocumentTypeDescriptor> GetAll()
    {
        return descriptors;
    }

    public bool TryGet(DocumentTypeKey key, out DocumentTypeDescriptor? descriptor)
    {
        return descriptorsByKey.TryGetValue(key, out descriptor);
    }

    public DocumentTypeDescriptor GetRequired(DocumentTypeKey key)
    {
        if (!descriptorsByKey.TryGetValue(key, out var descriptor))
        {
            throw new InvalidOperationException($"Тип документа {key} не зарегистрирован");
        }

        return descriptor;
    }

    private static IReadOnlyList<DocumentTypeDescriptor> CreateDefaultDescriptors()
    {
        return new[]
        {
            new DocumentTypeDescriptor("certificate_request","Заявка на справку",typeof(CertificateRequestDocument),"certificate_request.html"),

            new DocumentTypeDescriptor("student_application","Заявление студента", typeof(StudentApplicationDocument),"student_application.html"),

            new DocumentTypeDescriptor("service_contract","Договор оказания услуг",typeof(ServiceContractDocument),"service_contract.html"),

            new DocumentTypeDescriptor( "business_trip_request", "Заявка на командировку",typeof(BusinessTripRequestDocument),"business_trip_request.html"),

            new DocumentTypeDescriptor("room_booking_request","Заявка на бронирование аудитории",typeof(RoomBookingRequestDocument),"room_booking_request.html")
        };
    }
}
