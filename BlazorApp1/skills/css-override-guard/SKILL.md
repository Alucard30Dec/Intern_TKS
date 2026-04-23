---
name: css-override-guard
description: Prevent unintended CSS overrides and UI regressions in Blazor screens. Use when editing `.razor.css`, shared `wwwroot/app.css`, component class names, input/combo sizing, or responsive styles where selector specificity, style order, or scope leaks can silently override existing UI.
---

# CSS Override Guard

## Workflow

1. Identify the style boundary before editing.
- Prefer page-scoped CSS isolation (`Component.razor.css`) over global CSS.
- Touch `wwwroot/app.css` only when a style must be shared across many pages.

2. Reuse existing class contracts.
- Reuse current block/class names instead of inventing near-duplicate selectors.
- Add one modifier class for variant behavior instead of adding higher-specificity selectors.
- Avoid styling by tag selectors (`input`, `button`, `table`) inside page files unless strictly necessary.

3. Keep specificity stable.
- Do not use `!important` unless there is no alternative and document why.
- Keep selectors shallow (max: 2 levels in most cases).
- If a new style is intended to win, prefer class-level intent over selector weight escalation.

4. Tokenize size and radius first.
- Put size/radius variables in page root (`.product-page { --... }`) and consume from controls.
- Change variables before touching many selector blocks to reduce accidental drift.

5. Verify override safety after each CSS batch.
- Search for duplicated selectors before finalizing:
  - `rg -n "form-field__input|combo-box|<target-class>" Components/Pages/*.razor.css`
- Build for regression check:
  - `dotnet build`
- If UI appears unchanged, force cache refresh (`Ctrl+F5`) and inspect computed styles.

## Non-Negotiable Rules

- Never move page-specific UI styles into global CSS unless explicitly requested.
- Never patch override issues by stacking `!important`.
- Never refactor unrelated selectors while fixing one style issue.
- Always keep visual behavior consistent between create/edit modes if they share controls.

## Quick Checklist

Before final response, confirm all:
- Changed selectors are scoped to the intended page/component.
- No new conflicting selector with existing naming contract.
- Control size/radius come from shared local variables.
- `dotnet build` passes.

For detailed anti-patterns and fix strategy, read [references/css-override-playbook.md](references/css-override-playbook.md).
