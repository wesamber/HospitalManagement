using HospitalManagement.Application.Interfaces.Repositories;
using HospitalManagement.Application.Interfaces.Services;
using HospitalManagement.Domain.Contracts;
using HospitalManagement.Infrastructure.Persistence.Json.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Persistence.Json;

public class JsonSnapshotLogRepository<T , TJsonModel> : IRepository<T> where T : IEntity
{
    private readonly Func<T, TJsonModel> _toModel;
    private readonly Func<TJsonModel, T> _toDomain;
    private readonly string _snapshotPath;
    private readonly string _logPath;
    private readonly IFileStorage _fileStorage;
    private readonly ISerializer _serializer;

    protected List<T>? _cache;

    // لحتلى اقفل الملف يلي عم اشتغل عليه
    protected readonly SemaphoreSlim _lock = new(1,1);

    private int _pendingOperations = 0;
    private const int MaxPendingOperations = 20; // بعد 20 عمليات رح اعمل snapshot 

    public JsonSnapshotLogRepository(
        string snapshotPath,
        string logPath,
        IFileStorage fileStorage,
        ISerializer serializer,
        Func<T, TJsonModel> toModel,
        Func<TJsonModel ,T> toDomain)
    {
        _snapshotPath = snapshotPath;
        _logPath = logPath;
        _fileStorage = fileStorage;
        _serializer = serializer;
        _toModel = toModel;
        _toDomain = toDomain;
    }
    #region CRUD Operations
    public async Task<List<T>> GetAllAsync()
    {
        if (_cache != null)
            return _cache;

        await _lock.WaitAsync();
        try
        {
            if (_cache == null)
                _cache = await LoadStateAsync();
            return _cache;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task AddAsync(T entity)
    {
        var state = await GetAllAsync();
        await _lock.WaitAsync();
        try
        {
            if (state.Any(e => e.Id == entity.Id))
                throw new InvalidOperationException($"Entity with the same ID: {entity.Id} already exists.");

            state.Add(entity);

            var logEntry = new LogEntry<T>
            {
                Op = "add",
                Entity = entity
            };

            await MarkChangeAsync(logEntry);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task UpdateAsync(T entity)
    {
        var state = await GetAllAsync();
        await _lock.WaitAsync();
        try
        { 

            var index = state.FindIndex(e => e.Id == entity.Id);
            if(index == -1)
                throw new KeyNotFoundException($"Entity with ID: {entity.Id} not found.");

            state[index] = entity;

            LogEntry<T> logEntry = new LogEntry<T>
            {
                Op = "update",
                Entity = entity
            };
            await MarkChangeAsync(logEntry);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task DeleteAsync(T entity)
    {
        // لانو ال getallasync فيها lock كمان 
        var state = await GetAllAsync();
        await _lock.WaitAsync();
        try
        {
            var existing = state.FirstOrDefault(e => e.Id == entity.Id);
            if(existing is null)
                throw new KeyNotFoundException($"Entity with ID: {entity.Id} not found.");

            state.Remove(existing);

            var logEntry = new LogEntry<T>
            {
                Op = "delete",
                Id = entity.Id
            };
            await MarkChangeAsync(logEntry);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        var state = await GetAllAsync();

        await _lock.WaitAsync(); 
        try
        {
            return state.FirstOrDefault(e => e.Id == id);
        }
        finally
        {
            _lock.Release(); 
        }
    }

    #endregion


    /// <summary>
    /// ملاحظة بكل العملبات ال private  ما بحط قفل القفل فقط بعمليات ال public 
    /// مشان ما يصير في قفل للابد Deadlock
    /// </summary>
    /// <returns></returns>
    #region Private Helpers Snapshot & Log
    private async Task<List<T>> LoadSnapshotAsync()
    {
        if(!File.Exists(_snapshotPath))
            return new List<T>();

        var content = await _fileStorage.ReadAsync(_snapshotPath);

        if(string.IsNullOrWhiteSpace(content))
            return new List<T>();

        var jsonModels = _serializer.Deserialize<List<TJsonModel>>(content) ?? new List<TJsonModel>();
        return jsonModels.Select(_toDomain).ToList();
    }

    private async Task<List<LogEntry<T>>> LoadLogAsync()
    {
        if (!File.Exists(_logPath))
            return new List<LogEntry<T>>();

        var content = await _fileStorage.ReadAsync(_logPath);

        if (string.IsNullOrWhiteSpace(content))
            return new List<LogEntry<T>>();

        var lines = content.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        var logEntries = new List<LogEntry<T>>();

        foreach (var line in lines)
        {
            var entry = _serializer.Deserialize<LogEntry<TJsonModel>>(line);
            if (entry != null)
                // mapper
                logEntries.Add(new LogEntry<T>
                {
                    Op = entry.Op,
                    Id = entry.Id,
                    Entity = entry.Entity != null ? _toDomain(entry.Entity) : default
                });
        }
        
        return logEntries;
    } 

    private void ApplyLogEntry(LogEntry<T> entry , List<T> state)
    {
        switch (entry.Op)
        {
            case "add":
                if (entry.Entity != null && !state.Any(e => e.Id == entry.Entity.Id))
                    state.Add(entry.Entity);
                break;

            case "update":
                if (entry.Entity != null)
                {
                    var index = state.FindIndex(e => e.Id == entry.Entity.Id);
                    if (index >= 0)
                        state[index] = entry.Entity;
                }
                break;

            case "delete":
                if (entry.Id.HasValue)
                {
                    var index = state.FindIndex(e => e.Id == entry.Id.Value);
                    if (index >= 0)
                        state.RemoveAt(index);
                }
                break;
        }
    }

    private async Task<List<T>> LoadStateAsync()
    {
        var snapshot = await LoadSnapshotAsync();
        var logEntries = await LoadLogAsync();

        foreach (var entry in logEntries)
            ApplyLogEntry(entry, snapshot);

        return snapshot;
    }

    private async Task AppendLogAsync(LogEntry<T> entry)
    {
        var logToSave = new LogEntry<TJsonModel>
        {
            Op = entry.Op,
            Id = entry.Id,
            Entity = entry.Entity != null ? _toModel(entry.Entity) : default
        };
        var json = _serializer.Serialize(logToSave, indented: false);
        await _fileStorage.AppendLineAsync(_logPath, json);
    }

    private async Task CreateSnapshotAsync()
    {
        if(_cache == null)
            return;

        var snapshotJson = _serializer.Serialize(_cache.Select(_toModel).ToList(), indented: true);
        await _fileStorage.WriteAsync(_snapshotPath, snapshotJson);

        // بعد ما اعمل snapshot بفرغ اللوق
        await _fileStorage.WriteAsync(_logPath, "");

        _pendingOperations = 0; // رجع العداد للصفر
    }

    private async Task MarkChangeAsync(LogEntry<T> entry)
    {
        await AppendLogAsync(entry);
        _pendingOperations++;

        if(_pendingOperations >= MaxPendingOperations) 
            await CreateSnapshotAsync();
    }
    #endregion
}
