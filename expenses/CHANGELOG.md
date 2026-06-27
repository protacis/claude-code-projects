# Expenses App — Changelog

All notable changes to this project are documented here.
Format: `[vX.Y.Z] YYYY-MM-DD — Description`

---

## [v2.3.0] 2026-06-27 — Filters, yearly stats, quick-add, category reorder, more currencies

### New features
- **List filters** — category chips + user chips below search bar; filter expenses by category and/or person
- **Yearly stats** — Month/Year toggle in Stats; Year mode aggregates all months Jan–today with pie chart, category breakdown, by-person, and monthly bar chart
- **Quick-add from Home** — first 6 categories shown as tappable emoji chips; tap to jump to Add tab with category pre-selected
- **Category reordering** — ↑↓ arrows in Settings → Categories; reorder persists to Firebase
- **More currencies** — GBP, CHF, SAR added to all currency pickers (Add, Edit, Stats, Setup, Budget, Recurring)

---

## [v2.2.0] 2026-06-27 — Custom categories

### New features
- **Custom categories** — create new categories with emoji icon, name, and color
- **Rename categories** — edit label and icon of any existing category
- All categories stored in Firebase (`config/categories`); synced across users
- Built-in categories (Food, Shopping, etc.) can be renamed but not deleted
- Custom categories can be fully deleted (existing expenses fall back to Other)

---

## [v2.0.0] 2026-06-21 — Full multi-user rewrite

### New features
- **Multi-user support** — up to 5 users, add/edit/soft-delete in Settings → Users
- **Split expenses** — 🔀 Split chip; equal split by default; per-person amount override; stored as `split:[{userId,amount,pct}]`
- **Recurring expenses** — define recurring (desc/amount/currency/cat/day/who); monthly pending confirmation; manageable list in Settings
- **Notification bell 🔔** — badge with count; bottom sheet with pending recurring (Add / Skip); pending card on Home
- **Settings accordion** — 6 collapsible sections: Appearance / Users / Security / Budget / Recurring / Data
- **Multi-currency budget per category** — each category has its own amount + currency; compared in report currency in Stats
- **Export CSV** — all expenses from last 2 years; columns: date/desc/cat/amount/currency/EUR/who/split detail
- **PIN system overhaul** — Shared PIN (who-screen after auth) OR Individual PINs per user; change shared PIN; set per-user PIN
- **Data model migration** — auto-migrates old `name1`/`name2` config → `users[]` with IDs; backward compat via `resolveUserId()`

### Firebase data model
```
config/users:        [{ id, name, pinHash, deleted }]
config/pinMode:      "shared" | "individual"
config/person1Id:    "u_xxx"   (backward compat)
config/person2Id:    "u_yyy"   (backward compat)
config/categoryBudgets: { food: { amount, currency }, ... }
expenses/year/month/id: { amount, currency, cat, desc, date, ts, who | split }
recurring/id:        { desc, amount, currency, cat, day, who|split, active, lastAdded }
```

---

## [v1.4.0] 2026-06-20 — Who-screen personalization

### New features
- **Personalized greeting** — "Good morning, Byron!" vs "Androniki!" depending on who logs in
- Time-based greeting (morning / afternoon / evening)

---

## [v1.3.0] 2026-06-20 — Charts & Trends

### New features
- **Trends bar chart** — 6 months of spending history in Stats tab
- **Pie chart slice labels** — emoji + percentage shown on slices >5% (chartjs-plugin-datalabels)
- Pie chart tooltip shows report currency amount + percentage

---

## [v1.2.0] 2026-06-20 — Homepage & Appearance

### New features
- **Homepage dashboard** — monthly total, budget progress, top 3 categories, Add Expense button
- **Light / Dark / System theme toggle** — saved to localStorage
- 5-tab layout (Home / Add / List / Stats / Settings)

---

## [v1.1.0] 2026-06-19 — Report currency & FX

### New features
- **Report currency selector** — AED / EUR / USD in Stats tab
- **FX rate source** — switched to `open.er-api.com` (supports AED; ECB via frankfurter.app does not)
- FX rate timestamp shown in Stats ("updated at HH:MM")
- FX refresh: once per day (86 400 000 ms)

### Fixes
- `frankfurter.app` doesn't support AED → switched to `open.er-api.com`
- fx-note moved above the fold so it's always visible in Stats

---

## [v1.0.0] 2026-06-19 — Initial release

### Features
- PIN lock screen (sha256 via crypto.subtle)
- 5 expense categories with colour-coded icons
- AED / EUR / USD currency per expense
- Monthly list view with day grouping
- Budget progress bar
- Stats: pie chart by category, by person (person1 / person2)
- Firebase Realtime Database (europe-west1)
- Firebase Hosting deploy via `node deploy.js`
- Google Fonts: DM Sans
- Dark theme by default; mobile-first layout with safe-area insets
