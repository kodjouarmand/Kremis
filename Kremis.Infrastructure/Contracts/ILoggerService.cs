using System;

namespace Kremis.Infrastructure.Contracts
{
    public interface ILoggerService 
    { 
        void LogInfo(string message); 
        void LogWarning(string message); 
        void LogDebug(string message);
        void LogError(string message);
        void LogError(Exception ex);
    }
}
