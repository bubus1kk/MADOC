<p align="center">
  <img src="./MADOC.Tauri/app-icon.svg" width="112" alt="Логотип MADOC" />
</p>

<h1 align="center">MADOC</h1>

<p align="center">
  <strong>Настольный конструктор документов с валидацией, HTML/PDF-генерацией и локальным архивом</strong>
</p>

<p align="center">
  <a href="https://dotnet.microsoft.com/"><img src="https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&amp;logo=dotnet&amp;logoColor=white" alt=".NET 10" /></a>
  <a href="https://tauri.app/"><img src="https://img.shields.io/badge/Tauri-2-24C8DB?style=for-the-badge&amp;logo=tauri&amp;logoColor=white" alt="Tauri 2" /></a>
  <a href="https://www.rust-lang.org/"><img src="https://img.shields.io/badge/Rust-2021-000000?style=for-the-badge&amp;logo=rust&amp;logoColor=white" alt="Rust 2021" /></a>
  <a href="https://www.microsoft.com/windows/"><img src="https://img.shields.io/badge/Windows-x64-0078D4?style=for-the-badge&amp;logo=windows11&amp;logoColor=white" alt="Windows x64" /></a>
</p>

---

MADOC помогает заполнять типовые формы, проверять данные до создания документа и сохранять готовый результат в HTML или PDF. Приложение работает в нативном окне Windows, формирует документы локально.

## Возможности

- Динамические формы на основе доменных моделей и атрибутов валидации.
- Зависимые списки и автоматическая фильтрация доступных значений.
- Предпросмотр печатной формы до сохранения.
- Создание документов в форматах **PDF** и **HTML**.
- Кастомный PDF-просмотрщик на базе локального PDF.js:
- Локальный архив с поиском и фильтрами.
- Self-contained Desktop Bridge — установленному приложению не требуется отдельный .NET Runtime.

## Доступные документы

| Документ | Ключ шаблона |
|---|---|
| Заявка на справку | `certificate_request` |
| Заявление студента | `student_application` |
| Договор оказания услуг | `service_contract` |
| Заявка на командировку | `business_trip_request` |
| Заявка на бронирование аудитории | `room_booking_request` |

## Архитектура

```mermaid
flowchart LR
    UI["Tauri UI<br/>JavaScript + CSS"] --> HOST["Rust host<br/>окно, файлы, PDF"]
    HOST <-->|"JSON Lines"| BRIDGE["Desktop Bridge<br/>.NET 10"]
    BRIDGE --> APP["Application<br/>сценарии и DTO"]
    APP --> DOMAIN["Domain Core<br/>модели и валидация"]
    APP --> PRINT["Printing.Html<br/>шаблоны и якоря"]
    PRINT --> HTML["Готовый HTML"]
    HTML --> HOST
    HOST --> EDGE["Microsoft Edge<br/>HTML → PDF"]
    HOST --> ARCHIVE["Локальный архив<br/>HTML / PDF"]
    ARCHIVE --> PDFJS["PDF.js viewer"]
```

| Проект | Ответственность |
|---|---|
| `MADOC.Domain.Core` | Документы, ограничения, списки и зависимости полей |
| `MADOC.Printing.Html` | Печатные якоря и подстановка данных в HTML |
| `MADOC.Application` | Формы, валидация и сценарий генерации документа |
| `MADOC.DesktopBridge` | JSONL-интерфейс между frontend и .NET |
| `MADOC.Tauri` | Desktop UI, файловое хранилище, PDF и упаковка приложения |
| `*.Tests` | Модульные и интеграционные тесты |

## Требования для разработки

- Windows x64.
- [.NET SDK 10](https://dotnet.microsoft.com/).
- Node.js и npm.
- Rust stable с MSVC toolchain.
- Microsoft C++ Build Tools, необходимые для Tauri.
- Microsoft Edge — используется для локального преобразования HTML в PDF.

## Быстрый старт

```powershell
git clone https://github.com/bubus1kk/MADOC.git
cd MADOC

dotnet restore MADOC.slnx

cd MADOC.Tauri
npm install
npm run dev
```

## Сборка установщика

```powershell
cd MADOC.Tauri
npm install
npm run build
```

## Тесты

```powershell
dotnet restore MADOC.slnx
dotnet test MADOC.slnx
```

Тестовые проекты покрывают доменные ограничения, зависимости списков, HTML-якоря, генерацию печатных форм и протокол Desktop Bridge.


## Хранение документов

Готовые файлы сохраняются в:

```text
%USERPROFILE%\Documents\MADOC\Печатные формы
```

Служебный индекс архива находится отдельно:

```text
%APPDATA%\ru.madoc.desktop.concept\print-form-index
```


## Как добавить новый тип документа

1. Создать модель документа в `MADOC.Domain.Core/Documents`.
2. Добавить ограничения и конфигурации зависимых списков.
3. Зарегистрировать тип в `DocumentTypeRegistry`.
4. Добавить HTML-шаблон и CSS в `MADOC.Application/Printing`.
5. Покрыть модель, валидацию и генерацию тестами.

Шаблоны используют печатные якоря вида:

```html
<span>{{ doc:certificate_request.full_name }}</span>
```

Во время генерации MADOC подставляет значения модели, форматирует даты, диапазоны, списки и вычисляемые поля.
