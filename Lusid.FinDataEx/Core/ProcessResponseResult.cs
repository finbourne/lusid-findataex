using System.Collections.Generic;

namespace Lusid.FinDataEx.Core
{
    public class ProcessResponseResult
    {
        public readonly ProcessResponseResultStatus Status;
        public readonly string Message;
        public readonly Dictionary<string, object> Properties;

        public ProcessResponseResult(ProcessResponseResultStatus status, string message) : 
            this(status, message, new Dictionary<string,object>()) {}
        
        public ProcessResponseResult(ProcessResponseResultStatus status, string message, Dictionary<string,object> properties)
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

    public enum ProcessResponseResultStatus
    {
        Ok, Fail
    }
}