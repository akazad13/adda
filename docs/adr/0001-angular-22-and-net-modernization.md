# ADR-0001: Angular 22 and .NET 10 Modernization

## Status
Accepted

## Date
2026-11-07

## Context
Adda combines an ASP.NET Core Web API backend with an Angular frontend. In this dependency update cycle:
1. Backend packages (EF Core, Pomelo MySQL, Mapster, SignalR, xUnit) were upgraded to their latest supported versions for .NET 10.
2. Frontend Angular CLI and dependencies were upgraded to Angular 22.

During the Angular 22 upgrade:
- `@angular/compiler-cli` v22 requires TypeScript `>=6.0.0 and <6.1.0` (`~6.0.2`).
- TypeScript path aliases without `baseUrl` require `./` prefixes.
- `ngx-bootstrap` and third-party UI widgets require `--legacy-peer-deps` during npm install.

## Decision
1. **Frontend Pinning**: Keep TypeScript pinned to `~6.0.2` in `frontend/package.json`.
2. **Path Aliasing**: Maintain relative path aliases in `frontend/tsconfig.json`.
3. **Backend Packages**: Maintain up-to-date EF Core and SignalR dependencies while keeping authentication/authorization test coverage green.

## Consequences
- Clean builds and continuous compatibility with Angular 22 standalone features.
- All backend and frontend unit tests pass without deprecation warnings.
