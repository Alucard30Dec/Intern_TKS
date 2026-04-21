# BlazorApp1 - Bai 1 + Bai 2 + Bai 3 + Bai 4 + Bai 5 + Bai 6 + Bai 7 + Bai 8 + Bai 9 + Bai 10

Ung dung Blazor Server (.NET 8) cho bai thuc tap:

- Bai 1: Danh muc don vi tinh (`/` hoac `/danh-muc/don-vi-tinh`)
- Bai 2: Danh muc loai san pham (`/danh-muc/loai-san-pham`)
- Bai 3: Danh muc san pham (`/danh-muc/san-pham`)
- Bai 4: Danh muc nha cung cap (`/danh-muc/nha-cung-cap`)
- Bai 5: Danh muc kho (`/danh-muc/kho`)
- Bai 6: Phan quyen kho-user (`/phan-quyen/kho-user`)
- Bai 7: Quan ly phieu nhap kho (`/nhap-kho`)
- Bai 8: Hieu chinh thong tin phieu nhap (header only, dung tren `/nhap-kho`)
- Bai 9: Hieu chinh chi tiet phieu nhap (them/sua/xoa dong chi tiet, dung tren `/nhap-kho`)
- Bai 10: In phieu nhap tu man hinh bai 7 (`/nhap-kho/in/{id}`)

Du an dung PostgreSQL + EF Core, co validation va CRUD/phan quyen day du cho Bai 1 den Bai 10.

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
- `20260420071022_AlignTableColumnNamesWithSpec`
- `20260420071328_CleanupLegacyConstraintNames`
- `20260420072259_AlignCodePropertyNamesWithSpec`
- `20260420074221_AddSanPhamCatalogForBai3`
- `20260420075926_AddActiveStatusForCatalogSoftDelete`
- `20260420082628_AddNhaCungCapCatalogForBai4`
- `20260420084121_AddKhoCatalogAndRequireUniqueSupplierCode`
- `20260420085334_AddKhoUserPermissionForBai6`
- `20260420090847_AddNhapKhoManagementForBai7`
- `20260420181849_AddNhapKhoHeaderEditForBai8`

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

Mo URL in trong console, sau do vao menu Bai 1/Bai 2/Bai 3/Bai 4/Bai 5/Bai 6/Bai 7 de kiem tra.

## 6) Smoke test nhanh

1. Bai 1: tao, sua, tim kiem, xoa mem don vi tinh (an khoi UI).
2. Bai 2: tao, sua, tim kiem theo ma/ten/ghi chu, xoa mem loai san pham (an khoi UI).
3. Bai 3: tao, sua, tim kiem theo ma/ten/loai/don vi tinh, xoa mem san pham (an khoi UI).
4. Bai 4: tao, sua, tim kiem theo ma/ten/ghi chu, xoa mem nha cung cap (an khoi UI), ma NCC bat buoc va duy nhat.
5. Bai 5: tao, sua, tim kiem theo ten/ghi chu, xoa mem kho (an khoi UI).
6. Bai 6: them/sua/tim kiem phan quyen kho-user theo `Ma_Dang_Nhap`, `Kho_ID`; bo key `Ma_Dang_Nhap + Kho_ID` la duy nhat; xoa mem an khoi UI.
7. Bai 7: tao va xoa phieu nhap kho, bat buoc `So_Phieu_Nhap_Kho`, `Kho`, `NCC`, `Ngay_Nhap_Kho`; so phieu duy nhat; co luu chi tiet vao `tbl_DM_Nhap_Kho_Raw_Data`.
8. Bai 8: hieu chinh phan header phieu nhap (So phieu/Kho/NCC/Ngay/Ghi chu), du lieu header luu o `tbl_XNK_Nhap_Kho`.
9. Bai 9: hieu chinh chi tiet phieu nhap tren modal rieng gom khung thong tin header (label only) + luoi data chi tiet, cho phep them/sua/xoa dong.
10. Rang buoc Bai 9: khi them bat buoc nhap Ma san pham, So luong, Don gia; khi sua chi duoc sua So luong va Don gia, khong duoc doi Ma san pham.
11. Bai 10: tu danh sach bai 7 nhan nut In de mo form in phieu nhap, xem day du header + chi tiet + tong tri gia, in bang `window.print`.
12. Reload trang, xac nhan du lieu van ton tai.

Luu y nghiep vu xoa:

- Cac danh muc bai 1-3 dang ap dung `Xoa mem` (khong hard delete mac dinh).
- Du lieu lich su duoc giu lai qua cot `Is_Active`.

## 7) Loi thuong gap

- `28P01` (authentication failed): sai `Username/Password`.
- `3D000` (database does not exist): DB chua duoc tao.
- `connection refused`: PostgreSQL chua chay/sai `Host` `Port`.
- `permission denied`: user DB chua du quyen.
