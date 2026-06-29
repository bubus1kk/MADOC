(() => {
  "use strict";

  const app = document.querySelector("#app");
  const toastRegion = document.querySelector("#toast-region");
  const params = new URLSearchParams(window.location.search);
  const invoke = window.__TAURI__?.core?.invoke;

  const icons = {
    arrowLeft:
      '<path d="m15 18-6-6 6-6"/><path d="M9 12h12"/>',
    arrowRight:
      '<path d="m9 18 6-6-6-6"/><path d="M15 12H3"/>',
    archive:
      '<path d="M21 8v13H3V8"/><path d="M1 3h22v5H1zM10 12h4"/>',
    building:
      '<path d="M3 21h18"/><path d="M6 21V7l6-4 6 4v14"/><path d="M9 9h.01M15 9h.01M9 13h.01M15 13h.01M10 21v-4h4v4"/>',
    calendar:
      '<path d="M8 2v4M16 2v4M3 10h18"/><rect x="3" y="4" width="18" height="18" rx="3"/><path d="M8 15h.01M12 15h.01M16 15h.01M8 18h.01M12 18h.01"/>',
    check:
      '<path d="m5 12 4 4L19 6"/>',
    clock:
      '<circle cx="12" cy="12" r="9"/><path d="M12 7v5l3 2"/>',
    chevronDown:
      '<path d="m6 9 6 6 6-6"/>',
    chevronLeft:
      '<path d="m15 18-6-6 6-6"/>',
    chevronRight:
      '<path d="m9 18 6-6-6-6"/>',
    close:
      '<path d="M18 6 6 18M6 6l12 12"/>',
    clipboard:
      '<rect x="5" y="4" width="14" height="18" rx="2"/><path d="M9 4.5A2.5 2.5 0 0 1 11.5 2h1A2.5 2.5 0 0 1 15 4.5V6H9Z"/><path d="M9 12h6M9 16h6"/>',
    contract:
      '<path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8Z"/><path d="M14 2v6h6M8 13h8M8 17h5"/>',
    copy:
      '<rect x="9" y="9" width="13" height="13" rx="2"/><path d="M5 15H4a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2h9a2 2 0 0 1 2 2v1"/>',
    file:
      '<path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8Z"/><path d="M14 2v6h6M8 13h8M8 17h5"/>',
    filter:
      '<path d="M4 5h16M7 12h10M10 19h4"/>',
    graduation:
      '<path d="m2 10 10-5 10 5-10 5Z"/><path d="M6 12v5c3 2 9 2 12 0v-5M22 10v6"/>',
    info:
      '<circle cx="12" cy="12" r="9"/><path d="M12 11v5M12 8h.01"/>',
    layers:
      '<path d="m12 2 9 5-9 5-9-5Z"/><path d="m3 12 9 5 9-5M3 17l9 5 9-5"/>',
    lock:
      '<rect x="4" y="10" width="16" height="11" rx="2"/><path d="M8 10V7a4 4 0 0 1 8 0v3"/>',
    minus:
      '<path d="M5 12h14"/>',
    pencil:
      '<path d="M12 20h9"/><path d="M16.5 3.5a2.1 2.1 0 0 1 3 3L8 18l-4 1 1-4Z"/>',
    plane:
      '<path d="M22 2 11 13"/><path d="m22 2-7 20-4-9-9-4Z"/>',
    plus:
      '<path d="M12 5v14M5 12h14"/>',
    printer:
      '<path d="M6 9V2h12v7"/><path d="M6 18H4a2 2 0 0 1-2-2v-5a2 2 0 0 1 2-2h16a2 2 0 0 1 2 2v5a2 2 0 0 1-2 2h-2"/><rect x="6" y="14" width="12" height="8" rx="1"/>',
    refresh:
      '<path d="M20 7h-5V2"/><path d="M20 7a9 9 0 1 0 1 8"/>',
    search:
      '<circle cx="11" cy="11" r="7"/><path d="m20 20-4-4"/>',
    shield:
      '<path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10Z"/><path d="m9 12 2 2 4-4"/>',
    sparkles:
      '<path d="m12 3-1.2 3.3L7.5 7.5l3.3 1.2L12 12l1.2-3.3 3.3-1.2-3.3-1.2Z"/><path d="m5 14-.8 2.2L2 17l2.2.8L5 20l.8-2.2L8 17l-2.2-.8Z"/><path d="m19 14-.7 1.8-1.8.7 1.8.7L19 19l.7-1.8 1.8-.7-1.8-.7Z"/>',
    user:
      '<circle cx="12" cy="8" r="4"/><path d="M4 22a8 8 0 0 1 16 0"/>',
    warning:
      '<path d="M10.3 2.9 1.8 17a2 2 0 0 0 1.7 3h17a2 2 0 0 0 1.7-3L13.7 2.9a2 2 0 0 0-3.4 0Z"/><path d="M12 9v4M12 17h.01"/>',
  };

  const templateMeta = {
    certificate_request: {
      category: "Справка",
      icon: "graduation",
      color: "#188ed8",
      wash: "#e8f6ff",
      description: "Справка об обучении, доходах или составе семьи.",
    },
    student_application: {
      category: "Заявление",
      icon: "pencil",
      color: "#4a9bd0",
      wash: "#edf7fd",
      description: "Универсальное заявление студента по учебным вопросам.",
    },
    service_contract: {
      category: "Договор",
      icon: "contract",
      color: "#7b79d8",
      wash: "#f0efff",
      description: "Договор на оказание услуг с готовыми реквизитами.",
    },
    business_trip_request: {
      category: "Командировка",
      icon: "plane",
      color: "#2d9d8a",
      wash: "#e9f8f5",
      description: "Заявка на служебную поездку и маршрут.",
    },
    room_booking_request: {
      category: "Помещение",
      icon: "building",
      color: "#d58a35",
      wash: "#fff5e8",
      description: "Бронирование аудитории для занятия или мероприятия.",
      wide: true,
    },
  };

  const demoDocumentTypes = [
    { key: "certificate_request", displayName: "Заявка на справку" },
    { key: "student_application", displayName: "Заявление студента" },
    { key: "service_contract", displayName: "Договор оказания услуг" },
    { key: "business_trip_request", displayName: "Заявка на командировку" },
    { key: "room_booking_request", displayName: "Заявка на бронирование аудитории" },
  ];

  const demoSchemas = Object.fromEntries(
    demoDocumentTypes.map((type) => [
      type.key,
      {
        documentType: type.key,
        displayName: type.displayName,
        fields: [
          textField("RequesterFullName", "ФИО заявителя", true, 150),
          textField("RequesterGroup", "Учебная группа", true, 30),
          listField("RequestPurpose", "Назначение", true, [
            { value: "study", displayName: "Учебный процесс" },
            { value: "work", displayName: "Работа" },
            { value: "other", displayName: "Другое" },
          ]),
          dateField("DesiredDate", "Желаемая дата", true),
          textField("Comment", "Комментарий", false, 300, true),
        ],
      },
    ]),
  );

  let documentTypes = [];
  let activeSchema = null;
  let activeDescriptor = null;
  let generatedHtml = "";
  let draftValues = {};

  function textField(name, displayName, isRequired, maxLength, isMultiline = false) {
    return {
      name,
      displayName,
      type: isMultiline ? "MultilineText" : "Text",
      isReadOnly: false,
      constraints: { isRequired, maxLength, isMultiline },
      options: [],
    };
  }

  function dateField(name, displayName, isRequired) {
    return {
      name,
      displayName,
      type: "Date",
      isReadOnly: false,
      constraints: { isRequired },
      options: [],
    };
  }

  function listField(name, displayName, isRequired, options) {
    return {
      name,
      displayName,
      type: "List",
      isReadOnly: false,
      constraints: { isRequired },
      options,
    };
  }

  function icon(name, className = "") {
    return `<svg class="icon ${className}" viewBox="0 0 24 24" aria-hidden="true">${icons[name] ?? icons.file}</svg>`;
  }

  function escapeHtml(value) {
    return String(value ?? "")
      .replaceAll("&", "&amp;")
      .replaceAll("<", "&lt;")
      .replaceAll(">", "&gt;")
      .replaceAll('"', "&quot;")
      .replaceAll("'", "&#039;");
  }

  function enhanceSelects(root = document) {
    root
      .querySelectorAll("select:not([data-custom-select-ready])")
      .forEach(enhanceSelect);
  }

  function enhanceSelect(select) {
    select.dataset.customSelectReady = "true";
    select.classList.add("custom-select-native");
    select.tabIndex = -1;
    select.setAttribute("aria-hidden", "true");

    const wrapper = document.createElement("div");
    wrapper.className = "custom-select";
    wrapper.dataset.customSelect = "";

    const trigger = document.createElement("button");
    trigger.type = "button";
    trigger.className = "custom-select-trigger";
    trigger.id = `${select.id || `custom-select-${Math.random().toString(36).slice(2)}`}-trigger`;
    trigger.setAttribute("aria-haspopup", "listbox");
    trigger.setAttribute("aria-expanded", "false");

    const menu = document.createElement("div");
    menu.className = "custom-select-menu";
    menu.id = `${trigger.id}-menu`;
    menu.setAttribute("role", "listbox");
    menu.hidden = true;
    trigger.setAttribute("aria-controls", menu.id);

    const linkedLabel = select.id
      ? document.querySelector(`label[for="${CSS.escape(select.id)}"]`)
      : null;
    if (linkedLabel) {
      linkedLabel.htmlFor = trigger.id;
      trigger.setAttribute("aria-label", linkedLabel.textContent.trim());
    } else {
      trigger.setAttribute(
        "aria-label",
        select.getAttribute("aria-label") || "Выберите значение",
      );
    }

    select.before(wrapper);
    wrapper.append(select, trigger, menu);
    refreshCustomSelect(select);

    trigger.addEventListener("click", () => {
      if (select.disabled) return;
      const willOpen = !wrapper.classList.contains("custom-select--open");
      closeCustomSelects(wrapper);
      setCustomSelectOpen(wrapper, willOpen);
    });

    trigger.addEventListener("keydown", (event) => {
      if (!["ArrowDown", "Enter", " "].includes(event.key) || select.disabled) return;
      event.preventDefault();
      closeCustomSelects(wrapper);
      setCustomSelectOpen(wrapper, true);
      menu.querySelector(".custom-select-option--selected:not(:disabled)")?.focus();
    });

    menu.addEventListener("click", (event) => {
      const optionButton = event.target.closest("[data-option-index]");
      if (!optionButton || optionButton.disabled) return;
      select.selectedIndex = Number(optionButton.dataset.optionIndex);
      select.dispatchEvent(new Event("change", { bubbles: true }));
      refreshCustomSelect(select);
      setCustomSelectOpen(wrapper, false);
      trigger.focus();
    });

    menu.addEventListener("keydown", (event) => {
      if (event.key !== "Escape") return;
      event.preventDefault();
      setCustomSelectOpen(wrapper, false);
      trigger.focus();
    });

    select.addEventListener("change", () => refreshCustomSelect(select));
    new MutationObserver(() => refreshCustomSelect(select)).observe(select, {
      attributes: true,
      childList: true,
      subtree: true,
      attributeFilter: ["disabled"],
    });
  }

  function refreshCustomSelect(select) {
    const wrapper = select.closest("[data-custom-select]");
    if (!wrapper) return;
    const trigger = wrapper.querySelector(".custom-select-trigger");
    const menu = wrapper.querySelector(".custom-select-menu");
    const selectedOption = select.options[select.selectedIndex] ?? select.options[0];
    const showSelectionIcon = !["archive-type", "archive-date"].includes(select.id);

    trigger.disabled = select.disabled;
    wrapper.classList.toggle("custom-select--disabled", select.disabled);
    trigger.innerHTML = `
      <span>${escapeHtml(selectedOption?.textContent?.trim() || "Выберите значение")}</span>
      ${icon("chevronDown", "icon--small")}`;
    menu.innerHTML = Array.from(select.options)
      .map(
        (option, index) => `
          <button
            type="button"
            class="custom-select-option ${index === select.selectedIndex ? "custom-select-option--selected" : ""}"
            role="option"
            aria-selected="${index === select.selectedIndex}"
            data-option-index="${index}"
            ${option.disabled ? "disabled" : ""}
          >
            <span>${escapeHtml(option.textContent.trim())}</span>
            ${showSelectionIcon && index === select.selectedIndex ? icon("check", "icon--small") : ""}
          </button>`,
      )
      .join("");
  }

  function refreshAllCustomSelects(root = document) {
    root
      .querySelectorAll("select[data-custom-select-ready]")
      .forEach(refreshCustomSelect);
  }

  function setCustomSelectOpen(wrapper, isOpen) {
    const trigger = wrapper.querySelector(".custom-select-trigger");
    const menu = wrapper.querySelector(".custom-select-menu");
    wrapper.classList.toggle("custom-select--open", isOpen);
    trigger.setAttribute("aria-expanded", String(isOpen));
    menu.hidden = !isOpen;
  }

  function closeCustomSelects(except = null) {
    document.querySelectorAll(".custom-select--open").forEach((wrapper) => {
      if (wrapper !== except) setCustomSelectOpen(wrapper, false);
    });
  }

  function enhanceDateInputs(root = document) {
    root
      .querySelectorAll('input[type="date"]:not([data-custom-calendar-ready])')
      .forEach(enhanceDateInput);
  }

  function enhanceDateInput(input) {
    input.dataset.customCalendarReady = "true";
    input.classList.add("custom-date-native");
    input.tabIndex = -1;
    input.setAttribute("aria-hidden", "true");

    const wrapper = document.createElement("div");
    wrapper.className = "custom-calendar";
    wrapper.dataset.customCalendar = "";

    const trigger = document.createElement("button");
    trigger.type = "button";
    trigger.className = "custom-calendar-trigger";
    trigger.id = `${input.id || `custom-date-${Math.random().toString(36).slice(2)}`}-trigger`;
    trigger.setAttribute("aria-haspopup", "dialog");
    trigger.setAttribute("aria-expanded", "false");

    const panel = document.createElement("div");
    panel.className = "custom-calendar-panel";
    panel.setAttribute("role", "dialog");
    panel.setAttribute("aria-modal", "false");
    panel.setAttribute("aria-label", "Выбор даты");
    panel.hidden = true;

    const linkedLabel = input.id
      ? document.querySelector(`label[for="${CSS.escape(input.id)}"]`)
      : null;
    if (linkedLabel) {
      linkedLabel.htmlFor = trigger.id;
      trigger.setAttribute("aria-label", linkedLabel.textContent.trim());
    } else {
      trigger.setAttribute(
        "aria-label",
        input.getAttribute("aria-label") || "Выберите дату",
      );
    }

    let visibleMonth = monthStart(parseIsoDate(input.value) ?? new Date());
    const render = () => {
      panel.innerHTML = calendarPanelMarkup(input, visibleMonth);
      refreshCustomDateInput(input);
    };

    input.before(wrapper);
    wrapper.append(input, trigger, panel);
    render();

    trigger.addEventListener("click", () => {
      if (input.disabled) return;
      const willOpen = !wrapper.classList.contains("custom-calendar--open");
      closeCustomCalendars(wrapper);
      if (willOpen) {
        visibleMonth = monthStart(parseIsoDate(input.value) ?? new Date());
        render();
      }
      setCustomCalendarOpen(wrapper, willOpen);
    });

    panel.addEventListener("click", (event) => {
      const action = event.target.closest("[data-calendar-action]")?.dataset.calendarAction;
      const dateValue = event.target.closest("[data-calendar-date]")?.dataset.calendarDate;

      if (dateValue) {
        setCustomDateValue(input, dateValue);
        setCustomCalendarOpen(wrapper, false);
        trigger.focus();
        return;
      }

      if (action === "previous") {
        visibleMonth = new Date(visibleMonth.getFullYear(), visibleMonth.getMonth() - 1, 1);
        render();
      } else if (action === "next") {
        visibleMonth = new Date(visibleMonth.getFullYear(), visibleMonth.getMonth() + 1, 1);
        render();
      } else if (action === "today") {
        const today = toIsoDate(new Date());
        if (isDateAllowed(input, today)) {
          setCustomDateValue(input, today);
          setCustomCalendarOpen(wrapper, false);
          trigger.focus();
        }
      } else if (action === "clear") {
        setCustomDateValue(input, "");
        setCustomCalendarOpen(wrapper, false);
        trigger.focus();
      }
    });

    panel.addEventListener("keydown", (event) => {
      if (event.key !== "Escape") return;
      event.preventDefault();
      setCustomCalendarOpen(wrapper, false);
      trigger.focus();
    });

    input.addEventListener("change", () => refreshCustomDateInput(input));
    new MutationObserver(() => {
      refreshCustomDateInput(input);
      render();
    }).observe(input, {
      attributes: true,
      attributeFilter: ["disabled", "min", "max"],
    });
  }

  function calendarPanelMarkup(input, visibleMonth) {
    const year = visibleMonth.getFullYear();
    const month = visibleMonth.getMonth();
    const firstDay = new Date(year, month, 1);
    const firstWeekday = (firstDay.getDay() + 6) % 7;
    const gridStart = new Date(year, month, 1 - firstWeekday);
    const selectedDate = input.value;
    const today = toIsoDate(new Date());
    const monthTitle = new Intl.DateTimeFormat("ru-RU", {
      month: "long",
      year: "numeric",
    }).format(visibleMonth);
    const weekdays = ["Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс"];

    const days = Array.from({ length: 42 }, (_, index) => {
      const date = new Date(
        gridStart.getFullYear(),
        gridStart.getMonth(),
        gridStart.getDate() + index,
      );
      const value = toIsoDate(date);
      const isOutside = date.getMonth() !== month;
      const isSelected = value === selectedDate;
      const isToday = value === today;
      return `
        <button
          type="button"
          class="custom-calendar-day ${isOutside ? "custom-calendar-day--outside" : ""} ${isSelected ? "custom-calendar-day--selected" : ""} ${isToday ? "custom-calendar-day--today" : ""}"
          data-calendar-date="${value}"
          aria-label="${escapeHtml(formatLongDate(value))}"
          aria-pressed="${isSelected}"
          ${isDateAllowed(input, value) ? "" : "disabled"}
        >${date.getDate()}</button>`;
    }).join("");

    return `
      <div class="custom-calendar-header">
        <strong>${escapeHtml(monthTitle)}</strong>
        <span class="custom-calendar-navigation">
          <button type="button" data-calendar-action="previous" aria-label="Предыдущий месяц">
            ${icon("chevronLeft", "icon--small")}
          </button>
          <button type="button" data-calendar-action="next" aria-label="Следующий месяц">
            ${icon("chevronRight", "icon--small")}
          </button>
        </span>
      </div>
      <div class="custom-calendar-weekdays">
        ${weekdays.map((weekday) => `<span>${weekday}</span>`).join("")}
      </div>
      <div class="custom-calendar-grid">${days}</div>
      <div class="custom-calendar-footer">
        <button type="button" data-calendar-action="clear">Очистить</button>
        <button type="button" data-calendar-action="today">Сегодня</button>
      </div>`;
  }

  function refreshCustomDateInput(input) {
    const wrapper = input.closest("[data-custom-calendar]");
    if (!wrapper) return;
    const trigger = wrapper.querySelector(".custom-calendar-trigger");
    trigger.disabled = input.disabled;
    wrapper.classList.toggle("custom-calendar--disabled", input.disabled);
    trigger.innerHTML = `
      <span>${input.value ? formatDateValue(input.value) : "ДД.ММ.ГГГГ"}</span>
      ${icon("calendar", "icon--small")}`;
  }

  function refreshAllCustomCalendars(root = document) {
    root
      .querySelectorAll('input[type="date"][data-custom-calendar-ready]')
      .forEach(refreshCustomDateInput);
  }

  function setCustomCalendarOpen(wrapper, isOpen) {
    const trigger = wrapper.querySelector(".custom-calendar-trigger");
    const panel = wrapper.querySelector(".custom-calendar-panel");
    wrapper.classList.toggle("custom-calendar--open", isOpen);
    trigger.setAttribute("aria-expanded", String(isOpen));
    panel.hidden = !isOpen;
    if (isOpen) {
      window.requestAnimationFrame(() => positionCustomCalendar(wrapper));
    } else {
      panel.style.removeProperty("top");
      panel.style.removeProperty("left");
    }
  }

  function positionCustomCalendar(wrapper) {
    const trigger = wrapper.querySelector(".custom-calendar-trigger");
    const panel = wrapper.querySelector(".custom-calendar-panel");
    if (panel.hidden) return;

    const margin = 12;
    const gap = 8;
    const triggerRect = trigger.getBoundingClientRect();
    const panelRect = panel.getBoundingClientRect();
    const left = Math.min(
      Math.max(margin, triggerRect.left),
      window.innerWidth - panelRect.width - margin,
    );
    const below = triggerRect.bottom + gap;
    const above = triggerRect.top - panelRect.height - gap;
    const top =
      below + panelRect.height <= window.innerHeight - margin
        ? below
        : Math.max(margin, above);

    panel.style.left = `${left}px`;
    panel.style.top = `${top}px`;
  }

  function closeCustomCalendars(except = null) {
    document.querySelectorAll(".custom-calendar--open").forEach((wrapper) => {
      if (wrapper !== except) setCustomCalendarOpen(wrapper, false);
    });
  }

  function setCustomDateValue(input, value) {
    input.value = value;
    refreshCustomDateInput(input);
    input.dispatchEvent(new Event("change", { bubbles: true }));
  }

  function parseIsoDate(value) {
    const match = String(value || "").match(/^(\d{4})-(\d{2})-(\d{2})$/);
    if (!match) return null;
    return new Date(Number(match[1]), Number(match[2]) - 1, Number(match[3]));
  }

  function monthStart(date) {
    return new Date(date.getFullYear(), date.getMonth(), 1);
  }

  function toIsoDate(date) {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, "0");
    const day = String(date.getDate()).padStart(2, "0");
    return `${year}-${month}-${day}`;
  }

  function formatDateValue(value) {
    const date = parseIsoDate(value);
    if (!date) return value;
    return new Intl.DateTimeFormat("ru-RU").format(date);
  }

  function formatLongDate(value) {
    const date = parseIsoDate(value);
    if (!date) return value;
    return new Intl.DateTimeFormat("ru-RU", {
      day: "numeric",
      month: "long",
      year: "numeric",
    }).format(date);
  }

  function isDateAllowed(input, value) {
    return (!input.min || value >= input.min) && (!input.max || value <= input.max);
  }

  function enhanceTimeInputs(root = document) {
    root
      .querySelectorAll('input[type="time"]:not([data-custom-time-ready])')
      .forEach(enhanceTimeInput);
  }

  function enhanceTimeInput(input) {
    input.dataset.customTimeReady = "true";
    input.classList.add("custom-time-native");
    input.tabIndex = -1;
    input.setAttribute("aria-hidden", "true");

    const wrapper = document.createElement("div");
    wrapper.className = "custom-time";
    wrapper.dataset.customTime = "";

    const trigger = document.createElement("button");
    trigger.type = "button";
    trigger.className = "custom-time-trigger";
    trigger.id = `${input.id || `custom-time-${Math.random().toString(36).slice(2)}`}-trigger`;
    trigger.setAttribute("aria-haspopup", "dialog");
    trigger.setAttribute("aria-expanded", "false");

    const panel = document.createElement("div");
    panel.className = "custom-time-panel";
    panel.setAttribute("role", "dialog");
    panel.setAttribute("aria-modal", "false");
    panel.setAttribute("aria-label", "Выбор времени");
    panel.hidden = true;

    const linkedLabel = input.id
      ? document.querySelector(`label[for="${CSS.escape(input.id)}"]`)
      : null;
    if (linkedLabel) {
      linkedLabel.htmlFor = trigger.id;
      trigger.setAttribute("aria-label", linkedLabel.textContent.trim());
    } else {
      trigger.setAttribute(
        "aria-label",
        input.getAttribute("aria-label") || "Выберите время",
      );
    }

    const now = new Date();
    let draftHour = String(now.getHours()).padStart(2, "0");
    let draftMinute = String(now.getMinutes()).padStart(2, "0");

    const resetDraft = () => {
      const current = parseTimeValue(input.value);
      const currentTime = new Date();
      draftHour = current?.hour ?? String(currentTime.getHours()).padStart(2, "0");
      draftMinute =
        current?.minute ?? String(currentTime.getMinutes()).padStart(2, "0");
    };
    const render = () => {
      panel.innerHTML = timePanelMarkup(input, draftHour, draftMinute);
      refreshCustomTimeInput(input);
      window.requestAnimationFrame(() => scrollCustomTimeSelections(panel));
    };

    input.before(wrapper);
    wrapper.append(input, trigger, panel);
    resetDraft();
    render();

    trigger.addEventListener("click", () => {
      if (input.disabled) return;
      const willOpen = !wrapper.classList.contains("custom-time--open");
      closeCustomTimePickers(wrapper);
      if (willOpen) {
        resetDraft();
        render();
      }
      setCustomTimeOpen(wrapper, willOpen);
    });

    panel.addEventListener("click", (event) => {
      const hour = event.target.closest("[data-time-hour]")?.dataset.timeHour;
      const minute = event.target.closest("[data-time-minute]")?.dataset.timeMinute;
      const action = event.target.closest("[data-time-action]")?.dataset.timeAction;

      if (hour != null) {
        draftHour = hour;
        render();
      } else if (minute != null) {
        draftMinute = minute;
        render();
      } else if (action === "clear") {
        setCustomTimeValue(input, "");
        setCustomTimeOpen(wrapper, false);
        trigger.focus();
      } else if (action === "now") {
        const current = new Date();
        const value = `${String(current.getHours()).padStart(2, "0")}:${String(
          current.getMinutes(),
        ).padStart(2, "0")}`;
        if (isTimeAllowed(input, value)) {
          setCustomTimeValue(input, value);
          setCustomTimeOpen(wrapper, false);
          trigger.focus();
        }
      } else if (action === "apply") {
        const value = `${draftHour}:${draftMinute}`;
        if (isTimeAllowed(input, value)) {
          setCustomTimeValue(input, value);
          setCustomTimeOpen(wrapper, false);
          trigger.focus();
        }
      }
    });

    panel.addEventListener("keydown", (event) => {
      if (event.key !== "Escape") return;
      event.preventDefault();
      setCustomTimeOpen(wrapper, false);
      trigger.focus();
    });

    input.addEventListener("change", () => refreshCustomTimeInput(input));
    new MutationObserver(() => {
      refreshCustomTimeInput(input);
      render();
    }).observe(input, {
      attributes: true,
      attributeFilter: ["disabled", "min", "max"],
    });
  }

  function timePanelMarkup(input, selectedHour, selectedMinute) {
    const hours = Array.from({ length: 24 }, (_, index) =>
      String(index).padStart(2, "0"),
    );
    const minutes = Array.from({ length: 60 }, (_, index) =>
      String(index).padStart(2, "0"),
    );

    return `
      <div class="custom-time-header">
        <strong>Выберите время</strong>
        <span>${selectedHour}:${selectedMinute}</span>
      </div>
      <div class="custom-time-columns">
        <div class="custom-time-group">
          <span>Часы</span>
          <div class="custom-time-list" role="listbox" aria-label="Часы">
            ${hours
              .map((hour) => {
                const isSelected = hour === selectedHour;
                const isAllowed = minutes.some((minute) =>
                  isTimeAllowed(input, `${hour}:${minute}`),
                );
                return `
                  <button
                    type="button"
                    class="custom-time-option ${isSelected ? "custom-time-option--selected" : ""}"
                    role="option"
                    aria-selected="${isSelected}"
                    data-time-hour="${hour}"
                    ${isAllowed ? "" : "disabled"}
                  >${hour}</button>`;
              })
              .join("")}
          </div>
        </div>
        <div class="custom-time-group">
          <span>Минуты</span>
          <div class="custom-time-list" role="listbox" aria-label="Минуты">
            ${minutes
              .map((minute) => {
                const isSelected = minute === selectedMinute;
                return `
                  <button
                    type="button"
                    class="custom-time-option ${isSelected ? "custom-time-option--selected" : ""}"
                    role="option"
                    aria-selected="${isSelected}"
                    data-time-minute="${minute}"
                    ${isTimeAllowed(input, `${selectedHour}:${minute}`) ? "" : "disabled"}
                  >${minute}</button>`;
              })
              .join("")}
          </div>
        </div>
      </div>
      <div class="custom-time-footer">
        <button type="button" data-time-action="clear">Очистить</button>
        <button type="button" data-time-action="now">Сейчас</button>
        <button type="button" class="custom-time-apply" data-time-action="apply">Готово</button>
      </div>`;
  }

  function refreshCustomTimeInput(input) {
    const wrapper = input.closest("[data-custom-time]");
    if (!wrapper) return;
    const trigger = wrapper.querySelector(".custom-time-trigger");
    trigger.disabled = input.disabled;
    wrapper.classList.toggle("custom-time--disabled", input.disabled);
    trigger.innerHTML = `
      <span>${input.value ? input.value.slice(0, 5) : "ЧЧ:ММ"}</span>
      ${icon("clock", "icon--small")}`;
  }

  function refreshAllCustomTimeInputs(root = document) {
    root
      .querySelectorAll('input[type="time"][data-custom-time-ready]')
      .forEach(refreshCustomTimeInput);
  }

  function scrollCustomTimeSelections(panel) {
    panel.querySelectorAll(".custom-time-list").forEach((list) => {
      const selected = list.querySelector(".custom-time-option--selected");
      if (!selected) return;
      list.scrollTop =
        selected.offsetTop - list.clientHeight / 2 + selected.offsetHeight / 2;
    });
  }

  function setCustomTimeOpen(wrapper, isOpen) {
    const trigger = wrapper.querySelector(".custom-time-trigger");
    const panel = wrapper.querySelector(".custom-time-panel");
    wrapper.classList.toggle("custom-time--open", isOpen);
    trigger.setAttribute("aria-expanded", String(isOpen));
    panel.hidden = !isOpen;
    if (isOpen) {
      window.requestAnimationFrame(() => positionCustomTimePicker(wrapper));
    } else {
      panel.style.removeProperty("top");
      panel.style.removeProperty("left");
    }
  }

  function positionCustomTimePicker(wrapper) {
    const trigger = wrapper.querySelector(".custom-time-trigger");
    const panel = wrapper.querySelector(".custom-time-panel");
    if (panel.hidden) return;

    const margin = 12;
    const gap = 8;
    const triggerRect = trigger.getBoundingClientRect();
    const panelRect = panel.getBoundingClientRect();
    const left = Math.min(
      Math.max(margin, triggerRect.left),
      window.innerWidth - panelRect.width - margin,
    );
    const below = triggerRect.bottom + gap;
    const above = triggerRect.top - panelRect.height - gap;
    const top =
      below + panelRect.height <= window.innerHeight - margin
        ? below
        : Math.max(margin, above);

    panel.style.left = `${left}px`;
    panel.style.top = `${top}px`;
  }

  function closeCustomTimePickers(except = null) {
    document.querySelectorAll(".custom-time--open").forEach((wrapper) => {
      if (wrapper !== except) setCustomTimeOpen(wrapper, false);
    });
  }

  function setCustomTimeValue(input, value) {
    input.value = value;
    refreshCustomTimeInput(input);
    input.dispatchEvent(new Event("change", { bubbles: true }));
  }

  function parseTimeValue(value) {
    const match = String(value || "").match(/^(\d{2}):(\d{2})/);
    if (!match) return null;
    return { hour: match[1], minute: match[2] };
  }

  function isTimeAllowed(input, value) {
    const min = String(input.min || "").slice(0, 5);
    const max = String(input.max || "").slice(0, 5);
    return (!min || value >= min) && (!max || value <= max);
  }

  document.addEventListener("pointerdown", (event) => {
    if (!event.target.closest("[data-custom-select]")) closeCustomSelects();
    if (!event.target.closest("[data-custom-calendar]")) closeCustomCalendars();
    if (!event.target.closest("[data-custom-time]")) closeCustomTimePickers();
  });

  document.addEventListener("keydown", (event) => {
    if (event.key === "Escape") {
      closeCustomSelects();
      closeCustomCalendars();
      closeCustomTimePickers();
    }
  });

  function resetPageScroll() {
    window.scrollTo(0, 0);
    window.setTimeout(() => window.scrollTo(0, 0), 0);
  }

  async function bridge(command, payload = {}) {
    if (!invoke) {
      return demoBridge(command, payload);
    }

    return invoke("bridge_request", { command, payload });
  }

  async function demoBridge(command, payload = {}) {
    if (command === "getDocumentTypes") {
      return { documentTypes: demoDocumentTypes };
    }

    if (command === "getDocumentSchema") {
      return demoSchemas[payload.documentType] ?? demoSchemas.certificate_request;
    }

    if (command === "getFieldOptions") {
      return { isSuccess: true, options: [], errors: [] };
    }

    if (command === "validateDocument") {
      return { isValid: true, errors: [] };
    }

    if (command === "generatePrintHtml") {
      return {
        isSuccess: true,
        htmlContent: createDemoPrintHtml(payload.values ?? {}),
        errors: [],
      };
    }

    throw new Error(`Команда ${command} недоступна.`);
  }

  function brandMarkup(showCaption = false) {
    return `
      <span class="brand" aria-label="MADOC">
        <span class="brand-mark" aria-hidden="true"><span></span><span></span><span></span></span>
        <span class="brand-copy">
          <span class="brand-name">MADOC</span>
          ${showCaption ? '<span class="brand-caption">Новая визуальная концепция</span>' : ""}
        </span>
      </span>`;
  }

  function appHeaderMarkup(activeView = "templates") {
    return `
      <header class="app-header">
        ${brandMarkup()}
        <nav class="app-nav" aria-label="Основная навигация">
          <button class="nav-item ${activeView === "templates" ? "nav-item--active" : ""}" type="button" data-app-view="templates">Шаблоны</button>
          <button class="nav-item ${activeView === "print-forms" ? "nav-item--active" : ""}" type="button" data-app-view="print-forms">Печатные формы</button>
        </nav>
      </header>`;
  }

  function bindHomeLinks() {
    document.querySelectorAll("[data-app-view]").forEach((button) => {
      button.addEventListener("click", () => {
        if (button.dataset.appView === "print-forms") {
          return;
        }

        navigateHome(true);
      });
    });
  }

  async function initHome() {
    try {
      const response = await bridge("getDocumentTypes");
      documentTypes = response.documentTypes ?? demoDocumentTypes;
      renderHome();
    } catch (error) {
      renderFatalError("Шаблоны не загрузились", error);
    }
  }

  function renderHome() {
    activeSchema = null;
    activeDescriptor = null;
    document.title = "MADOC Concept - Документы";
    app.innerHTML = `
      <div class="app-shell">
        ${appHeaderMarkup("templates")}
        <main class="home-content">
          <section class="hero">
            <p class="eyebrow">Конструктор документов</p>
            <h1>Какой документ <span>подготовить?</span></h1>
            <p class="hero-description">
              Выберите шаблон, заполните данные и получите готовую печатную форму.
              Всё работает локально на этом компьютере.
            </p>
          </section>

          <section class="catalog-panel" aria-labelledby="catalog-title">
            <div class="catalog-toolbar">
              <div class="catalog-heading">
                <h2 id="catalog-title">Шаблоны документов</h2>
                <p>Выберите форму для заполнения</p>
              </div>
            </div>
            <div class="template-grid" id="template-grid"></div>
          </section>

          <p class="privacy-note">${icon("shield", "icon--small")} Данные не покидают устройство и не отправляются в интернет</p>
        </main>
      </div>`;

    bindHomeLinks();
    renderTemplateCards();
  }

  function renderTemplateCards() {
    const grid = document.querySelector("#template-grid");
    grid.innerHTML = documentTypes
      .map((descriptor) => {
        const meta = documentMeta(descriptor.key);
        return `
          <button class="template-card ${meta.wide ? "template-card--wide" : ""}" type="button" data-document-type="${escapeHtml(descriptor.key)}">
            <span class="template-card-top">
              <span class="template-icon">${icon(meta.icon, "icon--large")}</span>
              <span class="template-badge">${escapeHtml(meta.category)}</span>
            </span>
            <h3>${escapeHtml(descriptor.displayName)}</h3>
            <p>${escapeHtml(meta.description)}</p>
            <span class="template-card-footer">Заполнить форму ${icon("chevronRight", "icon--small")}</span>
          </button>`;
      })
      .join("");

    grid.querySelectorAll("[data-document-type]").forEach((card) => {
      card.addEventListener("click", () => openDocument(card.dataset.documentType));
    });
  }

  function documentMeta(documentType = activeDescriptor?.key) {
    return (
      templateMeta[documentType] ?? {
        category: "Документ",
        icon: "file",
        description: "Готовая форма документа.",
      }
    );
  }

  async function openDocument(documentType) {
    const card = document.querySelector(`[data-document-type="${CSS.escape(documentType)}"]`);
    try {
      setCardBusy(card, true);
      await initDocument(documentType);
      const nextParams = new URLSearchParams();
      nextParams.set("document", documentType);
      window.history.pushState({ documentType }, "", `${window.location.pathname}?${nextParams}`);
    } catch (error) {
      setCardBusy(card, false);
      showToast("Не удалось открыть форму", error?.message ?? error, "error");
    }
  }

  function setCardBusy(card, busy) {
    if (!card) return;
    card.disabled = busy;
    const footer = card.querySelector(".template-card-footer");
    if (footer) {
      footer.innerHTML = busy
        ? "Открываем форму..."
        : `Заполнить форму ${icon("chevronRight", "icon--small")}`;
    }
  }

  async function initDocument(documentType) {
    const [typesResponse, schema] = await Promise.all([
      documentTypes.length ? Promise.resolve({ documentTypes }) : bridge("getDocumentTypes"),
      bridge("getDocumentSchema", { documentType }),
    ]);

    documentTypes = typesResponse.documentTypes ?? documentTypes;
    activeSchema = schema;
    activeDescriptor =
      documentTypes.find((item) => item.key === documentType) ?? {
        key: documentType,
        displayName: schema.displayName,
      };
    document.title = `${schema.displayName} — MADOC Concept`;
    generatedHtml = "";
    draftValues = {};
    renderDocumentForm();
  }

  function editableFields() {
    return (activeSchema?.fields ?? []).filter((field) => !field.isReadOnly);
  }

  function sidebarMarkup(activeStep = 1) {
    const meta = documentMeta();
    const steps = [
      ["Заполнение", "Введите исходные данные"],
      ["Предпросмотр", "Проверьте печатную форму"],
      ["Печать", "Распечатайте HTML-документ"],
    ];

    return `
      <aside class="form-sidebar">
        ${brandMarkup()}
        <button class="sidebar-back" id="close-document" type="button">${icon("arrowLeft", "icon--small")} Ко всем шаблонам</button>
        <div class="document-identity">
          <span class="template-icon" style="color:${meta.color};background:${meta.wash}">${icon(meta.icon)}</span>
          <div>
            <small>${escapeHtml(meta.category)}</small>
            <h1>${escapeHtml(activeSchema.displayName)}</h1>
            <p>${editableFields().length} полей для заполнения</p>
          </div>
        </div>
        <div class="steps">
          ${steps
            .map(([title, description], index) => {
              const stepNumber = index + 1;
              const status = stepNumber === activeStep ? "step--active" : stepNumber < activeStep ? "step--complete" : "";
              return `
                <div class="step ${status}" ${stepNumber === 2 ? 'id="validation-step"' : ""}>
                  <span class="step-number">${stepNumber < activeStep ? icon("check", "icon--small") : stepNumber}</span>
                  <span><strong>${title}</strong><span>${description}</span></span>
                </div>`;
            })
            .join("")}
        </div>
        <div class="sidebar-note">
          <strong>${icon("shield", "icon--small")} Локальная обработка</strong>
          Данные формы остаются на вашем компьютере.
        </div>
      </aside>`;
  }

  function toolbarMarkup(mode = "form") {
    return `
      <header class="form-toolbar">
        <div class="breadcrumb">
          <span>Шаблоны</span>${icon("chevronRight", "icon--small")}
          <strong>${escapeHtml(activeSchema.displayName)}</strong>
        </div>
        ${
          mode === "preview"
            ? '<span class="toolbar-state toolbar-state--ready">Форма сформирована</span>'
            : ""
        }
      </header>`;
  }

  function renderDocumentForm() {
    app.innerHTML = `
      <div class="form-shell">
        ${sidebarMarkup()}
        <main class="form-workspace">
          ${toolbarMarkup()}
          <div class="workspace-content">
            <div class="workspace-heading">
              <div>
                <p class="eyebrow">Заполнение документа</p>
                <h2>Заполните данные</h2>
                <p>Обязательные поля отмечены звёздочкой. После заполнения можно проверить данные правилами документа.</p>
              </div>
              <div class="progress-ring">
                <span class="progress-ring-visual" id="progress-ring"></span>
                <span><strong id="progress-value">0% заполнено</strong><span>Прогресс формы</span></span>
              </div>
            </div>

            <form class="form-card" id="document-form" novalidate>
              <section class="form-section">
                <div class="section-heading">
                  <span class="section-icon">${icon("clipboard")}</span>
                  <div>
                    <h3>Данные документа</h3>
                    <p>Поля построены по схеме выбранного шаблона</p>
                  </div>
                </div>
                <div class="fields-grid">
                  ${editableFields().map(renderField).join("")}
                </div>
              </section>

              <div class="form-message" id="form-message">
                ${icon("warning", "icon--small")}
                <span>Проверьте отмеченные поля.</span>
              </div>

              <footer class="form-actions">
                <button class="ghost-button" type="button" id="reset-form">${icon("refresh", "icon--small")} Очистить</button>
                <button class="primary-button" type="submit" id="preview-button">
                  Предпросмотр ${icon("arrowRight", "icon--small")}
                </button>
              </footer>
            </form>
          </div>
        </main>
      </div>`;

    document.querySelector("#close-document").addEventListener("click", () => navigateHome(true));
    enhanceSelects(document);
    enhanceDateInputs(document);
    enhanceTimeInputs(document);
    bindFormActions();
    restoreDraftValues();
    updateProgress();
    resetPageScroll();
  }

  function renderField(field) {
    const constraints = field.constraints ?? {};
    const isFull = ["MultilineText", "DateRange", "DateTimeRange"].includes(field.type);
    return `
      <div class="field ${isFull ? "field--full" : ""}" data-field-wrapper="${escapeHtml(field.name)}">
        <label class="field-label" for="field-${escapeHtml(field.name)}">
          <span>${escapeHtml(field.displayName)}${constraints.isRequired ? ' <span class="required-mark">*</span>' : ""}</span>
          ${constraints.isRequired ? "" : '<span class="optional-label">необязательно</span>'}
        </label>
        ${renderControl(field)}
        <p class="field-help">${escapeHtml(fieldHelp(field))}</p>
        <p class="field-error" id="error-${escapeHtml(field.name)}"></p>
      </div>`;
  }

  function renderControl(field) {
    const constraints = field.constraints ?? {};
    const attributes = [
      `id="field-${escapeHtml(field.name)}"`,
      `data-field="${escapeHtml(field.name)}"`,
      constraints.isRequired ? "required" : "",
      constraints.minLength != null ? `minlength="${constraints.minLength}"` : "",
      constraints.maxLength != null ? `maxlength="${constraints.maxLength}"` : "",
      constraints.minNumber != null ? `min="${constraints.minNumber}"` : "",
      constraints.maxNumber != null ? `max="${constraints.maxNumber}"` : "",
    ]
      .filter(Boolean)
      .join(" ");

    switch (field.type) {
      case "MultilineText":
        return `<textarea class="field-control" ${attributes} placeholder="Введите текст"></textarea>`;
      case "Integer":
      case "Decimal":
        return `<input class="field-control" ${attributes} type="number" step="${field.type === "Integer" ? "1" : "any"}" placeholder="Введите значение" />`;
      case "Date":
        return `<input class="field-control" ${attributes} type="date" ${dateBounds(constraints)} />`;
      case "Time":
        return `<input class="field-control" ${attributes} type="time" ${timeBounds(constraints)} />`;
      case "DateTime":
        return `<input class="field-control" ${attributes} type="datetime-local" />`;
      case "Boolean":
        return `
          <select class="field-control" ${attributes}>
            <option value="">Выберите значение</option>
            <option value="true">Да</option>
            <option value="false">Нет</option>
          </select>`;
      case "List": {
        const hasDependency = field.dependency?.parentFieldNames?.length;
        return `
          <select class="field-control" ${attributes} ${hasDependency ? "disabled" : ""}>
            <option value="">${hasDependency ? "Сначала заполните предыдущее поле" : "Выберите вариант"}</option>
            ${(field.options ?? [])
              .map((option) => `<option value="${escapeHtml(option.value)}">${escapeHtml(option.displayName)}</option>`)
              .join("")}
          </select>`;
      }
      case "DateRange":
      case "DateTimeRange": {
        const inputType = field.type === "DateRange" ? "date" : "datetime-local";
        return `
          <div class="range-control" data-range-field="${escapeHtml(field.name)}">
            <input class="field-control" id="field-${escapeHtml(field.name)}" data-range-part="from" type="${inputType}" aria-label="${escapeHtml(field.displayName)}: начало" />
            <span class="range-separator">-</span>
            <input class="field-control" data-range-part="to" type="${inputType}" aria-label="${escapeHtml(field.displayName)}: окончание" />
          </div>`;
      }
      default:
        return `<input class="field-control" ${attributes} type="text" placeholder="Введите значение" />`;
    }
  }

  function dateBounds(constraints) {
    const min = normalizeConstraintDate(constraints.minDate);
    const max = normalizeConstraintDate(constraints.maxDate);
    return `${min ? `min="${min}"` : ""} ${max ? `max="${max}"` : ""}`;
  }

  function timeBounds(constraints) {
    return `${constraints.minTime ? `min="${escapeHtml(constraints.minTime)}"` : ""} ${
      constraints.maxTime ? `max="${escapeHtml(constraints.maxTime)}"` : ""
    }`;
  }

  function normalizeConstraintDate(value) {
    if (!value) return "";
    const parts = value.split(/[-.]/);
    if (parts.length !== 3) return value;
    if (parts[0].length === 4) return parts.join("-");
    return `${parts[2]}-${parts[1]}-${parts[0]}`;
  }

  function fieldHelp(field) {
    const constraints = field.constraints ?? {};
    if (field.dependency?.parentFieldNames?.length) {
      return "Список обновится после выбора предыдущего значения";
    }
    if (constraints.maxLength) return `До ${constraints.maxLength} символов`;
    if (constraints.minNumber != null || constraints.maxNumber != null) {
      const min = constraints.minNumber ?? "-∞";
      const max = constraints.maxNumber ?? "∞";
      return `Допустимое значение: от ${min} до ${max}`;
    }
    if (field.type === "DateRange" || field.type === "DateTimeRange") {
      return "Укажите начало и окончание периода";
    }
    if (field.type === "Date") return "Дата в формате ДД.ММ.ГГГГ";
    if (field.type === "Time") return "Время в 24-часовом формате";
    return constraints.isRequired ? "Обязательное поле" : "Можно оставить пустым";
  }

  function bindFormActions() {
    const form = document.querySelector("#document-form");
    form.addEventListener("submit", handlePreview);
    form.addEventListener("input", handleFormChange);
    form.addEventListener("change", handleFormChange);

    document.querySelector("#reset-form").addEventListener("click", () => {
      form.reset();
      draftValues = {};
      clearAllErrors();
      resetDependentFields();
      refreshAllCustomSelects(form);
      refreshAllCustomCalendars(form);
      refreshAllCustomTimeInputs(form);
      updateProgress();
      showToast("Форма очищена", "Все введённые значения удалены.");
    });
  }

  function handleFormChange(event) {
    const fieldName = event.target.dataset.field;
    clearFieldError(fieldName);
    clearFormMessage();
    draftValues = collectValues();
    updateProgress();

    if (event.type === "change" && fieldName) {
      refreshDependentFields(fieldName);
    }
  }

  async function restoreDraftValues() {
    for (const field of editableFields()) {
      if (field.dependency?.parentFieldNames?.length) continue;
      setControlValue(field, draftValues[field.name]);
    }

    for (const field of editableFields().filter((item) => item.dependency?.parentFieldNames?.length)) {
      const value = draftValues[field.name];
      if (value == null || value === "") continue;

      const parentsReady = field.dependency.parentFieldNames.every((parentName) => {
        const parentValue = draftValues[parentName];
        return parentValue != null && parentValue !== "";
      });
      if (!parentsReady) continue;

      await loadOptionsForField(field, value);
    }
  }

  function setControlValue(field, value) {
    if (value == null || value === "") return;

    if (field.type === "DateRange" || field.type === "DateTimeRange") {
      const range = document.querySelector(`[data-range-field="${CSS.escape(field.name)}"]`);
      const from = range?.querySelector('[data-range-part="from"]');
      const to = range?.querySelector('[data-range-part="to"]');
      if (from) from.value = value.from ?? "";
      if (to) to.value = value.to ?? "";
      refreshAllCustomCalendars(range ?? document);
      refreshAllCustomTimeInputs(range ?? document);
      return;
    }

    const control = document.querySelector(`[data-field="${CSS.escape(field.name)}"]`);
    if (control) {
      control.value = String(value);
      refreshCustomSelect(control);
      refreshCustomDateInput(control);
      refreshCustomTimeInput(control);
    }
  }

  function collectValues() {
    const values = {};

    editableFields().forEach((field) => {
      if (field.type === "DateRange" || field.type === "DateTimeRange") {
        const range = document.querySelector(`[data-range-field="${CSS.escape(field.name)}"]`);
        const from = range?.querySelector('[data-range-part="from"]')?.value;
        const to = range?.querySelector('[data-range-part="to"]')?.value;
        if (from || to) values[field.name] = { from: from ?? "", to: to ?? "" };
        return;
      }

      const control = document.querySelector(`[data-field="${CSS.escape(field.name)}"]`);
      if (!control || control.value === "") return;

      switch (field.type) {
        case "Integer":
          values[field.name] = Number.parseInt(control.value, 10);
          break;
        case "Decimal":
          values[field.name] = Number.parseFloat(control.value.replace(",", "."));
          break;
        case "Boolean":
          values[field.name] = control.value === "true";
          break;
        default:
          values[field.name] = control.value;
      }
    });

    return values;
  }

  function updateProgress() {
    const fields = editableFields();
    const values = collectValues();
    const completed = fields.filter((field) => {
      const value = values[field.name];
      if (value == null || value === "") return false;
      if (typeof value === "object") return Boolean(value.from && value.to);
      return true;
    }).length;
    const percent = fields.length ? Math.round((completed / fields.length) * 100) : 100;
    document.querySelector("#progress-ring")?.style.setProperty("--progress", `${percent * 3.6}deg`);
    const label = document.querySelector("#progress-value");
    if (label) label.textContent = `${percent}% заполнено`;
  }

  function resetDependentFields() {
    editableFields()
      .filter((field) => field.dependency?.parentFieldNames?.length)
      .forEach((field) => {
        const select = document.querySelector(`[data-field="${CSS.escape(field.name)}"]`);
        if (!select) return;
        select.innerHTML = '<option value="">Сначала заполните предыдущее поле</option>';
        select.disabled = true;
      });
  }

  async function refreshDependentFields(changedFieldName) {
    const descendants = collectDependentDescendants(changedFieldName);

    for (const field of descendants) {
      const select = document.querySelector(`[data-field="${CSS.escape(field.name)}"]`);
      if (!select) continue;
      select.value = "";
      clearFieldError(field.name);

      const parentsReady = field.dependency.parentFieldNames.every((parentName) => {
        const parent = document.querySelector(`[data-field="${CSS.escape(parentName)}"]`);
        return parent?.value;
      });

      if (!parentsReady) {
        select.disabled = true;
        select.innerHTML = '<option value="">Сначала заполните предыдущее поле</option>';
        continue;
      }

      await loadOptionsForField(field);
    }

    draftValues = collectValues();
    updateProgress();
  }

  async function loadOptionsForField(field, selectedValue = "") {
    const select = document.querySelector(`[data-field="${CSS.escape(field.name)}"]`);
    if (!select) return;

    select.disabled = true;
    select.innerHTML = '<option value="">Загружаем варианты...</option>';

    try {
      const response = await bridge("getFieldOptions", {
        documentType: activeSchema.documentType,
        fieldName: field.name,
        values: collectValues(),
      });

      if (!response.isSuccess) {
        throw new Error((response.errors ?? []).join("\n") || "Не удалось загрузить варианты.");
      }

      select.innerHTML = `
        <option value="">Выберите вариант</option>
        ${(response.options ?? [])
          .map((option) => `<option value="${escapeHtml(option.value)}">${escapeHtml(option.displayName)}</option>`)
          .join("")}`;
      select.disabled = false;
      if (selectedValue) select.value = String(selectedValue);
      refreshCustomSelect(select);
    } catch (error) {
      select.innerHTML = '<option value="">Не удалось загрузить варианты</option>';
      setFieldError(field.name, error?.message ?? error);
    }
  }

  function collectDependentDescendants(parentName) {
    const result = [];
    const queue = [parentName];
    const visited = new Set();

    while (queue.length) {
      const current = queue.shift();
      editableFields().forEach((field) => {
        if (!visited.has(field.name) && field.dependency?.parentFieldNames?.includes(current)) {
          visited.add(field.name);
          result.push(field);
          queue.push(field.name);
        }
      });
    }

    return result;
  }

  async function handlePreview(event) {
    event.preventDefault();
    clearAllErrors();
    const button = document.querySelector("#preview-button");
    setButtonBusy(button, true, "Проверяем данные...");

    try {
      const values = collectValues();
      draftValues = values;
      const localErrors = validateRequiredFields(values);
      if (localErrors.length) {
        showValidationErrors(localErrors);
        return;
      }

      const validation = await bridge("validateDocument", {
        documentType: activeSchema.documentType,
        values,
      });

      if (!validation.isValid) {
        showValidationErrors(validation.errors ?? []);
        return;
      }

      setButtonBusy(button, true, "Формируем документ...");
      const result = await bridge("generatePrintHtml", {
        documentType: activeSchema.documentType,
        values,
        generatedFields: {},
        templateFileName: activeDescriptor.templateFileName,
        strictMode: true,
        htmlEncodeValues: true,
      });

      if (!result.isSuccess) {
        throw new Error(
          (result.errors ?? []).join("\n") || "Печатная форма не создана.",
        );
      }

      generatedHtml = result.htmlContent;
      renderPreview();
    } catch (error) {
      showFormMessage(error?.message ?? error);
      showToast("Не удалось создать предпросмотр", error?.message ?? error, "error");
    } finally {
      if (document.body.contains(button)) {
        setButtonBusy(button, false);
      }
    }
  }

  function validateRequiredFields(values) {
    return editableFields()
      .filter((field) => field.constraints?.isRequired)
      .filter((field) => {
        const value = values[field.name];
        if (value == null || value === "") return true;
        if (typeof value === "object") return !value.from || !value.to;
        return false;
      })
      .map((field) => ({
        fieldName: field.name,
        message: "Заполнение обязательно.",
      }));
  }

  function showValidationErrors(errors) {
    const localizedErrors = errors.map(localizeValidationError);
    localizedErrors.forEach((error) => setFieldError(error.fieldName, error.message));
    showFormMessage(`Найдены ошибки: ${errors.length}. Исправьте отмеченные поля.`);
    document.querySelector(".field--error")?.scrollIntoView({ behavior: "smooth", block: "center" });
  }

  function localizeValidationError(error) {
    let message = String(error.message || "Поле заполнено некорректно.");
    if (/^The .+ field is required\.?$/i.test(message)) {
      message = "Заполнение обязательно.";
    }

    activeSchema.fields.forEach((schemaField) => {
      message = message.split(schemaField.name).join(`"${schemaField.displayName}"`);
    });

    return { fieldName: error.fieldName, message };
  }

  function setFieldError(fieldName, message) {
    if (!fieldName) return;
    const wrapper = document.querySelector(`[data-field-wrapper="${CSS.escape(fieldName)}"]`);
    if (!wrapper) return;
    wrapper.classList.add("field--error");
    const error = wrapper.querySelector(".field-error");
    if (error) error.textContent = message;
  }

  function clearFieldError(fieldName) {
    if (!fieldName) return;
    const wrapper = document.querySelector(`[data-field-wrapper="${CSS.escape(fieldName)}"]`);
    wrapper?.classList.remove("field--error");
  }

  function clearAllErrors() {
    document.querySelectorAll(".field--error").forEach((field) => field.classList.remove("field--error"));
    clearFormMessage();
  }

  function clearFormMessage() {
    const message = document.querySelector("#form-message");
    message?.classList.remove("form-message--visible", "form-message--success");
  }

  function showFormMessage(message) {
    const messageElement = document.querySelector("#form-message");
    if (!messageElement) return;
    messageElement.querySelector("span").textContent = message;
    messageElement.classList.remove("form-message--success");
    messageElement.classList.add("form-message--visible");
  }

  function showFormSuccess(message) {
    const messageElement = document.querySelector("#form-message");
    if (!messageElement) return;
    messageElement.querySelector("span").textContent = message;
    messageElement.classList.add("form-message--visible", "form-message--success");
  }

  function setButtonBusy(button, busy, text = "") {
    if (!button) return;
    if (!button.dataset.originalHtml) button.dataset.originalHtml = button.innerHTML;
    button.disabled = busy;
    button.innerHTML = busy
      ? `${icon("refresh", "icon--small")} ${escapeHtml(text)}`
      : button.dataset.originalHtml;
  }

  function renderPreview() {
    app.innerHTML = `
      <div class="form-shell preview-layout">
        ${sidebarMarkup(2)}
        <main class="form-workspace">
          ${toolbarMarkup("preview")}
          <div class="preview-workspace">
            <div class="preview-toolbar">
              <div class="preview-toolbar-copy">
                <p class="eyebrow">Шаг 2 из 3</p>
                <h2>Предпросмотр документа</h2>
              </div>
              <div class="output-format-control">
                <span class="sr-only">Формат документа</span>
                <select id="output-format" aria-label="Формат документа">
                  <option value="html">HTML</option>
                </select>
              </div>
              <button class="secondary-button" type="button" id="edit-document">
                ${icon("pencil", "icon--small")} Редактировать
              </button>
              <button class="primary-button" type="button" id="save-document">
                ${icon("printer", "icon--small")} Создать документ
              </button>
            </div>
            <div class="preview-frame-wrap">
              <iframe
                class="preview-frame"
                id="print-preview"
                title="Печатная форма"
              ></iframe>
            </div>
          </div>
        </main>
      </div>`;

    const frame = document.querySelector("#print-preview");
    frame.srcdoc = generatedHtml;

    document
      .querySelector("#close-document")
      .addEventListener("click", () => navigateHome(true));
    document
      .querySelector("#edit-document")
      .addEventListener("click", renderDocumentForm);
    document
      .querySelector("#save-document")
      .addEventListener("click", printPreviewHtml);

    enhanceSelects(document);
    resetPageScroll();
  }

  function printPreviewHtml() {
    const frame = document.querySelector("#print-preview");
    if (!frame?.contentWindow) {
      showToast("Печать недоступна", "Предпросмотр документа ещё не загружен.", "error");
      return;
    }

    frame.contentWindow.focus();
    frame.contentWindow.print();
  }

  function createDemoPrintHtml(values) {
    const rows = editableFields()
      .map((field) => {
        const rawValue = values[field.name];
        const value =
          rawValue && typeof rawValue === "object"
            ? `${rawValue.from ?? ""} — ${rawValue.to ?? ""}`
            : rawValue ?? "";

        return `
          <div class="row">
            <span>${escapeHtml(field.displayName)}</span>
            <strong>${escapeHtml(value)}</strong>
          </div>`;
      })
      .join("");

    return `<!doctype html>
      <html lang="ru">
        <head>
          <meta charset="UTF-8">
          <title>${escapeHtml(activeSchema?.displayName ?? "Документ")}</title>
          <style>
            * { box-sizing: border-box; }
            html, body { margin: 0; min-height: 100%; }
            body {
              padding: 44px 52px;
              color: #111;
              background: #fff;
              font-family: "Times New Roman", Times, serif;
            }
            header { margin-bottom: 42px; text-align: right; font-weight: 700; }
            h1 {
              margin: 0 0 48px;
              font-size: 24px;
              text-align: center;
              text-transform: uppercase;
            }
            .row {
              display: grid;
              grid-template-columns: 230px 1fr;
              align-items: end;
              gap: 12px;
              margin-bottom: 22px;
              font-size: 17px;
            }
            .row strong {
              min-height: 24px;
              padding: 0 5px 3px;
              border-bottom: 1px solid #111;
              font-weight: 400;
            }
            @media print {
              body { padding: 0; }
              @page { size: A4; margin: 1cm; }
            }
          </style>
        </head>
        <body>
          <header>Форма MADOC</header>
          <h1>${escapeHtml(activeSchema?.displayName ?? "Документ")}</h1>
          ${rows}
        </body>
      </html>`;
  }

  function navigateHome(pushHistory) {
    if (pushHistory) {
      window.history.pushState({ documentType: null }, "", window.location.pathname);
    }
    activeSchema = null;
    activeDescriptor = null;
    generatedHtml = "";
    draftValues = {};
    renderHome();
    resetPageScroll();
  }

  function renderFatalError(title, error) {
    app.innerHTML = `
      <main class="error-state">
        <section class="error-card">
          <span class="error-icon">${icon("warning")}</span>
          <h1>${escapeHtml(title)}</h1>
          <p>${escapeHtml(error?.message ?? error)}</p>
          <button class="primary-button" type="button" id="retry-home">Повторить</button>
        </section>
      </main>`;
    document.querySelector("#retry-home").addEventListener("click", initHome);
  }

  function showToast(title, message, tone = "success") {
    if (!toastRegion) return;
    const toast = document.createElement("div");
    toast.className = `toast ${tone === "error" ? "toast--error" : ""}`;
    toast.innerHTML = `
      ${icon(tone === "error" ? "warning" : "check")}
      <div class="toast-copy"><strong>${escapeHtml(title)}</strong><p>${escapeHtml(message)}</p></div>
      <button class="toast-close" type="button" aria-label="Закрыть уведомление">
        ${icon("close", "icon--small")}
      </button>`;
    toastRegion.append(toast);

    let removeTimer;
    const dismiss = () => {
      window.clearTimeout(removeTimer);
      toast.style.opacity = "0";
      toast.style.transform = "translateY(8px)";
      window.setTimeout(() => toast.remove(), 180);
    };
    toast.querySelector(".toast-close").addEventListener("click", dismiss);
    removeTimer = window.setTimeout(dismiss, 5200);
  }

  window.addEventListener("popstate", () => {
    const routeParams = new URLSearchParams(window.location.search);
    const documentType = routeParams.get("document");
    if (documentType) {
      initDocument(documentType).catch((error) => renderFatalError("Форма не загрузилась", error));
    } else {
      navigateHome(false);
    }
  });

  const initialDocumentType = params.get("document");
  if (initialDocumentType) {
    initDocument(initialDocumentType).catch((error) => renderFatalError("Форма не загрузилась", error));
  } else {
    initHome();
  }
})();
