# WorkCheck — Context for Claude

Αρχείο πλαισίου για μελλοντικές συνομιλίες. Διάβασε αυτό πρώτα όταν δουλεύεις με το checkin project.

---

## Τι είναι

PWA εφαρμογή check-in/check-out εργαζομένων για την εταιρεία Aura. Deployed στο Firebase Hosting (`checkinaura-4cde2.web.app`). Χρησιμοποιεί Firebase Auth + Firestore.

---

## Τρέχουσα έκδοση

**v · 13:06** · deploy 2026-06-27

---

## Αρχεία

| Αρχείο | Ρόλος |
|---|---|
| `checkin.html` | Ολόκληρη η εφαρμογή (~800 γραμμές) |
| `checkin-sw.js` | Service Worker — network-first για HTML/version, cache-first για assets |
| `checkin-manifest.json` | PWA manifest — name: "WorkCheck" |
| `checkin-icon.svg` | App icon |
| `checkin-version.json` | `{"v":"HH:MM"}` — auto-update trigger |
| `deploy-checkin.js` | Deploy μέσω Firebase Hosting REST API |
| `checkinaura-4cde2-firebase-adminsdk-fbsvc-689b7e655d.json` | Service account key (μην το commitάρεις) |
| `CONTEXT.md` | Αυτό το αρχείο |

---

## Deploy

```
node checkin/deploy-checkin.js
```
(από το root του repo)

Πριν κάθε deploy:
1. Πάρε Greece time: PowerShell `[System.TimeZoneInfo]::ConvertTimeBySystemTimeZoneId([DateTime]::UtcNow, 'GTB Standard Time').ToString('HH:mm')`
2. Ενημέρωσε `checkin-version.json`
3. Ενημέρωσε `_THIS_VERSION` στο checkin.html
4. Ενημέρωσε τα δύο `v · HH:MM` στο HTML (Settings card + employee footer)
5. Git commit + push

---

## Firebase

- **Hosting**: `checkinaura-4cde2` — servέρει τα 5 αρχεία (html, sw, manifest, icon, version.json)
- **Auth**: Email/Password — το email είναι `{phone}@workcheckin.app` (π.χ. `6912345678@workcheckin.app`)
- **Firestore** (europe-west1):
  - `users/{uid}`: `{ name, phone, role: 'manager'|'employee'|'pending', createdAt }`
  - `checkins/{id}`: `{ userId, userName, phone, checkIn, checkOut, checkInLat, checkInLng, checkOutLat, checkOutLng, date, status: 'in'|'out' }`
  - `settings/workplace`: `{ lat, lng, radius (meters, default 200) }`

---

## Ρόλοι χρηστών

- **manager**: Πρώτος που κάνει register γίνεται αυτόματα manager. Βλέπει: Live view, Approvals, History, Settings.
- **employee**: Εγκεκριμένος από manager. Βλέπει: check-in/out button, GPS pill, Today's Log.
- **pending**: Μόλις κάνει register — περιμένει έγκριση. Βλέπει μόνο waiting screen.

---

## Features (τρέχουσα κατάσταση)

### Employee screen
- Live ρολόι (ώρα + ημερομηνία)
- GPS pill (button) — "Tap to get location"
  - Δοκιμάζει πρώτα low accuracy (network-based, γρήγορο), μετά high accuracy
  - Δείχνει απόσταση από workplace ή σφάλμα με οδηγίες
  - Tap για retry
- Check-in button — **disabled** αν δεν υπάρχει GPS. Γίνεται πράσινο μόνο αν GPS OK και εντός radius.
- Check-out — πάντα enabled αν checked in (αποδέχεται και χωρίς GPS)
- Today's Log — λίστα check-in/out της ημέρας
- Version footer: `WorkCheck · v · HH:MM`

### Manager screen (4 tabs)
- **LIVE**: Real-time λίστα ποιοι είναι IN / OUT με elapsed time
- **APPROVALS**: Pending registrations — Approve / Reject
- **HISTORY**: Φίλτρο per date + per employee, export CSV
- **SETTINGS**: Set workplace GPS (current location ή manual coords), change radius, team count, app version

---

## GPS — Γνωστά προβλήματα iOS

**Πρόβλημα**: Όταν η εφαρμογή είναι εγκατεστημένη ως PWA στο home screen, το iOS απαιτεί ξεχωριστή άδεια location για το WorkCheck — δεν κληρονομεί από το Safari.

**Λύση για χρήστη**:
→ iPhone Ρυθμίσεις → Απόρρητο & Ασφάλεια → Υπηρεσίες τοποθεσίας → WorkCheck → "Κατά τη χρήση"

Η εφαρμογή δείχνει αυτόματα τις οδηγίες (iOS standalone vs Safari detection).

**Σημείωση**: Σε iOS < 16.4, το geolocation δεν δουλεύει καθόλου σε PWA standalone mode — χρειάζεται Safari.

---

## Service Worker

- Cache name: `workcheck-v2` (αν αλλάξεις, άλλαξε και τη version)
- **Network-first**: `checkin.html`, `checkin-version.json` — πάντα από network, fallback cache
- **Cache-first**: Fonts, icons, scripts — cache πρώτα, network για update
- Για να αναγκάσεις reload μετά από SW update: κλείσε τελείως την εφαρμογή και ξανάνοιξε

---

## Auto-update

Κατά το boot, φορτώνει `checkin-version.json`. Αν `v !== _THIS_VERSION` → `location.reload(true)`.
Εξασφαλίζει ότι το κινητό παίρνει πάντα το νέο build.

---

## Pending Features (συμφωνημένα, δεν έχουν υλοποιηθεί)

1. **Αναφορές εργαζομένου** — History με σύνολο ωρών για date range (εβδομάδα/μήνας)
2. **Manual check-out** — ο manager να κάνει checkout εργαζόμενο που ξέχασε
3. **Ειδοποίηση αργής άφιξης** — αν κάποιος δεν έχει κάνει check-in μετά από ώρα X
4. **Employee ώρες** — εβδομαδιαίο/μηνιαίο σύνολο ωρών για τον ίδιο τον εργαζόμενο
5. **Δεύτερος manager** — promote υπάλληλο σε manager role

---

## Git

Branch: `multi-tenant` → push στο `origin/multi-tenant`
