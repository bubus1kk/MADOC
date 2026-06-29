use serde_json::{json, Value};
use std::{
    env,
    io::{BufRead, BufReader, Write},
    path::{Path, PathBuf},
    process::{Child, ChildStdin, ChildStdout, Command, Stdio},
    sync::{
        atomic::{AtomicU64, Ordering},
        Arc, Mutex,
    },
};
use tauri::{AppHandle, Manager, State};

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
                "DesktopBridge не найден: {}. Сначала соберите MADOC.DesktopBridge.",
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
            "tauri-{}",
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

#[cfg_attr(mobile, tauri::mobile_entry_point)]
pub fn run() {
    tauri::Builder::default()
        .setup(|app| {
            app.manage(BridgeState {
                process: Arc::new(Mutex::new(None)),
                bridge_path: resolve_bridge_path(&app.handle()),
                request_sequence: Arc::new(AtomicU64::new(1)),
            });
            Ok(())
        })
        .invoke_handler(tauri::generate_handler![bridge_request])
        .run(tauri::generate_context!())
        .expect("error while running MADOC.Tauri");
}
