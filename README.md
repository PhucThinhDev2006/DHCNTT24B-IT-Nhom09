# AgriOCOP — Hệ thống quản lý kinh doanh truy xuất nguồn gốc sản phẩm nông sản chuẩn OCOP 3 miền tại Trường Đại học Đồng Tháp

[![CI Build & Test](https://github.com/PhucThinhDev2006/IN421104-Nhom09/actions/workflows/ci.yml/badge.svg)](https://github.com/PhucThinhDev2006/IN421104-Nhom09/actions/workflows/ci.yml)
[![Coverage](https://img.shields.io/badge/Coverage-%E2%89%A580%25-brightgreen.svg)]()
[![.NET Version](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/)
[![Database](https://img.shields.io/badge/PostgreSQL-16-blue.svg)](https://www.postgresql.org/)
[![Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-orange.svg)]()

> **Học phần**: Phát triển phần mềm hướng đối tượng  
> **Lớp học phần**: IN421104 — ĐHCNTT24B-IT  
> **Giảng viên hướng dẫn**: ThS. Nguyễn Tuấn Kiệt — Trường Đại học Đồng Tháp  
> **Mã đề tài**: **ĐT08-AgriOCOP (Đề xuất mở rộng)**  
> **Nhóm thực hiện**: **Nhóm 09**  
> **Kho mã nguồn GitHub**: [https://github.com/PhucThinhDev2006/IN421104-Nhom09](https://github.com/PhucThinhDev2006/IN421104-Nhom09)  

---

## 📌 Giới thiệu đề tài

**AgriOCOP** là nền tảng số hoá tích hợp hai trục giá trị: **Quản lý chuỗi sản xuất nông sản sạch gắn với truy xuất nguồn gốc (Farm)** và **Kinh doanh phân phối đặc sản OCOP 3 miền bảo vệ sức khoẻ cộng đồng tại Trường Đại học Đồng Tháp (Table)**.

### Mục tiêu cốt lõi:
1. **Kiểm soát an toàn thực phẩm nghiêm ngặt**: Ứng dụng động cơ kiểm soát thời gian cách ly thuốc BVTV (**Pre-Harvest Interval - PHI**), kiên quyết chặn thu hoạch và ngừng bán nếu vi phạm ngưỡng an toàn hóa chất.
2. **Minh bạch hóa dòng thời gian canh tác**: Số hóa nhật ký đồng ruộng điện tử, cơ chế niêm phong đóng băng nhật ký chống gian lận dữ liệu.
3. **Tem truy xuất nguồn gốc QR Code công khai**: Khách hàng quét mã bằng điện thoại để xem toàn bộ hành trình sản phẩm từ lúc gieo hạt đến khi đóng gói mà không cần đăng nhập.
4. **Sàn kết nối đặc sản OCOP 3 miền**: Quảng bá và tiêu thụ các sản phẩm OCOP đạt chuẩn (Xoài Cát Chu Cao Lãnh, Sen Đồng Tháp, Trà Thái Nguyên, Mè xửng Huế...) phục vụ trực tiếp cán bộ, giảng viên, sinh viên Trường Đại học Đồng Tháp.

---

## 🏛 Kiến trúc hệ thống (Clean Architecture)

Dự án được cấu trúc theo mô hình **Clean Architecture 4 tầng** chuẩn mực trên nền tảng **.NET 10**:

```text
IN421104-Nhom09/
├── src/
│   ├── AgriOCOP.Domain/           # Thực thể (Entities), Value Objects, Enums, Invariants, Repository Interfaces
│   ├── AgriOCOP.Application/      # Use Cases, DTOs, Mapping, Validators, Dapper Query Interfaces
│   ├── AgriOCOP.Infrastructure/   # EF Core 10 (PostgreSQL, Migrations, Seeding), Dapper Queries, JWT/Argon2id, QR Service
│   └── AgriOCOP.API/              # Controllers RESTful API, Minimal API, ProblemDetails (RFC 7807), Serilog, Swagger OpenAPI
├── tests/
│   └── AgriOCOP.Tests/            # Kiểm thử xUnit, Moq, SQLite in-memory, WebApplicationFactory
├── .github/workflows/
│   └── ci.yml                     # Pipeline GitHub Actions (Build -> Test -> Coverage)
├── docker-compose.yml             # Cấu hình khởi chạy nhanh API và PostgreSQL
├── Dockerfile                     # Đóng gói Multi-stage build .NET 10 tối ưu dung lượng
└── README.md                      # Tài liệu hướng dẫn dự án
```

---

## ⚖️ Quy tắc nghiệp vụ cốt lõi (Business Invariants)

* **QT01 (Kiểm soát cách ly thuốc BVTV - PHI)**: Hệ thống chặn hoàn toàn thao tác tạo lệnh thu hoạch cho lô canh tác nếu:
  $$\text{Ngày thu hoạch} - \text{Ngày phun thuốc cuối cùng} < \text{Số ngày cách ly quy định (PHI)}$$
  *(Ném ngoại lệ `PreHarvestIntervalViolationException` kèm số ngày còn thiếu).*
* **QT02 (Tính bất biến của nhật ký canh tác)**: Nhật ký một khi đã được Kỹ sư HTX xác minh và chuyển trạng thái `Locked` thì không thể sửa đổi hoặc xóa nhằm chống gian lận. *(Ném `FarmingLogLockedException`).*
* **QT03 (Điều kiện niêm yết bán sản phẩm)**: Nông sản tươi chỉ được mở bán khi liên kết với một Lô thu hoạch (`HarvestBatch`) hợp lệ và có chứng nhận OCOP (3-5 sao) hoặc VietGAP còn thời hạn.
* **QT04 (Bảo mật Tem QR tra cứu)**: Mã QR trên bao bì chứa chuỗi mã định danh duy nhất (`TraceToken`), cho phép người tiêu dùng tra cứu công khai không cần tài khoản, đồng thời ghi log lượt quét.
* **QT05 (Toàn vẹn tồn kho đơn hàng)**: Đơn đặt hàng nông sản tươi kiểm tra tức thời tồn kho khả dụng; không cho phép đặt vượt quá số lượng hàng sẵn có tại kho.

---

## 🚀 Hướng dẫn cài đặt và chạy bằng Docker

### Yêu cầu môi trường
* Đã cài đặt [Docker Desktop](https://www.docker.com/products/docker-desktop/) (Windows/Linux/macOS).
* Git.

### Khởi chạy toàn bộ hệ thống (Chỉ 1 câu lệnh)

```bash
# 1. Clone kho mã nguồn chính thức của Nhóm 09
git clone https://github.com/PhucThinhDev2006/IN421104-Nhom09.git
cd IN421104-Nhom09

# 2. Khởi chạy hệ thống bằng Docker Compose (Tự động build API, chạy Postgres, chạy Migrations và Seeding dữ liệu)
docker compose up -d --build
```

> **Lưu ý**: Nếu cổng `5432` trên máy của bạn đã bị chiếm bởi dịch vụ PostgreSQL khác, bạn có thể chỉnh biến môi trường `DB_PORT=5433` trong file `.env` hoặc file `docker-compose.yml`.

### Dừng hệ thống
```bash
docker compose down -v
```

---

## 🌐 Đường dẫn truy cập & Swagger UI

Sau khi container khởi chạy thành công:
* **Giao diện tài liệu Swagger UI**: [http://localhost:8080/swagger](http://localhost:8080/swagger)
* **Endpoint API gốc**: `http://localhost:8080/api/v1`
* **Endpoint tra cứu nguồn gốc nông sản công khai (Không cần token)**:
  `GET http://localhost:8080/api/v1/public/traceability/{traceToken}`

---

## 🔑 Danh sách tài khoản mẫu (Seed Accounts)

Hệ thống đã tự động gieo sẵn dữ liệu mẫu (`SeedAsync`) cho từng vai trò trong chuỗi cung ứng:

| Vai trò | Email đăng nhập | Mật khẩu mặc định | Quyền hạn và Mô tả |
| :--- | :--- | :--- | :--- |
| **Quản trị viên HTX (Admin)** | `xxxxxx` | `xxxxxx` | Quản lý hệ thống, duyệt nhà vườn, quản lý danh mục OCOP 3 miền. |
| **Kỹ sư kiểm định (Inspector)** | `xxxxxx` | `xxxxxx` | Giám sát kỹ thuật, kiểm tra vườn, chốt khóa nhật ký nông vụ (`Locked`). |
| **Xã viên / Nhà vườn (Farmer)** | `xxxxxx` | `xxxxxx` | Quản lý Lô Xoài Cát Chu Cao Lãnh, ghi nhật ký phân bón, phun thuốc BVTV. |
| **Khách hàng / Sinh viên (Customer)** | `xxxxxx` | `xxxxxx` | Đặt mua nông sản đặc sản OCOP, xem lịch sử đơn hàng. |
| **Khách vãng lai (Public)** | *(Không cần đăng nhập)* | *(Không cần mật khẩu)* | Quét mã QR tem nhãn để tra cứu dòng thời gian canh tác sạch. |

> *Mật khẩu đều được băm an toàn bằng thuật toán **Argon2id**. Xác thực qua chuẩn **JWT (Access Token & Refresh Token)**.*

---

## 🧪 Hướng dẫn chạy kiểm thử tự động (Testing)

Dự án sử dụng **xUnit** kết hợp **Moq** và CSDL **SQLite in-memory** nên không phụ thuộc vào PostgreSQL khi chạy test.

### Chạy toàn bộ các ca kiểm thử:
```bash
dotnet test tests/AgriOCOP.Tests/AgriOCOP.Tests.csproj --logger "console;verbosity=detailed"
```

### Đo độ bao phủ mã nguồn (Code Coverage $\ge 50\%$ theo chuẩn BTL):
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:Threshold=50
```

---

## 👥 Bảng phân công nhiệm vụ thành viên (Nhóm 09)

*(Căn cứ theo Bảng 2 và Bảng 5.1 trong mẫu báo cáo BTL của Khoa — dùng đối chiếu với lịch sử Commit trên GitHub)*

| STT | MSSV | Họ và tên | Vai trò | Nhánh Git phụ trách | Nhiệm vụ phụ trách cụ thể (Chức năng, Lớp, Tệp mã nguồn) |
| :---: | :---: | :--- | :---: | :--- | :--- |
| **1** | `0024417329` | **Nguyễn Huỳnh Phúc Thịnh** | **Nhóm trưởng** | `feat/coop-farms-cropbatch-phi` | - Khởi tạo kiến trúc Clean Architecture 4 tầng (.NET 10), cấu hình CI GitHub Actions và bộ khung kiểm thử xUnit.<br>- **Nền tảng Domain dùng chung**: Lớp cơ sở trừu tượng `BaseEntity` (Guid Id, Audit timestamps), các `Enums` (`CropBatchStatus`, `OcopStarRating`), `DomainException`.<br>- **Quản lý HTX, Nông hộ & Lô mùa vụ (QT01)**: Thực thể `Cooperative`, `Farm`, `CropBatch`, thuật toán tính toán ngày cách ly thuốc BVTV an toàn, ném ngoại lệ `PreHarvestIntervalViolationException`.<br>- **Kiểm thử tự động (Unit Test AAA)**: Viết bộ kiểm thử `CropBatchTests` xác thực 100% quy tắc thu hoạch an toàn PHI. |
| **2** | `0024415463` | **Đặng Hiếu** | Thành viên | `feat/farming-logs-lock` | - **Nhật ký đồng ruộng điện tử VietGAP (QT02)**: Thực thể `FarmingLog`, nhật ký bón phân, tưới tiêu, phun thuốc BVTV.<br>- **Cơ chế niêm phong chống gian lận**: Trạng thái `Locked`, ném ngoại lệ `FarmingLogLockedException` khi có hành vi sửa đổi trái phép.<br>- **Tầng Hạ tầng EF Core 10**: Cấu hình `AgriOCOPDbContext`, Fluent API mapping, Migrations và Seeding dữ liệu mẫu.<br>- Viết Unit Test cho nghiệp vụ nhật ký đồng ruộng. |
| **3** | `0024417763` | **Nguyễn Diệp Phát** | Thành viên | `feat/ocop-orders-dapper-reports` | - **Danh mục Đặc sản OCOP & Đơn hàng (QT03, QT05)**: Thực thể `OcopProduct`, `Order`, `OrderItem`, kiểm soát tồn kho nông sản tươi an toàn.<br>- **Báo cáo tối ưu Dapper**: Xây dựng truy vấn SQL thuần `DapperTraceabilityReportQuery` thống kê sản lượng HTX, tỷ lệ đạt chuẩn PHI và Timeline mùa vụ.<br>- Viết Integration Test cho luồng đặt hàng nông sản và báo cáo Dapper. |
| **4** | `0024417853` | **Nguyễn Thành Lộc** | Thành viên | `feat/auth-security-traceability-qr` | - **Bảo mật & Xác thực**: `AuthService`, `JwtTokenGenerator`, băm mật khẩu `Argon2idPasswordHasher`, phân quyền 4 Role (Admin, Inspector, Farmer, Customer).<br>- **Quản lý Thu hoạch & Tem QR Truy xuất (QT04)**: Thực thể `HarvestBatch`, phát hành mã token bảo mật `TraceToken`, động cơ sinh tem QR (`QRCodeService`), Controller tra cứu công khai `TraceabilityPublicController`.<br>- Viết Unit Test cho tầng Xác thực và Động cơ tra cứu QR. |

---

## 📦 Cấu trúc gói nộp bài cuối kỳ (Đúng quy cách)

```text
IN421104-DT08-Nhom09.zip
├── BaoCao/
│   ├── IN421104-DT08-Nhom09-BaoCao.docx
│   ├── IN421104-DT08-Nhom09-BaoCao.pdf
│   └── IN421104-DT08-Nhom09-TrinhBay.pptx
├── MinhChung/
│   ├── link-video.txt
│   ├── 01-swagger.png
│   ├── 02-dang-nhap-admin.png
│   ├── 03-kiem-thu-coverage.png
│   ├── 04-github-actions.png
│   └── …
└── link-github.txt  (https://github.com/PhucThinhDev2006/IN421104-Nhom09.git)
```

---

## 📄 Tài liệu tham khảo
1. Bộ bài giảng & Code mẫu môn học *Phát triển phần mềm hướng đối tượng (IN4211)* — ThS. Nguyễn Tuấn Kiệt, Trường ĐH Đồng Tháp.
2. Microsoft Learn (2025), *ASP.NET Core Documentation & Clean Architecture Guidance*.
3. Bộ Nông nghiệp & PTNT, *Quy chuẩn kỹ thuật quốc gia về thực hành sản xuất nông nghiệp tốt (VietGAP) & Chương trình mỗi xã một sản phẩm (OCOP)*.
