# 🏥 Hospital Management System

A hospital management REST API built on **Clean Architecture** and **Rich Domain Model** principles using C# / .NET.

---

## 📋 Overview

The system manages core hospital operations — doctors, patients, treatments, and departments — with support for multiple role types and automatic salary calculation based on each doctor's role. The API serves as the backend for a companion Flutter mobile application.

---

## 🏗️ Project Structure

HospitalManagement/
│
├── HospitalManagement.API/
│ ├── Controllers/
│ ├── Middleware/
│ ├── Data/
│ └── Program.cs
│
├── HospitalManagement.Application/
│ ├── Common/
│ ├── DependencyInjection/
│ ├── DTOs/
│ ├── Interfaces/
│ ├── Mappers/
│ ├── Services/
│ └── Validators/
│
├── HospitalManagement.Domain/
│ ├── Contracts/
│ └── Entities/
│
└── HospitalManagement.Infrastructure/
├── Configuration/
├── Data/
├── DependencyInjection/
├── Persistence/
└── Serialization/


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

DoctorRole (abstract)
├── PermanentRole → Full-time permanent staff
├── TraineeRole → Trainee / resident
└── ContractedRole → Contracted doctor


Each role contains:
- `StartDate` / `EndDate`
- `IsActive` — computed from dates
- `SalaryHistory` — monthly salary records
- `CalculateSalary()` — abstract, each type implements its own logic

---

### `Patient` — abstract

Patient (abstract)
├── InternalPatient → Admitted patient (linked to a department)
└── ExternalPatient → Outpatient / walk-in


---

### `Treatment` — abstract

Treatment (abstract)
├── TreatmentInternal
└── TreatmentExternal


The link between a doctor and a treatment is stored in the junction table `DoctorTreatment`, which also holds the `TreatmentRole` (primary, assistant, etc.).

---

## 💰 Salary Calculation

### Permanent — `PermanentRole`

Salary = BaseSalary × 1.10 ^ (yearsOfService / 2)

Increment is calculated automatically based on `StartDate`.

### Trainee — `TraineeRole`

Year 1 → BaseSalary × 50%
Year 2 → BaseSalary × 75%

Values are sourced from `SalaryConfig` in the Application layer.

### Contracted — `ContractedRole`

Salary = 50% of the total cost of treatments the doctor participated in


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

## 💾 Storage Strategy

The system supports **two interchangeable storage backends** — JSON file storage and a database — controlled by a **single configuration key** in `appsettings.json`. No changes are needed in the Domain or Application layers to switch between them, demonstrating the architecture's adherence to the Dependency Inversion Principle.

---

## 🎯 Design Principles

| Principle | Application |
|---|---|
| **Clean Architecture** | API → Application → Domain → Infrastructure |
| **Rich Domain Model** | Business logic lives inside the entity, not in Services |
| **Dependency Inversion** | Storage and salary logic are abstracted behind interfaces |
| **Single Responsibility** | Each class is responsible for one thing only |

---

## 🔌 Client Integration

This API serves as the backend for a companion **Flutter** mobile application (developed separately).

---

## 🛠️ Tech Stack

- **C# / .NET**
- **ASP.NET Core Web API**
- **Clean Architecture**
- **Rich Domain Model**
- **Configurable storage (JSON / Database)**

---

## 🚀 Getting Started

```bash
# Clone the repository
git clone https://github.com/wesamber/HospitalManagement.git
cd HospitalManagement

# Build
dotnet build

# Run the API
dotnet run --project HospitalManagement.API
```
