---
name: blazor-postgres-internship
description: Build, fix, and polish the internship warehouse-management web app using Blazor Web (.NET 8) with PostgreSQL. Use when tasks involve enterprise admin UI/UX for catalog screens, CRUD/search behavior, EF Core/Npgsql database connectivity, migration/schema troubleshooting, and preserving existing Blazor binding/validation/service/event logic.
---

# Blazor Postgres Internship Skill

Use this skill to deliver features and fixes for this repository with stable business behavior first.

## Load Project Context Fast

Open these files first:

1. `README.md`
2. `Program.cs`
3. `Components/Layout/MainLayout.razor`
4. `Components/Layout/NavMenu.razor`
5. `Components/Pages/DonViTinhPage.razor`
6. `Components/Pages/DonViTinhPage.razor.css`
7. `Components/Pages/LoaiSanPhamPage.razor`
8. `Components/Pages/LoaiSanPhamPage.razor.css`
9. `Services/Interfaces/*.cs`
10. `Services/*.cs`
11. `Infrastructure/Data/AppDbContext.cs`
12. `Infrastructure/Data/Migrations/*`

Read [`references/project-runbook.md`](references/project-runbook.md) when you need detailed verification and PostgreSQL troubleshooting.

## Follow This Delivery Workflow

1. Restate target in 1-2 lines.
2. List assumptions and risks before editing.
3. Change only the minimum files needed.
4. Keep Blazor business logic stable:
   - Keep route, service calls, and event handlers working.
   - Keep `EditForm`, `DataAnnotationsValidator`, `ValidationMessage`, and model binding behavior unless change is explicitly requested.
5. Verify after each meaningful batch.
6. Report changed files, commands run, and remaining risks.

## Respect Existing Architecture

- Keep current layer boundaries:
  - `Components/Pages`: UI and event flow.
  - `Services`: business operations and DB calls.
  - `Infrastructure/Data`: EF Core context and migrations.
  - `Domain/Entities` + `Models/*`: data contracts and view models.
- Do not move business rules into CSS or layout files.
- Do not add JavaScript unless no Blazor-native solution can satisfy the requirement.

## Code Commenting Standard (Blazor / C# / .NET)

Apply these rules for all code changes in this project.

1. Prefer self-explanatory code first:
   - choose clear names for class/method/variable
   - extract methods for complex branches
   - refactor before adding comments
2. Do not add comments when code is already clear.
3. Do not add comments only to look "professional".
4. Every comment must answer at least one question:
   - why this approach is used
   - which business rule is enforced
   - which risk is avoided
   - what can be misunderstood without the comment
5. If none of the questions above is answered, remove the comment.
6. Comment priority order:
   - Why
   - Business meaning
   - Constraint / validation reason
   - Assumption
   - Side effect
   - Performance caveat
   - Migration / backward-compatibility note

Use `///` XML docs for:

1. public class
2. public interface
3. public method
4. public property only when business meaning is not obvious
5. important service, DTO, and reusable public component

Use `//` for:

1. non-obvious business rule
2. tricky logic branch that is hard to infer from code alone
3. workaround / temporary fix with reason
4. risk warning or side effect warning

`/* ... */` policy:

1. avoid by default
2. use only for short block notes when `//` is not practical

Forbidden comment patterns:

1. line-by-line narration of obvious code actions
2. comments like "assign x", "loop list", "return result"
3. stale comments tied to old business rules
4. long, generic, low-information comments

Blazor-specific comment rules:

1. do not comment obvious `.razor` markup labels and controls
2. comment only when needed for:
   - special render behavior
   - critical lifecycle behavior
   - tricky binding/validation flow
   - non-obvious state transitions
   - explicit reason for `StateHasChanged`, `OnInitializedAsync`, `OnParametersSetAsync`, debounce, or JS interop
3. never add comments like "display title", "save button", "data table"

Service/repository/business-logic rules:

1. comment mandatory business-rule conditions when not self-evident
2. comment non-obvious validation constraints with business reason
3. comment company policy / legacy-data constraints with source context
4. for formulas, comment business meaning of the formula, not arithmetic steps

Examples:

Bad:

```csharp
// increase i
i++;
// loop list
foreach (var item in items) { }
// return result
return result;
```

Good:

```csharp
// Keep old rounding rule to match warehouse reports generated before 2025-01 migration.
// Changing this to MidpointRounding.ToEven would break monthly reconciliation totals.
var roundedQuantity = Math.Round(quantity, 0, MidpointRounding.AwayFromZero);
```

## UI/UX Standard For Internal Admin Screens

Apply these defaults for pages like "Danh mục đơn vị tính" and "Danh mục loại sản phẩm":

1. Use compact header with title on left and primary action on right.
2. Place search in same control row when possible.
3. Render list as primary card and form as secondary card.
4. Keep text minimal and action-oriented.
5. Keep buttons with clear priority:
   - Primary: save/create.
   - Secondary: cancel/reset.
   - Danger: delete.
6. Keep interaction states visible:
   - focus ring
   - hover state
   - disabled state
7. Handle empty list explicitly with a clean empty-state block.
8. Keep validation messages near inputs and avoid duplicated error lines.
9. Prefer CSS isolation (`.razor.css`) over global overrides.

## Reuse Bai 1 UI Format For All Other Screens

Treat `DonViTinhPage` as the baseline UI template and apply the same structure to other internship screens.

Required reuse checklist:

1. Keep page skeleton consistent:
   - compact header
   - control row with search/action
   - list card
   - form card
2. Keep wording minimal and parallel across screens:
   - short title
   - short labels
   - short placeholders
3. Keep interaction behavior consistent:
   - click row to open edit mode
   - click create to open create mode
   - hide form when idle if requested by task
4. Keep form-mode title explicit:
   - create mode uses `Tạo <đối tượng>`
   - edit mode uses `Sửa <đối tượng>`
5. Keep button set consistent:
   - `Lưu`
   - `Hủy`
   - optional `Xóa` only when business flow requires it
6. Keep realtime UX consistent:
   - search updates list as user types
   - character counter updates on input and delete
7. Keep visual consistency:
   - spacing scale and border radius match Bai 1
   - table action style and empty state style match Bai 1
8. Do not copy domain text blindly:
   - only replace entity-specific terms (e.g., đơn vị tính -> loại sản phẩm)
   - keep business rules and validation of each entity intact

## PostgreSQL + EF Core Rules

1. Read connection from `ConnectionStrings:DefaultConnection`.
2. Never hardcode secrets; prefer user secrets or environment variables.
3. Ensure `Npgsql.EntityFrameworkCore.PostgreSQL` usage stays aligned with .NET 8 packages.
4. Keep migration updates deterministic:
   - `dotnet ef migrations add <Name> --output-dir Infrastructure/Data/Migrations`
   - `dotnet ef database update`
5. For startup schema readiness, preserve existing initialization flow in `Program.cs`.
6. For auth errors (`28P01`), surface clear actionable messages and keep stack trace.

## Naming Alignment With Internship Spec (Mandatory)

When the task references `Bai Tap Thuc Tap.docx`, treat document naming as source of truth for both DB schema and code property names.

1. Keep table names exactly as documented (case and underscore sensitive).
2. Keep column names exactly as documented.
3. Keep C# model/view-model property names aligned with documented field names (use the same underscore naming if the spec uses underscore naming).
4. Do not silently normalize names to camelCase/PascalCase variants that differ from the spec fields.
5. Ensure `OnModelCreating` maps are exact and explicit even when property names already match DB fields.
6. If renaming from old style to spec style:
   - update all references in `Domain`, `Models`, `Services`, and `Components`
   - add migration to align model snapshot
   - run `dotnet ef database update` and verify resulting schema names
7. For Bai 1 + Bai 2 + Bai 3 + Bai 4 in this repo, enforce these canonical names:
   - table `tbl_DM_Don_Vi_Tinh`: `Don_Vi_Tinh_ID`, `Ten_Don_Vi_Tinh`, `Ghi_Chu`
   - table `tbl_DM_Loai_San_Pham`: `Loai_San_Pham_ID`, `Ma_LSP`, `Ten_LSP`, `Ghi_Chu`
   - table `tbl_DM_San_Pham`: `San_Pham_ID`, `Ma_San_Pham`, `Ten_San_Pham`, `Loai_San_Pham_ID`, `Don_Vi_Tinh_ID`, `Ghi_Chu`
   - table `tbl_DM_NCC`: `NCC_ID`, `Ma_NCC`, `Ten_NCC`, `Ghi_Chu`

## Delete Hygiene (Mandatory)

Project override for this repository:

1. Keep UI action label as `Xóa` (same UX style as old screens).
2. `Xóa` on UI must be soft delete only:
   - remove record from UI list
   - keep record in DB (`Is_Active = false`)
   - do not hard delete from UI flow
3. List/query methods for catalog screens must only load active records (`Is_Active = true`).
4. Service delete methods must implement: `DeleteAsync(id)` -> set `Is_Active = false`.
5. Confirmation text must be explicit:
   - `Xóa <đối tượng> khỏi danh sách?`
6. If record was already soft-deleted, return a specific message, not generic failure.
7. Do not add extra status column/badge unless explicitly requested by user.

Quick check before finishing:

1. UI still uses old layout/wording and has `Xóa` button.
2. After delete, record disappears from UI.
3. Deleted record still exists in DB with `Is_Active = false`.
4. No hard-delete call remains in service/UI path.

## Definition Of Done

Treat a task complete only when all checks pass:

1. Build succeeds: `dotnet build`
2. App boots without startup exception: `dotnet run`
3. Target page route opens and key user flow works.
4. CRUD/search behavior matches requested business behavior.
5. Validation still works for required fields and max lengths.
6. Data persists and reloads correctly from PostgreSQL.
7. No unrelated files are reformatted or modified.

## Response Contract

When finishing work, always provide:

1. What changed and why.
2. Exact files touched.
3. Commands executed and outcomes.
4. Any unresolved risk and how to test it quickly.
