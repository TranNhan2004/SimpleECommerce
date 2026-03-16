# Frontend Design Styles

## Purpose

This document defines the visual design direction for the SimpleECommerce frontend.

It complements the structural rules in `ARCHITECTURE.md` by describing how the UI should look and feel.

---

## Core Design Direction

The frontend should follow a modern **light-theme ecommerce** style.

Main intention:

- Look clean, commercial, and trustworthy.
- Feel professional rather than playful.
- Keep color soft, but keep shape language sharp enough to create contrast.
- Avoid styles that look like a portfolio, landing page template, or experimental AI-generated design.

This project should visually feel closer to established ecommerce products than to a generic SaaS dashboard.

---

## Theme

### Preferred Theme

- Default theme is **light**.
- Backgrounds should stay bright and readable.
- Primary surfaces should usually be white or near-white.
- Accent color should come from gradient, borders, hover states, and focused highlights, not from dark surfaces.

### Avoid

- Dark-first UI direction.
- Heavy glassmorphism.
- Overly colorful backgrounds.
- Large dark blocks for core shopping surfaces.

---

## Color Direction

### Brand Gradient

Preferred brand gradient is:

- blue
- violet transition
- warm rose / coral finish

The gradient should **not** jump directly from pure blue to pure red.

Reason:

- Pure blue to pure red feels harsh.
- It creates an artificial or cheap-looking transition.
- Softer intermediary hues make the UI feel more premium.

### Gradient Character

Good gradient behavior:

- Start with a clean blue.
- Move through indigo or violet.
- End with rose, coral, or soft warm red.

Bad gradient behavior:

- Pure RGB-like blue to pure red.
- Neon purple midpoints.
- Over-saturated transitions.

### General Palette Usage

- Base text: slate / neutral dark.
- Secondary text: muted slate.
- Panels: white or very light neutral.
- Borders: subtle gray or cool neutral.
- Accent fills: light sky, light rose, light violet.

---

## Shape Language

### Preferred Shape Style

Use **close-square styling** instead of soft rounded styling.

Meaning:

- Cards should use small radius.
- Buttons should use small radius.
- Inputs should use small radius.
- Panels and list items should feel structured and controlled.

Recommended radius direction:

- Prefer `rounded-sm` or similarly restrained corners.
- Do not default to pill shapes.
- Do not use large rounded corners across the whole UI system.

### Reasoning

The color palette is already soft.

To create visual balance, the geometry should be more disciplined.

This makes the product look:

- more professional
- more commerce-oriented
- less decorative
- less toy-like

---

## Layout Feel

### Desired Feel

- Clean spacing.
- Strong alignment.
- Clear hierarchy.
- Easy scanning for products, prices, actions, and categories.

### Avoid

- Excessive decorative blobs.
- Overlapping floating layers without purpose.
- Too many visual tricks competing with product content.
- Hero sections that dominate the page more than the catalog.

In ecommerce, products and actions should stay central.

---

## Typography

Typography should be modern and clean.

Rules:

- Use a readable sans-serif family.
- Keep headings strong but not overly stylized.
- Do not rely on novelty fonts.
- Avoid typography that feels too editorial or artistic for shopping flows.

Typography should support conversion clarity first.

---

## Components

### Buttons

- Primary buttons can use the brand gradient.
- Secondary buttons should usually be white or neutral with border emphasis.
- Buttons should keep close-square corners.

### Cards

- Product cards should be clean and structured.
- Card background should usually stay white.
- Use border and subtle shadow more than heavy background color.

### Inputs

- Inputs should look sharp and usable.
- Use small-radius corners.
- Favor strong clarity over decorative styling.

### Badges and chips

- Use carefully.
- They should not become overly rounded pills by default.
- They should support information density, not decoration.

---

## Tailwind Usage Strategy

### Required Styling Approach

Prefer short semantic classes defined with Tailwind `@apply`.

Example intention:

```css
.se-btn {
  @apply inline-flex items-center justify-center rounded-sm px-5 py-3 text-sm font-semibold;
}
```

Reason:

- Keeps templates readable.
- Prevents very long class strings in HTML.
- Makes design language reusable and consistent.
- Makes future global restyling easier.

### Naming Direction

- Use short semantic aliases.
- Keep naming consistent across pages and shared UI.
- Reuse the same aliases for the same visual role.

Good examples:

- `se-btn`
- `se-btn-pri`
- `se-card`
- `se-label`
- `se-stat`

Avoid:

- one-off utility chains repeated everywhere
- inconsistent alias naming for the same role
- mixing multiple visual systems in one page

---

## Visual Principles

When making design decisions, prioritize these principles:

1. Clarity over decoration.
2. Trust over novelty.
3. Structured geometry over soft playful shapes.
4. Soft premium color transitions over harsh primary-color gradients.
5. Reusable semantic styling over long template utility strings.

---

## Summary

The SimpleECommerce frontend should look like a professional light-theme marketplace UI with:

- light surfaces
- blue to violet to rose gradient accents
- close-square component shapes
- readable typography
- restrained decoration
- Tailwind `@apply`-based semantic class aliases

This style should remain consistent across landing areas, product listing pages, product details, cart, checkout, and account flows.
