# ADR-0002: Testing Paradigm and Agentic Verification

## Status
Accepted

## Date
2026-11-07

## Context
As `Adda` evolves with both human contributors and autonomous AI agents (Antigravity, Claude Code, GitHub Copilot, Cursor), maintaining code quality, regression safety, and architectural consistency requires a formalized, machine-verifiable testing paradigm.

Without explicit testing guidelines:
- Agents may write fragile interaction tests (over-mocking internal call sequences) that break upon trivial refactoring.
- Tests may become overly DRY, obscuring scenario context and making failures difficult to diagnose.
- Async SignalR hubs and EF Core database transactions risk going unverified or causing brittle external dependencies (e.g. attempting to reach live MySQL during unit runs).
- Frontend Angular 22 signals and standalone components risk lacking automated state verification.

## Decision
We establish a mandatory **Dual-Tier Testing Paradigm** anchored by Test-Driven Development (TDD) principles and optimized for autonomous agent verification.

### 1. Dual-Tier Testing Architecture

```
                 ╱╲
                ╱  ╲         E2E & Flow Verification (~5%)
               ╱    ╲        Critical user auth and chat journeys
              ╱──────╲
             ╱        ╲      Integration Tier (~25%)
            ╱          ╲     Backend: WebApplicationFactory with InMemory/SQLite
           ╱            ╲    Frontend: TestBed with router/http mocks
          ╱──────────────╲
         ╱                ╲  Unit Tier (~70%)
        ╱                  ╲ Backend: xUnit + FluentAssertions (Services, Domain)
       ╱────────────────────╲Frontend: Jasmine/Karma (Signals, Pipes, Components)
```

#### A. Backend Tier (.NET 10, xUnit, FluentAssertions)
1. **Unit Tests (`Adda.API.Tests/Unit/`)**:
   - Isolate domain and service business logic (`MessageService`, `UserService`, `PhotoService`, `AuthService`).
   - Use `FluentAssertions` for readable, expressive assertions.
   - Use `TestDataFactory` / `TestDataBuilder` fixtures to generate deterministic domain entities.
   - Mock external boundaries only (`ICloudinary`, external APIs) using `Moq`. Prefer real domain models and in-memory repository fakes over mocking internal entity getters/setters.
2. **Database Integration Tests (`Adda.API.Tests/Integration/Database/`)**:
   - Inherit from `DatabaseIntegrationTestBase`.
   - Use EF Core InMemory or SQLite in-memory databases with unique GUID database names (`Guid.NewGuid().ToString()`) per test class to guarantee test isolation.
   - Test repository queries, pagination filters, role mappings, and entity relationships against actual EF Core query providers.
3. **API & Controller Tests (`Adda.API.Tests/Integration/Api/`)**:
   - Inherit from `ApiIntegrationTestBase` using `WebApplicationFactory<Program>`.
   - Explicitly configure `builder.UseEnvironment("Testing")` and supply test configuration (in-memory JWT secrets and connection strings) to isolate from external MySQL instances.
   - Assert HTTP status codes, response headers, and serialized DTO payloads.
4. **SignalR Hub Testing (`ChatHub`)**:
   - Mock `HubCallerContext` (`ConnectionId`, `UserIdentifier`), `IHubClients`, and `IGroupManager`.
   - Verify group assignments (`Groups.AddToGroupAsync`) and message broadcasts (`Clients.Group(...).SendAsync`) based on authenticated caller identity.

#### B. Frontend Tier (Angular 22, Karma, Jasmine)
1. **Standalone Component Testing**:
   - Use Angular `TestBed` with standalone imports (`imports: [MyComponent]`).
   - Provide minimal required providers (`provideRouter([])`, `provideHttpClient()`, `provideAnimations()`, `provideToastr()`).
   - Render components and verify DOM state against rendered outputs, not component internal variables.
2. **Signal & Reactive State Testing**:
   - Assert on signal values (`signal()`) and computed signals (`computed()`) across state transitions.
   - For components utilizing Angular Signals, test reactive state updates by triggering user inputs or service emits and verifying resulting DOM updates via `fixture.detectChanges()`.
3. **Pipe & Utility Unit Tests**:
   - Pure function unit tests (`date-ago.pipe.spec.ts`, `has-error.pipe.spec.ts`) without TestBed overhead for sub-millisecond execution.

---

### 2. Core Testing Invariants

1. **State Over Interaction Verification**:
   - Assert on the *observable outcome* (returned DTO, HTTP status, persisted entity state in database, rendered DOM element, or signal value), NOT on internal method call sequences or mock execution counts.
   - Avoid `mock.Verify(x => x.InternalMethod(), Times.Once)` unless the interaction itself is the boundary contract (e.g., logging an audit event or calling a third-party payment gateway).
2. **DAMP Over DRY in Test Suites**:
   - Prioritize **DAMP** (*Descriptive And Meaningful Phrases*) over DRY (*Don't Repeat Yourself*).
   - Each test method must tell a self-contained story with clear Arrange-Act-Assert blocks. Avoid deeply nested shared test setups that obscure what is being validated.
3. **The Agentic Red-Green-Refactor Loop**:
   - When introducing new features or modifying behavior:
     1. **RED**: Write a failing test proving the absence of the feature or exposing the bug.
     2. **GREEN**: Implement minimal production code to pass the test.
     3. **REFACTOR**: Clean up code and abstractions while preserving green tests.
   - **The Prove-It Pattern**: Bug fixes must never be committed without a reproduction test that failed prior to the fix.
4. **The Beyoncé Rule**:
   - *"If you liked it, you should have put a test on it."* Never rely on manual QA to guard critical invariants. Uncovered behavior is considered unsupported.

---

### 3. Test Execution & Verification Commands

- **Backend Test Suite**:
  ```bash
  dotnet test backend/Adda.sln
  ```
- **Focused Backend Test**:
  ```bash
  dotnet test backend/Adda.sln --filter "FullyQualifiedName~MessageServiceTests"
  ```
- **Frontend Test Suite**:
  ```bash
  cd frontend && npx ng test --watch=false
  ```

---

## Consequences

### Positive
- **Deterministic Agent Verification**: AI agents can independently verify backend and frontend behaviors without risking accidental regression or external network/database lockouts.
- **Refactoring Resilience**: State-based assertions allow internal architecture, query optimizations, and service refactoring without breaking test suites.
- **Predictable Diagnostics**: Self-contained DAMP tests provide clear failure contexts, accelerating root-cause discovery.

### Trade-offs & Mitigations
- In-memory EF Core does not enforce relational MySQL foreign-key constraints or specific SQL dialects. 
  - *Mitigation*: Complement in-memory tests with SQLite integration tests or staged schema migration smoke tests.
- Standalone component `TestBed` setup requires explicit provider mocks (`provideHttpClient`, `provideRouter`).
  - *Mitigation*: Maintain reusable testing harnesses and lightweight provider configurations.
