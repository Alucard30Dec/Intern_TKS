# BlazorApp1 - Bai 1 + Bai 2 Danh Muc

Ung dung Blazor Server (.NET 8) cho bai thuc tap:

- Bai 1: Danh muc don vi tinh (`/` hoac `/danh-muc/don-vi-tinh`)
- Bai 2: Danh muc loai san pham (`/danh-muc/loai-san-pham`)

Du an dung PostgreSQL + EF Core, co validation va CRUD day du cho ca 2 danh muc.

## 1) Yeu cau moi truong

- .NET SDK 8.0+
- PostgreSQL 14+ (hoac Docker)
- EF CLI:

```powershell
dotnet tool install --global dotnet-ef
dotnet ef --version
```

## 2) Khoi tao PostgreSQL nhanh

```powershell
docker run --name tks-postgres -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=tks_intern_catalog -p 5432:5432 -d postgres:16
```

## 3) Cau hinh connection string

App doc tu `ConnectionStrings:DefaultConnection`.

Khuyen nghi dung User Secrets:

```powershell
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=tks_intern_catalog;Username=postgres;Password=postgres;Include Error Detail=true;Pooling=true"
```

## 4) Tao/cap nhat schema

Migration hien co:

- `20260420030829_InitialPostgres`
- `20260420064607_AlignSchemaForBai2`

Cap nhat DB:

```powershell
dotnet ef database update
```

Neu thay doi model:

```powershell
dotnet ef migrations add TenMigrationMoi --output-dir Infrastructure/Data/Migrations
dotnet ef database update
```

## 5) Chay ung dung

```powershell
dotnet restore
dotnet build
dotnet run
```

Mo URL in trong console, sau do vao menu Bai 1/Bai 2 de kiem tra.

## 6) Smoke test nhanh

1. Bai 1: tao, sua, tim kiem, xoa don vi tinh.
2. Bai 2: tao, sua, tim kiem theo ma/ten/ghi chu, xoa loai san pham.
3. Reload trang, xac nhan du lieu van ton tai.

## 7) Loi thuong gap

- `28P01` (authentication failed): sai `Username/Password`.
- `3D000` (database does not exist): DB chua duoc tao.
- `connection refused`: PostgreSQL chua chay/sai `Host` `Port`.
- `permission denied`: user DB chua du quyen.
