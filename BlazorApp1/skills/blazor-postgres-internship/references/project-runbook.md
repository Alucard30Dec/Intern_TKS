# Project Runbook (Blazor .NET 8 + PostgreSQL)

## 1) Environment Baseline

1. Install `.NET SDK 8.x`.
2. Ensure PostgreSQL is running.
3. Ensure package restore works.

Commands:

```powershell
dotnet --info
dotnet restore
dotnet build
```

## 2) Configure Connection String

Use key: `ConnectionStrings:DefaultConnection`

Sample:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=tks_intern_bai1;Username=postgres;Password=postgres;Include Error Detail=true;Pooling=true"
  }
}
```

Prefer secrets for real credentials:

```powershell
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=tks_intern_bai1;Username=<user>;Password=<password>;Include Error Detail=true;Pooling=true"
```

## 3) Database Creation (If Missing)

```sql
CREATE DATABASE tks_intern_bai1;
CREATE USER tks_user WITH PASSWORD 'StrongPassword123!';
GRANT ALL PRIVILEGES ON DATABASE tks_intern_bai1 TO tks_user;
```

## 4) Migration And Schema

```powershell
dotnet ef database update
```

When model changes:

```powershell
dotnet ef migrations add <MigrationName> --output-dir Infrastructure/Data/Migrations
dotnet ef database update
```

## 5) Common Runtime Errors

`28P01: password authentication failed`

- Wrong user/password in connection string.
- Fix credential, restart app.

`3D000: database does not exist`

- Database name is wrong or not created.
- Create DB and rerun migration.

`connection refused`

- PostgreSQL service is down or wrong host/port.
- Start service, verify `localhost:5432`.

`permission denied`

- User lacks privilege on DB/schema.
- Grant required permissions.

## 6) Manual Smoke Test Checklist

1. Open app and navigate to Bài 1 (Đơn vị tính).
2. Create a valid record.
3. Edit the same record.
4. Search by name and note, verify realtime filtering.
5. Delete a record and verify list refresh.
6. Repeat the same flow for Bài 2 (Loại sản phẩm).
7. Reload page and verify data still exists.

## 7) UI/UX Acceptance Snapshot

1. Header, search, and actions align on one control row (desktop).
2. Labels are short and consistent (`Tên`, `Ghi chú`).
3. Form mode clearly indicates create vs edit state.
4. Validation messages are concise and not duplicated.
5. Table empty state is visible and understandable.

## 8) Rollout To Other Internship Screens (Based On Bai 1)

Use this sequence whenever applying Bai 1 format to another page:

1. Copy layout structure only, not business logic.
2. Keep target page route and service calls unchanged.
3. Map terms:
   - `Tên`, `Ghi chú` remain short and contextual.
   - Use `<entity>` labels in create/edit mode titles.
4. Align behavior:
   - realtime search
   - row-click edit mode
   - create-mode form toggle
   - `Lưu` and `Hủy` action pair
5. Validate target page end-to-end:
   - create
   - update
   - delete (if applicable)
   - search
   - empty state
