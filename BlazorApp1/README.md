# BlazorApp1 - Bài 1 đến Bài 17

Ứng dụng Blazor Server (.NET 8) phục vụ bài thực tập quản lý kho:

- Danh mục: đơn vị tính, loại sản phẩm, sản phẩm, nhà cung cấp, kho.
- Phân quyền kho theo người dùng.
- Quản lý phiếu nhập kho và phiếu xuất kho.
- In phiếu nhập/xuất theo mẫu.
- Báo cáo chi tiết nhập, chi tiết xuất, xuất-nhập-tồn.

Dự án sử dụng PostgreSQL + EF Core, đã có validation nghiệp vụ và seed dữ liệu mẫu cho toàn bộ luồng Bài 1-17.

## 1) Yêu cầu môi trường

- .NET SDK 8.0 trở lên
- PostgreSQL 14 trở lên (hoặc Docker)
- EF Core CLI

```powershell
dotnet tool install --global dotnet-ef
dotnet ef --version
```

## 2) Khởi tạo PostgreSQL nhanh (Docker)

```powershell
docker run --name tks-postgres -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=tks_intern_catalog -p 5432:5432 -d postgres:16
```

## 3) Cấu hình kết nối database

Ứng dụng đọc chuỗi kết nối từ `ConnectionStrings:DefaultConnection`.

Khuyến nghị dùng User Secrets:

```powershell
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=tks_intern_catalog;Username=postgres;Password=postgres;Include Error Detail=true;Pooling=true"
```

## 4) Migration và cập nhật schema

Cập nhật database theo migration:

```powershell
dotnet ef database update
```

Nếu thay đổi model:

```powershell
dotnet ef migrations add TenMigrationMoi --output-dir Infrastructure/Data/Migrations
dotnet ef database update
```

Lưu ý khi dùng `Update-Database`:

- Lệnh `Update-Database` chỉ chạy trong **Package Manager Console** của Visual Studio.
- Nếu dùng terminal (PowerShell/CMD), dùng `dotnet ef database update`.

## 5) Seed dữ liệu mẫu

Dữ liệu mẫu được gắn trong migration và thiết kế theo hướng idempotent.  
Mỗi lần chạy:

```powershell
dotnet ef database update
```

hệ thống sẽ đảm bảo dữ liệu mẫu được bổ sung/cập nhật lại theo logic seed hiện có.

Mặc định ứng dụng không tự chạy migrate khi startup:

- `Database:AutoMigrateOnStartup=false`
- `Database:EnableLegacySchemaBootstrapOnStartup=false`

## 6) Chạy ứng dụng

```powershell
dotnet restore
dotnet build
dotnet run
```

Mở URL được in trong console, sau đó truy cập các màn hình từ Bài 1 đến Bài 17 để kiểm tra.

## 7) Danh sách route theo bài

- Bài 1: Danh mục đơn vị tính (`/` hoặc `/danh-muc/don-vi-tinh`)
- Bài 2: Danh mục loại sản phẩm (`/danh-muc/loai-san-pham`)
- Bài 3: Danh mục sản phẩm (`/danh-muc/san-pham`)
- Bài 4: Danh mục nhà cung cấp (`/danh-muc/nha-cung-cap`)
- Bài 5: Danh mục kho (`/danh-muc/kho`)
- Bài 6: Phân quyền kho-user (`/phan-quyen/kho-user`)
- Bài 7-10: Nhập kho + in phiếu nhập (`/nhap-kho`, `/nhap-kho/{id}/print`)
- Bài 11-14: Xuất kho + in phiếu xuất (`/xuat-kho`, `/xuat-kho/{id}/print`)
- Bài 15: Báo cáo chi tiết hàng nhập (`/bao-cao/chi-tiet-nhap`)
- Bài 16: Báo cáo chi tiết hàng xuất (`/bao-cao/chi-tiet-xuat`)
- Bài 17: Báo cáo xuất nhập tồn (`/bao-cao/xuat-nhap-ton`)

## 8) Smoke test nhanh

1. Kiểm tra CRUD + tìm kiếm cho các danh mục Bài 1-5.
2. Kiểm tra phân quyền kho-user (Bài 6), không cho trùng cặp đăng nhập/kho.
3. Tạo phiếu nhập và phiếu xuất, kiểm tra validation header + chi tiết.
4. Kiểm tra đơn vị tiền `VNĐ` và `USD`:
   - `VNĐ`: không có phần thập phân.
   - `USD`: tối đa 2 chữ số thập phân.
5. In phiếu nhập/xuất, đối chiếu dữ liệu in với dữ liệu phiếu.
6. Chạy 3 báo cáo và đối chiếu số liệu tổng hợp.

## 9) Lưu ý nghiệp vụ

- Danh mục áp dụng xóa mềm qua cột `Is_Active`.
- Chi tiết phiếu phải còn ít nhất 1 dòng hợp lệ.
- Database đã có ràng buộc kiểm tra tiền tệ, ngày chứng từ, số lượng, đơn giá và khóa chống trùng dòng chi tiết đang hoạt động.

## 10) Lỗi thường gặp

- `28P01`: sai `Username` hoặc `Password`.
- `3D000`: database chưa tồn tại.
- `connection refused`: PostgreSQL chưa chạy hoặc sai `Host/Port`.
- `permission denied`: user database chưa đủ quyền.
