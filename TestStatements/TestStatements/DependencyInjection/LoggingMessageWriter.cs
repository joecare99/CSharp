using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStatements.DependencyInjection
{
    public class LoggingMessageWriter(ILogger<LoggingMessageWriter> logger) : IMessageWriter
    {
        public void Write(string message) 
            => logger.LogInformation("Info: {Msg}", message);
    }
    
    
}
