using HospitalManagement.Application.Interfaces.Persistence;
using HospitalManagement.Application.Interfaces.Repositories;
using HospitalManagement.Domain.Entities.Doctors;
using HospitalManagement.Domain.Entities.Enums;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Persistence.Ado;

public class AdoDoctorRepository : AdoRepository<Doctor>, IDoctorRepository
{
    public AdoDoctorRepository(ISqlConnectionFactory connectionFactory)
        : base(connectionFactory) { }

    #region CRUD METHODS
    public async Task<List<Doctor>> GetAllAsync()
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync();

        var cmd = CreateCommand(@"
            SELECT Id, Name, DoctorNumber, Specialization,
                   DateOfBirth, Address, PhoneNumber, Email
            FROM Doctors", conn);

        var doctors = await QueryAsync(cmd, ReadDoctor);

        foreach (var doctor in doctors) 
            await LoadRelationsAsync(doctor , conn); 

        return doctors;
    }

    public async Task<Doctor?> GetByIdAsync(Guid id)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync();

        var cmd = CreateCommand(@"
            SELECT Id, Name, DoctorNumber, Specialization,
                   DateOfBirth, Address, PhoneNumber, Email
            FROM Doctors WHERE Id = @Id", conn);
        AddGuidParam(cmd, "@Id", id);

        var doctor = await QuerySingleAsync(cmd, ReadDoctor);
        if(doctor is null)
            return null;

        await LoadRelationsAsync(doctor, conn);
        return doctor;
    }

    public async Task AddAsync(Doctor doctor)
    { 
        await ExecuteInTransactionAsync(async (conn , tx) =>
        {
            await InsertDoctorAsync(doctor, conn, tx);

            foreach (var role in doctor.Roles)
                await InsertRoleAsync(role, doctor.Id, conn, tx);

            foreach (var deptId in doctor.DepartmentsIds)
                await InsertDepartmentDoctorAsync(deptId, doctor.Id, conn, tx);

            foreach (var t in doctor.Treatments)
                await InsertDoctorTreatmentAsync(t, conn, tx);
        });
    }

    public async Task UpdateAsync(Doctor doctor)
    {
        await ExecuteInTransactionAsync(async (conn, tx) =>
        {
            // 1. update doctor info
            var cmd = CreateCommand(@"
                UPDATE Doctors SET
                    Name           = @Name,
                    Specialization = @Specialization,
                    DateOfBirth    = @DateOfBirth,
                    Address        = @Address,
                    PhoneNumber    = @PhoneNumber,
                    Email          = @Email
                WHERE Id = @Id", conn, tx);
            AddDoctorParams(cmd, doctor);
            await cmd.ExecuteNonQueryAsync();

            // 2. delete and insert roles
            /// 2.1 delete salary history
            var deleteSalary = CreateCommand(@"
                DELETE FROM SalaryHistory 
                WHERE RoleId IN (SELECT Id FROM DoctorRoles WHERE DoctorId = @DoctorId)",
                    conn, tx);
            AddGuidParam(deleteSalary, "@DoctorId", doctor.Id);
            await deleteSalary.ExecuteNonQueryAsync();
            
            /// 2.2 delete roles
            var deleteRoles = CreateCommand(
                "DELETE FROM DoctorRoles WHERE DoctorId = @DoctorId", conn, tx);
            AddGuidParam(deleteRoles, "@DoctorId", doctor.Id);
            await deleteRoles.ExecuteNonQueryAsync();

            /// 2.3 insert new roles
            foreach (var role in doctor.Roles)
                await InsertRoleAsync(role, doctor.Id, conn, tx);

            // 3. delete and insert departments
            var deleteDepts = CreateCommand(
                "DELETE FROM DepartmentDoctors WHERE DoctorId = @DoctorId", conn, tx);
            AddGuidParam(deleteDepts, "@DoctorId", doctor.Id);
            await deleteDepts.ExecuteNonQueryAsync();

            foreach (var deptId in doctor.DepartmentsIds)
                await InsertDepartmentDoctorAsync(deptId, doctor.Id, conn, tx);

            // 4. delete and insert treatments
            var deleteTreatments = CreateCommand(
                "DELETE FROM DoctorTreatments WHERE DoctorId = @DoctorId", conn, tx);
            AddGuidParam(deleteTreatments, "@DoctorId", doctor.Id);
            await deleteTreatments.ExecuteNonQueryAsync();

            foreach (var t in doctor.Treatments)
                await InsertDoctorTreatmentAsync(t, conn, tx);
        });
    }

    public async Task DeleteAsync(Doctor doctor)
    {
        await ExecuteInTransactionAsync(async (conn , tx) =>
        {
            var cmd = CreateCommand(
                "DELETE FROM Doctors WHERE Id = @Id", conn, tx);
            AddGuidParam(cmd, "@Id", doctor.Id);
            await cmd.ExecuteNonQueryAsync();
        });
    }

    #endregion

    #region IDoctorRepository METHODS
    public async Task<bool> ExistAsync(string name , DateOnly dob)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync();
        var cmd = CreateCommand(@"
            SELECT COUNT(1) FROM Doctors
            WHERE Name = @Name AND DateOfBirth = @DateOfBirth", conn);
        AddParam(cmd, "@Name", name);
        AddParam(cmd, "@DateOfBirth", dob.ToDateTime(TimeOnly.MinValue));
        return (int)(await cmd.ExecuteScalarAsync())! > 0;
    }

    public async Task<Doctor?> GetByNumberAsync(string doctorNumber)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync();

        var cmd = CreateCommand(@"
            SELECT Id, Name, DoctorNumber, Specialization,
                   DateOfBirth, Address, PhoneNumber, Email
            FROM Doctors WHERE DoctorNumber = @DoctorNumber", conn);
        AddParam(cmd, "@DoctorNumber", doctorNumber);

        var doctor = await QuerySingleAsync(cmd, ReadDoctor);
        if (doctor is null) 
            return null;

        await LoadRelationsAsync(doctor, conn);
        return doctor;
    }

    public async Task<IReadOnlyList<Doctor>> GetBySpecializationAsync(Specialization specialization)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync();

        var cmd = CreateCommand(@"
            SELECT Id, Name, DoctorNumber, Specialization,
                   DateOfBirth, Address, PhoneNumber, Email
            FROM Doctors WHERE Specialization = @Specialization", conn);
        AddParam(cmd, "@Specialization", specialization.ToString());

        var doctors = await QueryAsync(cmd, ReadDoctor);

        foreach (var doctor in doctors)
            await LoadRelationsAsync(doctor, conn);

        return doctors;
    }

    public async Task<IReadOnlyList<Doctor>> GetByDepartmentAsync(Guid departmentId)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync();

        var cmd = CreateCommand(@"
            SELECT d.Id, d.Name, d.DoctorNumber, d.Specialization,
                   d.DateOfBirth, d.Address, d.PhoneNumber, d.Email
            FROM Doctors d
            INNER JOIN DepartmentDoctors dd ON d.Id = dd.DoctorId
            WHERE dd.DepartmentId = @DepartmentId", conn);
        AddGuidParam(cmd, "@DepartmentId", departmentId);

        var doctors = await QueryAsync(cmd, ReadDoctor);

        foreach (var doctor in doctors)
            await LoadRelationsAsync(doctor, conn);

        return doctors;
    }

    // Background
    public async Task UpdateSalaryHistoryAsync(Doctor doctor)
    {
        await ExecuteInTransactionAsync(async (conn, tx) =>
        {
            foreach (var role in doctor.Roles)
            {
                using var deleteCmd = CreateCommand(
                    "DELETE FROM SalaryHistory WHERE RoleId = @RoleId",
                    conn, tx);
                AddGuidParam(deleteCmd, "@RoleId", role.Id);
                await deleteCmd.ExecuteNonQueryAsync();

                foreach (var record in role.SalaryHistory)
                {
                    var salaryCmd = CreateCommand(@"
                        INSERT INTO SalaryHistory (Id, RoleId, Year, Month, Amount)
                        VALUES (NEWID(), @RoleId, @Year, @Month, @Amount)", conn, tx);

                    AddGuidParam(salaryCmd, "@RoleId", role.Id);
                    AddParam(salaryCmd, "@Year", record.Year);
                    AddParam(salaryCmd, "@Month", record.Month);
                    AddParam(salaryCmd, "@Amount", record.Amount);

                    await salaryCmd.ExecuteNonQueryAsync();
                }
                
            }
        });
    }

    #endregion

    #region PRIVATE METHODS - Read & Load
    private static Doctor ReadDoctor(SqlDataReader r) => new Doctor(
        id: GetGuid(r, "Id"),
        name: GetString(r, "Name"),
        doctorNumber: GetString(r, "DoctorNumber"),
        specialization: GetEnum<Specialization>(r, "Specialization"),
        dob: GetDateOnly(r, "DateOfBirth"),
        address: GetNullableString(r, "Address"),
        phoneNumber: GetNullableString(r, "PhoneNumber"),
        email: GetNullableString(r, "Email")
    );

    private async Task LoadRelationsAsync(Doctor doctor, SqlConnection conn)
    {
        doctor.LoadRoles(await LoadRolesAsync(doctor.Id, conn));
        doctor.LoadDepartmentIds(await LoadDepartmentIdsAsync(doctor.Id, conn));
        doctor.LoadTreatments(await LoadTreatmentsAsync(doctor.Id, conn));
    }

    private async Task<List<DoctorRole>> LoadRolesAsync(Guid doctorId, SqlConnection conn)
    {
        var cmd = CreateCommand(@"
            SELECT Id, RoleType, StartDate, EndDate, IsActive, BaseSalary, SalaryPercent
            FROM DoctorRoles
            WHERE DoctorId = @DoctorId
            ORDER BY StartDate", conn);
        AddGuidParam(cmd, "@DoctorId", doctorId);

        var roles = await QueryAsync(cmd, r =>
        {
            var roleType = GetString(r, "RoleType").ToLower();
            var id = GetGuid(r, "Id");
            var startDate = GetDateTime(r, "StartDate");
            var endDate = GetNullableDateTime(r, "EndDate");
            var isActive = GetBool(r, "IsActive");

            return roleType switch
            {
                "permanent" => (DoctorRole)new PermanentRole(
                                    id, startDate, endDate, isActive,
                                    GetNullableDecimal(r, "BaseSalary") ?? 0),
                "trainee" => new TraineeRole(id, startDate, endDate, isActive),
                "contracted" => new ContractedRole(
                                    id, startDate, endDate, isActive,
                                    GetNullableDecimal(r, "SalaryPercent") ?? 0.5m),
                _ => throw new InvalidOperationException($"Unknown role: {roleType}")
            };
        });

        // حمّل SalaryHistory لكل role
        foreach (var role in roles)
        {
            var salaryCmd = CreateCommand(@"
                SELECT Year, Month, Amount
                FROM SalaryHistory WHERE RoleId = @RoleId", conn);
            AddGuidParam(salaryCmd, "@RoleId", role.Id);

            var records = await QueryAsync(salaryCmd, r =>
                new SalaryRecord(GetInt(r, "Year"), GetInt(r, "Month"), GetDecimal(r, "Amount")));

            foreach (var record in records)
                role.LoadSalaryRecord(record);
        }

        return roles;
    }

    private async Task<List<Guid>> LoadDepartmentIdsAsync(Guid doctorId, SqlConnection conn)
    {
        var cmd = CreateCommand(
            "SELECT DepartmentId FROM DepartmentDoctors WHERE DoctorId = @DoctorId", conn);
        AddGuidParam(cmd, "@DoctorId", doctorId);

        return await QueryAsync(cmd, r => GetGuid(r, "DepartmentId"));
    }

    private async Task<List<DoctorTreatment>> LoadTreatmentsAsync(Guid doctorId, SqlConnection conn)
    {
        var cmd = CreateCommand(@"
            SELECT DoctorId, TreatmentId, RoleInTreatment
            FROM DoctorTreatments WHERE DoctorId = @DoctorId", conn);
        AddGuidParam(cmd, "@DoctorId", doctorId);

        return await QueryAsync(cmd, r => new DoctorTreatment(
            GetGuid(r, "DoctorId"),
            GetGuid(r, "TreatmentId"),
            GetEnum<TreatmentRole>(r, "RoleInTreatment")));
    }
    #endregion

    #region PRIVATE METHODS - Insert
    private async Task InsertDoctorAsync(Doctor doctor, SqlConnection conn, SqlTransaction tx)
    {
        var cmd = CreateCommand(@"
            INSERT INTO Doctors
                (Id, Name, DoctorNumber, Specialization, DateOfBirth, Address, PhoneNumber, Email)
            VALUES
                (@Id, @Name, @DoctorNumber, @Specialization, @DateOfBirth, @Address, @PhoneNumber, @Email)",
            conn, tx);
        AddDoctorParams(cmd, doctor);
        await cmd.ExecuteNonQueryAsync();
    }

    private async Task InsertRoleAsync(DoctorRole role, Guid doctorId, SqlConnection conn, SqlTransaction tx)
    {
        var cmd = CreateCommand(@"
            INSERT INTO DoctorRoles
                (Id, DoctorId, RoleType, StartDate, EndDate, IsActive, BaseSalary, SalaryPercent)
            VALUES
                (@Id, @DoctorId, @RoleType, @StartDate, @EndDate, @IsActive, @BaseSalary, @SalaryPercent)",
            conn, tx);

        AddGuidParam(cmd, "@Id", role.Id);
        AddGuidParam(cmd, "@DoctorId", doctorId);
        AddParam(cmd, "@RoleType", role.RoleName);
        AddParam(cmd, "@StartDate", role.StartDate);
        AddNullableDateParam(cmd, "@EndDate", role.EndDate);
        AddParam(cmd, "@IsActive", role.IsActive);
        AddParam(cmd, "@BaseSalary", role is PermanentRole p ? p.BaseSalary : null);
        AddParam(cmd, "@SalaryPercent", role is ContractedRole c ? c.Percent : null);

        await cmd.ExecuteNonQueryAsync();

        // SalaryHistory
        foreach (var record in role.SalaryHistory)
        {
            var salaryCmd = CreateCommand(@"
                INSERT INTO SalaryHistory (Id, RoleId, Year, Month, Amount)
                VALUES (NEWID(), @RoleId, @Year, @Month, @Amount)", conn, tx);

            AddGuidParam(salaryCmd, "@RoleId", role.Id);
            AddParam(salaryCmd, "@Year", record.Year);
            AddParam(salaryCmd, "@Month", record.Month);
            AddParam(salaryCmd, "@Amount", record.Amount);

            await salaryCmd.ExecuteNonQueryAsync();
        }
    }

    private async Task InsertDepartmentDoctorAsync(Guid doctorId, Guid deptId, SqlConnection conn, SqlTransaction tx)
    {
        var cmd = CreateCommand(@"
            INSERT INTO DepartmentDoctors (DepartmentId, DoctorId)
            VALUES (@DepartmentId, @DoctorId)", conn, tx);

        AddGuidParam(cmd, "@DepartmentId", deptId);
        AddGuidParam(cmd, "@DoctorId", doctorId);
        await cmd.ExecuteNonQueryAsync();
    }

    private async Task InsertDoctorTreatmentAsync(DoctorTreatment t, SqlConnection conn, SqlTransaction tx)
    {
        var cmd = CreateCommand(@"
            INSERT INTO DoctorTreatments (DoctorId, TreatmentId, RoleInTreatment)
            VALUES (@DoctorId, @TreatmentId, @RoleInTreatment)", conn, tx);

        AddGuidParam(cmd, "@DoctorId", t.DoctorId);
        AddGuidParam(cmd, "@TreatmentId", t.TreatmentId);
        AddParam(cmd, "@RoleInTreatment", t.RoleInTreatment.ToString());
        await cmd.ExecuteNonQueryAsync();
    }

    private static void AddDoctorParams(SqlCommand cmd, Doctor doctor)
    {
        AddGuidParam(cmd, "@Id", doctor.Id);
        AddParam(cmd, "@Name", doctor.Name);
        AddParam(cmd, "@DoctorNumber", doctor.DoctorNumber);
        AddParam(cmd, "@Specialization", doctor.Specialization.ToString());
        AddParam(cmd, "@DateOfBirth", doctor.DateOfBirth.ToDateTime(TimeOnly.MinValue));
        AddParam(cmd, "@Address", doctor.Address);
        AddParam(cmd, "@PhoneNumber", doctor.PhoneNumber);
        AddParam(cmd, "@Email", doctor.Email);
    }
    #endregion
}