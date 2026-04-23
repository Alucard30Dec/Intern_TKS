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

## Code-Change Guardrails (Mandatory)

Apply these defaults for all Blazor / C# / .NET tasks in this repo.

1. Fix correct business behavior first; keep scope tight and controllable.
2. Prefer focused, low-risk changes; keep unrelated behavior unchanged.
3. Do not patch temporarily, do not refactor lan man, do not generic hóa sớm.
4. "Minimal change" means minimal business scope and risk, not "fewest lines at any cost".
5. Cleanup nhỏ is allowed only inside touched area when it is directly related, low risk, and clearly improves readability/testability.
6. Allowed cleanup in touched area:
   - rename unclear local symbols
   - split a long method into small focused methods
   - remove small local duplication
   - add missing validation/error message clarity
   - remove dead code in the same area
7. Disallowed in small fixes:
   - architecture-level refactor
   - wide rename across unrelated modules
   - new abstractions/generic layers without stable pattern
8. Every change must serve at least one goal:
   - bug fix
   - business-rule clarity
   - maintainability risk reduction
   - testability improvement
   - readability improvement

## Naming Consistency (Mandatory)

1. One business concept must use one canonical name across entity/DTO/service/component/route/print model.
2. C# naming rules:
   - PascalCase: class, record, enum, method, public property
   - camelCase: parameter, local variable
   - `_camelCase`: private field
   - interface starts with `I`
3. Prefer clear names over short names.
4. Avoid vague names (`data`, `info`, `item`, `manager`, `common`) unless role is explicit.
5. Keep DB schema names exactly as spec; map them explicitly in EF Core (`ToTable`, `HasColumnName`) instead of forcing underscore naming into C# public properties.

## Readability, Reuse, And Refactor Rules

1. Readability/maintainability > cleverness.
2. Keep business rules in service/domain; UI only orchestrates interaction state and event flow.
3. Reuse only when logic has same business nature and changes together over time.
4. If duplication is still small and pattern is unclear, keep explicit code; avoid wrong abstraction.
5. If an abstraction requires many type flags/if-switch branches, prefer simpler design.

## Pre-Edit Decision Gate (Mandatory)

Before editing, check:

1. What exact business outcome must change?
2. Which files/functions are truly required?
3. Is cleanup needed so touched code does not become harder to maintain?
4. Does cleanup increase review/test/rollback risk?

Do cleanup in same patch only when: direct + low risk + clearly improves readability.
Otherwise, split into a separate task.

## Hotfix Rules (Production-Sensitive)

1. For hotfix/critical bugs, minimize behavior change surface.
2. Avoid large refactor in same patch.
3. Keep rollback/test/review easy.
4. Only allow tiny, safe cleanup.

## Comment Rules When Editing

1. Do not comment what code already says.
2. Comment only for: why, business rule, assumption, edge case, workaround, technical debt.
3. Prefer better naming/extraction over explanatory noise comments.

## Quick Review Checklist: "Sửa Code Đúng Chuẩn"

1. Scope focused to requested behavior.
2. No unrelated module changes.
3. Naming consistent with project terms and C# conventions.
4. No temporary patch / no early over-abstraction.
5. Touched code is clearer than before.
6. Validation/error messages are explicit enough for business/debug flow.
7. Build passes; key flow manually testable.
8. Patch is reviewable and rollback-friendly.

## Decision Matrix: Focused Fix vs Cleanup

- Bugfix nhỏ, code vẫn rõ -> focused fix only.
- Bugfix nhỏ, vùng code rối -> focused fix + small local cleanup.
- Medium business change -> allow moderate refactor only to keep code clear/testable.
- Production hotfix -> behavior-minimal patch, cleanup cực hạn chế.
- Reuse/generic idea but pattern chưa ổn -> keep explicit code for now.
- Small duplication but current code still clear -> duplication can stay.

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

## DB Schema Alignment With Internship Spec (Mandatory)

When task references `Bai Tap Thuc Tap.docx`, treat DB schema naming in the document as source of truth.

1. Keep DB table/column names exactly as documented (case/underscore sensitive).
2. Keep EF Core mapping explicit in `OnModelCreating` via `ToTable` and `HasColumnName`.
3. In C# code, keep naming clean and consistent (PascalCase for public members); do not leak DB underscore style into all application layers by default.
4. If schema change is required, update migrations and verify update path:
   - `dotnet ef migrations add <Name> --output-dir Infrastructure/Data/Migrations`
   - `dotnet ef database update`

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
