using HospitalManagement.Application.Interfaces.Persistence;
using HospitalManagement.Application.Interfaces.Repositories;
using HospitalManagement.Domain.Entities.Patients;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Persistence.Ado;

public class AdoPatientRepository : AdoRepository<Patient>, IPatientRepository
{
    public AdoPatientRepository(ISqlConnectionFactory sqlConnectionFactory) 
        : base(sqlConnectionFactory)
    {
    }

    #region CRUD METHODS
    public async Task<List<Patient>> GetAllAsync()
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync();

        var cmd = CreateCommand(@"
            SELECT Id, Type, PatientNumber, Name, DateOfBirth,
                   Address, PhoneNumber, Email,
                   DepartmentId, IsDischarged, DischargeDate,
                   IsAdmitted, AdmissionDate
            FROM Patients", conn);

        var patients = await QueryAsync(cmd, ReadPatient);

        foreach (var patient in patients)
            await LoadRelationsAsync(patient, conn);

        return patients;
    }

    public async Task<Patient?> GetByIdAsync(Guid id)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync();

        var cmd = CreateCommand(@"
            SELECT Id, Type, PatientNumber, Name, DateOfBirth,
                   Address, PhoneNumber, Email,
                   DepartmentId, IsDischarged, DischargeDate,
                   IsAdmitted, AdmissionDate
            FROM Patients WHERE Id = @Id", conn);
        AddGuidParam(cmd, "@Id", id);

        var patient = await QuerySingleAsync(cmd, ReadPatient);
        if (patient is null) 
            return null;

        await LoadRelationsAsync(patient, conn);
        return patient;
    }

    public async Task AddAsync(Patient patient)
    {
        await ExecuteInTransactionAsync(async (conn, tx) =>
        {
            await InsertPatientAsync(patient, conn, tx);

            foreach (var treatmentId in patient.ExternalTreatmentIds)
                await InsertPatientTreatmentAsync(patient.Id, treatmentId, "external", conn, tx);

            if (patient is InternalPatient ip)
                foreach (var treatmentId in ip.InternalTreatments)
                    await InsertPatientTreatmentAsync(patient.Id, treatmentId, "internal", conn, tx);
        });
    }

    public async Task UpdateAsync(Patient patient)
    {
        await ExecuteInTransactionAsync(async (conn, tx) =>
        {
            // 1. حدّث بيانات المريض
            var cmd = CreateCommand(@"
                UPDATE Patients SET
                    Name          = @Name,
                    DateOfBirth   = @DateOfBirth,
                    Address       = @Address,
                    PhoneNumber   = @PhoneNumber,
                    Email         = @Email,
                    IsDischarged  = @IsDischarged,
                    DischargeDate = @DischargeDate,
                    IsAdmitted    = @IsAdmitted,
                    AdmissionDate = @AdmissionDate
                WHERE Id = @Id", conn, tx);
            AddPatientParams(cmd, patient);
            await cmd.ExecuteNonQueryAsync();

            // 2. حدّث المعالجات
            var deleteCmd = CreateCommand(
                "DELETE FROM PatientTreatments WHERE PatientId = @PatientId", conn, tx);
            AddGuidParam(deleteCmd, "@PatientId", patient.Id);
            await deleteCmd.ExecuteNonQueryAsync();

            foreach (var treatmentId in patient.ExternalTreatmentIds)
                await InsertPatientTreatmentAsync(patient.Id, treatmentId, "external", conn, tx);

            if (patient is InternalPatient ip)
                foreach (var treatmentId in ip.InternalTreatments)
                    await InsertPatientTreatmentAsync(patient.Id, treatmentId, "internal", conn, tx);
        });
    }

    public async Task DeleteAsync(Patient patient)
    {
        await ExecuteInTransactionAsync(async (conn, tx) =>
        {
            var cmd = CreateCommand(
                "DELETE FROM Patients WHERE Id = @Id", conn, tx);
            AddGuidParam(cmd, "@Id", patient.Id);
            await cmd.ExecuteNonQueryAsync();
        });
    }
    #endregion

    #region IPatientRepository Methods
    public async Task<bool> ExistsAsync(string name, DateOnly? dateOfBirth)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync();

        var cmd = CreateCommand(@"
            SELECT COUNT(1) FROM Patients
            WHERE Name = @Name AND DateOfBirth = @DateOfBirth", conn);
        AddParam(cmd, "@Name", name);
        AddParam(cmd, "@DateOfBirth", dateOfBirth.HasValue
            ? dateOfBirth.Value.ToDateTime(TimeOnly.MinValue)
            : DBNull.Value);

        return (int)(await cmd.ExecuteScalarAsync())! > 0;
    }

    public async Task<Patient?> GetByNumberAsync(string patientNumber)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync();

        var cmd = CreateCommand(@"
            SELECT Id, Type, PatientNumber, Name, DateOfBirth,
                   Address, PhoneNumber, Email,
                   DepartmentId, IsDischarged, DischargeDate,
                   IsAdmitted, AdmissionDate
            FROM Patients WHERE PatientNumber = @PatientNumber", conn);
        AddParam(cmd, "@PatientNumber", patientNumber);

        var patient = await QuerySingleAsync(cmd, ReadPatient);
        if (patient is null) return null;

        await LoadRelationsAsync(patient, conn);
        return patient;
    }

    public async Task<IReadOnlyList<InternalPatient>> GetByDepartmentAsync(Guid departmentId)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync();

        var cmd = CreateCommand(@"
            SELECT Id, Type, PatientNumber, Name, DateOfBirth,
                   Address, PhoneNumber, Email,
                   DepartmentId, IsDischarged, DischargeDate,
                   IsAdmitted, AdmissionDate
            FROM Patients
            WHERE Type = 'internal' AND DepartmentId = @DepartmentId", conn);
        AddGuidParam(cmd, "@DepartmentId", departmentId);

        var patients = await QueryAsync(cmd, ReadPatient);

        foreach (var patient in patients)
            await LoadRelationsAsync(patient, conn);

        return patients.OfType<InternalPatient>().ToList();
    }

    public async Task<IReadOnlyList<Patient>> GetByTypeAsync(string type)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync();

        var cmd = CreateCommand(@"
            SELECT Id, Type, PatientNumber, Name, DateOfBirth,
                   Address, PhoneNumber, Email,
                   DepartmentId, IsDischarged, DischargeDate,
                   IsAdmitted, AdmissionDate
            FROM Patients WHERE Type = @Type", conn);
        AddParam(cmd, "@Type", type.ToLower());

        var patients = await QueryAsync(cmd, ReadPatient);

        foreach (var patient in patients)
            await LoadRelationsAsync(patient, conn);

        return patients;
    }
    #endregion

    #region Private
    private static Patient ReadPatient(SqlDataReader r)
    {
        var type = GetString(r, "Type").ToLower();

        if (type == "internal")
            return new InternalPatient(
                id: GetGuid(r, "Id"),
                patientNumber: GetString(r, "PatientNumber"),
                name: GetString(r, "Name"),
                dateOfBirth: DateOnly.FromDateTime(GetDateTime(r, "DateOfBirth")),
                address: GetNullableString(r, "Address"),
                phone: GetNullableString(r, "PhoneNumber"),
                email: GetNullableString(r, "Email"),
                departmentId: GetGuid(r, "DepartmentId"),
                internalTreatments: null,
                externalTreatments: null,
                isDischarged: GetBool(r, "IsDischarged"),
                dischargeDate: GetNullableDateTime(r, "DischargeDate"));

        return new ExternalPatient(
            patientId: GetGuid(r, "Id"),
            patientNumber: GetString(r, "PatientNumber"),
            name: GetString(r, "Name"),
            dateOfBirth: DateOnly.FromDateTime(GetDateTime(r, "DateOfBirth")),
            address: GetNullableString(r, "Address"),
            phone: GetNullableString(r, "PhoneNumber"),
            email: GetNullableString(r, "Email"),
            externalTreatmentIds: null,
            isAdmitted: GetBool(r, "IsAdmitted"),
            admissionDate: GetNullableDateTime(r, "AdmissionDate"));
    }

    private async Task LoadRelationsAsync(Patient patient, SqlConnection conn)
    {
        var externalCmd = CreateCommand(@"
            SELECT TreatmentId FROM PatientTreatments
            WHERE PatientId = @PatientId AND TreatmentType = 'external'", conn);
        AddGuidParam(externalCmd, "@PatientId", patient.Id);

        var externalIds = await QueryAsync(externalCmd, r => GetGuid(r, "TreatmentId"));
        patient.LoadExternalTreatmentIds(externalIds);

        if (patient is InternalPatient ip)
        {
            var internalCmd = CreateCommand(@"
                SELECT TreatmentId FROM PatientTreatments
                WHERE PatientId = @PatientId AND TreatmentType = 'internal'", conn);
            AddGuidParam(internalCmd, "@PatientId", patient.Id);

            var internalIds = await QueryAsync(internalCmd, r => GetGuid(r, "TreatmentId"));
            foreach (var id in internalIds)
                ip.LoadInternalTreatment(id);
        }
    }

    private async Task InsertPatientAsync(Patient patient, SqlConnection conn, SqlTransaction tx)
    {
        var cmd = CreateCommand(@"
            INSERT INTO Patients
                (Id, Type, PatientNumber, Name, DateOfBirth, Address, PhoneNumber, Email,
                 DepartmentId, IsDischarged, DischargeDate, IsAdmitted, AdmissionDate)
            VALUES
                (@Id, @Type, @PatientNumber, @Name, @DateOfBirth, @Address, @PhoneNumber, @Email,
                 @DepartmentId, @IsDischarged, @DischargeDate, @IsAdmitted, @AdmissionDate)",
            conn, tx);
        AddPatientParams(cmd, patient);
        await cmd.ExecuteNonQueryAsync();
    }

    private async Task InsertPatientTreatmentAsync(
        Guid patientId, Guid treatmentId, string type, SqlConnection conn, SqlTransaction tx)
    {
        var cmd = CreateCommand(@"
            IF NOT EXISTS (
                SELECT 1 FROM PatientTreatments
                WHERE PatientId = @PatientId AND TreatmentId = @TreatmentId)
            INSERT INTO PatientTreatments (PatientId, TreatmentId, TreatmentType)
            VALUES (@PatientId, @TreatmentId, @TreatmentType)", conn, tx);

        AddGuidParam(cmd, "@PatientId", patientId);
        AddGuidParam(cmd, "@TreatmentId", treatmentId);
        AddParam(cmd, "@TreatmentType", type);
        await cmd.ExecuteNonQueryAsync();
    }

    private static void AddPatientParams(SqlCommand cmd, Patient patient)
    {
        AddGuidParam(cmd, "@Id", patient.Id);
        AddParam(cmd, "@Type", patient.Type);
        AddParam(cmd, "@PatientNumber", patient.PatientNumber);
        AddParam(cmd, "@Name", patient.Name);
        AddParam(cmd, "@DateOfBirth", patient.DateOfBirth.HasValue
            ? patient.DateOfBirth.Value.ToDateTime(TimeOnly.MinValue)
            : DBNull.Value);
        AddParam(cmd, "@Address", patient.Address);
        AddParam(cmd, "@PhoneNumber", patient.PhoneNumber);
        AddParam(cmd, "@Email", patient.Email);

        if (patient is InternalPatient ip)
        {
            AddNullableGuidParam(cmd, "@DepartmentId", ip.DepartmentId);
            AddParam(cmd, "@IsDischarged", ip.IsDischarged);
            AddNullableDateParam(cmd, "@DischargeDate", ip.DischargeDate);
            AddParam(cmd, "@IsAdmitted", DBNull.Value);
            AddNullableDateParam(cmd, "@AdmissionDate", null);
        }
        else if (patient is ExternalPatient ep)
        {
            AddNullableGuidParam(cmd, "@DepartmentId", null);
            AddParam(cmd, "@IsDischarged", DBNull.Value);
            AddNullableDateParam(cmd, "@DischargeDate", null);
            AddParam(cmd, "@IsAdmitted", ep.IsAdmitted);
            AddNullableDateParam(cmd, "@AdmissionDate", ep.AdmissionDate);
        }
    }
    #endregion
}
