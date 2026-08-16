# Pharmacy Inventory & Dispensing System — Implementation Guide

> **Purpose:** This document is the single source of truth for implementing the backend API. It resolves the ambiguity between **business state**, **soft delete / archive**, **DELETE endpoints**, and **restore** operations. Follow it as written unless the System Analysis is formally revised.

> **Scope:** Backend (.NET) only. No code changes are implied by this document alone; it describes what to build and why.

---

## 1. Project Overview

The Pharmacy Inventory & Dispensing System supports:

- Medicine catalog management
- Batch-level inventory with expiry tracking
- Append-only stock movement history
- Prescription creation and lifecycle management
- FEFO-based dispensing with atomic stock updates
- Role-based access (Admin, Pharmacist, Doctor)

**Primary references:**

| Document / code | Role |
|-----------------|------|
| `docs/Pharmacy-System-Analysis.md` | Requirements, business rules, high-level API surface |
| `src/PharmacyInventoryDispensingSystem.Domain` | Entity model |
| `src/PharmacyInventoryDispensingSystem.Infrastructure/Persistence/Configurations` | EF Core mappings |

**Current implementation status:** Foundation layer, domain entities, EF configurations, and health check exist. Application handlers, auth, and business controllers are not yet implemented.

---

## 2. Architecture

### 2.1 Layered structure

| Layer | Project | Responsibility |
|-------|---------|----------------|
| API | `PharmacyInventoryDispensingSystem.WebApi` | Controllers, auth middleware, Swagger, CORS, HTTP mapping |
| Application | `PharmacyInventoryDispensingSystem.Application` | Commands, queries, validators, DTOs, MediatR handlers |
| Domain | `PharmacyInventoryDispensingSystem.Domain` | Entities, enums, domain invariants |
| Infrastructure | `PharmacyInventoryDispensingSystem.Infrastructure` | EF Core, Identity, repositories, Unit of Work, migrations |

### 2.2 Dependency direction

```
WebApi → Application, Infrastructure
Application → Domain
Infrastructure → Application, Domain
```

Domain must not reference Application or Infrastructure.

### 2.3 Required patterns

| Pattern | Usage |
|---------|-------|
| CQRS + MediatR | One command/query per write/read operation |
| Repository + Unit of Work | Abstract persistence; coordinate multi-entity transactions |
| FluentValidation | Request validation in Application layer |
| Policy-based authorization | `AdminOnly`, `PharmacistOrAdmin`, `DoctorOrAdmin` |
| JWT + ASP.NET Core Identity | Authentication and role management |

### 2.4 Transaction boundaries

Any operation that changes inventory must be atomic within a single Unit of Work commit:

1. Update `MedicineBatch.QuantityInStock`
2. Insert `StockMovement`
3. Insert/update `Dispense` / `DispenseItem` / `PrescriptionItem` as applicable

Never commit batch quantity changes without a corresponding stock movement.

---

## 3. Domain Model

### 3.1 Base type hierarchy

```csharp
BaseEntity                          // Id (Guid v7)
└── BaseAuditableEntity             // CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
    ├── SoftDeletableEntity         // IsDeleted, DeletedAt, DeletedBy  ← archive only
    │   ├── Medicine
    │   ├── MedicineBatch           // technical capability; API restricted
    │   └── Prescription
    └── (non-deletable auditable)
        ├── StockMovement           // append-only audit
        ├── Dispense                // permanent fulfillment record
        └── DispenseItem            // permanent allocation record
```

**PrescriptionItem** inherits audit fields via `BaseAuditableEntity` (recommended) but does **not** inherit `SoftDeletableEntity`. Its visibility is governed by its parent prescription.

### 3.2 Entity summary

| Entity | Key business fields | Base type (target) |
|--------|---------------------|-------------------|
| `Medicine` | Code, Name, Strength, Form, Unit, ReorderLevel, **IsActive** | `SoftDeletableEntity` |
| `MedicineBatch` | MedicineId, BatchNumber, ExpiryDate, QuantityInStock, ReceivedAt | `SoftDeletableEntity` |
| `StockMovement` | MedicineBatchId, MovementType, QuantityChange, Reason | `BaseAuditableEntity` |
| `Prescription` | PrescriptionNumber, Patient*, DoctorId, ValidFrom/To, MaxRefills, RefillsUsed, **Status** | `SoftDeletableEntity` |
| `PrescriptionItem` | MedicineId, QuantityPrescribed, QuantityDispensed, DosageInstructions | `BaseAuditableEntity` |
| `Dispense` | PrescriptionId, PharmacistId, DispensedAt | `BaseAuditableEntity` |
| `DispenseItem` | PrescriptionItemId, MedicineBatchId, Quantity | `BaseAuditableEntity` |

### 3.3 Enums

**PrescriptionStatus** (business state — not deletion):

| Value | Meaning |
|-------|---------|
| `Active` | Valid for dispensing while within ValidFrom/ValidTo |
| `Cancelled` | Manually cancelled; still visible in history |
| `Expired` | ValidTo passed or system-marked expired; still visible in history |

**MovementType** (immutable after creation):

| Value | QuantityChange sign | Created by |
|-------|---------------------|------------|
| `Receive` | Positive | Batch receive |
| `Dispense` | Negative | Dispense workflow |
| `Adjustment` | Either; reason required | Manual adjustment |
| `Expired` | Negative (write-off) | Expiry processing (optional future) |

### 3.4 Known code gaps to resolve during implementation

These are documented contradictions between current code and this guide:

| Item | Current state | Target state |
|------|---------------|--------------|
| `Dispense` base type | `SoftDeletableEntity` | `BaseAuditableEntity` — remove query filter |
| `SoftDeletableEntity.DeletedBy` | Missing | Add `string? DeletedBy` |
| `Prescription` → `Items` delete behavior | `Cascade` | `Restrict` — protect line items |
| `PrescriptionItem` base type | Plain class, no audit | `BaseAuditableEntity` |
| `MedicineBatch` unique constraint | Not configured | Unique `(MedicineId, BatchNumber)` |
| `DispenseItem` FK configs | Mostly commented out | Explicit Restrict FKs |
| `PrescriptionItem` → `Medicine` FK | Not configured | Explicit Restrict FK |

---

## 4. Entity Lifecycle Strategy

Every entity has up to **three independent dimensions**. They must never be conflated.

### 4.1 Dimension 1 — Business state

Answers: *"What is the current business meaning of this record?"*

| Entity | Business state mechanism | Examples |
|--------|-------------------------|----------|
| Medicine | `IsActive` | Active catalog item vs discontinued |
| MedicineBatch | `QuantityInStock`, `ExpiryDate`, `IsExpired()` | In stock, depleted, expired |
| Prescription | `PrescriptionStatus` + validity dates | Active, Cancelled, Expired |
| PrescriptionItem | `QuantityDispensed` vs `QuantityPrescribed` | Partially dispensed, fully dispensed |
| StockMovement | `MovementType` | Receive, Dispense, Adjustment |
| Dispense | Immutable event (`DispensedAt`) | Permanent record |

### 4.2 Dimension 2 — Archive / soft delete

Answers: *"Should this record be hidden from normal operational queries while remaining in the database?"*

Represented only by:

```csharp
IsDeleted, DeletedAt, DeletedBy
```

**Never infer deletion from:** `IsActive = false`, `Status = Cancelled`, `Status = Expired`, `QuantityInStock = 0`.

### 4.3 Dimension 3 — API operations

Answers: *"What HTTP actions are exposed, and what do they actually do?"*

| Operation type | HTTP pattern | Sets |
|----------------|--------------|------|
| Business action | `POST /{resource}/{id}/{action}` | Business state fields |
| Archive | `DELETE /{resource}/{id}` | `IsDeleted = true` (+ audit) |
| Restore | `POST /{resource}/{id}/restore` | `IsDeleted = false` (+ audit) |
| Forbidden | — | No endpoint |

### 4.4 Lifecycle examples

**Medicine — deactivate vs archive:**

```http
POST /api/v1/medicines/2/deactivate   → IsActive = false,  IsDeleted = false
DELETE /api/v1/medicines/2             → IsDeleted = true,   IsActive unchanged
```

**Prescription — cancel vs archive:**

```http
POST /api/v1/prescriptions/2/cancel   → Status = Cancelled, IsDeleted = false
DELETE /api/v1/prescriptions/2        → IsDeleted = true,    Status unchanged
```

A cancelled prescription remains in normal lists. An archived prescription is excluded from normal lists but can be retrieved via archive/admin queries or restore.

---

## 5. Soft Delete / Archive Strategy

### 5.1 Decision

**Retain soft delete as a limited technical archive mechanism.**

### 5.2 Justification (from System Analysis + engineering constraints)

The System Analysis does **not** use the words "soft delete" or "archive," but it **does** require:

1. Historical prescription, dispense, and stock records remain traceable (§6.7 rule 10 implied by relationships and Restrict FKs).
2. Stock movements are append-only — never deleted (§6.7 rule 9).
3. Prescriptions support **cancel**, not delete (§6.8 API surface).
4. Medicine CRUD is listed, but physical DELETE would violate `DeleteBehavior.Restrict` on batches and prescription items.

Given Restrict FKs across the graph, **physical deletion is unsafe** for any entity referenced by history. Soft delete provides:

- A way to hide catalog/clinical header records from operational UI
- FK-safe retention for dispense and stock audit trails
- Optional admin recovery via restore

Soft delete is **not** a substitute for business state transitions.

### 5.3 Where soft delete applies

| Applies | Does not apply |
|---------|----------------|
| `Medicine` — retire catalog entries from normal search | `StockMovement` |
| `Prescription` — admin archive of erroneous/duplicate entries | `Dispense` |
| `MedicineBatch` — technical capability only; API heavily restricted | `DispenseItem` |
| | `PrescriptionItem` (follows parent prescription) |

### 5.4 Archive guard rules

Before setting `IsDeleted = true`, validate:

| Entity | Block archive when |
|--------|-------------------|
| Medicine | Any non-deleted batch has `QuantityInStock > 0` (optional strict rule) OR any active prescription item references it — prefer **block if referenced by any non-archived prescription** |
| MedicineBatch | Any `DispenseItem` or non-zero stock movement history exists — **block if any dispense reference exists**; allow only for zero-stock, never-dispensed batches (admin correction) |
| Prescription | Any `Dispense` record exists — **block archive if dispensed**; allow archive only for never-dispensed prescriptions (admin cleanup) |

### 5.5 Identity users

ASP.NET Core Identity users are **not** domain soft-delete entities. Use Identity's `LockoutEnabled` / custom `IsActive` flag on an application user profile if user deactivation is needed. Do not apply `SoftDeletableEntity` to Identity tables.

---

## 6. Business States vs Archive State

| Concept | Field(s) | User-facing term | Reversible via |
|---------|----------|------------------|----------------|
| Medicine availability | `IsActive` | Active / Inactive | `POST .../activate`, `POST .../deactivate` |
| Prescription validity | `Status`, `ValidFrom`, `ValidTo` | Active / Cancelled / Expired | Cancel (`POST .../cancel`); Expire (system/job); **not** via DELETE |
| Batch usability | `QuantityInStock`, `ExpiryDate` | In stock / Depleted / Expired | Stock operations only |
| Operational visibility | `IsDeleted` | Archived | `DELETE` (archive), `POST .../restore` |

**Critical rule:** `DELETE` never sets `IsActive = false`, `Status = Cancelled`, or `Status = Expired`.

---

## 7. Entity Delete / Archive Matrix

| Entity | Business state | Soft delete? | DELETE API? | Restore? | Reason |
|--------|----------------|--------------|-------------|----------|--------|
| **Medicine** | `IsActive` | **Yes** | **Yes** → archive | **Yes** (Admin) | Catalog entity; FK Restrict prevents hard delete; deactivate ≠ archive |
| **MedicineBatch** | Quantity + Expiry | **Yes** (technical) | **No** (public) | Admin-only if archived | Inventory history; receive/adjust only; admin may archive never-dispensed zero-stock batches via internal command |
| **StockMovement** | `MovementType` | **No** | **No** | **No** | Append-only audit ledger |
| **Prescription** | `PrescriptionStatus` | **Yes** | **Yes** → archive (Admin, guarded) | **Yes** (Admin) | Clinical history; cancel ≠ delete; block archive if dispensed |
| **PrescriptionItem** | Dispensed vs prescribed | **No** | **No** | **No** | Managed through prescription commands; no standalone lifecycle |
| **Dispense** | Immutable event | **No** | **No** | **No** | Permanent fulfillment record |
| **DispenseItem** | Allocation qty | **No** | **No** | **No** | Audit traceability |

### 7.1 Technical capability vs API exposure

```
SoftDeletableEntity on entity  ≠  public DELETE endpoint
```

Example: `MedicineBatch` may inherit `SoftDeletableEntity` for admin data correction, but there is **no** public `DELETE /batches/{id}` in the v1 API.

---

## 8. Relationship & DeleteBehavior Rules

All FKs use explicit configuration. Default to **Restrict** to protect audit history.

| Relationship | DeleteBehavior | Reason |
|--------------|----------------|--------|
| Medicine → Batches | **Restrict** | Cannot remove medicine referenced by batches |
| Medicine → PrescriptionItems | **Restrict** | Prescription lines must keep medicine reference |
| MedicineBatch → StockMovements | **Restrict** | Movements are permanent audit |
| MedicineBatch → DispenseItems | **Restrict** | Dispense allocations must keep batch reference |
| Prescription → Items | **Restrict** | Line items are part of clinical record |
| Prescription → Dispenses | **Restrict** | Dispense history must remain |
| Dispense → Items | **Restrict** | Allocation lines are permanent |

**Do not use Cascade** on any relationship that participates in historical or audit data. The current `Prescription → Items Cascade` must be changed to Restrict.

---

## 9. EF Core Configuration

### 9.1 Global query filters

Apply `HasQueryFilter(e => !e.IsDeleted)` **only** to:

- `Medicine`
- `MedicineBatch`
- `Prescription`

**Do not** apply to: `StockMovement`, `Dispense`, `DispenseItem`, `PrescriptionItem`.

# Remark: back to this section again (Mohammad):
### 9.2 Query filter risks and mitigations

| Scenario | Risk | Mitigation |
|----------|------|------------|
| Dispense history loads prescription | Archived prescription invisible | In dispense detail/history handlers, use `IgnoreQueryFilters()` when loading related `Prescription` by FK |
| Dispense item loads medicine batch | Archived batch invisible | Same — explicit `IgnoreQueryFilters()` in historical read paths |
| Operational medicine list | Shows deleted items | Global filter correct — no action |
| Admin archive view | Needs deleted records | Dedicated queries with `IgnoreQueryFilters()` + `Where(e => e.IsDeleted)` |
| Inventory FEFO selection | Must exclude expired/zero, not deleted | Query active batches: global filter + `QuantityInStock > 0` + `ExpiryDate >= today` |

**Rule:** Do not sprinkle `IgnoreQueryFilters()` broadly. Use it only in named query handlers: `GetDispenseById`, `GetDispenseHistory`, `GetArchivedMedicines`, etc.

### 9.3 Indexes

| Table | Index | Purpose |
|-------|-------|---------|
| Medicines | Unique `Code` | Catalog lookup |
| Medicines | `Name` | Search |
| Medicines | `IsActive` (optional composite with Name) | Active catalog filtering |
| MedicineBatches | Unique `(MedicineId, BatchNumber)` | Prevent duplicate lot numbers |
| MedicineBatches | `(MedicineId, ExpiryDate, ReceivedAt)` | FEFO ordering |
| Prescriptions | Unique `PrescriptionNumber` | Lookup |
| Prescriptions | `(Status, ValidTo)` | Active prescription queries |
| PrescriptionItems | `PrescriptionId`, `MedicineId` | Line lookups |
| StockMovements | `MedicineBatchId`, `CreatedAt` | History |
| Dispenses | `PrescriptionId`, `DispensedAt` | History |
| DispenseItems | `DispenseId`, `PrescriptionItemId`, `MedicineBatchId` | Traceability |

### 9.4 Constraints

- `MedicineBatch.QuantityInStock >= 0` — CHECK constraint (already present)
- `PrescriptionItem.QuantityDispensed <= QuantityPrescribed` — enforce in domain/application
- `Prescription.RefillsUsed <= MaxRefills` — enforce in application

### 9.5 Auditing

Populate `CreatedBy`, `UpdatedBy`, `DeletedBy` from the authenticated user's Identity ID in the Application layer or an EF interceptor. Set `DeletedAt`/`DeletedBy` only on archive; clear them on restore.

---

## 10. Authentication & Authorization

### 10.1 Roles

| Role | Capabilities |
|------|-------------|
| **Admin** | Full access; user management; archive/restore; all pharmacist and doctor capabilities |
| **Pharmacist** | Batches, inventory, stock movements (read), dispensing, medicine read/update, prescription read |
| **Doctor** | Create/view/update own prescriptions (before dispense); cancel own prescriptions; medicine read |

### 10.2 Policies

```csharp
AdminOnly          → role Admin
PharmacistOrAdmin  → Pharmacist, Admin
DoctorOrAdmin      → Doctor, Admin
```

### 10.3 Identity endpoints (separate from domain soft delete)

| Endpoint | Auth | Notes |
|----------|------|-------|
| `POST /api/v1/auth/register` | AdminOnly (or public in dev seed only) | Create Identity user |
| `POST /api/v1/auth/login` | Public | Returns JWT |
| `POST /api/v1/auth/refresh` | Authenticated | Optional refresh token |
| `POST /api/v1/auth/change-password` | Authenticated | Self-service |
| `GET /api/v1/auth/me` | Authenticated | Current user profile + roles |

User deactivation (if needed): `POST /api/v1/users/{id}/deactivate` — Identity lockout or `IsActive` flag, **not** `IsDeleted`.

---

## 11. API Design Principles

1. **Business actions are POST sub-resources**, not DELETE: `/cancel`, `/activate`, `/deactivate`.
2. **DELETE means archive** (`IsDeleted = true`) only where the matrix allows it.
3. **Command names match behavior:** `ArchiveMedicineCommand`, not `DeleteMedicineCommand`, unless the handler explicitly performs archive.
4. **Transactional records have no DELETE or PUT.**
5. **Return correct HTTP status codes:**

| Situation | Code |
|-----------|------|
| Success create | 201 Created |
| Success read/update/action | 200 OK |
| Archive success | 204 No Content |
| Validation failure | 400 Bad Request |
| Unauthorized | 401 Unauthorized |
| Forbidden role | 403 Forbidden |
| Not found (or archived in normal query) | 404 Not Found |
| Business rule violation | 409 Conflict |
| Blocked archive (has dispense refs) | 409 Conflict |

6. **Version prefix:** all routes under `/api/v1/`.

---

## 12. Controller Inventory

| Controller | Exists | Purpose |
|------------|--------|---------|
| `HealthController` | Yes | Liveness check |
| `AuthController` | Planned | Login, token, current user |
| `MedicinesController` | Planned | Catalog CRUD + activate/deactivate + archive/restore |
| `MedicineBatchesController` | Planned | Receive stock, list batches, adjustments |
| `InventoryController` | Planned | Summary, low-stock, expiring (read models) |
| `StockMovementsController` | Planned | Read-only movement history |
| `PrescriptionsController` | Planned | Prescription CRUD + cancel + archive/restore |
| `DispensesController` | Planned | Dispense workflow + history |
| `UsersController` | Planned | Admin user management |

**Not created:** `PrescriptionItemsController` — items are nested in prescription commands.

---

## 13. Endpoint Inventory

### 13.1 Health

| Method | Route | Roles | Description |
|--------|-------|-------|-------------|
| GET | `/api/v1/health` | Public | Service health |

### 13.2 Auth

| Method | Route | Roles | Description |
|--------|-------|-------|-------------|
| POST | `/api/v1/auth/register` | AdminOnly | Register user with role |
| POST | `/api/v1/auth/login` | Public | JWT login |
| POST | `/api/v1/auth/refresh` | Authenticated | Refresh token |
| POST | `/api/v1/auth/change-password` | Authenticated | Change password |
| GET | `/api/v1/auth/me` | Authenticated | Current user |

### 13.3 Medicines

| Method | Route | Roles | Description |
|--------|-------|-------|-------------|
| GET | `/api/v1/medicines` | All authenticated | List active, non-archived medicines; filter by `isActive`, search |
| GET | `/api/v1/medicines/{id}` | All authenticated | Get by id (non-archived) |
| POST | `/api/v1/medicines` | PharmacistOrAdmin | Create medicine |
| PUT | `/api/v1/medicines/{id}` | PharmacistOrAdmin | Update catalog fields (not IsActive/IsDeleted) |
| POST | `/api/v1/medicines/{id}/activate` | PharmacistOrAdmin | Set `IsActive = true` |
| POST | `/api/v1/medicines/{id}/deactivate` | PharmacistOrAdmin | Set `IsActive = false`; block if used in new prescriptions going forward |
| DELETE | `/api/v1/medicines/{id}` | AdminOnly | **Archive** — set `IsDeleted = true` |
| POST | `/api/v1/medicines/{id}/restore` | AdminOnly | Restore archived medicine |
| GET | `/api/v1/medicines/archived` | AdminOnly | List archived medicines (`IgnoreQueryFilters`) |

**Forbidden:** Using DELETE to deactivate. Using deactivate to archive.

### 13.4 Medicine batches

| Method | Route | Roles | Description |
|--------|-------|-------|-------------|
| GET | `/api/v1/medicines/{medicineId}/batches` | PharmacistOrAdmin | List batches for medicine (operational filter) |
| GET | `/api/v1/batches/{id}` | PharmacistOrAdmin | Batch detail |
| POST | `/api/v1/batches` | PharmacistOrAdmin | Receive stock — creates batch (or adds to existing lot if same batch number policy) + `StockMovement(Receive)` |
| POST | `/api/v1/batches/{id}/adjust` | PharmacistOrAdmin | Manual adjustment — updates quantity + `StockMovement(Adjustment)` with required reason |

**Not exposed in v1:**

- `PUT /batches/{id}` — arbitrary batch edits forbidden; use receive/adjust
- `DELETE /batches/{id}` — no public archive; admin correction via dedicated `ArchiveBatchCommand` internal/admin-only if needed

**Batch rules affecting endpoints:**

- FEFO ordering: `ExpiryDate ASC`, then `ReceivedAt ASC`
- Expired batches (`ExpiryDate < today`) excluded from FEFO selection, still visible in GET
- Zero-quantity batches excluded from FEFO, still visible in GET
- Quantity cannot go negative — reject with 409

### 13.5 Inventory (read models)

| Method | Route | Roles | Description |
|--------|-------|-------|-------------|
| GET | `/api/v1/inventory/summary` | PharmacistOrAdmin | Aggregated stock by medicine |
| GET | `/api/v1/inventory/low-stock` | PharmacistOrAdmin | Medicines at or below ReorderLevel |
| GET | `/api/v1/inventory/expiring` | PharmacistOrAdmin | Batches expiring within threshold (query param `days`) |

### 13.6 Stock movements (read-only)

| Method | Route | Roles | Description |
|--------|-------|-------|-------------|
| GET | `/api/v1/stock-movements` | PharmacistOrAdmin | Paginated history; filter by batch, medicine, date, type |
| GET | `/api/v1/stock-movements/{id}` | PharmacistOrAdmin | Single movement |

**Forbidden:** `POST`, `PUT`, `DELETE` on this controller. Creation happens only inside receive, adjust, and dispense workflows.

### 13.7 Prescriptions

| Method | Route | Roles | Description |
|--------|-------|-------|-------------|
| GET | `/api/v1/prescriptions` | DoctorOrAdmin, PharmacistOrAdmin (read) | List non-archived; doctors see own by default |
| GET | `/api/v1/prescriptions/{id}` | DoctorOrAdmin, PharmacistOrAdmin | Detail with items |
| POST | `/api/v1/prescriptions` | DoctorOrAdmin | Create with nested items |
| PUT | `/api/v1/prescriptions/{id}` | DoctorOrAdmin | Update header + items **only if** no dispense has occurred |
| POST | `/api/v1/prescriptions/{id}/cancel` | DoctorOrAdmin | Set `Status = Cancelled` — **not** archive |
| DELETE | `/api/v1/prescriptions/{id}` | AdminOnly | **Archive** — only if never dispensed |
| POST | `/api/v1/prescriptions/{id}/restore` | AdminOnly | Restore archived prescription |
| GET | `/api/v1/prescriptions/archived` | AdminOnly | Admin archive list |

**Forbidden:** DELETE to cancel. PATCH for cancel (use POST sub-resource).

**Expiry:** Background job or query-time evaluation sets `Status = Expired` when `ValidTo < today` and status is Active. Expiry is not deletion.

### 13.8 Prescription items

No dedicated controller. Managed via:

- `CreatePrescriptionCommand` — add items
- `UpdatePrescriptionCommand` — modify items before any dispense
- Partial dispense updates `QuantityDispensed` via `DispensePrescriptionCommand`

**Item rules:**

- All medicines on items must exist, not archived, and `IsActive = true` at creation
- `QuantityPrescribed > 0`
- Cannot remove or reduce below `QuantityDispensed` after partial dispense
- Cannot add new items after first dispense

### 13.9 Dispenses

| Method | Route | Roles | Description |
|--------|-------|-------|-------------|
| POST | `/api/v1/dispenses` | PharmacistOrAdmin | Dispense against prescription (body: prescriptionId, line quantities) |
| GET | `/api/v1/dispenses` | PharmacistOrAdmin | Dispense history |
| GET | `/api/v1/dispenses/{id}` | PharmacistOrAdmin | Detail with items; loads related entities with `IgnoreQueryFilters` where needed |

**Alternative route (align with System Analysis):**

| POST | `/api/v1/dispensing/prescriptions/{prescriptionId}` | PharmacistOrAdmin | Same as above |

Pick one route in implementation; document in Swagger. Prefer `/api/v1/dispenses` for REST consistency.

**Forbidden:** DELETE, PUT on dispense and dispense items.

### 13.10 Users (Admin)

| Method | Route | Roles | Description |
|--------|-------|-------|-------------|
| GET | `/api/v1/users` | AdminOnly | List users |
| GET | `/api/v1/users/{id}` | AdminOnly | User detail |
| POST | `/api/v1/users` | AdminOnly | Create user + assign role |
| PUT | `/api/v1/users/{id}` | AdminOnly | Update profile |
| POST | `/api/v1/users/{id}/deactivate` | AdminOnly | Identity deactivation |
| POST | `/api/v1/users/{id}/activate` | AdminOnly | Identity activation |

### 13.11 Dashboard (optional, later phase)

| Method | Route | Roles | Description |
|--------|-------|-------|-------------|
| GET | `/api/v1/dashboard/summary` | Authenticated (role-filtered) | KPIs |

---

## 14. Endpoint → Command/Query Mapping

### 14.1 Medicines

| Endpoint | Operation | Type |
|----------|-----------|------|
| GET `/medicines` | `GetMedicinesQuery` | Query |
| GET `/medicines/{id}` | `GetMedicineByIdQuery` | Query |
| GET `/medicines/archived` | `GetArchivedMedicinesQuery` | Query |
| POST `/medicines` | `CreateMedicineCommand` | Command |
| PUT `/medicines/{id}` | `UpdateMedicineCommand` | Command |
| POST `/medicines/{id}/activate` | `ActivateMedicineCommand` | Command |
| POST `/medicines/{id}/deactivate` | `DeactivateMedicineCommand` | Command |
| DELETE `/medicines/{id}` | `ArchiveMedicineCommand` | Command |
| POST `/medicines/{id}/restore` | `RestoreMedicineCommand` | Command |

### 14.2 Batches & inventory

| Endpoint | Operation | Type |
|----------|-----------|------|
| GET `/medicines/{id}/batches` | `GetBatchesByMedicineQuery` | Query |
| GET `/batches/{id}` | `GetBatchByIdQuery` | Query |
| POST `/batches` | `ReceiveBatchCommand` | Command |
| POST `/batches/{id}/adjust` | `AdjustBatchStockCommand` | Command |
| GET `/inventory/summary` | `GetInventorySummaryQuery` | Query |
| GET `/inventory/low-stock` | `GetLowStockQuery` | Query |
| GET `/inventory/expiring` | `GetExpiringStockQuery` | Query |

### 14.3 Stock movements

| Endpoint | Operation | Type |
|----------|-----------|------|
| GET `/stock-movements` | `GetStockMovementsQuery` | Query |
| GET `/stock-movements/{id}` | `GetStockMovementByIdQuery` | Query |

### 14.4 Prescriptions

| Endpoint | Operation | Type |
|----------|-----------|------|
| GET `/prescriptions` | `GetPrescriptionsQuery` | Query |
| GET `/prescriptions/{id}` | `GetPrescriptionByIdQuery` | Query |
| GET `/prescriptions/archived` | `GetArchivedPrescriptionsQuery` | Query |
| POST `/prescriptions` | `CreatePrescriptionCommand` | Command |
| PUT `/prescriptions/{id}` | `UpdatePrescriptionCommand` | Command |
| POST `/prescriptions/{id}/cancel` | `CancelPrescriptionCommand` | Command |
| DELETE `/prescriptions/{id}` | `ArchivePrescriptionCommand` | Command |
| POST `/prescriptions/{id}/restore` | `RestorePrescriptionCommand` | Command |

### 14.5 Dispenses

| Endpoint | Operation | Type |
|----------|-----------|------|
| POST `/dispenses` | `DispensePrescriptionCommand` | Command |
| GET `/dispenses` | `GetDispensesQuery` | Query |
| GET `/dispenses/{id}` | `GetDispenseByIdQuery` | Query |

### 14.6 Auth & users

| Endpoint | Operation | Type |
|----------|-----------|------|
| POST `/auth/register` | `RegisterUserCommand` | Command |
| POST `/auth/login` | `LoginCommand` | Command |
| POST `/auth/refresh` | `RefreshTokenCommand` | Command |
| POST `/auth/change-password` | `ChangePasswordCommand` | Command |
| GET `/auth/me` | `GetCurrentUserQuery` | Query |
| GET/POST/PUT `/users/...` | `*UserCommand`/`*UserQuery` | Mixed |

---

## 15. Business Rules

These must be enforced in Application command handlers (and optionally domain methods). **Never replace a business rule with soft delete.**

| # | Rule | Enforcement point |
|---|------|-------------------|
| 1 | Stock movements are append-only | No update/delete handlers; no soft delete on entity |
| 2 | Batch quantity is inventory source of truth | All stock changes update `MedicineBatch.QuantityInStock` |
| 3 | Batch quantity never negative | DB CHECK + handler validation before commit |
| 4 | FEFO dispensing | `DispensePrescriptionCommand` selects batches ordered by ExpiryDate, ReceivedAt |
| 5 | Expired and zero-stock batches excluded from FEFO | Filter in dispense allocation query |
| 6 | Only valid/active prescriptions can be dispensed | `IsValidOn(today) && HasRefillsRemaining()` |
| 7 | Dispensed qty ≤ remaining prescribed qty | Per-line check in dispense handler |
| 8 | RefillsUsed ≤ MaxRefills | Increment RefillsUsed on each dispense event; reject if exceeded |
| 9 | Inactive medicines cannot be used in new prescriptions | Validate on create/update prescription |
| 10 | Historical records remain traceable | Restrict FKs; no delete on dispense/movement |
| 11 | Dispense records permanently queryable | No soft delete, no query filter on Dispense |
| 12 | Medicine/batch references resolvable in history | `IgnoreQueryFilters` on historical reads |

---

## 16. Validation Rules

### 16.1 Medicine

| Field | Rule |
|-------|------|
| Code | Required, unique, max 50 |
| Name | Required, max 100 |
| ReorderLevel | >= 0 |
| Deactivate | Allowed anytime; does not affect past prescriptions |
| Archive | Admin only; return 409 if blocking references exist |

### 16.2 MedicineBatch

| Field | Rule |
|-------|------|
| BatchNumber | Required, unique per medicine |
| ExpiryDate | Required, should be future on receive (warn if past) |
| QuantityInStock | >= 0 |
| Adjust | Reason required for Adjustment movements |

### 16.3 Prescription

| Field | Rule |
|-------|------|
| PrescriptionNumber | Required, unique |
| ValidFrom / ValidTo | ValidFrom <= ValidTo |
| MaxRefills | >= 0 |
| Items | At least one item |
| Cancel | Cannot cancel if already Cancelled; allowed if dispensed (history retained) |
| Archive | Reject if any dispense exists |

### 16.4 Dispense

| Rule | Detail |
|------|--------|
| Prescription state | Active + within dates + refills remaining |
| Lines | At least one line with quantity > 0 |
| Partial dispense | Allowed; update QuantityDispensed cumulatively |
| Atomicity | Batch update + movements + dispense records in one transaction |

---

## 17. Inventory / FEFO Flow

### 17.1 Receive stock

```
POST /batches
  → Validate medicine exists, active, not archived
  → Create or update MedicineBatch
  → Insert StockMovement (Receive, +qty)
  → Commit
```

### 17.2 Adjust stock

```
POST /batches/{id}/adjust
  → Validate reason provided
  → Compute new quantity >= 0
  → Update batch quantity
  → Insert StockMovement (Adjustment, ±qty)
  → Commit
```

### 17.3 FEFO allocation (internal to DispensePrescriptionCommand)

```
For each prescription line to dispense:
  remaining = requested quantity
  batches = query batches for medicine
            WHERE NOT IsDeleted (global filter)
              AND QuantityInStock > 0
              AND ExpiryDate >= today
            ORDER BY ExpiryDate, ReceivedAt

  foreach batch in batches:
    allocate min(batch.QuantityInStock, remaining)
    remaining -= allocated
    if remaining == 0: break

  if remaining > 0: return 409 InsufficientStock
```

---

## 18. Prescription Flow

```
Create (Doctor)
  → Validate medicines active
  → Status = Active, RefillsUsed = 0
  → Save items

Update (Doctor) — only if Dispenses.Count == 0
  → Replace/modify items with same validations

Cancel (Doctor/Admin)
  → Status = Cancelled
  → IsDeleted unchanged
  → Record remains in normal lists

Expire (system)
  → Status = Expired when ValidTo passed
  → IsDeleted unchanged

Archive (Admin)
  → IsDeleted = true
  → Status unchanged
  → Hidden from normal lists

Restore (Admin)
  → IsDeleted = false
```

---

## 19. Dispensing Flow

```
POST /dispenses { prescriptionId, lines: [{ prescriptionItemId, quantity }] }
  1. Load prescription + items (non-archived)
  2. Validate prescription IsValidOn, HasRefillsRemaining
  3. Validate each line quantity <= RemainingQuantity
  4. For each line, run FEFO allocation
  5. Begin transaction:
     a. Create Dispense + DispenseItems
     b. Decrement batch quantities
     c. Insert StockMovement (Dispense, -qty) per allocation
     d. Update PrescriptionItem.QuantityDispensed
     e. Increment Prescription.RefillsUsed
  6. Commit
  7. Return 201 with dispense detail
```

**Post-dispense:** Prescription items cannot be structurally edited. Further fulfillment uses additional dispense events (partial dispense/refill).

---

## 20. Archive / Restore Flow

### 20.1 Archive (DELETE verb)

```
DELETE /{resource}/{id}
  → Authorize (AdminOnly for medicine/prescription)
  → Load entity (respect global filter — 404 if already archived)
  → Run archive guard validations
  → Set IsDeleted = true, DeletedAt = now, DeletedBy = current user
  → SaveChanges
  → Return 204
```

### 20.2 Restore

```
POST /{resource}/{id}/restore
  → Authorize AdminOnly
  → Load with IgnoreQueryFilters
  → If not deleted: return 409
  → Set IsDeleted = false, clear DeletedAt/DeletedBy
  → SaveChanges
  → Return 200
```

### 20.3 What archive does NOT do

- Does not reverse stock quantities
- Does not cancel prescriptions
- Does not deactivate medicines
- Does not delete or hide dispense/stock movement records

---

## 21. Error Handling

Use a consistent problem-details or envelope response:

| Exception / condition | HTTP | Message example |
|----------------------|------|-----------------|
| FluentValidation failures | 400 | Field-level errors |
| Entity not found | 404 | "Medicine not found" |
| Archive blocked | 409 | "Cannot archive medicine referenced by active prescriptions" |
| Insufficient stock | 409 | "Insufficient stock for medicine X" |
| Invalid prescription state | 409 | "Prescription is not active" |
| Unauthorized | 401 | — |
| Forbidden | 403 | — |

Log unexpected exceptions; do not leak internal details in production.

---

## 22. API Versioning

- All routes prefixed with `/api/v1/`.
- Breaking changes require `/api/v2/` and parallel support period.
- Archive/restore semantics are v1 contract — changing DELETE meaning would be breaking.

---

## 23. Implementation Order

| Phase | Module | Deliverables |
|-------|--------|--------------|
| 1 | Foundation | DI, MediatR, UoW, repositories, exception middleware, Swagger, EF migrations |
| 2 | Auth | Identity, JWT, policies, AuthController, seed roles/users |
| 3 | Medicines | CRUD + activate/deactivate + archive/restore |
| 4 | Batches | Receive + list + adjust + stock movements (implicit) |
| 5 | Inventory queries | Summary, low-stock, expiring |
| 6 | Stock movements | Read-only controller |
| 7 | Prescriptions | CRUD + cancel + archive/restore |
| 8 | Dispenses | FEFO dispense workflow |
| 9 | Users + dashboard | Admin user management, optional summary |
| 10 | Frontend + docs | Angular 22 SPA, README runbook |

### 23.1 Domain correction priority (before Dispense module)

1. Change `Dispense` from `SoftDeletableEntity` to `BaseAuditableEntity`
2. Remove query filter from `DispenseConfiguration`
3. Add `DeletedBy` to `SoftDeletableEntity`
4. Change Prescription → Items to Restrict
5. Complete `DispenseItem` and `PrescriptionItem` FK configurations

---

## 24. Testing Strategy

| Area | Test type | Examples |
|------|-----------|----------|
| Domain invariants | Unit | `CanAllocate`, `IsValidOn`, `CanDispense` |
| Command handlers | Unit + in-memory/mock UoW | Archive blocked when dispensed |
| FEFO allocation | Unit | Expired batches skipped, earliest expiry first |
| Dispense atomicity | Integration | Batch qty + movement + dispense committed together |
| Query filters | Integration | Archived medicine hidden from list, visible in admin archive query |
| Historical reads | Integration | Dispense detail resolves archived prescription via IgnoreQueryFilters |
| Authorization | Integration | Doctor cannot dispense; Pharmacist cannot archive |

---

## 25. Migration Considerations

1. **Initial migration** creates all tables with Restrict FKs and query filters on Medicine, MedicineBatch, Prescription.
2. If `Dispense` soft-delete columns already exist in a prior migration, add a migration to drop `IsDeleted`, `DeletedAt`, `DeletedBy` from Dispenses table.
3. Seed: Admin, Pharmacist, Doctor users; sample medicines optional.
4. Prescription expiry job: optional hosted service or documented SQL script for MVP.

---


## Appendix A — Quick reference: DELETE vs business actions

| User intent | Correct endpoint | Wrong endpoint |
|-------------|------------------|----------------|
| Stop ordering a medicine | `POST /medicines/{id}/deactivate` | ~~DELETE /medicines/{id}~~ |
| Hide medicine from catalog | `DELETE /medicines/{id}` (archive) | ~~POST /deactivate~~ |
| Void a prescription | `POST /prescriptions/{id}/cancel` | ~~DELETE /prescriptions/{id}~~ |
| Hide erroneous prescription | `DELETE /prescriptions/{id}` (archive) | ~~POST /cancel~~ |
| Remove stock audit entry | **Not allowed** | ~~DELETE /stock-movements/{id}~~ |
| Undo a dispense | **Not allowed** | ~~DELETE /dispenses/{id}~~ |

---

## Appendix B — PrescriptionItem design decision

**Decision:** No standalone `PrescriptionItemsController`.

**Why:** Items lack an independent lifecycle; they are always owned by a prescription. Nested DTOs in create/update commands reduce API surface area and prevent bypass of prescription-level validations (status, dispense lock, inactive medicine checks).

**Partial dispensing** updates `QuantityDispensed` through `DispensePrescriptionCommand`, not through prescription item PUT.

---

*Last updated: regenerated to resolve soft delete, archive, business state, and API lifecycle ambiguity.*
