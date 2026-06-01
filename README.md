# 🏥 Hospital Management System

A hospital management system built on **Clean Architecture** and **Rich Domain Model** principles using C# / .NET.

---

## 📋 Overview

The system manages core hospital operations — doctors, patients, treatments, and departments — with support for multiple role types and automatic salary calculation based on each doctor's role.

---

## 🏗️ Project Structure

```
HospitalManagement/
│
├── Domain/
│   ├── Entities/
│   │   ├── Doctors/          # Doctor, DoctorRole, PermanentRole, TraineeRole, ContractedRole
│   │   ├── Patients/         # Patient (abstract), InternalPatient, ExternalPatient
│   │   ├── Treatments/       # Treatment (abstract), TreatmentInternal, TreatmentExternal
│   │   └── Departments/      # Department
│   ├── Enums/                # Specialization, TreatmentRole
│   └── Contracts/            # IEntity
│
├── Application/
│   └── Services/             # DoctorService, PatientService, TreatmentService...
│
├── Infrastructure/
│   └── Repositories/
│       └── DTOs/             # JsonXRepository (current) — SqlXRepository (planned)
│
└── Presentation/
    ├── ConsoleApp/           # Program.cs (current)
    └── WebApp/               # (planned)
```

---

## 🧩 Core Entities

### `Doctor`
The central entity of the system.

| Property | Type | Description |
|---|---|---|
| `Id` | `Guid` | Unique identifier |
| `DoctorNumber` | `string` | Doctor's assigned number |
| `Specialization` | `enum` | Medical specialization |
| `DepartmentsIds` | `List<Guid>` | Assigned departments |
| `Roles` | `List<DoctorRole>` | All roles history |
| `ActiveRole` | `DoctorRole?` | Currently active role (computed) |

---

### `DoctorRole` — abstract
Each doctor has one active role at a time. Three concrete types:

```
DoctorRole (abstract)
├── PermanentRole     → Full-time permanent staff
├── TraineeRole       → Trainee / resident
└── ContractedRole    → Contracted doctor
```

Each role contains:
- `StartDate` / `EndDate`
- `IsActive` — computed from dates
- `SalaryHistory` — monthly salary records
- `CalculateSalary()` — abstract, each type implements its own logic

---

### `Patient` — abstract

```
Patient (abstract)
├── InternalPatient   → Admitted patient (linked to a department)
└── ExternalPatient   → Outpatient / walk-in
```

---

### `Treatment` — abstract

```
Treatment (abstract)
├── TreatmentInternal
└── TreatmentExternal
```

The link between a doctor and a treatment is stored in the junction table `DoctorTreatment`, which also holds the `TreatmentRole` (primary, assistant, etc.).

---

## 💰 Salary Calculation

### Permanent — `PermanentRole`
```
Salary = BaseSalary × 1.10 ^ (yearsOfService / 2)
```
Increment is calculated automatically based on `StartDate`.

### Trainee — `TraineeRole`
```
Year 1 → BaseSalary × 50%
Year 2 → BaseSalary × 75%
```
Values are sourced from `SalaryConfig` in the Application layer.

### Contracted — `ContractedRole`
```
Salary = 50% of the total cost of treatments the doctor participated in
```

---

## 🔗 Entity Relationships

| Relationship | Storage Strategy |
|---|---|
| Doctor ↔ Department | `List<Guid> DepartmentsIds` on `Doctor` |
| Doctor ↔ Treatment | Junction table `DoctorTreatment` |
| Treatment → Patient | `int PatientId` on `Treatment` |
| Patient → Department | `int? DepartmentId` on `InternalPatient` |
| DoctorRole → Doctor | `int DoctorId` on `DoctorRole` |

---

## 🎯 Design Principles

| Principle | Application |
|---|---|
| **Clean Architecture** | Domain → Application → Infrastructure → Presentation |
| **Rich Domain Model** | Business logic lives inside the entity, not in Services |
| **Dependency Inversion** | `IRepository`, `INumberGenerator`, `IPermanentSalary` |
| **Single Responsibility** | Each class is responsible for one thing only |

---

## ⚠️ Known Issues (TODO)

- [ ] **`JsonIgnore` in Domain layer** — must be moved entirely to DTOs in Infrastructure
- [ ] **`IsActive` is a plain property** — must be computed: `EndDate == null || EndDate > DateTime.Now`
- [ ] **Permanent salary calculation is wrong** — must use `Math.Pow(1.10, increments)` not simple multiplication
- [ ] **`Roles` and `Treatments` are nullable** — must use `= new()` without `?`
- [ ] **`Console.WriteLine` in Domain** — Domain layer must have zero knowledge of the UI
- [ ] **Missing `PatientNumber` in constructors** — `ExternalPatient` and the second `Patient` constructor

---

## 🚀 Getting Started

```bash
# Clone the repository
git clone https://github.com/your-username/HospitalManagement.git
cd HospitalManagement

# Build
dotnet build

# Run the Console App
dotnet run --project Presentation/ConsoleApp
```

---

## 🗺️ Roadmap

- [x] Domain layer — Doctors, Roles, Patients, Treatments
- [x] JSON Repository (Infrastructure)
- [ ] Fix known issues listed above
- [ ] Application Services (DoctorService, PatientService...)
- [ ] SQL Repository
- [ ] Web API (ASP.NET Core)

---

## 🛠️ Tech Stack

- **C# / .NET 8**
- **Clean Architecture**
- **JSON** for storage (SQL planned)