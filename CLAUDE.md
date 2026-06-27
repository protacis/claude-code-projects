# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project overview

Multiple standalone projects, each in its own subfolder. No build step for the HTML apps — open any `.html` file directly in a browser.

To serve locally (useful for iPhone testing on the same Wi-Fi):
```
python -m http.server 8080
```
Then open `http://<local-ip>:8080/<subfolder>/<file>.html` on the device.

## Folder structure

| Folder | What it is |
|---|---|
| `expenses/` | Expenses tracker PWA — deployed to Firebase Hosting (`expenses-558cd`) |
| `checkin/` | WorkCheck check-in app PWA — deployed to Firebase Hosting (`checkinaura-4cde2`) |
| `games/` | Mini HTML apps: Tic Tac Toe, Snake, Weekly Meal Planner |
| `gym/` | Gym tracker app |
| `laser/` | Laser project app |
| `inventor-launcher/` | Inventor 2026 & AutoCAD launcher (PowerShell/HTA) + InventorParamsAddin (C#) |
| `3d Models/` | Inventor .ipt part files (Pulley assembly) |

## expenses/ — Expenses tracker PWA

Deployed to Firebase Hosting. Key files:
- `expenses.html` — single-file app, all logic inside
- `sw.js`, `manifest.json`, `icon.svg` — PWA support
- `version.json` — current version string
- `CHANGELOG.md` — change log (update on every deploy)
- `deploy.js` — deploys via Firebase Hosting REST API (no Firebase CLI needed)
- `firebase.json`, `.firebaserc` — Firebase CLI config (backup deploy method)

**Deploy:** `node expenses/deploy.js` from repo root, or `cd expenses && node deploy.js`

**Version & changelog:** bump `version.json` and add entry to `CHANGELOG.md` on every deploy. Also update the in-app version string and deploy timestamp (Greece time, UTC+3) in the Home tab + Settings.

## checkin/ — WorkCheck PWA

Deployed to Firebase Hosting. Key files:
- `checkin.html` — single-file app (multi-tenant, Firebase Realtime Database)
- `checkin-sw.js`, `checkin-manifest.json`, `checkin-icon.svg` — PWA support
- `deploy-checkin.js` — deploys via Firebase Hosting REST API
- `checkinaura-4cde2-firebase-adminsdk-fbsvc-689b7e655d.json` — service account key

**Deploy:** `node checkin/deploy-checkin.js` from repo root, or `cd checkin && node deploy-checkin.js`

## games/ — Mini apps

| File | What it is |
|---|---|
| `tictactoe.html` | Two-player Tic Tac Toe (players I and Y), score tracking |
| `snake.html` | Snake game, Nokia 3310 LCD aesthetic, mobile arrow-button controls |
| `meals.html` | Weekly meal planner — the most complex file |

### meals.html architecture

All logic is in a single `<script>` block at the bottom. No framework.

**Data layer** — `MEALS` object at the top of the script:
```js
const MEALS = {
  breakfast: [ { name, ingredients: [...], macros: { calories, protein, carbs, fat } }, ... ],
  lunch:     [ ... ],   // 15 meals each
  dinner:    [ ... ],
}
```

**State** — array of 7 day objects, one per day of the current week:
```js
state[i] = { name, date, meals: { breakfast: <meal obj>, lunch: <meal obj>, dinner: <meal obj> } }
```
`state[i].meals[type]` always holds a reference to one of the objects inside `MEALS[type]`. Shuffling replaces the reference; the dropdown syncs via `MEALS[type].indexOf(newMeal)`.

**Three tabs** (`data-view` on each `.tab-btn`):
- `meals` → `#grid` (the weekly cards, rendered by `render()`)
- `library` → `#view-library` (built by `buildLibrary()`, read-only browse of all 45 meals)
- `shopping` → `#view-shopping` (built by `buildShoppingList()`, parsed + merged ingredients)

**Modal** (`#modal` + `#scrim`): slides up from bottom. `fillModal(meal, type, fromDay)` populates it. When `fromDay` is false (library context) the shuffle button is hidden.

**Shopping list ingredient parser** (`parseIng`): strips unicode fractions, quantities, units, and descriptor adjectives from raw ingredient strings, then merges by `name|unit` key and groups into five supermarket categories (`CATS` array).

**`refreshMacros(di)`**: updates the macro summary row on a day card without re-rendering the whole card. Must be called after any meal change (dropdown, shuffle button, modal shuffle).

### Design tokens (CSS variables in `:root`)

```
--bg, --card, --accent (#4c6e5d), --accent2, --text, --sub, --border, --tag-bg, --tag-text
--p-color (protein teal), --c-color (carbs amber), --f-color (fat coral)
```

## inventor-launcher/

PowerShell/HTA launcher for Inventor 2026 & AutoCAD. The `InventorParamsAddin/` subfolder is a C# add-in that exposes a parameters viewer panel via the Inventor COM API.

## Git workflow

After every meaningful change, commit and push immediately so no work is ever lost:

```bash
git add <file>
git commit -m "Short present-tense description of what changed"
git push
```

Commit rules:
- One logical change per commit — don't bundle unrelated edits
- Message format: `<verb> <what>` e.g. `Fix macro totals not updating on dropdown change`
- Push to `origin/master` after every commit (remote: https://github.com/protacis/claude-code-projects)

## Key constraints (HTML apps)

- **No persistence** — state is in-memory only (except expenses.html which uses localStorage). Refreshing resets to defaults unless persistence is explicitly implemented.
- **All fonts** load from Google Fonts (`DM Serif Display` + `DM Sans`). Use `<link rel="preconnect">` + `<link rel="stylesheet">` — not `@import` inside `<style>`, which blocks Safari rendering.
- **Mobile layout** in meals.html: the weekly grid scrolls horizontally (`overflow-x: auto`, `scroll-snap-type: x proximity`); each card is `minmax(260px, 1fr)`. The tab bar uses `env(safe-area-inset-bottom)` for iPhone home-bar clearance.
