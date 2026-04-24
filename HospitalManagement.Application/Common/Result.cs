using HospitalManagement.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Common;

public class Result<T> 
{
    public bool Success { get; init; }
    public T? Value { get; init; }
    public List<string> Errors { get; init; }


    private Result(bool success, T? value, List<string>? errors)
    {
        Success = success;
        Value = value;
        Errors = errors ?? new List<string>();
    }

    public static Result<T> SuccessResult(T value)
        => new(true, value, null);

    public static Result<T> Failure(List<string> errors)
        => new(false, default, errors);

    public static Result<T> Failure(string error)
                => new(false, default, new List<string> { error });
}
