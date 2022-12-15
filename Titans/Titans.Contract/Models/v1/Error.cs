﻿namespace Titans.Contract.Models.v1;

public class Error
{
    public string Message { get; }

    public Error(string message)
    {
        Message = message;
    }
}