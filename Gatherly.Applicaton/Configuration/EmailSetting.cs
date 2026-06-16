using System;
using System.Collections.Generic;
using System.Text;

namespace Gatherly.Application.Configuration
{
    public class EmailSettings
    {
        public string Host { get; init; } = string.Empty;
        public int Port { get; init; } 
        public string Username { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }
}
