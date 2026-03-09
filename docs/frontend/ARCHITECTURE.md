# Frontend Architecture

## Scope

This document defines the folder responsibilities for the Angular frontend under `src/app`.

## App Structure

The application is organized into three main subfolders:

### `app/shared`

Purpose:

- Contains shared UI components.
- Holds reusable presentation pieces that can be used across multiple pages.

Rules:

- Keep this folder focused on reusable view elements.
- Do not place route-level page components here.
- Do not place business or application logic here unless it is strictly required by the shared UI component.

### `app/core`

Purpose:

- Contains pure logic.
- Contains data-related code.

Rules:

- This folder is for non-page application concerns.
- Keep it focused on logic, state shaping, models, configuration, and data access coordination.
- Do not place user-facing page components here.
- Avoid mixing reusable UI with core logic.

### `app/pages`

Purpose:

- Contains all user-facing pages.
- Each page component represents one displayed route.

Rules:

- Every route-level screen should live in this folder.
- Each page component should map to a single route when rendered for the user.
- Page components can compose shared components and consume logic from `core`.
- Do not place reusable shared components here unless they are specific to one page only.

## Dependency Direction

Expected usage flow:

- `pages` can use `shared`.
- `pages` can use `core`.
- `shared` should remain reusable and should not depend on page-level components.
- `core` should not contain route UI.

## Summary

- `shared` = reusable UI.
- `core` = pure logic and data.
- `pages` = route-level displayed components.
