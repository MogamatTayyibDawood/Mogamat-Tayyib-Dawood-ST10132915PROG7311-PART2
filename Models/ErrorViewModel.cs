using System;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace PROG7311_PART2_AgriEnergyConnect.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public string? ErrorMessage { get; set; }
        public string? StackTrace { get; set; }
        public string? Path { get; set; }
        public DateTime ErrorTime { get; set; } = DateTime.UtcNow;
        public string? UserId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public bool ShowStackTrace => !string.IsNullOrEmpty(StackTrace) && DebugMode; // Only show in Debug mode
        public bool ShowPath => !string.IsNullOrEmpty(Path);

        private bool DebugMode =>
#if DEBUG
            true;
#else
            false;
#endif

        // Helper method to create from exception
        public static ErrorViewModel FromException(Exception ex, HttpContext context)
        {
            return new ErrorViewModel
            {
                RequestId = context.TraceIdentifier,
                ErrorMessage = ex.Message,
                StackTrace = ex.StackTrace,
                Path = context.Request.Path,
                UserId = context.User.Identity?.Name
            };
        }
    }
}