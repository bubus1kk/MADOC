using MADOC.Domain.Core.Validation.ListDependencies;
using MADOC.Domain.Core.Validation.Lists;

namespace MADOC.Domain.Core.Documents.ListConfigurations
{
    public static class RoomBookingRequestConfiguration
    {
        private const string KeyPrefix = "room_booking_request";

        public static readonly DocumentListCatalog ListCatalog = CreateListCatalog();

        public static readonly ListDependencySchema DependencySchema = CreateDependencySchema();

        public static class EventFormat
        {
            public static readonly ListOption Lecture = CreateOption("event_format.lecture", "Лекция");

            public static readonly ListOption Practice = CreateOption("event_format.practice", "Практика");

            public static readonly ListOption Conference = CreateOption("event_format.conference", "Конференция");

            public static readonly ListOption Exam = CreateOption("event_format.exam", "Экзамен");

            public static readonly ListOption Meeting = CreateOption("event_format.meeting", "Собрание");
        }

        public static class Building
        {
            public static readonly ListOption Main = CreateOption("building.main", "Главный");

            public static readonly ListOption Educational = CreateOption("building.educational", "Учебный");

            public static readonly ListOption Laboratory = CreateOption("building.laboratory", "Лабораторный");
        }

        public static class RoomType
        {
            public static readonly ListOption LectureRoom = CreateOption("room_type.lecture_room", "Лекционная");

            public static readonly ListOption AssemblyHall = CreateOption("room_type.assembly_hall", "Актовый зал");

            public static readonly ListOption ComputerClass = CreateOption("room_type.computer_class", "Компьютерный класс");

            public static readonly ListOption StudyRoom = CreateOption("room_type.study_room", "Учебная");

            public static readonly ListOption ConferenceHall = CreateOption("room_type.conference_hall", "Конференц-зал");
        }

        public static class Room
        {
            public static readonly ListOption Auditorium101 = CreateOption("room.auditorium_101", "Аудитория 101");

            public static readonly ListOption Auditorium201 = CreateOption("room.auditorium_201", "Аудитория 201");

            public static readonly ListOption Auditorium305 = CreateOption("room.auditorium_305", "Аудитория 305");

            public static readonly ListOption Auditorium307 = CreateOption("room.auditorium_307", "Аудитория 307");

            public static readonly ListOption AssemblyHall1 = CreateOption("room.assembly_hall_1", "Актовый зал 1");

            public static readonly ListOption ConferenceHall1 = CreateOption("room.conference_hall_1", "Конференц-зал 1");

            public static readonly ListOption ConferenceHall2 = CreateOption("room.conference_hall_2", "Конференц-зал 2");

            public static readonly ListOption ComputerClass1 = CreateOption("room.computer_class_1", "Компьютерный класс 1");

            public static readonly ListOption ComputerClass2 = CreateOption("room.computer_class_2", "Компьютерный класс 2");

            public static readonly ListOption Laboratory1 = CreateOption("room.laboratory_1", "Лаборатория 1");

            public static readonly ListOption Laboratory2 = CreateOption("room.laboratory_2", "Лаборатория 2");
        }

        public static class EquipmentSet
        {
            public static readonly ListOption Projector = CreateOption("equipment_set.projector", "Проектор");

            public static readonly ListOption NotRequired = CreateOption("equipment_set.not_required", "Не требуется");

            public static readonly ListOption ProjectorMicrophoneAndSpeakers = CreateOption("equipment_set.projector_microphone_and_speakers", "Проектор, микрофон и колонки");

            public static readonly ListOption Computers = CreateOption("equipment_set.computers", "ПК");

            public static readonly ListOption ChemicalEquipment = CreateOption( "equipment_set.chemical_equipment", "Химическое оборудование");
        }

        private static DocumentListCatalog CreateListCatalog()
        {
            var catalog = new DocumentListCatalog();

            catalog.AddList(
                nameof(RoomBookingRequestDocument.EventFormat),
                EventFormat.Lecture,
                EventFormat.Practice,
                EventFormat.Conference,
                EventFormat.Exam,
                EventFormat.Meeting);

            catalog.AddList(
                nameof(RoomBookingRequestDocument.Building),
                Building.Main,
                Building.Educational,
                Building.Laboratory);

            catalog.AddList(
                nameof(RoomBookingRequestDocument.RoomType),
                RoomType.LectureRoom,
                RoomType.AssemblyHall,
                RoomType.ComputerClass,
                RoomType.StudyRoom,
                RoomType.ConferenceHall);

            catalog.AddList(
                nameof(RoomBookingRequestDocument.Room),
                Room.Auditorium101,
                Room.Auditorium201,
                Room.Auditorium305,
                Room.Auditorium307,
                Room.AssemblyHall1,
                Room.ConferenceHall1,
                Room.ConferenceHall2,
                Room.ComputerClass1,
                Room.ComputerClass2,
                Room.Laboratory1,
                Room.Laboratory2);

            catalog.AddList(
                nameof(RoomBookingRequestDocument.EquipmentSet),
                EquipmentSet.Projector,
                EquipmentSet.NotRequired,
                EquipmentSet.ProjectorMicrophoneAndSpeakers,
                EquipmentSet.Computers,
                EquipmentSet.ChemicalEquipment);

            return catalog;
        }

        private static ListDependencySchema CreateDependencySchema()
        {
            var schema = new ListDependencySchema();

            schema.AddRule(
                nameof(RoomBookingRequestDocument.Building),
                nameof(RoomBookingRequestDocument.EventFormat),
                EventFormat.Lecture,
                Building.Main);

            schema.AddRule(
                nameof(RoomBookingRequestDocument.Building),
                nameof(RoomBookingRequestDocument.EventFormat),
                EventFormat.Practice,
                Building.Educational);

            schema.AddRule(
                nameof(RoomBookingRequestDocument.Building),
                nameof(RoomBookingRequestDocument.EventFormat),
                EventFormat.Conference,
                Building.Educational,
                Building.Laboratory);

            schema.AddRule(
                nameof(RoomBookingRequestDocument.Building),
                nameof(RoomBookingRequestDocument.EventFormat),
                EventFormat.Exam,
                Building.Educational);

            schema.AddRule(
                nameof(RoomBookingRequestDocument.Building),
                nameof(RoomBookingRequestDocument.EventFormat),
                EventFormat.Meeting,
                Building.Main,
                Building.Laboratory);

            schema.AddRule(
                nameof(RoomBookingRequestDocument.RoomType),
                nameof(RoomBookingRequestDocument.Building),
                Building.Main,
                RoomType.LectureRoom,
                RoomType.AssemblyHall);

            schema.AddRule(
                nameof(RoomBookingRequestDocument.RoomType),
                nameof(RoomBookingRequestDocument.Building),
                Building.Educational,
                RoomType.AssemblyHall,
                RoomType.ComputerClass,
                RoomType.StudyRoom,
                RoomType.ConferenceHall);

            schema.AddRule(
                nameof(RoomBookingRequestDocument.RoomType),
                nameof(RoomBookingRequestDocument.Building),
                Building.Laboratory,
                RoomType.StudyRoom,
                RoomType.ConferenceHall);

            schema.AddRule(
                nameof(RoomBookingRequestDocument.Room),
                nameof(RoomBookingRequestDocument.RoomType),
                RoomType.LectureRoom,
                Room.Auditorium101,
                Room.Auditorium201);

            schema.AddRule(
                nameof(RoomBookingRequestDocument.Room),
                nameof(RoomBookingRequestDocument.RoomType),
                RoomType.AssemblyHall,
                Room.AssemblyHall1);

            schema.AddRule(
                nameof(RoomBookingRequestDocument.Room),
                nameof(RoomBookingRequestDocument.RoomType),
                RoomType.ComputerClass,
                Room.ComputerClass1,
                Room.ComputerClass2);

            schema.AddRule(
                nameof(RoomBookingRequestDocument.Room),
                nameof(RoomBookingRequestDocument.RoomType),
                RoomType.StudyRoom,
                Room.Auditorium305,
                Room.Auditorium307);

            schema.AddRule(
                nameof(RoomBookingRequestDocument.Room),
                nameof(RoomBookingRequestDocument.RoomType),
                RoomType.ConferenceHall,
                Room.ConferenceHall1,
                Room.ConferenceHall2);

            schema.AddRule(
                nameof(RoomBookingRequestDocument.Room),
                nameof(RoomBookingRequestDocument.Building),
                Building.Main,
                Room.Auditorium101,
                Room.Auditorium201,
                Room.AssemblyHall1);

            schema.AddRule(
                nameof(RoomBookingRequestDocument.Room),
                nameof(RoomBookingRequestDocument.Building),
                Building.Educational,
                Room.Auditorium305,
                Room.Auditorium307,
                Room.ConferenceHall1,
                Room.ConferenceHall2,
                Room.ComputerClass1,
                Room.ComputerClass2);

            schema.AddRule(
                nameof(RoomBookingRequestDocument.Room),
                nameof(RoomBookingRequestDocument.Building),
                Building.Laboratory,
                Room.Laboratory1,
                Room.Laboratory2);

            schema.AddRule(
                nameof(RoomBookingRequestDocument.EquipmentSet),
                nameof(RoomBookingRequestDocument.Room),
                Room.Auditorium101,
                EquipmentSet.Projector);

            schema.AddRule(
                nameof(RoomBookingRequestDocument.EquipmentSet),
                nameof(RoomBookingRequestDocument.Room),
                Room.Auditorium201,
                EquipmentSet.Projector);

            schema.AddRule(
                nameof(RoomBookingRequestDocument.EquipmentSet),
                nameof(RoomBookingRequestDocument.Room),
                Room.Auditorium305,
                EquipmentSet.NotRequired);

            schema.AddRule(
                nameof(RoomBookingRequestDocument.EquipmentSet),
                nameof(RoomBookingRequestDocument.Room),
                Room.Auditorium307,
                EquipmentSet.NotRequired);

            schema.AddRule(
                nameof(RoomBookingRequestDocument.EquipmentSet),
                nameof(RoomBookingRequestDocument.Room),
                Room.AssemblyHall1,
                EquipmentSet.ProjectorMicrophoneAndSpeakers);

            schema.AddRule(
                nameof(RoomBookingRequestDocument.EquipmentSet),
                nameof(RoomBookingRequestDocument.Room),
                Room.ConferenceHall1,
                EquipmentSet.ProjectorMicrophoneAndSpeakers);

            schema.AddRule(
                nameof(RoomBookingRequestDocument.EquipmentSet),
                nameof(RoomBookingRequestDocument.Room),
                Room.ConferenceHall2,
                EquipmentSet.ProjectorMicrophoneAndSpeakers);

            schema.AddRule(
                nameof(RoomBookingRequestDocument.EquipmentSet),
                nameof(RoomBookingRequestDocument.Room),
                Room.ComputerClass1,
                EquipmentSet.Computers);

            schema.AddRule(
                nameof(RoomBookingRequestDocument.EquipmentSet),
                nameof(RoomBookingRequestDocument.Room),
                Room.ComputerClass2,
                EquipmentSet.Computers);

            schema.AddRule(
                nameof(RoomBookingRequestDocument.EquipmentSet),
                nameof(RoomBookingRequestDocument.Room),
                Room.Laboratory1,
                EquipmentSet.ChemicalEquipment);

            schema.AddRule(
                nameof(RoomBookingRequestDocument.EquipmentSet),
                nameof(RoomBookingRequestDocument.Room),
                Room.Laboratory2,
                EquipmentSet.ChemicalEquipment);

            return schema;
        }

        private static ListOption CreateOption(string keyPart, string displayName)
        {
            return new ListOption(
                new ListOptionKey($"{KeyPrefix}.{keyPart}"),
                displayName);
        }
    }
}
