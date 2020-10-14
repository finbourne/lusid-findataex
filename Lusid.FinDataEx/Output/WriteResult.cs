﻿using System.Collections.Generic;

 namespace Lusid.FinDataEx.Output
{
    /// <summary>
    /// Contains status and any error messages of writing FinDataOutput
    /// returned from BBG DLWS
    /// </summary>
    public class WriteResult
    {
        public readonly WriteResultStatus Status;
        public readonly List<string> FilesWritten;
        public readonly List<string> FailureMessages;

        public WriteResult(WriteResultStatus status, List<string> filesWritten, List<string> failureMessages)
        {
            Status = status;
            FilesWritten = filesWritten;
            FailureMessages = failureMessages;
        }

        public override string ToString()
        {
            return $"{nameof(Status)}: {Status}, {nameof(FilesWritten)}: {FilesWritten}";
        }
    }

    public enum WriteResultStatus
    {
        Ok, Fail
    }
}