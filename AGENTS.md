# AGENTS.md — AI Agent Guidance & Repository Rules

This document provides mandatory guidance for autonomous AI coding agents (Antigravity, Claude Code, GitHub Copilot, Cursor, Windsurf, Devin) operating in `akazad13/adda`.

All architectural decisions and technical trade-offs are governed by the [Architecture Decision Records (docs/adr/README.md)](docs/adr/README.md).

---

## 1. System Identity & Architecture

**Product**: `Adda` — Full-stack social networking and messaging platform featuring real-time SignalR chat, Cloudinary photo moderation, and role-based access control.
- **Backend**: .NET 10, C# 13, ASP.NET Core Web API, Entity Framework Core 10, SignalR WebSockets, MySQL / SQLite.
- **Frontend**: Angular 22 (Standalone Components, Signals, Resolvers), Bootstrap 5, FontAwesome, `ngx-bootstrap`.
- **Media**: Cloudinary image upload and moderation pipeline.
- **Mapping**: Mapster compiled object mapper.
- **Architecture Decisions**: Managed in [`docs/adr/`](docs/adr/README.md) per [ADR-0000](docs/adr/0000-record-architecture-decisions.md).

### Project Layout:
- `backend/Adda.API/`:
  - `Controllers/`: REST endpoints (`AuthController`, `UsersController`, `MessagesController`, `AdminController`).
  - `Hubs/ChatHub.cs`: SignalR WebSockets hub managing connections, group presence, and real-time direct messages.
  - `Data/`: EF Core `DataContext`, migration snapshots, and database seeds.
  - `Helpers/`: Mapster mapping configs, pagination parameters, and user activity filters.
- `backend/Adda.API.Tests/`: xUnit unit and integration tests.
- `frontend/`: Angular SPA (`src/app/members`, `src/app/messages`, `src/app/admin`).
- `docs/adr/`: [Architecture Decision Records](docs/adr/README.md) cataloging system design and evolution.

---

## 2. Inviolable Architectural Invariants

1. **Secret & Credential Safety**:
   - `backend/Adda.API/appsettings.Development.json` is untracked and strictly `.gitignore`d.
   - Never commit or log real Cloudinary API secrets, JWT private keys, or database passwords. Baseline configurations live in `appsettings.json` with empty placeholders.
2. **SignalR Connection & Group Tracking**:
   - Private chat messages must be delivered strictly to the authenticated recipient's SignalR connection or persisted to the database if offline.
   - SignalR hub calls must be authenticated using the query string access token (`access_token`) validated in `JwtBearerOptions`.
3. **Photo Approval Workflow**:
   - Unapproved member photos must have `IsApproved = false`. The public gallery and member card endpoints must query only `IsApproved == true` photos unless requested by `Admin`/`Moderator`.
4. **Role-Based Access Control (RBAC)**:
   - Protected actions must enforce ASP.NET Core Identity policies (`RequireAdminRole`, `ModeratePhotoRole`).
   - Admin management endpoints must remain locked to authorized roles; user role adjustments must be logged and validated.
5. **Data Mapping & DTO Boundaries**:
   - Database entities must never be returned directly across public API boundaries. All incoming requests and outgoing responses must pass through dedicated DTOs mapped via Mapster.
6. **Angular 22 & TypeScript Pinning ([ADR-0001](docs/adr/0001-angular-22-and-net-modernization.md))**:
   - Always install frontend packages with `npm install --legacy-peer-deps`.
   - Keep `typescript` pinned to `~6.0.2` in `frontend/package.json` as required by the Angular 22 compiler.
   - Use relative path aliases (`./src/app/*`) in `frontend/tsconfig.json`.
7. **Architectural Decision Tracking ([ADR-0000](docs/adr/0000-record-architecture-decisions.md))**:
   - Any architectural modification, library replacement, or invariant alteration must be documented in a corresponding ADR under `docs/adr/` and indexed in `docs/adr/README.md`.

---

## 3. Commands Cheat Sheet

```bash
# Backend build & test
cd backend
dotnet build Adda.sln
dotnet test

# Run backend API
cd backend/Adda.API
dotnet run

# Frontend install & run
cd frontend
npm install --legacy-peer-deps
npm start

# Run frontend tests
cd frontend
npx ng test --watch=false
```

---

## 4. Verification Gates for AI Agents

Before submitting code changes or finishing a turn:
1. Run `dotnet build backend/Adda.sln` — must compile with **0 errors**.
2. Verify that no database connection strings or JWT secrets were checked into tracked files.
