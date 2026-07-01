use base64::{engine::general_purpose::STANDARD as BASE64, Engine as _};
use serde::{Deserialize, Serialize};
use serde_json::{json, Value};
use std::{
    env, fs,
    io::{BufRead, BufReader, Write},
    path::{Path, PathBuf},
    process::{Child, ChildStdin, ChildStdout, Command, Stdio},
    sync::{
        atomic::{AtomicU64, Ordering},
        Arc, Mutex,
    },
    thread,
    time::{SystemTime, UNIX_EPOCH},
};
use tauri::{AppHandle, Manager, State};
use url::Url;

struct BridgeProcess {
    child: Child,
    stdin: ChildStdin,
    stdout: BufReader<ChildStdout>,
}

#[derive(Clone)]
struct BridgeState {
    process: Arc<Mutex<Option<BridgeProcess>>>,
    bridge_path: PathBuf,
    request_sequence: Arc<AtomicU64>,
}

impl Drop for BridgeProcess {
    fn drop(&mut self) {
        let _ = self.child.kill();
        let _ = self.child.wait();
    }
}

impl BridgeProcess {
    fn start(bridge_path: &Path) -> Result<Self, String> {
        if !bridge_path.exists() {
            return Err(format!(
                "DesktopBridge не найден: {}. Сначала соберите проект MADOC.DesktopBridge.",
                bridge_path.display()
            ));
        }

        let is_native_executable = bridge_path
            .extension()
            .and_then(|extension| extension.to_str())
            .is_some_and(|extension| extension.eq_ignore_ascii_case("exe"));
        let mut command = if is_native_executable {
            Command::new(bridge_path)
        } else {
            let mut dotnet = Command::new("dotnet");
            dotnet.arg(bridge_path);
            dotnet
        };
        let mut child = command
            .stdin(Stdio::piped())
            .stdout(Stdio::piped())
            .stderr(Stdio::inherit())
            .creation_flags_no_window()
            .spawn()
            .map_err(|error| format!("Не удалось запустить DesktopBridge: {error}"))?;

        let stdin = child
            .stdin
            .take()
            .ok_or_else(|| "Не удалось открыть вход DesktopBridge.".to_owned())?;
        let stdout = child
            .stdout
            .take()
            .ok_or_else(|| "Не удалось открыть выход DesktopBridge.".to_owned())?;

        Ok(Self {
            child,
            stdin,
            stdout: BufReader::new(stdout),
        })
    }

    fn request(&mut self, request: &Value) -> Result<Value, String> {
        let serialized = serde_json::to_string(request)
            .map_err(|error| format!("Не удалось сериализовать bridge-запрос: {error}"))?;

        writeln!(self.stdin, "{serialized}")
            .and_then(|_| self.stdin.flush())
            .map_err(|error| format!("DesktopBridge не принял запрос: {error}"))?;

        let mut response_line = String::new();
        self.stdout
            .read_line(&mut response_line)
            .map_err(|error| format!("Не удалось прочитать ответ DesktopBridge: {error}"))?;

        if response_line.trim().is_empty() {
            return Err("DesktopBridge завершился без ответа.".to_owned());
        }

        serde_json::from_str(response_line.trim())
            .map_err(|error| format!("DesktopBridge вернул некорректный JSON: {error}"))
    }
}

trait CommandExt {
    fn creation_flags_no_window(&mut self) -> &mut Self;
}

impl CommandExt for Command {
    #[cfg(target_os = "windows")]
    fn creation_flags_no_window(&mut self) -> &mut Self {
        use std::os::windows::process::CommandExt;
        self.creation_flags(0x08000000)
    }

    #[cfg(not(target_os = "windows"))]
    fn creation_flags_no_window(&mut self) -> &mut Self {
        self
    }
}

#[tauri::command]
async fn bridge_request(
    state: State<'_, BridgeState>,
    command: String,
    payload: Value,
) -> Result<Value, String> {
    let state = state.inner().clone();

    tauri::async_runtime::spawn_blocking(move || {
        let id = format!(
            "desktop-{}",
            state.request_sequence.fetch_add(1, Ordering::Relaxed)
        );
        let request = json!({
            "id": id,
            "command": command,
            "payload": payload
        });

        let mut process_guard = state
            .process
            .lock()
            .map_err(|_| "DesktopBridge занят другим запросом.".to_owned())?;

        if process_guard.is_none() {
            *process_guard = Some(BridgeProcess::start(&state.bridge_path)?);
        }

        let response = match process_guard
            .as_mut()
            .expect("bridge initialized")
            .request(&request)
        {
            Ok(response) => response,
            Err(_) => {
                *process_guard = Some(BridgeProcess::start(&state.bridge_path)?);
                process_guard
                    .as_mut()
                    .expect("bridge restarted")
                    .request(&request)?
            }
        };

        if response
            .get("success")
            .and_then(Value::as_bool)
            .unwrap_or(false)
        {
            Ok(response.get("data").cloned().unwrap_or(Value::Null))
        } else {
            let message = response
                .pointer("/error/message")
                .and_then(Value::as_str)
                .unwrap_or("DesktopBridge вернул ошибку.");
            let details = response
                .pointer("/error/details")
                .and_then(Value::as_str)
                .filter(|value| !value.is_empty());

            Err(match details {
                Some(details) => format!("{message}\n{details}"),
                None => message.to_owned(),
            })
        }
    })
    .await
    .map_err(|error| format!("Bridge-задача была прервана: {error}"))?
}

#[derive(Clone, Debug, Deserialize, Serialize)]
#[serde(rename_all = "camelCase")]
struct StoredPrintDocument {
    id: String,
    document_type: String,
    document_name: String,
    format: String,
    file_name: String,
    file_path: String,
    created_at: u64,
}

#[derive(Serialize)]
#[serde(rename_all = "camelCase")]
struct StoredPrintDocumentContent {
    document: StoredPrintDocument,
    html_content: Option<String>,
    data_url: Option<String>,
}

fn print_forms_directory(app: &AppHandle) -> Result<PathBuf, String> {
    let documents_directory = app
        .path()
        .document_dir()
        .map_err(|error| format!("Не удалось определить папку документов: {error}"))?;
    let target_directory = documents_directory
        .join("MADOC")
        .join("Печатные формы");
    fs::create_dir_all(&target_directory)
        .map_err(|error| format!("Не удалось создать папку печатных форм: {error}"))?;
    Ok(target_directory)
}

fn print_forms_metadata_directory(app: &AppHandle) -> Result<PathBuf, String> {
    let app_data_directory = app
        .path()
        .app_data_dir()
        .map_err(|error| format!("Не удалось определить папку данных приложения: {error}"))?;
    let metadata_directory = app_data_directory.join("print-form-index");
    fs::create_dir_all(&metadata_directory)
        .map_err(|error| format!("Не удалось создать индекс печатных форм: {error}"))?;
    Ok(metadata_directory)
}

fn is_print_form_metadata(path: &Path) -> bool {
    path.file_name()
        .and_then(|name| name.to_str())
        .is_some_and(|name| name.ends_with(".madoc.json"))
}

fn migrate_legacy_print_form_metadata(
    print_forms_directory: &Path,
    metadata_directory: &Path,
) -> Result<(), String> {
    for entry in fs::read_dir(print_forms_directory)
        .map_err(|error| format!("Не удалось проверить старый индекс печатных форм: {error}"))?
    {
        let entry = match entry {
            Ok(entry) => entry,
            Err(_) => continue,
        };
        let legacy_path = entry.path();
        if !is_print_form_metadata(&legacy_path) {
            continue;
        }

        let Some(file_name) = legacy_path.file_name() else {
            continue;
        };
        let metadata_path = metadata_directory.join(file_name);

        if metadata_path.exists() {
            fs::remove_file(&legacy_path)
                .map_err(|error| format!("Не удалось удалить старый файл индекса: {error}"))?;
            continue;
        }

        if fs::rename(&legacy_path, &metadata_path).is_err() {
            fs::copy(&legacy_path, &metadata_path)
                .map_err(|error| format!("Не удалось перенести индекс печатных форм: {error}"))?;
            fs::remove_file(&legacy_path)
                .map_err(|error| format!("Не удалось удалить старый файл индекса: {error}"))?;
        }
    }

    Ok(())
}

fn prepare_print_form_storage(app: &AppHandle) -> Result<(PathBuf, PathBuf), String> {
    let print_forms_directory = print_forms_directory(app)?;
    let metadata_directory = print_forms_metadata_directory(app)?;
    migrate_legacy_print_form_metadata(&print_forms_directory, &metadata_directory)?;
    Ok((print_forms_directory, metadata_directory))
}

fn safe_file_component(value: &str) -> String {
    let normalized = value
        .chars()
        .map(|character| {
            if character.is_alphanumeric() {
                character
            } else {
                '-'
            }
        })
        .collect::<String>()
        .split('-')
        .filter(|part| !part.is_empty())
        .collect::<Vec<_>>()
        .join("-");

    if normalized.is_empty() {
        "document".to_owned()
    } else {
        normalized
    }
}

fn unix_time_millis() -> Result<u64, String> {
    let duration = SystemTime::now()
        .duration_since(UNIX_EPOCH)
        .map_err(|error| format!("Системное время недоступно: {error}"))?;
    Ok(duration.as_millis() as u64)
}

fn find_edge_executable() -> Option<PathBuf> {
    let mut candidates = Vec::new();

    if let Some(program_files_x86) = env::var_os("ProgramFiles(x86)") {
        candidates.push(
            PathBuf::from(program_files_x86)
                .join("Microsoft")
                .join("Edge")
                .join("Application")
                .join("msedge.exe"),
        );
    }
    if let Some(program_files) = env::var_os("ProgramFiles") {
        candidates.push(
            PathBuf::from(program_files)
                .join("Microsoft")
                .join("Edge")
                .join("Application")
                .join("msedge.exe"),
        );
    }
    if let Some(local_app_data) = env::var_os("LOCALAPPDATA") {
        candidates.push(
            PathBuf::from(local_app_data)
                .join("Microsoft")
                .join("Edge")
                .join("Application")
                .join("msedge.exe"),
        );
    }

    candidates.into_iter().find(|candidate| candidate.exists())
}

fn render_pdf(html_path: &Path, pdf_path: &Path) -> Result<(), String> {
    let edge_path = find_edge_executable().ok_or_else(|| {
        "Для создания PDF требуется Microsoft Edge, но он не найден на компьютере.".to_owned()
    })?;
    let html_url = Url::from_file_path(html_path)
        .map_err(|_| "Не удалось сформировать локальный адрес HTML-файла.".to_owned())?;
    let profile_directory = env::temp_dir().join("MADOC Concept").join("EdgePdfProfile");
    fs::create_dir_all(&profile_directory)
        .map_err(|error| format!("Не удалось подготовить конвертер PDF: {error}"))?;

    let output = Command::new(edge_path)
        .arg("--headless=new")
        .arg("--disable-gpu")
        .arg("--disable-extensions")
        .arg("--no-pdf-header-footer")
        .arg("--print-to-pdf-no-header")
        .arg(format!(
            "--user-data-dir={}",
            profile_directory.to_string_lossy()
        ))
        .arg(format!("--print-to-pdf={}", pdf_path.to_string_lossy()))
        .arg(html_url.as_str())
        .stdin(Stdio::null())
        .stdout(Stdio::null())
        .stderr(Stdio::piped())
        .creation_flags_no_window()
        .output()
        .map_err(|error| format!("Не удалось запустить конвертацию в PDF: {error}"))?;

    if !output.status.success() {
        let details = String::from_utf8_lossy(&output.stderr);
        return Err(format!(
            "Microsoft Edge не смог создать PDF. {}",
            details.trim()
        ));
    }

    for _ in 0..30 {
        if pdf_path
            .metadata()
            .map(|metadata| metadata.len() > 0)
            .unwrap_or(false)
        {
            return Ok(());
        }
        thread::sleep(std::time::Duration::from_millis(100));
    }

    Err("Конвертация завершилась, но PDF-файл не был создан.".to_owned())
}

fn save_print_document_sync(
    app: AppHandle,
    document_type: String,
    document_name: String,
    html: String,
    output_format: String,
) -> Result<StoredPrintDocument, String> {
    let format = output_format.trim().to_ascii_lowercase();
    if format != "html" && format != "pdf" {
        return Err("Поддерживаются только форматы HTML и PDF.".to_owned());
    }

    let (target_directory, metadata_directory) = prepare_print_form_storage(&app)?;
    let timestamp = unix_time_millis()?;
    let safe_name = safe_file_component(&document_name);
    let id = format!(
        "{}-{timestamp}",
        safe_file_component(&document_type).to_ascii_lowercase()
    );
    let file_name = format!("{safe_name}-{timestamp}.{format}");
    let file_path = target_directory.join(&file_name);

    if format == "html" {
        fs::write(&file_path, &html)
            .map_err(|error| format!("Не удалось сохранить HTML-форму: {error}"))?;
    } else {
        let temporary_html_path = target_directory.join(format!(".{id}.html"));
        fs::write(&temporary_html_path, &html)
            .map_err(|error| format!("Не удалось подготовить HTML для PDF: {error}"))?;
        let render_result = render_pdf(&temporary_html_path, &file_path);
        let _ = fs::remove_file(&temporary_html_path);
        render_result?;
    }

    let document = StoredPrintDocument {
        id: id.clone(),
        document_type,
        document_name,
        format,
        file_name,
        file_path: file_path.to_string_lossy().into_owned(),
        created_at: timestamp,
    };
    let metadata_path = metadata_directory.join(format!("{id}.madoc.json"));
    let metadata = serde_json::to_string_pretty(&document)
        .map_err(|error| format!("Не удалось подготовить описание документа: {error}"))?;
    fs::write(metadata_path, metadata)
        .map_err(|error| format!("Не удалось сохранить описание документа: {error}"))?;

    Ok(document)
}

#[tauri::command]
async fn save_print_document(
    app: AppHandle,
    document_type: String,
    document_name: String,
    html: String,
    output_format: String,
) -> Result<StoredPrintDocument, String> {
    tauri::async_runtime::spawn_blocking(move || {
        save_print_document_sync(app, document_type, document_name, html, output_format)
    })
    .await
    .map_err(|error| format!("Создание документа было прервано: {error}"))?
}

fn load_stored_documents(app: &AppHandle) -> Result<Vec<StoredPrintDocument>, String> {
    let (target_directory, metadata_directory) = prepare_print_form_storage(app)?;
    let mut documents = Vec::new();

    for entry in fs::read_dir(&metadata_directory)
        .map_err(|error| format!("Не удалось прочитать индекс печатных форм: {error}"))?
    {
        let entry = match entry {
            Ok(entry) => entry,
            Err(_) => continue,
        };
        let path = entry.path();
        if !is_print_form_metadata(&path) {
            continue;
        }

        let content = match fs::read_to_string(&path) {
            Ok(content) => content,
            Err(_) => continue,
        };
        let mut document: StoredPrintDocument = match serde_json::from_str(&content) {
            Ok(document) => document,
            Err(_) => continue,
        };
        let document_path = target_directory.join(&document.file_name);
        if !document_path.exists() {
            continue;
        }
        document.file_path = document_path.to_string_lossy().into_owned();
        documents.push(document);
    }

    documents.sort_by(|left, right| right.created_at.cmp(&left.created_at));
    Ok(documents)
}

#[tauri::command]
fn list_print_documents(app: AppHandle) -> Result<Vec<StoredPrintDocument>, String> {
    load_stored_documents(&app)
}

#[tauri::command]
fn read_print_document(
    app: AppHandle,
    document_id: String,
) -> Result<StoredPrintDocumentContent, String> {
    let document = load_stored_documents(&app)?
        .into_iter()
        .find(|document| document.id == document_id)
        .ok_or_else(|| "Печатная форма не найдена.".to_owned())?;
    let file_path = PathBuf::from(&document.file_path);

    if document.format == "html" {
        let html_content = fs::read_to_string(&file_path)
            .map_err(|error| format!("Не удалось открыть HTML-форму: {error}"))?;
        Ok(StoredPrintDocumentContent {
            document,
            html_content: Some(html_content),
            data_url: None,
        })
    } else {
        let content = fs::read(&file_path)
            .map_err(|error| format!("Не удалось открыть PDF-форму: {error}"))?;
        Ok(StoredPrintDocumentContent {
            document,
            html_content: None,
            data_url: Some(format!(
                "data:application/pdf;base64,{}",
                BASE64.encode(content)
            )),
        })
    }
}

fn resolve_bridge_path(app: &AppHandle) -> PathBuf {
    if let Ok(configured_path) = env::var("MADOC_BRIDGE_PATH") {
        return PathBuf::from(configured_path);
    }

    if let Ok(resource_directory) = app.path().resource_dir() {
        let bundled_executable = resource_directory
            .join("bridge")
            .join("MADOC.DesktopBridge.exe");
        if bundled_executable.exists() {
            return bundled_executable;
        }

        let bundled_path = resource_directory
            .join("bridge")
            .join("MADOC.DesktopBridge.dll");
        if bundled_path.exists() {
            return bundled_path;
        }
    }

    let workspace_directory = PathBuf::from(env!("CARGO_MANIFEST_DIR"))
        .parent()
        .and_then(Path::parent)
        .map(Path::to_path_buf)
        .unwrap_or_else(|| PathBuf::from("."));
    let debug_path = workspace_directory
        .join("MADOC.DesktopBridge")
        .join("bin")
        .join("Debug")
        .join("net10.0")
        .join("MADOC.DesktopBridge.dll");

    if debug_path.exists() {
        return debug_path;
    }

    workspace_directory
        .join("MADOC.DesktopBridge")
        .join("bin")
        .join("Release")
        .join("net10.0")
        .join("publish")
        .join("MADOC.DesktopBridge.dll")
}

pub fn run() {
    tauri::Builder::default()
        .setup(|app| {
            let bridge_path = resolve_bridge_path(&app.handle());
            if let Err(error) = prepare_print_form_storage(&app.handle()) {
                eprintln!("Не удалось подготовить хранилище печатных форм: {error}");
            }
            app.manage(BridgeState {
                process: Arc::new(Mutex::new(None)),
                bridge_path,
                request_sequence: Arc::new(AtomicU64::new(1)),
            });
            Ok(())
        })
        .invoke_handler(tauri::generate_handler![
            bridge_request,
            save_print_document,
            list_print_documents,
            read_print_document
        ])
        .run(tauri::generate_context!())
        .expect("error while running MADOC desktop application");
}
