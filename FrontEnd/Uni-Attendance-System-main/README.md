# AttendFlow — University Attendance System

## What was built

| Module | What it does |
|---|---|
| **Dashboard** | Live counters (projects/in-progress/completed/members) + two Chart.js charts |
| **Projects** | Cards with progress bar, Details, Create/Edit/Delete with confirmation, cascade delete of tasks |
| **Kanban** | 4 columns, drag-and-drop with AJAX save + optimistic UI and revert-on-error, quick filters |
| **Tasks** | Search + status/priority/project filters, status/priority badges, Create/Edit with dropdowns and date picker, Delete |
| **Team Members** | Searchable list, Details, Create/Edit/Delete, unassign on delete |
| **Attendance** | Live records, date/status filters, CSV export with padded columns (no `####` in Excel) |
| **QR Scanner** | Generated QR + PNG download, continuous camera scan, check-in with toast and last-5 history |
| **UI/UX** | Shared layout, responsive sidebar, notifications/settings/account modals, toast system, footer |
| **Themes** | `Dark — Midnight` and `Light — Cloud`; sidebar follows theme, saved in `localStorage`, charts redraw |
| **Validation** | Data Annotations + `IValidatableObject` + jQuery Validation + server `ModelState` |

---

## Run it

```bash
dotnet restore
dotnet build
dotnet run --launch-profile http
```

Open **http://localhost:5220**

If restore fails: `dotnet restore -p:RestoreFallbackFolders="" --force`

> Use the folder containing `Controllers/TasksController.cs`. Ignore any older duplicate folder outside this one.

---

## Main routes

| Route | Purpose |
|---|---|
| `/` | Dashboard |
| `/Projects`, `/Projects/Kanban` | Projects list and Kanban board |
| `/Tasks` | Tasks table and filters |
| `/TeamMembers` | Members list |
| `/Attendance` | Records, filters, CSV export |
| `/Attendance/QRScanner` | QR generation and scanner |
| `POST /Tasks/Move` | Kanban status change → `{ success: true }` |
| `POST /Attendance/CheckIn` | QR attendance check-in |

All POSTs use antiforgery tokens; missing token → `400`.

---

## QR check-in contract (for the backend team)

```http
POST /Attendance/CheckIn
Content-Type: application/json

{ "studentId": "2026001", "scannedAt": "2026-11-26T09:12:00" }
```

```json
{
  "success": true,
  "message": "Attendance recorded for Student Demo.",
  "studentId": "2026001",
  "studentName": "Student Demo",
  "time": "09:12 AM",
  "status": "Present"
}
```

Errors: `success: false` with `Unknown student ID` or `already checked in today`.
Rules: `Present` ≤ 09:00, `Late` after 09:00, one check-in per student per day. The UI accepts JSON QR or a plain student ID. Replace `AppDataStore.Students` with a DB lookup later — no UI changes needed.

---

## Testing done

- `dotnet build` → **0 warnings / 0 errors**; all pages `200`, missing IDs `404`.
- **65 functional tests passed, 0 failed**: CRUD, validation, duplicates, progress bar, badges, Kanban Move, check-in success/duplicate/unknown/no-token, attendance table and export.
- **Browser checks**: no console errors, theme toggle + persistence, filters, exported CSV verified, delete modal population, real drag-and-drop + toast, QR image and PNG download.

---

## Notes for submission

- No database, no real authentication; **Sign out** and the **Admin** account menu are UI demos.
- Data resets on restart — expected for an in-memory prototype.
- QR libraries load from CDN and the camera needs permission (`localhost` is a secure context).
- Source only, not a hosted website. Old local screenshots are excluded by `.gitignore`.

