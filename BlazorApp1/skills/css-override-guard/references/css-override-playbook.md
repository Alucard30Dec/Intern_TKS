# CSS Override Playbook

## 1) Symptoms and Root Causes

- Symptom: Edited CSS but UI does not change.
  Cause: Browser cache, wrong scoped file, selector not matching generated markup.
- Symptom: One control changed, many others broken.
  Cause: Global selector leak or broad selector (`input`, `.card input`).
- Symptom: Style works in one state but breaks in another.
  Cause: Inconsistent modifier classes between create/edit mode.

## 2) Safe Selector Strategy

- Prefer `.page-root .block__element` over bare element selectors.
- Reuse existing classes first; add one modifier class if needed.
- Keep selector depth low; avoid long chained selectors.
- Avoid `:not(...)` complexity unless required.

## 3) Size and Radius Contract

For control consistency on one page, define:

```css
.page-root {
    --control-height: 36px;
    --control-radius: 14px;
}
```

Use variables in all controls:

```css
.form-field__input {
    min-height: var(--control-height);
    border-radius: var(--control-radius);
}
```

For combo with caret:

```css
.form-field__input--combo {
    min-height: var(--control-height);
    padding-right: 2.4rem;
}
```

## 4) Anti-Patterns to Reject

- `!important` used to force winning without understanding cause.
- Moving page-specific fixes into `wwwroot/app.css`.
- Creating duplicate class names with tiny differences.
- Refactoring unrelated selectors in the same patch.

## 5) Verification Loop

1. Search target selectors:
   `rg -n "<class-name>|<related-class>" Components/Pages/*.razor.css`
2. Verify markup class usage:
   `rg -n "<class-name>" Components/Pages/*.razor`
3. Build:
   `dotnet build`
4. Browser check:
   hard refresh (`Ctrl+F5`) and inspect computed styles.
