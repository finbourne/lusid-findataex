﻿ namespace Lusid.FinDataEx.Output
{
    /// <summary>
    /// Contains status and any error messages of writing FinDataOutput
    /// returned from BBG DLWS
    /// </summary>
    public class WriteResult
    {
        public readonly WriteResultStatus Status;
        public readonly string FileOutputPath;
        public readonly string FailureMessage;

        private WriteResult(WriteResultStatus status, string fileOutputPath, string failureMessage)
        {
            Status = status;
            FileOutputPath = fileOutputPath;
            FailureMessage = failureMessage;
        }

        public static WriteResult Ok(string fileOutputPath)
        {
            return new WriteResult(WriteResultStatus.Ok, fileOutputPath, "");
        }

        public static WriteResult NotRun()
        {
            return new WriteResult(WriteResultStatus.NotRun, "", "");
        }
        
        public static WriteResult Fail(string failureMessage)
        {
            return new WriteResult(WriteResultStatus.Fail, "", failureMessage);
        }

        public override string ToString()
        {
            return $"{nameof(Status)}: {Status}, {nameof(FileOutputPath)}: {FileOutputPath}, {nameof(FailureMessage)}: {FailureMessage}";
        }
        
    }

    public enum WriteResultStatus
    {
        Ok, Fail, NotRun
    }
}