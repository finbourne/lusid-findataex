﻿using System.Collections.Generic;

 namespace Lusid.FinDataEx.Output
{
    public class WriteResult
    {
        public readonly WriteResultStatus Status;
        public readonly string Message;
        public readonly Dictionary<string, object> Properties;

        public WriteResult(WriteResultStatus status, string message) : 
            this(status, message, new Dictionary<string,object>()) {}
        
        public WriteResult(WriteResultStatus status, string message, Dictionary<string,object> properties)
        {
            Status = status;
            Message = message;
            Properties = properties;
        }

        public override string ToString()
        {
            return $"{nameof(Status)}: {Status}, {nameof(Message)}: {Message}, {nameof(Properties)}: {Properties}";
        }
    }

    public enum WriteResultStatus
    {
        Ok, Fail
    }
}