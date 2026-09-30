namespace IDR.Core.Models;

public sealed record DelphiExceptionHandler(
    uint? ExceptionInfoAddress,
    uint? ProcedureAddress);
