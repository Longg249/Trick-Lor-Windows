# Changelog — TrickLor

Tất cả thay đổi đáng chú ý của dự án được ghi lại tại đây.

---

## [v2.0] — 2026-04-28

### Tính năng mới — Công cụ thực dụng

#### 🚦 Startup Manager
- Xem toàn bộ ứng dụng đăng ký khởi động cùng Windows từ hai nguồn: `HKCU\...\Run` (người dùng) và `HKLM\...\Run` (hệ thống)
- Hiển thị tên ứng dụng, nguồn registry và đường dẫn lệnh
- Xóa mục được chọn khỏi registry với xác nhận trước khi thực hiện
- Nút làm mới để cập nhật danh sách tức thì

#### 🧽 Disk Cleaner
- Ước tính dung lượng có thể giải phóng trước khi xóa (không xóa trước khi người dùng đồng ý)
- Hỗ trợ 6 loại file rác:
  - Windows Update Cache (`SoftwareDistribution\Download`)
  - Prefetch (`C:\Windows\Prefetch`)
  - Windows Error Reports (`WER\ReportQueue`)
  - Thumbnail Cache (`Explorer\thumbcache_*.db`)
  - Temp Files (`%TEMP%`, `%windir%\Temp`)
  - Recycle Bin
- Thanh trạng thái hiển thị tiến trình từng bước

#### 🔌 Driver Info
- Liệt kê toàn bộ driver thiết bị qua WMI (`Win32_PnPSignedDriver`)
- Hiển thị: tên thiết bị, phiên bản, nhà sản xuất, trạng thái, tình trạng chữ ký số
- Cảnh báo driver chưa ký (`⚠️`) hoặc có trạng thái bất thường
- Bộ lọc tìm kiếm nhanh theo tên hoặc nhà sản xuất
- Thanh loading vì WMI query có thể mất vài giây

#### ⚙ Process Manager
- Liệt kê tất cả tiến trình đang chạy, sắp xếp theo RAM từ cao xuống thấp
- Hiển thị: tên tiến trình, PID, RAM đang dùng (MB)
- Kết thúc tiến trình được chọn với xác nhận trước khi thực hiện
- Nút làm mới để lấy snapshot mới nhất

---

### Tính năng mới — Nâng cao

#### 💾 Backup & Restore
- Xuất (`Export`): sao lưu file `settings.json` ra vị trí tùy chọn qua Save dialog
- Nhập (`Import`): khôi phục cài đặt từ file đã sao lưu qua Open dialog
- Hiển thị đường dẫn file nguồn và trạng thái tồn tại
- Cảnh báo nếu chưa có file cài đặt để xuất

#### 🗓 Scheduled Tasks
- Xem toàn bộ tác vụ trong Windows Task Scheduler qua `schtasks /query`
- Hiển thị: tên tác vụ, lần chạy tiếp theo, trạng thái
- Bật / Tắt / Xóa tác vụ được chọn trực tiếp từ giao diện
- Thanh loading vì schtasks có thể mất vài giây trên hệ thống nhiều tác vụ

#### 🔄 Windows Update Manager
- Kiểm tra trạng thái hiện tại của Windows Update (đang chạy hay đã tạm dừng)
- Tạm dừng cập nhật tự động qua Group Policy registry (`NoAutoUpdate`, `AUOptions`)
- Tiếp tục cập nhật bằng cách xóa các khóa registry đã đặt
- Nút mở thẳng Windows Update Settings
- Hiển thị trạng thái bằng màu sắc trực quan (xanh / cam)

#### 📋 Event Log Viewer
- Đọc nhật ký sự kiện Windows từ 3 nguồn: System, Application, Security
- Tùy chọn lọc chỉ hiển thị lỗi (`Error`) và cảnh báo (`Warning`), bỏ qua `Information`
- Giới hạn 300 mục gần nhất để tránh quá tải giao diện
- Hiển thị: thời gian, mức độ, nguồn, dòng đầu thông báo

---

### Cải thiện giao diện & cấu trúc

- **Sidebar** tổ chức lại thành 5 nhóm rõ ràng: TỔNG QUAN / TÍNH NĂNG / CÔNG CỤ / NÂNG CAO / HỆ THỐNG
- **Version badge** nâng từ `v1.0` → `v2.0` trong sidebar
- **RootNamespace** và **AssemblyName** đổi từ `WinDeployPro` → `TrickLor` (toàn bộ 39 file)
- Tất cả trang mới đều hỗ trợ dark/light mode qua Dynamic Resources

---

## [v1.0] — khởi đầu

### Tính năng ban đầu
- 💻 **System Info** — Theo dõi CPU, RAM, Disk, Pin theo thời gian thực (cập nhật mỗi 2 giây)
- 🚀 **Deploy** — Cài đặt 34+ ứng dụng phổ biến qua `winget`
- ⚡ **Optimize** — Tắt dịch vụ nền, tối ưu hiệu suất, dọn dẹp bộ nhớ, cài đặt quyền riêng tư
- 🧹 **Debloat** — Xóa ứng dụng Windows tích hợp với System Restore Point tự động
- 🔒 **Security** — Quản lý UAC, SmartScreen, Firewall, thông báo bảo mật
- 🔧 **Network Fix** — Reset Winsock/TCP-IP, flush DNS, bật chia sẻ file, kiểm tra kết nối
- ⚡ **Quick Setup** — Tùy chỉnh Explorer, menu chuột phải, taskbar, đổi tên/mật khẩu tài khoản
- 🔑 **BitLocker** — Mã hóa/giải mã ổ đĩa, sao lưu recovery key
- 📋 **Activity Log** — Ghi lại toàn bộ thao tác kèm timestamp
- ⚙ **Settings** — Chế độ sáng/tối, accent color, lưu tùy chỉnh tự động
