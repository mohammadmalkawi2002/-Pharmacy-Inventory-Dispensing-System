# Functional and Non-functional Requirements

## Functional Requirements

### Authentication and Authorization

- **FR-01:** The system shall authenticate users using email and password and issue JWT access and refresh tokens.
- **FR-02:** The system shall support logout, refresh-token rotation and revocation, password change, forgot password, reset password, and current-user retrieval.
- **FR-03:** The system shall enforce role and permission policies on the server and return `403 Forbidden` to authenticated users without permission.
- **FR-04:** The system shall restrict doctors to owned prescriptions where ownership is required while allowing administrator bypass.
- **FR-05:** The system shall rate-limit authentication endpoints and return `429 Too Many Requests` when limits are exceeded.

### Patients

- **FR-06:** Authorized users shall create, update, search, view, paginate, archive, and restore patient records.
- **FR-07:** Patient document IDs shall be unique, and archived patients shall be excluded from normal queries.

### Medicines and Stock

- **FR-08:** Authorized users shall create, update, search, filter, sort, paginate, activate, deactivate, archive, and restore medicines.
- **FR-09:** The system shall track medicine quantities in a base `StockUnit` and convert received packages using `UnitsPerPackage`.
- **FR-10:** The system shall calculate `Normal`, `LowStock`, and `OutOfStock` status without allowing negative stock.
- **FR-11:** Archived or inactive medicines shall not be added to new prescriptions.
- **FR-12:** Stock availability shall not prevent a doctor from creating a prescription; stock shall be validated during dispensing.

### Prescriptions

- **FR-13:** Authorized doctors or administrators shall create prescriptions for existing, non-archived patients.
- **FR-14:** The server shall generate a unique prescription number in the `RX-000001` format and assign the authenticated creator as `DoctorId`.
- **FR-15:** A prescription shall contain at least one item and shall not contain the same medicine more than once.
- **FR-16:** Each prescription item shall define its prescribed quantity, dosage instructions, maximum fill count, and used fill count.
- **FR-17:** Authorized users shall list, view, update, cancel, and look up prescriptions according to their permissions and ownership rules.
- **FR-18:** Pharmacist lookup shall require both prescription number and patient document ID.
- **FR-19:** Active prescriptions whose `ValidTo` date has passed shall be marked expired by a background service.

### Dispensing

- **FR-20:** Authorized pharmacists or administrators shall dispense selected prescription items from a valid active prescription.
- **FR-21:** Partial dispensing shall operate at item level: the user may select some prescription items, but each selected item must be dispensed in its complete prescribed quantity.
- **FR-22:** Before dispensing, the system shall validate prescription dates/status, patient document ID, medicine activity, remaining fills, and available stock.
- **FR-23:** A successful dispensing operation shall create `Dispense` and `DispenseItem` records, decrease stock, and increment fill usage in one atomic save operation.
- **FR-24:** The system shall provide paginated dispensing history and dispensing details.

### Users and Dashboard

- **FR-25:** Administrators shall manage staff users according to user-management permissions.
- **FR-26:** The dashboard shall display only statistics allowed by the authenticated user's permissions.
- **FR-27:** Doctor prescription statistics shall be limited to the doctor's records, pharmacist dispensing statistics shall be limited to the pharmacist's records, and administrators shall receive system-wide statistics.

## Non-functional Requirements

- **NFR-01 — Security:** Passwords shall be managed by ASP.NET Core Identity, refresh tokens shall be stored securely, secrets shall not be committed, and authorization shall be enforced server-side.
- **NFR-02 — Reliability:** Dispensing database changes shall be committed atomically so failed operations do not leave partial state.
- **NFR-03 — Performance:** List endpoints shall use database-side filtering, sorting, projection/loading strategies, and pagination.
- **NFR-04 — Maintainability:** The codebase shall follow Clean Architecture, dependency inversion, CQRS, repository, Unit of Work, and centralized dependency injection.
- **NFR-05 — Validation:** Requests shall be validated consistently using FluentValidation and business errors shall use the shared Result/error model.
- **NFR-06 — API Consistency:** The API shall use RESTful versioned routes and consistent status codes and error responses.
- **NFR-07 — Observability:** The backend shall use structured logging and include trace information in unexpected error responses.
- **NFR-08 — Usability:** The Angular interface shall be responsive and hide unavailable navigation and actions based on permissions, while treating backend authorization as authoritative.
- **NFR-09 — Compatibility:** The application shall run with .NET 10, Angular 22, Node.js 24 LTS, and SQL Server 2022.
- **NFR-10 — Documentation:** A new developer shall be able to clone, configure, migrate, and run both applications using the repository README.

## Out of Scope

- Medicine batches
- Batch expiry tracking
- FEFO stock allocation
- Stock-movement accounting
- Automatic strength-to-quantity conversion

