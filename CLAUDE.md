# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project overview

Three standalone single-file HTML games/apps. No build step, no dependencies, no package manager. Open any file directly in a browser to run it.

To serve locally (useful for iPhone testing on the same Wi-Fi):
```
python -m http.server 8080
```
Then open `http://<local-ip>:8080/<file>.html` on the device.

## Files

| File | What it is |
|---|---|
| `tictactoe.html` | Two-player Tic Tac Toe (players I and Y), score tracking |
| `snake.html` | Snake game, Nokia 3310 LCD aesthetic, mobile arrow-button controls |
| `meals.html` | Weekly meal planner — the most complex file |

## meals.html architecture

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

## Design tokens (CSS variables in `:root`)

```
--bg, --card, --accent (#4c6e5d), --accent2, --text, --sub, --border, --tag-bg, --tag-text
--p-color (protein teal), --c-color (carbs amber), --f-color (fat coral)
```

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

## Key constraints

- **Persistence** — the week's chosen meals are saved to `localStorage` (key `meals.weekplan`) as meal indices keyed by the week's Monday. `loadPlan()` restores them on startup; `savePlan()` must be called after any meal change (dropdown, shuffle button, modal shuffle, shuffle-all). A new week (different Monday) starts fresh with random meals.
- **All fonts** load from Google Fonts (`DM Serif Display` + `DM Sans`). Use `<link rel="preconnect">` + `<link rel="stylesheet">` — not `@import` inside `<style>`, which blocks Safari rendering.
- **Mobile layout**: the weekly grid scrolls horizontally (`overflow-x: auto`, `scroll-snap-type: x proximity`); each card is `minmax(260px, 1fr)`. The tab bar uses `env(safe-area-inset-bottom)` for iPhone home-bar clearance.
