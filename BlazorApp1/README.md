# BlazorApp1 - Bai 1 Danh Muc Don Vi Tinh

Ung dung da duoc toi gian de mo web vao truc tiep Bai 1 (`/`) va su dung PostgreSQL voi EF Core migration.

## 1) Yeu cau moi truong

- .NET SDK 8.0+
- PostgreSQL 14+ (hoac Docker)
- Cong cu EF CLI:

```powershell
dotnet tool install --global dotnet-ef
```

Neu da cai roi:

```powershell
dotnet ef --version
```

## 2) Khoi tao PostgreSQL

### Cach A: Dung Docker (nhanh nhat)

```powershell
docker run --name tks-postgres -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=tks_intern_bai1 -p 5432:5432 -d postgres:16
```

### Cach B: Dung PostgreSQL da cai san

Dang nhap `psql` bang tai khoan admin va chay:

```sql
CREATE DATABASE tks_intern_bai1;
CREATE USER tks_user WITH PASSWORD 'StrongPassword123!';
GRANT ALL PRIVILEGES ON DATABASE tks_intern_bai1 TO tks_user;
```

## 3) Cau hinh chuoi ket noi

Du an doc connection string tu `ConnectionStrings:DefaultConnection`.

### Cach 1: Sua `appsettings.Development.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=tks_intern_bai1;Username=postgres;Password=postgres;Include Error Detail=true;Pooling=true"
  }
}
```

### Cach 2 (khuyen nghi): Dung User Secrets

```powershell
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=tks_intern_bai1;Username=tks_user;Password=StrongPassword123!;Include Error Detail=true;Pooling=true"
```

## 4) Tao/cap nhat schema

Du an da cau hinh `Database.MigrateAsync()` luc startup, nen khi app ket noi duoc DB thi migration se tu ap dung.

Ban co the chay tay truoc:

```powershell
dotnet ef database update
```

Neu sau nay doi model:

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

Mo trinh duyet tai URL duoc in ra console. Trang mac dinh se vao thang Bai 1 (`/`).

## 6) Loi thuong gap va cach xu ly

- `Connection refused`: PostgreSQL chua chay hoac sai `Host/Port`.
- `password authentication failed`: sai `Username/Password`.
- `database does not exist`: tao DB `tks_intern_bai1` truoc.
- `permission denied`: user chua du quyen tren database/schema.
- App chay nhung khong co bang: chay `dotnet ef database update`, kiem tra migration trong `Infrastructure/Data/Migrations`.
