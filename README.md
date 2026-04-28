# TrickLor — Windows System Toolkit 100% Claudecode = ))

> **v2.0** — 8 tính năng mới: Startup Manager, Disk Cleaner, Driver Info, Process Manager, Backup & Restore, Scheduled Tasks, Windows Update Manager, Event Log Viewer

<img width="261" height="193" alt="image" src="https://github.com/user-attachments/assets/e6581edf-0e7a-4d89-95a3-a1d4ca951242" />


Công cụ tối ưu hóa, bảo trì và triển khai Windows toàn diện, được xây dựng bằng WPF (.NET 10).

---

## Tính năng

### Tổng quan
| Module | Mô tả |
|--------|-------|
| **Thông tin hệ thống** | Theo dõi CPU, RAM, Disk, Pin theo thời gian thực (cập nhật mỗi 2 giây) |

### Tính năng chính
| Module | Mô tả |
|--------|-------|
| **Triển khai phần mềm** | Cài đặt 34+ ứng dụng phổ biến qua `winget` với thanh tiến trình |
| **Tối ưu hóa** | Tắt dịch vụ nền, điều chỉnh hiệu ứng đồ họa, quản lý khởi động, dọn dẹp bộ nhớ |
| **Gỡ bloatware** | Xóa ứng dụng Windows tích hợp không cần thiết (Xbox, OneDrive, Teams Chat, …) với điểm khôi phục hệ thống tự động |
| **Bảo mật** | Quản lý UAC, SmartScreen, Firewall, thông báo, cảnh báo bảo mật |
| **Sửa mạng** | Reset Winsock/TCP-IP, flush DNS, bật chia sẻ file, kích hoạt SMB, kiểm tra kết nối internet |
| **Cấu hình nhanh** | Tùy chỉnh Explorer, menu chuột phải, Photo Viewer, taskbar, đổi tên/mật khẩu tài khoản |

### Công cụ (v2.0)
| Module | Mô tả |
|--------|-------|
| **BitLocker** | Mã hóa/giải mã ổ đĩa, sao lưu recovery key, tạm dừng/tiếp tục BitLocker |
| **Startup Manager** ✨ | Xem và xóa ứng dụng khởi động cùng Windows (HKCU + HKLM registry) |
| **Disk Cleaner** ✨ | Ước tính và xóa Windows Update cache, Prefetch, WER, Thumbnail cache, Temp, Recycle Bin |
| **Driver Info** ✨ | Danh sách toàn bộ driver thiết bị via WMI, cảnh báo driver chưa ký hoặc có lỗi, lọc theo tên |
| **Process Manager** ✨ | Xem tiến trình đang chạy theo RAM, kết thúc tiến trình được chọn |

### Nâng cao (v2.0)
| Module | Mô tả |
|--------|-------|
| **Backup & Restore** ✨ | Xuất / nhập file `settings.json` — sao lưu giao diện và theme sang máy khác |
| **Scheduled Tasks** ✨ | Xem toàn bộ tác vụ Task Scheduler, bật/tắt/xóa tác vụ được chọn |
| **Windows Update** ✨ | Tạm dừng hoặc tiếp tục cập nhật tự động qua Group Policy registry |
| **Event Log Viewer** ✨ | Đọc System / Application / Security event log, lọc chỉ lỗi & cảnh báo |

### Hệ thống
| Module | Mô tả |
|--------|-------|
| **Nhật ký** | Ghi lại toàn bộ thao tác đã thực hiện kèm timestamp |
| **Giao diện** | Chế độ sáng/tối, chọn màu nhấn (accent color), lưu tùy chỉnh tự động |

---

## Yêu cầu

- Windows 10 / 11 (x64)
- .NET 10 Runtime (hoặc dùng bản self-contained)
- Quyền Administrator (ứng dụng tự yêu cầu UAC khi khởi động)
- `winget` đã được cài (có sẵn trên Windows 11, Windows 10 1709+)

---

## Cài đặt & Chạy

### Build từ source

```bash
git clone https://github.com/your-username/TrickLor.git
cd TrickLor/TrickLor
dotnet build -c Release
dotnet run
```

### Chạy file thực thi

```
TrickLor.exe   # Chạy với quyền Administrator
```

> Ứng dụng tự động yêu cầu nâng quyền qua UAC khi khởi động.

---

## Changelog

### v2.0 (2026-04-28)
**Tính năng mới — Công cụ thực dụng:**
- 🚦 **Startup Manager** — Xem và xóa ứng dụng khởi động cùng Windows từ registry HKCU/HKLM
- 🧽 **Disk Cleaner** — Ước tính dung lượng và dọn sạch 6 loại file rác (Update cache, Prefetch, WER, Thumbnail, Temp, Recycle Bin)
- 🔌 **Driver Info** — Liệt kê toàn bộ driver thiết bị via WMI, phát hiện driver chưa ký hoặc lỗi, hỗ trợ lọc nhanh
- ⚙ **Process Manager** — Xem tiến trình đang chạy (sắp xếp theo RAM), kết thúc tiến trình được chọn

**Tính năng mới — Nâng cao:**
- 💾 **Backup & Restore** — Xuất/nhập `settings.json` dễ dàng, bảo toàn cài đặt giao diện khi cài lại máy
- 🗓 **Scheduled Tasks** — Xem toàn bộ Task Scheduler, bật/tắt/xóa tác vụ qua `schtasks`
- 🔄 **Windows Update Manager** — Tạm dừng/tiếp tục cập nhật tự động qua Group Policy registry
- 📋 **Event Log Viewer** — Đọc System/Application/Security event log, lọc theo mức độ lỗi

**Cải thiện khác:**
- Sidebar tổ chức lại thành 4 nhóm: TỔNG QUAN / TÍNH NĂNG / CÔNG CỤ / NÂNG CAO / HỆ THỐNG
- Nâng version lên v2.0
- Đổi RootNamespace và AssemblyName từ `WinDeployPro` → `TrickLor`

### v1.0 (khởi đầu)
- System Info, Deploy, Optimize, Debloat, Security, Network Fix, Quick Setup, BitLocker, Activity Log, Settings

---

## Cấu trúc dự án

```
TrickLor/
├── App.xaml / App.xaml.cs          # Khởi động ứng dụng, nạp theme
├── MainWindow.xaml / .cs           # Cửa sổ chính, sidebar điều hướng
├── Pages/
│   ├── SystemInfoPage              # Thông tin & theo dõi phần cứng
│   ├── DeployPage                  # Triển khai phần mềm
│   ├── OptimizePage                # Tối ưu hóa hệ thống
│   ├── DebloatPage                 # Gỡ bloatware
│   ├── SecurityPage                # Cài đặt bảo mật
│   ├── NetworkFixPage              # Sửa lỗi mạng
│   ├── QuickSetupPage              # Cấu hình nhanh
│   ├── BitLockerPage               # Quản lý BitLocker
│   ├── StartupManagerPage  ✨      # Quản lý startup registry
│   ├── DiskCleanerPage     ✨      # Dọn file rác hệ thống
│   ├── DriverPage          ✨      # Thông tin driver thiết bị (WMI)
│   ├── ProcessManagerPage  ✨      # Quản lý tiến trình
│   ├── BackupRestorePage   ✨      # Xuất / nhập cài đặt
│   ├── ScheduledTaskPage   ✨      # Task Scheduler viewer
│   ├── WindowsUpdatePage   ✨      # Bật/tắt cập nhật tự động
│   ├── EventLogPage        ✨      # Windows Event Log viewer
│   ├── LogPage                     # Nhật ký hoạt động TrickLor
│   └── SettingsPage                # Giao diện & theme
└── Services/
    ├── SystemInfoService           # Truy vấn phần cứng (WMI, P/Invoke)
    ├── InstallerService            # Gọi winget
    ├── OptimizeService             # Registry + dịch vụ Windows
    ├── DebloatService              # Xóa AppX package, OneDrive
    ├── SecurityService             # Cài đặt bảo mật qua Registry
    ├── NetworkService              # Lệnh sửa mạng
    ├── BitLockerService            # Quản lý BitLocker qua PowerShell
    ├── QuickSetupService           # Tùy chỉnh Explorer & tài khoản
    ├── StartupManagerService ✨    # Đọc/xóa registry Run key
    ├── DiskCleanerService    ✨    # Ước tính + xóa file rác
    ├── DriverService         ✨    # WMI Win32_PnPSignedDriver
    ├── ProcessManagerService ✨    # System.Diagnostics.Process
    ├── BackupRestoreService  ✨    # Copy settings.json
    ├── ScheduledTaskService  ✨    # Gọi schtasks.exe
    ├── WindowsUpdateService  ✨    # Registry Group Policy AU
    ├── EventLogViewerService ✨    # System.Diagnostics.EventLog
    ├── ThemeService                # Áp dụng theme động
    ├── SettingsService             # Lưu/tải cài đặt (JSON)
    └── LogService                  # Ghi nhật ký trong bộ nhớ
```

---

## Công nghệ sử dụng

- **Framework**: WPF trên .NET 10 (`net10.0-windows`)
- **NuGet**: `System.Management` v8.0.0 (WMI queries)
- **Công cụ hệ thống**: `winget`, `PowerShell`, `netsh`, `ipconfig`, `cmd.exe`
- **Registry**: `Microsoft.Win32` cho các thay đổi hệ thống
- **Build**: Self-contained single-file, Windows x64

---

## Lưu ý

- Mọi thao tác thay đổi hệ thống đều được ghi lại trong tab **Nhật ký**.
- Module **Gỡ bloatware** tự tạo điểm khôi phục (System Restore Point) trước khi xóa.
- Cài đặt giao diện (theme, accent color) được lưu tại `%AppData%\TrickLor\settings.json`.
- Nên tạo bản sao lưu hoặc điểm khôi phục trước khi dùng các tính năng tối ưu hóa lần đầu.

---

## Đóng góp

Pull request và issue luôn được chào đón. Vui lòng mở issue trước khi thực hiện thay đổi lớn.

---

## Giấy phép

MIT License — xem file [LICENSE](LICENSE) để biết chi tiết.
