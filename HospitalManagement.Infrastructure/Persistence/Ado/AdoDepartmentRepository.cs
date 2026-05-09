using HospitalManagement.Application.Interfaces.Persistence;
using HospitalManagement.Application.Interfaces.Repositories;
using HospitalManagement.Domain.Entities.Departments;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace HospitalManagement.Infrastructure.Persistence.Ado;

public class AdoDepartmentRepository : AdoRepository<Department> , IDepartmentRepository
{
    public AdoDepartmentRepository(ISqlConnectionFactory connectionFactory)
        : base(connectionFactory)
    {
    }
    #region CRUD METHODS
    public async Task<List<Department>> GetAllAsync()
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync();

        var cmd = CreateCommand("SELECT Id, Name FROM Departments", conn);
        var departments = await QueryAsync(cmd, ReadDepartment);

        foreach (var dept in departments)
            await LoadRelationsAsync(dept, conn);

        return departments;
    }

    public async Task<Department?> GetByIdAsync(Guid id)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync();

        var cmd = CreateCommand(
            "SELECT Id, Name FROM Departments WHERE Id = @Id", conn);
        AddGuidParam(cmd, "@Id", id);

        var dept = await QuerySingleAsync(cmd, ReadDepartment);
        if (dept is null) return null;

        await LoadRelationsAsync(dept, conn);
        return dept;
    }

    public async Task AddAsync(Department department)
    {
        await ExecuteInTransactionAsync(async (conn, tx) =>
        {
            var cmd = CreateCommand(@"
                INSERT INTO Departments (Id, Name)
                VALUES (@Id, @Name)", conn, tx);

            AddGuidParam(cmd, "@Id", department.Id);
            AddParam(cmd, "@Name", department.Name);
            await cmd.ExecuteNonQueryAsync();

            foreach (var doctorId in department.DoctorIds)
                await InsertDepartmentDoctorAsync(department.Id, doctorId, conn, tx);

            foreach (var patientId in department.PatientIds)
                await InsertDepartmentPatientAsync(department.Id, patientId, conn, tx);
        });
    }

    public async Task UpdateAsync(Department department)
    {
        await ExecuteInTransactionAsync(async (conn, tx) =>
        {
            // 1. update department.name
            var cmd = CreateCommand(@"
                UPDATE Departments SET Name = @Name
                WHERE Id = @Id", conn, tx);
            AddGuidParam(cmd, "@Id", department.Id);
            AddParam(cmd, "@Name", department.Name);
            await cmd.ExecuteNonQueryAsync();

            // 2. update department.doctors
            var deleteDoctors = CreateCommand(
                "DELETE FROM DepartmentDoctors WHERE DepartmentId = @DepartmentId", conn, tx);
            AddGuidParam(deleteDoctors, "@DepartmentId", department.Id);
            await deleteDoctors.ExecuteNonQueryAsync();

            foreach (var doctorId in department.DoctorIds)
                await InsertDepartmentDoctorAsync(department.Id, doctorId, conn, tx);

            // 3. update department.patients
            var deletePatients = CreateCommand(
                "DELETE FROM DepartmentPatients WHERE DepartmentId = @DepartmentId", conn, tx);
            AddGuidParam(deletePatients, "@DepartmentId", department.Id);
            await deletePatients.ExecuteNonQueryAsync();

            foreach (var patientId in department.PatientIds)
                await InsertDepartmentPatientAsync(department.Id, patientId, conn, tx);
        });
    }

    public async Task DeleteAsync(Department department)
    {
        await ExecuteInTransactionAsync(async (conn, tx) =>
        {
            var cmd = CreateCommand(
                "DELETE FROM Departments WHERE Id = @Id", conn, tx);
            AddGuidParam(cmd, "@Id", department.Id);
            await cmd.ExecuteNonQueryAsync();
        });
    }
    #endregion
    #region IDepartmentRepository Methods

    public async Task<bool> ExistsAsync(string name)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync();

        var cmd = CreateCommand(
            "SELECT COUNT(1) FROM Departments WHERE Name = @Name", conn);
        AddParam(cmd, "@Name", name);

        return (int)(await cmd.ExecuteScalarAsync())! > 0;
    }

    public async Task<Department?> GetByNameAsync(string name)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync();

        var cmd = CreateCommand(
            "SELECT Id, Name FROM Departments WHERE Name = @Name", conn);
        AddParam(cmd, "@Name", name);

        var dept = await QuerySingleAsync(cmd, ReadDepartment);
        if (dept is null)
            return null;

        await LoadRelationsAsync(dept, conn);
        return dept;
    }

    #endregion

    #region Private 
    private static Department ReadDepartment(SqlDataReader r) => new Department(
       id: GetGuid(r, "Id"),
       name: GetString(r, "Name"),
       patientIds: Enumerable.Empty<Guid>(),
       doctorIds: Enumerable.Empty<Guid>()
   );

    private async Task LoadRelationsAsync(Department dept, SqlConnection conn)
    {
        var doctorsCmd = CreateCommand(
            "SELECT DoctorId FROM DepartmentDoctors WHERE DepartmentId = @DepartmentId", conn);
        AddGuidParam(doctorsCmd, "@DepartmentId", dept.Id);

        var doctorIds = await QueryAsync(doctorsCmd, r => GetGuid(r, "DoctorId"));
        dept.LoadDoctorIds(doctorIds);

        var patientsCmd = CreateCommand(
            "SELECT PatientId FROM DepartmentPatients WHERE DepartmentId = @DepartmentId", conn);
        AddGuidParam(patientsCmd, "@DepartmentId", dept.Id);

        var patientIds = await QueryAsync(patientsCmd, r => GetGuid(r, "PatientId"));
        dept.LoadPatientIds(patientIds);
    }

    private async Task InsertDepartmentDoctorAsync(
        Guid departmentId, Guid doctorId, SqlConnection conn, SqlTransaction tx)
    {
        var cmd = CreateCommand(@"
            IF NOT EXISTS (
                SELECT 1 FROM DepartmentDoctors 
                WHERE DepartmentId = @DepartmentId AND DoctorId = @DoctorId)
            INSERT INTO DepartmentDoctors (DepartmentId, DoctorId)
            VALUES (@DepartmentId, @DoctorId)", conn, tx);

        AddGuidParam(cmd, "@DepartmentId", departmentId);
        AddGuidParam(cmd, "@DoctorId", doctorId);
        await cmd.ExecuteNonQueryAsync();
    }

    private async Task InsertDepartmentPatientAsync(
        Guid departmentId, Guid patientId, SqlConnection conn, SqlTransaction tx)
    {
        var cmd = CreateCommand(@"
            IF NOT EXISTS (
                SELECT 1 FROM DepartmentPatients 
                WHERE DepartmentId = @DepartmentId AND PatientId = @PatientId)
            INSERT INTO DepartmentPatients (DepartmentId, PatientId)
            VALUES (@DepartmentId, @PatientId)", conn, tx);

        AddGuidParam(cmd, "@DepartmentId", departmentId);
        AddGuidParam(cmd, "@PatientId", patientId);
        await cmd.ExecuteNonQueryAsync();
    }
    #endregion
}
