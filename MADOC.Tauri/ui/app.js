(() => {
  "use strict";

  const icons = {
    building:
      '<path d="M3 21h18"/><path d="M6 21V7l6-4 6 4v14"/><path d="M9 9h.01M15 9h.01M9 13h.01M15 13h.01M10 21v-4h4v4"/>',
    chevronRight:
      '<path d="m9 18 6-6-6-6"/>',
    contract:
      '<path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8Z"/><path d="M14 2v6h6M8 13h8M8 17h5"/>',
    graduation:
      '<path d="m2 10 10-5 10 5-10 5Z"/><path d="M6 12v5c3 2 9 2 12 0v-5M22 10v6"/>',
    pencil:
      '<path d="M12 20h9"/><path d="M16.5 3.5a2.1 2.1 0 0 1 3 3L8 18l-4 1 1-4Z"/>',
    plane:
      '<path d="M22 2 11 13"/><path d="m22 2-7 20-4-9-9-4Z"/>',
  };

  const templateMeta = {
    certificate_request: {
      category: "Справка",
      icon: "graduation",
      description: "Справка об обучении, доходах или составе семьи.",
    },
    student_application: {
      category: "Заявление",
      icon: "pencil",
      description: "Универсальное заявление студента по учебным вопросам.",
    },
    service_contract: {
      category: "Договор",
      icon: "contract",
      description: "Договор на оказание услуг с готовыми реквизитами.",
    },
    business_trip_request: {
      category: "Командировка",
      icon: "plane",
      description: "Заявка на служебную поездку и маршрут.",
    },
    room_booking_request: {
      category: "Помещение",
      icon: "building",
      description: "Бронирование аудитории для занятия или мероприятия.",
    },
  };

  const documentTypes = [
    {
      key: "certificate_request",
      displayName: "Заявка на справку",
    },
    {
      key: "student_application",
      displayName: "Заявление студента",
    },
    {
      key: "service_contract",
      displayName: "Договор оказания услуг",
    },
    {
      key: "business_trip_request",
      displayName: "Заявка на командировку",
    },
    {
      key: "room_booking_request",
      displayName: "Заявка на бронирование аудитории",
    },
  ];

  function icon(name, className = "") {
    return `<svg class="icon ${className}" viewBox="0 0 24 24" aria-hidden="true">${icons[name]}</svg>`;
  }

  function escapeHtml(value) {
    return String(value ?? "")
      .replaceAll("&", "&amp;")
      .replaceAll("<", "&lt;")
      .replaceAll(">", "&gt;")
      .replaceAll('"', "&quot;")
      .replaceAll("'", "&#039;");
  }

  function renderTemplateCards() {
    const grid = document.querySelector("#template-grid");
    if (!grid) {
      return;
    }

    grid.innerHTML = documentTypes
      .map((descriptor) => {
        const meta = templateMeta[descriptor.key];

        return `
          <button
            class="template-card"
            type="button"
            data-document-type="${escapeHtml(descriptor.key)}"
          >
            <span class="template-card-top">
              <span class="template-icon">${icon(meta.icon, "icon--large")}</span>
              <span class="template-badge">${escapeHtml(meta.category)}</span>
            </span>
            <h3>${escapeHtml(descriptor.displayName)}</h3>
            <p>${escapeHtml(meta.description)}</p>
            <span class="template-card-footer">
              Заполнить форму ${icon("chevronRight", "icon--small")}
            </span>
          </button>`;
      })
      .join("");
  }

  renderTemplateCards();
})();
