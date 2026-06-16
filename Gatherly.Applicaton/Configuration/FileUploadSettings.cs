using System;
using System.Collections.Generic;
using System.Text;

namespace Gatherly.Applicaton.Configuration
{
    public class FileUploadSettings
    {
        public long MaxSizeBytes { get; set; }
        public string[] AllowedExtensions { get; set; } = Array.Empty<string>();
        public string BaseUrl { get; set; } = string.Empty;
    }
}
