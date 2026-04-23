# Repository AGENTS.md

## CSS Override Guardrails (Mandatory)

Apply these rules for every UI/CSS change in this repository.

1. Scope first:
- Prefer `Component.razor.css` (CSS isolation) for page/component styling.
- Edit `wwwroot/app.css` only for truly shared global styles.

2. Selector discipline:
- Reuse existing class names and patterns before creating new selectors.
- Avoid broad tag selectors (`input`, `button`, `table`) for page tweaks.
- Keep selector specificity low; do not stack deep selector chains.

3. No-force override policy:
- Do not use `!important` unless explicitly approved.
- Do not solve style conflicts by increasing selector weight blindly.

4. Size/radius consistency:
- Use page-level CSS variables (height/radius/spacing) for control sizing.
- Keep input, combobox, and textarea contracts aligned unless user asks otherwise.

5. Verification loop after each CSS batch:
- Confirm class exists in markup (`.razor`) and style file (`.razor.css`).
- Run `dotnet build`.
- If visual result seems unchanged, hard refresh browser (`Ctrl+F5`) and inspect computed style.

6. Change scope:
- Do not refactor unrelated CSS while fixing one UI request.
- Keep diffs small and reviewable.

## Combobox Điền Chữ Contract (Mandatory)

When the user says: `combobox điền chữ`, `combobox gõ để lọc`, or equivalent wording,
always implement the same pattern used in `SanPhamPage` and `KhoUserPage`.

Required implementation:

1. Use a custom text-input combobox, not plain `InputSelect`.
- `input` + dropdown list + toggle button.
- Typing filters options in-memory by keyword.

2. Keep selected id in form model.
- Display text keyword separately (`<entity>Keyword`).
- Selecting an option must update both keyword text and bound id (e.g., `Kho_ID`).

3. Interaction behavior must match:
- Focus opens dropdown.
- Blur closes dropdown with short delay (`Task.Delay`) to allow option click.
- Toggle button opens/closes dropdown.
- Clicking option selects and closes.

4. Validation behavior must match:
- Empty/invalid typed text must reset bound id to `0`.
- Show lookup warning message near the field when needed.
- Keep `ValidationMessage` for the bound id property.

5. Reuse CSS class contract:
- `combo-box`, `combo-box__control`, `form-field__input--combo`
- `combo-box__toggle`, `combo-box__caret`, `combo-box__menu`
- `combo-box__option`, `combo-box__option--active`, `combo-box__empty`
- `form-field__lookup-warning`

6. Blazor CSS isolation rule:
- For `InputText`/`InputTextArea` styling, use `.form-shell ::deep ...` when needed
  so styles apply correctly to rendered input elements.
