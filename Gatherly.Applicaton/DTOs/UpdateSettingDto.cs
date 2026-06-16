namespace Gatherly.Application.DTOs.Events
{
    public class UpdateSettingsRequestDto
    {
        public bool AllowReEntry { get; set; }
        public bool VipEnabled { get; set; }
        public bool RsvpApprovalRequired { get; set; }
        public int Capacity { get; set; }
        public string? RsvpDeadline { get; set; }
        public string Visibility { get; set; } = "Public";
    }
}