using HospitalManagement.Application.Interfaces.Persistence;
using HospitalManagement.Application.Interfaces.Repositories;
using HospitalManagement.Domain.Entities.Doctors;
using HospitalManagement.Domain.Entities.Enums;
using HospitalManagement.Domain.Entities.Treatments;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Persistence.Ado;

public class AdoTreatmentRepository : AdoRepository<Treatment>, ITreatmentRepository
{
    public AdoTreatmentRepository(ISqlConnectionFactory sqlConnectionFactory)
        : base(sqlConnectionFactory)
    {
    }
    #region CRUD Methods

    public async Task<List<Treatment>> GetAllAsync()
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync();

        var cmd = CreateCommand(@"
            SELECT Id, Type, NumberTreatment, Name, StartDate, Cost, PatientId,
                   DepartmentId, DateDischarge, ClinicNumber, DoctorName
            FROM Treatments", conn);

        var treatments = await QueryAsync(cmd, ReadTreatment);

        foreach (var t in treatments.OfType<TreatmentInternal>())
            await LoadDoctorsAsync(t, conn);

        return treatments;
    }

    public async Task<Treatment?> GetByIdAsync(Guid id)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync();

        var cmd = CreateCommand(@"
            SELECT Id, Type, NumberTreatment, Name, StartDate, Cost, PatientId,
                   DepartmentId, DateDischarge, ClinicNumber, DoctorName
            FROM Treatments WHERE Id = @Id", conn);
        AddGuidParam(cmd, "@Id", id);

        var treatment = await QuerySingleAsync(cmd, ReadTreatment);
        if (treatment is TreatmentInternal ti)
            await LoadDoctorsAsync(ti, conn);

        return treatment;
    }

    public async Task AddAsync(Treatment treatment)
    {
        await ExecuteInTransactionAsync(async (conn, tx) =>
        {
            await InsertTreatmentAsync(treatment, conn, tx);

            if (treatment is TreatmentInternal ti)
                foreach (var doctor in ti.Doctors)
                    await InsertDoctorTreatmentAsync(doctor, conn, tx);
        });
    }

    public async Task UpdateAsync(Treatment treatment)
    {
        await ExecuteInTransactionAsync(async (conn, tx) =>
        {
            var cmd = CreateCommand(@"
                UPDATE Treatments SET
                    Name          = @Name,
                    StartDate     = @StartDate,
                    Cost          = @Cost,
                    DateDischarge = @DateDischarge,
                    ClinicNumber  = @ClinicNumber,
                    DoctorName    = @DoctorName
                WHERE Id = @Id", conn, tx);

            AddGuidParam(cmd, "@Id", treatment.Id);
            AddParam(cmd, "@Name", treatment.Name);
            AddParam(cmd, "@StartDate", treatment.StartDate);
            AddParam(cmd, "@Cost", treatment.Cost);

            if (treatment is TreatmentInternal ti)
            {
                AddNullableDateParam(cmd, "@DateDischarge", ti.DateDischarge);
                AddParam(cmd, "@ClinicNumber", DBNull.Value);
                AddParam(cmd, "@DoctorName", DBNull.Value);
                await cmd.ExecuteNonQueryAsync();

                // حدّث الدكاترة
                var deleteCmd = CreateCommand(
                    "DELETE FROM DoctorTreatments WHERE TreatmentId = @TreatmentId", conn, tx);
                AddGuidParam(deleteCmd, "@TreatmentId", treatment.Id);
                await deleteCmd.ExecuteNonQueryAsync();

                foreach (var doctor in ti.Doctors)
                    await InsertDoctorTreatmentAsync(doctor, conn, tx);
            }
            else if (treatment is TreatmentExternal te)
            {
                AddNullableDateParam(cmd, "@DateDischarge", null);
                AddParam(cmd, "@ClinicNumber", te.ClinicNumber);
                AddParam(cmd, "@DoctorName", te.DoctorName);
                await cmd.ExecuteNonQueryAsync();
            }
        });
    }

    public async Task DeleteAsync(Treatment treatment)
    {
        await ExecuteInTransactionAsync(async (conn, tx) =>
        {
            var cmd = CreateCommand(
                "DELETE FROM Treatments WHERE Id = @Id", conn, tx);
            AddGuidParam(cmd, "@Id", treatment.Id);
            await cmd.ExecuteNonQueryAsync();
        });
    }
    #endregion

    #region ITreatmentRepository
    public async Task<List<Treatment>> GetByPatientIdAsync(Guid patientId)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync();

        var cmd = CreateCommand(@"
            SELECT Id, Type, NumberTreatment, Name, StartDate, Cost, PatientId,
                   DepartmentId, DateDischarge, ClinicNumber, DoctorName
            FROM Treatments WHERE PatientId = @PatientId", conn);
        AddGuidParam(cmd, "@PatientId", patientId);

        var treatments = await QueryAsync(cmd, ReadTreatment);

        foreach (var t in treatments.OfType<TreatmentInternal>())
            await LoadDoctorsAsync(t, conn);

        return treatments;
    }

    public async Task<List<Treatment>> GetByIdsAsync(List<Guid> ids)
    {
        if (!ids.Any()) return new List<Treatment>();

        await using var conn = await _connectionFactory.CreateOpenConnectionAsync();

        // بناء IN clause
        var paramNames = ids.Select((_, i) => $"@Id{i}").ToList();
        var cmd = CreateCommand($@"
            SELECT Id, Type, NumberTreatment, Name, StartDate, Cost, PatientId,
                   DepartmentId, DateDischarge, ClinicNumber, DoctorName
            FROM Treatments
            WHERE Id IN ({string.Join(", ", paramNames)})", conn);

        for (int i = 0; i < ids.Count; i++)
            AddGuidParam(cmd, $"@Id{i}", ids[i]);

        var treatments = await QueryAsync(cmd, ReadTreatment);

        foreach (var t in treatments.OfType<TreatmentInternal>())
            await LoadDoctorsAsync(t, conn);

        return treatments;
    }

    public async Task<Treatment?> GetByNumberAsync(string number)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync();

        var cmd = CreateCommand(@"
            SELECT Id, Type, NumberTreatment, Name, StartDate, Cost, PatientId,
                   DepartmentId, DateDischarge, ClinicNumber, DoctorName
            FROM Treatments WHERE NumberTreatment = @Number", conn);
        AddParam(cmd, "@Number", number);

        var treatment = await QuerySingleAsync(cmd, ReadTreatment);
        if (treatment is TreatmentInternal ti)
            await LoadDoctorsAsync(ti, conn);

        return treatment;
    }

    public async Task<List<Treatment>> GetByDoctorAsync(Guid doctorId)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync();

        var cmd = CreateCommand(@"
            SELECT t.Id, t.Type, t.NumberTreatment, t.Name, t.StartDate, t.Cost, t.PatientId,
                   t.DepartmentId, t.DateDischarge, t.ClinicNumber, t.DoctorName
            FROM Treatments t
            INNER JOIN DoctorTreatments dt ON t.Id = dt.TreatmentId
            WHERE dt.DoctorId = @DoctorId", conn);
        AddGuidParam(cmd, "@DoctorId", doctorId);

        var treatments = await QueryAsync(cmd, ReadTreatment);

        foreach (var t in treatments.OfType<TreatmentInternal>())
            await LoadDoctorsAsync(t, conn);

        return treatments;
    }

    public async Task<List<Treatment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync();

        var cmd = CreateCommand(@"
            SELECT Id, Type, NumberTreatment, Name, StartDate, Cost, PatientId,
                   DepartmentId, DateDischarge, ClinicNumber, DoctorName
            FROM Treatments
            WHERE StartDate >= @StartDate AND StartDate <= @EndDate", conn);
        AddParam(cmd, "@StartDate", startDate);
        AddParam(cmd, "@EndDate", endDate);

        var treatments = await QueryAsync(cmd, ReadTreatment);

        foreach (var t in treatments.OfType<TreatmentInternal>())
            await LoadDoctorsAsync(t, conn);

        return treatments;
    }

    public async Task<List<Treatment>> GetByDoctorAndPeriodAsync(
        Guid doctorId, DateTime startDate, DateTime endDate)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync();

        var cmd = CreateCommand(@"
            SELECT t.Id, t.Type, t.NumberTreatment, t.Name, t.StartDate, t.Cost, t.PatientId,
                   t.DepartmentId, t.DateDischarge, t.ClinicNumber, t.DoctorName
            FROM Treatments t
            INNER JOIN DoctorTreatments dt ON t.Id = dt.TreatmentId
            WHERE dt.DoctorId  = @DoctorId
              AND t.StartDate >= @StartDate
              AND t.StartDate <= @EndDate", conn);

        AddGuidParam(cmd, "@DoctorId", doctorId);
        AddParam(cmd, "@StartDate", startDate);
        AddParam(cmd, "@EndDate", endDate);

        var treatments = await QueryAsync(cmd, ReadTreatment);

        foreach (var t in treatments.OfType<TreatmentInternal>())
            await LoadDoctorsAsync(t, conn);

        return treatments;
    }
    #endregion

    #region Private
    private static Treatment ReadTreatment(SqlDataReader r)
    {
        var type = GetString(r, "Type").ToLower();

        if (type == "internal")
            return new TreatmentInternal(
                id: GetGuid(r, "Id"),
                numberTreatment: GetString(r, "NumberTreatment"),
                name: GetString(r, "Name"),
                datestart: GetDateTime(r, "StartDate"),
                cost: GetDecimal(r, "Cost"),
                dateDicharge: GetNullableDateTime(r, "DateDischarge"),
                departmentId: GetGuid(r, "DepartmentId"),
                patientId: GetGuid(r, "PatientId"));

        return new TreatmentExternal(
            id: GetGuid(r, "Id"),
            numberTreatment: GetString(r, "NumberTreatment"),
            name: GetString(r, "Name"),
            startDate: GetDateTime(r, "StartDate"),
            cost: GetDecimal(r, "Cost"),
            clinicNumber: GetNullableString(r, "ClinicNumber") ?? string.Empty,
            doctorName: GetNullableString(r, "DoctorName") ?? string.Empty,
            patientId: GetGuid(r, "PatientId"));
    }

    private async Task LoadDoctorsAsync(TreatmentInternal treatment, SqlConnection conn)
    {
        var cmd = CreateCommand(@"
            SELECT DoctorId, TreatmentId, RoleInTreatment
            FROM DoctorTreatments WHERE TreatmentId = @TreatmentId", conn);
        AddGuidParam(cmd, "@TreatmentId", treatment.Id);

        var doctors = await QueryAsync(cmd, r => new DoctorTreatment(
            GetGuid(r, "DoctorId"),
            GetGuid(r, "TreatmentId"),
            GetEnum<TreatmentRole>(r, "RoleInTreatment")));

        foreach (var doctor in doctors)
            treatment.AddDoctor(doctor);
    }

    private async Task InsertTreatmentAsync(Treatment treatment, SqlConnection conn, SqlTransaction tx)
    {
        var cmd = CreateCommand(@"
            INSERT INTO Treatments
                (Id, Type, NumberTreatment, Name, StartDate, Cost, PatientId,
                 DepartmentId, DateDischarge, ClinicNumber, DoctorName)
            VALUES
                (@Id, @Type, @NumberTreatment, @Name, @StartDate, @Cost, @PatientId,
                 @DepartmentId, @DateDischarge, @ClinicNumber, @DoctorName)", conn, tx);

        AddGuidParam(cmd, "@Id", treatment.Id);
        AddParam(cmd, "@Type", treatment is TreatmentInternal ? "internal" : "external");
        AddParam(cmd, "@NumberTreatment", treatment.NumberTreatment);
        AddParam(cmd, "@Name", treatment.Name);
        AddParam(cmd, "@StartDate", treatment.StartDate);
        AddParam(cmd, "@Cost", treatment.Cost);
        AddGuidParam(cmd, "@PatientId", treatment.PatientId);

        if (treatment is TreatmentInternal ti)
        {
            AddNullableGuidParam(cmd, "@DepartmentId", ti.DepartmentId);
            AddNullableDateParam(cmd, "@DateDischarge", ti.DateDischarge);
            AddParam(cmd, "@ClinicNumber", DBNull.Value);
            AddParam(cmd, "@DoctorName", DBNull.Value);
        }
        else if (treatment is TreatmentExternal te)
        {
            AddNullableGuidParam(cmd, "@DepartmentId", null);
            AddNullableDateParam(cmd, "@DateDischarge", null);
            AddParam(cmd, "@ClinicNumber", te.ClinicNumber);
            AddParam(cmd, "@DoctorName", te.DoctorName);
        }

        await cmd.ExecuteNonQueryAsync();
    }

    private async Task InsertDoctorTreatmentAsync(
        DoctorTreatment dt, SqlConnection conn, SqlTransaction tx)
    {
        var cmd = CreateCommand(@"
            IF NOT EXISTS (
                SELECT 1 FROM DoctorTreatments
                WHERE DoctorId = @DoctorId AND TreatmentId = @TreatmentId)
            INSERT INTO DoctorTreatments (DoctorId, TreatmentId, RoleInTreatment)
            VALUES (@DoctorId, @TreatmentId, @RoleInTreatment)", conn, tx);

        AddGuidParam(cmd, "@DoctorId", dt.DoctorId);
        AddGuidParam(cmd, "@TreatmentId", dt.TreatmentId);
        AddParam(cmd, "@RoleInTreatment", dt.RoleInTreatment.ToString());
        await cmd.ExecuteNonQueryAsync();
    }
    #endregion
}