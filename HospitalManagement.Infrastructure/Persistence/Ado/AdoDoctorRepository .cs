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

public class AdoDoctorRepository : AdoBase, IDoctorRepository
{
    public AdoDoctorRepository(string connectionString)
        : base(connectionString) { }

    #region IRepository<Doctor>
    public async Task<List<Doctor>> GetAllAsync()
    {
        var doctors = new List<Doctor>();

        using var connection = CreateConnection();
        await connection.OpenAsync();

        var command = new SqlCommand("SELECT * FROM Doctors", connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
            doctors.Add(MapToDomain(reader));

        // نحمل الـ Roles والـ Departments لكل دكتور
        foreach (var doctor in doctors)
        {
            await LoadRolesAsync(doctor, connection);
            await LoadDepartmentsAsync(doctor, connection);
            await LoadTreatmentsAsync(doctor, connection);
        }

        return doctors;
    }

    public async Task<Doctor?> GetByIdAsync(Guid id)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();

        var command = new SqlCommand(
            "SELECT * FROM Doctors WHERE Id = @Id", connection);
        command.Parameters.AddWithValue("@Id", id);

        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        var doctor = MapToDomain(reader);
        reader.Close();

        await LoadRolesAsync(doctor, connection);
        await LoadDepartmentsAsync(doctor, connection);
        await LoadTreatmentsAsync(doctor, connection);

        return doctor;
    }

    public async Task AddAsync(Doctor doctor)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            // 1. أضف الدكتور
            var cmd = new SqlCommand(@"
                INSERT INTO Doctors 
                (Id, Name, DoctorNumber, Specialization, DateOfBirth, Address, PhoneNumber, Email)
                VALUES 
                (@Id, @Name, @DoctorNumber, @Specialization, @DateOfBirth, @Address, @PhoneNumber, @Email)",
                connection, transaction);

            MapToParameters(cmd, doctor);
            await cmd.ExecuteNonQueryAsync();

            // 2. أضف الـ Roles
            await SaveRolesAsync(doctor, connection, transaction);

            // 3. أضف الـ Departments
            await SaveDepartmentsAsync(doctor, connection, transaction);

            // 4. أضف الـ Treatments
            await SaveTreatmentsAsync(doctor, connection, transaction);

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task UpdateAsync(Doctor doctor)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            // 1. عدّل الدكتور
            var cmd = new SqlCommand(@"
                UPDATE Doctors SET
                    Name = @Name,
                    Specialization = @Specialization,
                    DateOfBirth = @DateOfBirth,
                    Address = @Address,
                    PhoneNumber = @PhoneNumber,
                    Email = @Email
                WHERE Id = @Id",
                connection, transaction);

            MapToParameters(cmd, doctor);
            await cmd.ExecuteNonQueryAsync();

            // 2. امسح القديم وأضف الجديد
            await DeleteRelatedAsync(doctor.Id, connection, transaction);
            await SaveRolesAsync(doctor, connection, transaction);
            await SaveDepartmentsAsync(doctor, connection, transaction);
            await SaveTreatmentsAsync(doctor, connection, transaction);

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task DeleteAsync(Doctor doctor)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            await DeleteRelatedAsync(doctor.Id, connection, transaction);

            var cmd = new SqlCommand(
                "DELETE FROM Doctors WHERE Id = @Id",
                connection, transaction);
            cmd.Parameters.AddWithValue("@Id", doctor.Id);
            await cmd.ExecuteNonQueryAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    #endregion

    #region IDoctorRepository
    public async Task<Doctor?> GetByNumberAsync(string doctorNumber)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();

        var cmd = new SqlCommand(
            "SELECT * FROM Doctors WHERE DoctorNumber = @Number", connection);
        cmd.Parameters.AddWithValue("@Number", doctorNumber);

        using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;

        var doctor = MapToDomain(reader);
        reader.Close();

        await LoadRolesAsync(doctor, connection);
        await LoadDepartmentsAsync(doctor, connection);
        await LoadTreatmentsAsync(doctor, connection);

        return doctor;
    }

    public async Task<IReadOnlyList<Doctor>> GetBySpecializationAsync(Specialization specialization)
    {
        var doctors = new List<Doctor>();

        using var connection = CreateConnection();
        await connection.OpenAsync();

        var cmd = new SqlCommand(
            "SELECT * FROM Doctors WHERE Specialization = @Spec", connection);
        cmd.Parameters.AddWithValue("@Spec", specialization.ToString());

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            doctors.Add(MapToDomain(reader));

        reader.Close();

        foreach (var doctor in doctors)
        {
            await LoadRolesAsync(doctor, connection);
            await LoadDepartmentsAsync(doctor, connection);
            await LoadTreatmentsAsync(doctor, connection);
        }

        return doctors;
    }

    public async Task<IReadOnlyList<Doctor>> GetByDepartmentAsync(Guid departmentId)
    {
        var doctors = new List<Doctor>();

        using var connection = CreateConnection();
        await connection.OpenAsync();

        var cmd = new SqlCommand(@"
            SELECT d.* FROM Doctors d
            INNER JOIN DoctorDepartments dd ON d.Id = dd.DoctorId
            WHERE dd.DepartmentId = @DeptId", connection);
        cmd.Parameters.AddWithValue("@DeptId", departmentId);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            doctors.Add(MapToDomain(reader));

        reader.Close();

        foreach (var doctor in doctors)
        {
            await LoadRolesAsync(doctor, connection);
            await LoadDepartmentsAsync(doctor, connection);
            await LoadTreatmentsAsync(doctor, connection);
        }

        return doctors;
    }

    public async Task<bool> ExistAsync(string name, DateOnly dob)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();

        var cmd = new SqlCommand(
            "SELECT COUNT(1) FROM Doctors WHERE Name = @Name AND DateOfBirth = @Dob",
            connection);
        cmd.Parameters.AddWithValue("@Name", name);
        cmd.Parameters.AddWithValue("@Dob", dob.ToDateTime(TimeOnly.MinValue));

        var count = (int)await cmd.ExecuteScalarAsync()!;
        return count > 0;
    }
    #endregion

    #region Private Helpers — Mapping
    private static Doctor MapToDomain(SqlDataReader reader)
    {
        return new Doctor(
            id: reader.GetGuid(reader.GetOrdinal("Id")),
            name: reader.GetString(reader.GetOrdinal("Name")),
            doctorNumber: reader.GetString(reader.GetOrdinal("DoctorNumber")),
            specialization: Enum.Parse<Specialization>(
                reader.GetString(reader.GetOrdinal("Specialization"))),
            dob: DateOnly.FromDateTime(
                reader.GetDateTime(reader.GetOrdinal("DateOfBirth"))),
            address: reader.IsDBNull(reader.GetOrdinal("Address"))
                ? null : reader.GetString(reader.GetOrdinal("Address")),
            phoneNumber: reader.IsDBNull(reader.GetOrdinal("PhoneNumber"))
                ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
            email: reader.IsDBNull(reader.GetOrdinal("Email"))
                ? null : reader.GetString(reader.GetOrdinal("Email"))
        );
    }

    private static void MapToParameters(SqlCommand cmd, Doctor doctor)
    {
        cmd.Parameters.AddWithValue("@Id", doctor.Id);
        cmd.Parameters.AddWithValue("@Name", doctor.Name);
        cmd.Parameters.AddWithValue("@DoctorNumber", doctor.DoctorNumber);
        cmd.Parameters.AddWithValue("@Specialization", doctor.Specialization.ToString());
        cmd.Parameters.AddWithValue("@DateOfBirth",
            doctor.DateOfBirth.ToDateTime(TimeOnly.MinValue));
        cmd.Parameters.AddWithValue("@Address", (object?)doctor.Address ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PhoneNumber", (object?)doctor.PhoneNumber ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Email", (object?)doctor.Email ?? DBNull.Value);
    }
    #endregion

    #region Private Helpers — Load
    private async Task LoadRolesAsync(Doctor doctor, SqlConnection connection)
    {
        var cmd = new SqlCommand(
            "SELECT * FROM DoctorRoles WHERE DoctorId = @DoctorId ORDER BY StartDate",
            connection);
        cmd.Parameters.AddWithValue("@DoctorId", doctor.Id);

        using var reader = await cmd.ExecuteReaderAsync();
        var roles = new List<DoctorRole>();

        while (await reader.ReadAsync())
        {
            var roleType = reader.GetString(reader.GetOrdinal("RoleType"));
            var id = reader.GetGuid(reader.GetOrdinal("Id"));
            var startDate = reader.GetDateTime(reader.GetOrdinal("StartDate"));
            var endDate = reader.IsDBNull(reader.GetOrdinal("EndDate"))
                ? (DateTime?)null
                : reader.GetDateTime(reader.GetOrdinal("EndDate"));
            var isActive = reader.GetBoolean(reader.GetOrdinal("IsActive"));

            DoctorRole role = roleType.ToLower() switch
            {
                "permanent" => new PermanentRole(
                    id, startDate, endDate, isActive,
                    reader.GetDecimal(reader.GetOrdinal("BaseSalary"))),

                "contracted" => new ContractedRole(
                    id, startDate, endDate, isActive,
                    reader.GetDecimal(reader.GetOrdinal("Percent"))),

                "trainee" => new TraineeRole(id, startDate, endDate, isActive),

                _ => throw new InvalidOperationException($"Unknown role: {roleType}")
            };

            roles.Add(role);
        }

        doctor.LoadRoles(roles);
    }

    private async Task LoadDepartmentsAsync(Doctor doctor, SqlConnection connection)
    {
        var cmd = new SqlCommand(
            "SELECT DepartmentId FROM DoctorDepartments WHERE DoctorId = @DoctorId",
            connection);
        cmd.Parameters.AddWithValue("@DoctorId", doctor.Id);

        using var reader = await cmd.ExecuteReaderAsync();
        var ids = new List<Guid>();

        while (await reader.ReadAsync())
            ids.Add(reader.GetGuid(0));

        doctor.LoadDepartmentIds(ids);
    }

    private async Task LoadTreatmentsAsync(Doctor doctor, SqlConnection connection)
    {
        var cmd = new SqlCommand(
            "SELECT * FROM DoctorTreatments WHERE DoctorId = @DoctorId",
            connection);
        cmd.Parameters.AddWithValue("@DoctorId", doctor.Id);

        using var reader = await cmd.ExecuteReaderAsync();
        var treatments = new List<DoctorTreatment>();

        while (await reader.ReadAsync())
        {
            treatments.Add(new DoctorTreatment(
                doctorId: reader.GetGuid(reader.GetOrdinal("DoctorId")),
                treatmentId: reader.GetGuid(reader.GetOrdinal("TreatmentId")),
                role: Enum.Parse<TreatmentRole>(
                    reader.GetString(reader.GetOrdinal("RoleInTreatment")))
            ));
        }

        doctor.LoadTreatments(treatments);
    }
    #endregion

    #region Private Helpers — Save & Delete
    private async Task SaveRolesAsync(Doctor doctor, SqlConnection conn, SqlTransaction tx)
    {
        foreach (var role in doctor.Roles)
        {
            var cmd = new SqlCommand(@"
                INSERT INTO DoctorRoles 
                (Id, DoctorId, RoleType, StartDate, EndDate, IsActive, BaseSalary, Percent)
                VALUES 
                (@Id, @DoctorId, @RoleType, @StartDate, @EndDate, @IsActive, @BaseSalary, @Percent)",
                conn, tx);

            cmd.Parameters.AddWithValue("@Id", role.Id);
            cmd.Parameters.AddWithValue("@DoctorId", doctor.Id);
            cmd.Parameters.AddWithValue("@RoleType", role.RoleName);
            cmd.Parameters.AddWithValue("@StartDate", role.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", (object?)role.EndDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsActive", role.IsActive);
            cmd.Parameters.AddWithValue("@BaseSalary",
                role is PermanentRole pr ? pr.BaseSalary : DBNull.Value);
            cmd.Parameters.AddWithValue("@Percent",
                role is ContractedRole cr ? cr.Percent : DBNull.Value);

            await cmd.ExecuteNonQueryAsync();
        }
    }

    private async Task SaveDepartmentsAsync(Doctor doctor, SqlConnection conn, SqlTransaction tx)
    {
        foreach (var deptId in doctor.DepartmentsIds)
        {
            var cmd = new SqlCommand(
                "INSERT INTO DoctorDepartments (DoctorId, DepartmentId) VALUES (@DId, @DeptId)",
                conn, tx);
            cmd.Parameters.AddWithValue("@DId", doctor.Id);
            cmd.Parameters.AddWithValue("@DeptId", deptId);
            await cmd.ExecuteNonQueryAsync();
        }
    }

    private async Task SaveTreatmentsAsync(Doctor doctor, SqlConnection conn, SqlTransaction tx)
    {
        foreach (var t in doctor.Treatments)
        {
            var cmd = new SqlCommand(@"
                INSERT INTO DoctorTreatments (DoctorId, TreatmentId, RoleInTreatment)
                VALUES (@DId, @TId, @Role)",
                conn, tx);
            cmd.Parameters.AddWithValue("@DId", t.DoctorId);
            cmd.Parameters.AddWithValue("@TId", t.TreatmentId);
            cmd.Parameters.AddWithValue("@Role", t.RoleInTreatment.ToString());
            await cmd.ExecuteNonQueryAsync();
        }
    }

    private async Task DeleteRelatedAsync(Guid doctorId, SqlConnection conn, SqlTransaction tx)
    {
        foreach (var table in new[] { "DoctorRoles", "DoctorDepartments", "DoctorTreatments" })
        {
            var cmd = new SqlCommand(
                $"DELETE FROM {table} WHERE DoctorId = @Id", conn, tx);
            cmd.Parameters.AddWithValue("@Id", doctorId);
            await cmd.ExecuteNonQueryAsync();
        }
    }
    #endregion
}
