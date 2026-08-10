# Pharmacy Inventory & Dispensing — Requirement Compliance Review

This document reviews the current system analysis against the internship requirements and preserves the required architecture and implementation patterns. The goal is not to add scope, but to verify that the design stays faithful to the specification.

---

## 1. Executive review

The current system analysis is largely directionally correct because it identifies the core domain: medicines, batches, stock movement, prescriptions, dispensing, and role-based access. It also preserves several explicit implementation expectations that should remain in the design:

- Clean Architecture layering
- CQRS with MediatR
- Repository + Unit of Work style persistence
- JWT-based authentication
- Angular frontend
- Swagger and documentation

However, the analysis still needs tightening. It is not yet fully compliant because it leaves several important requirements implicit rather than explicit. In particular, the document should state the authentication and authorization model more clearly, justify the persistence approach, specify the Angular version and frontend scope, and document how another developer can run the project locally.

---

## 2. Requirement compliance report

| Area | Status | Notes |
|------|--------|-------|
| Clean Architecture | ✅ Correct | The document uses a layered structure and preserves dependency direction. |
| CQRS with MediatR | ✅ Correct | The analysis already uses MediatR and command/query separation. |
| Repository + Unit of Work | ⚠ Missing justification | The document mentions repositories and UoW but does not justify the choice clearly enough. |
| JWT authentication | ⚠ Missing detail | Roles are listed, but the design should define the auth flow and authorization policies explicitly. |
| Angular requirement | ⚠ Missing detail | The analysis says Angular, but it should explicitly state the Angular version and the expected frontend scope. |
| Documentation and run instructions | ⚠ Missing | The analysis mentions Swagger and README, but the design should describe how to run the project locally. |
| Core domain entities | ✅ Correct | The seven entities are appropriate for the pharmacy workflow. |
| Core business rules | ✅ Correct | The stock, dispensing, and prescription rules are sound and aligned with the domain. |
| API surface | ✅ Correct | The endpoints are appropriate for the core workflow. |

---

## 3. Missing requirements

The following requirements are not yet stated clearly enough in the analysis:

1. Authentication and authorization model
   - The document should explicitly state that the API uses JWT authentication.
   - It should define at least the core policy groups, such as Admin, Pharmacist, and Doctor.
   - It should explain which endpoints require which roles.

2. Persistence approach justification
   - The requirement allows either CQRS with MediatR or a clean service layer, and either Repository + UoW or EF Core DbContext as UoW.
   - The document should make the chosen approach explicit and justify it.
   - The current document says “Hybrid repositories + Unit of Work,” which is acceptable, but it should explain why this approach was chosen instead of a simpler service-layer design.

3. Angular requirement detail
   - The current document says “Angular SPA,” but it should specify the Angular version and the intended feature structure more clearly.
   - If the official requirement is Angular 22, the analysis should say so explicitly.

4. Local setup and run instructions
   - The analysis should include a short runbook covering:
     - prerequisites,
     - database setup,
     - migration commands,
     - seed data,
     - default login credentials,
     - how to access Swagger and the frontend.

5. Project naming alignment
   - The current design uses generic names such as Pharmacy.Api and Pharmacy.Application.
   - The repository already uses PharmacyInventoryDispensingSystem.WebApi, PharmacyInventoryDispensingSystem.Application, PharmacyInventoryDispensingSystem.Domain, and PharmacyInventoryDispensingSystem.Infrastructure.
   - The analysis should use the repository’s actual names for consistency.

---

## 4. Incorrect assumptions

These are not necessarily wrong, but they should not be presented as if they are required by the internship specification unless the specification explicitly says so:

### 4.1 “No Category module” is an implementation choice, not a hard requirement

The analysis states that categories are out of scope. That is acceptable only if the official requirements clearly exclude them. If the requirements do not explicitly say that categories are forbidden, this should be treated as a scoped simplification rather than a design principle.

### 4.2 “No Patient entity” should be framed carefully

The design uses inline patient fields on the prescription. That is a valid simplification, but it should be described as a deliberate scope choice rather than as an absolute rule unless the requirements explicitly require it.

### 4.3 “StockMovement is audit-only” is acceptable, but it should not be overspecified

The current analysis treats stock movement as purely audit data. That is reasonable for a first version, but if the official requirements do not explicitly forbid ledger-style fields, this should remain a design decision instead of a hard architectural statement.

---

## 5. Recommended changes

The following changes are recommended because they improve compliance without violating the requirements:

1. Preserve the required architecture patterns
   - Keep Clean Architecture layering.
   - Keep CQRS with MediatR.
   - Keep Repository + Unit of Work style persistence.
   - Keep JWT-based security.
   - Keep Angular as the frontend technology.

2. Make the architecture explicit
   - State that the dependency flow is API → Application → Domain and Infrastructure supports the application layer.
   - State that commands and queries are separated using MediatR handlers.
   - State that repositories abstract persistence and the Unit of Work pattern manages transactions.

3. Strengthen the authentication and authorization section
   - Define roles clearly.
   - Define authorization policies.
   - Map each endpoint to the minimum required role.

4. Clarify the frontend requirement
   - Specify Angular 22 as the frontend framework.
   - Define the feature modules: auth, medicines, inventory, prescriptions, dispensing, users.

5. Add documentation requirements to the analysis
   - Include a short README section describing how to run the project.
   - Include migration and seeding steps.
   - Include Swagger and frontend access points.

---

## 6. Updated System Analysis

### 6.1 Project purpose

The system is a pharmacy inventory and dispensing application built to support medicine catalog management, batch-level stock tracking, stock movement history, prescription creation, and dispensing workflows. The project should demonstrate strong software engineering practices, including Clean Architecture, CQRS with MediatR, repository-based persistence, secure authentication, and a maintainable Angular frontend.

### 6.2 Architectural style

The solution should follow Clean Architecture with four main layers:

| Layer | Project | Responsibility |
|------|---------|----------------|
| API | PharmacyInventoryDispensingSystem.WebApi | Controllers, authentication, validation, Swagger, CORS |
| Application | PharmacyInventoryDispensingSystem.Application | Commands, queries, DTOs, validators, application services |
| Domain | PharmacyInventoryDispensingSystem.Domain | Entities, enums, domain rules, core business logic |
| Infrastructure | PharmacyInventoryDispensingSystem.Infrastructure | EF Core persistence, Identity, repositories, Unit of Work, migrations |

The dependency direction should remain inward:

- API depends on Application and Infrastructure.
- Application depends on Domain.
- Infrastructure depends on Application and Domain.

### 6.3 Required patterns

The design should preserve the required patterns unless the specification explicitly allows an alternative and the README justifies it.

#### CQRS with MediatR

The application layer should use CQRS with MediatR.

- Commands handle write operations such as create, update, and dispense.
- Queries handle read operations such as inventory, low stock, prescriptions, and stock history.
- This separation keeps the application layer organized and makes the business workflow easier to test.

#### Repository + Unit of Work

The persistence layer should use repositories and a Unit of Work abstraction.

- Repositories manage data access for specific aggregates or entities.
- Unit of Work coordinates transactions and ensures related writes are committed consistently.
- EF Core DbContext can be used as the underlying persistence mechanism, but the design should expose repository and UoW abstractions at the application boundary.

This is a valid implementation choice and should be documented clearly in the README.

### 6.4 Authentication and authorization

The API should use ASP.NET Core Identity with JWT authentication.

#### Roles

- Admin
- Pharmacist
- Doctor

#### Authorization policies

- AdminOnly
- PharmacistOrAdmin
- DoctorOrAdmin

#### Expected access control

- Admin can manage users, medicines, inventory, and view all prescriptions.
- Pharmacist can manage batches, stock movement, inventory, and dispensing.
- Doctor can create and view prescriptions.

The API should enforce these roles using policy-based authorization.

### 6.5 Domain entities

The core entities remain:

| Entity | Purpose | Key attributes |
|--------|---------|----------------|
| Medicine | Catalog item | Code, Name, Strength, Form, Unit, ReorderLevel, IsActive |
| MedicineBatch | Lot-level stock | MedicineId, BatchNumber, ExpiryDate, QuantityInStock, ReceivedAt |
| StockMovement | Historical stock change record | MedicineBatchId, MovementType, QuantityChange, Reason, CreatedBy, CreatedAt |
| Prescription | Clinical order | PrescriptionNumber, PatientName, PatientPhone, DoctorId, ValidFrom, ValidTo, MaxRefills, RefillsUsed, Status |
| PrescriptionItem | Prescription line | MedicineId, QuantityPrescribed, QuantityDispensed, DosageInstructions |
| Dispense | Fulfillment event | PrescriptionId, PharmacistId, DispensedAt |
| DispenseItem | Batch allocation | PrescriptionItemId, MedicineBatchId, Quantity |

### 6.6 Relationships

- One medicine has many batches.
- One batch has many stock movements.
- One prescription has many prescription items.
- One prescription has many dispenses.
- One dispense has many dispense items.
- One dispense item references one batch.
- Prescription and dispense records should reference the acting user identity.

### 6.7 Business rules

The system should enforce the following rules:

1. Batch stock is the source of truth for inventory.
2. Every stock-changing operation must update the batch quantity and create a stock movement in the same transaction.
3. Batch quantity cannot become negative.
4. Dispensing must follow FEFO ordering: earliest expiry first, then earliest received date.
5. Expired and zero-quantity batches must not be used for dispensing.
6. A prescription must be active and within its validity period before it can be dispensed.
7. A dispense cannot exceed the remaining prescribed quantity on a prescription line.
8. Refill usage must stay within the max refill allowance.
9. Stock movements are append-only and should not be updated or deleted.
10. Inactive medicines should not be used for new prescriptions.

### 6.8 API surface

The API should provide the following core routes under /api/v1:

| Module | Endpoints |
|--------|-----------|
| Auth | register, login, refresh, change-password |
| Medicines | CRUD operations |
| Batches | create and list batches for a medicine |
| Inventory | inventory summary, low-stock, expiring stock |
| Stock movements | list stock history |
| Prescriptions | create, list, view, cancel |
| Dispensing | dispense a prescription |
| Users | admin-only user management |

### 6.9 Angular frontend

The frontend should be built with Angular 22.

Suggested feature areas:

- auth
- medicines
- inventory
- prescriptions
- dispensing
- users

The frontend should consume the API through services and use route guards for authorization.

### 6.10 Documentation and README

The README must document:

- prerequisites,
- solution structure,
- how to restore packages,
- how to configure the database,
- how to apply EF Core migrations,
- how to seed admin data,
- how to run the API,
- how to run the Angular frontend,
- how to test the main workflow through Swagger and the UI.

---

## 7. Final checklist

| Requirement | Covered |
|------|----------|
| Clean Architecture | ✅ |
| CQRS with MediatR | ✅ |
| Repository + Unit of Work | ✅ |
| JWT authentication | ✅ |
| Role-based authorization | ✅ |
| Angular frontend | ✅ |
| Swagger support | ✅ |
| Core pharmacy domain entities | ✅ |
| Stock and dispensing rules | ✅ |
| Documentation and run instructions | ✅ |

The analysis is now aligned more closely with the internship requirements while preserving the required architecture and implementation patterns.
