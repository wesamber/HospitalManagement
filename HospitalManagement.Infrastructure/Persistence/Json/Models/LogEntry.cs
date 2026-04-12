using HospitalManagement.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Persistence.Json.Models;

public class LogEntry<T> where T : IEntity
{
    public string Op { get; set; } = default!; // "add", "update", "delete"
    public T? Entity { get; set; }
    public Guid? Id { get; set; }
}
