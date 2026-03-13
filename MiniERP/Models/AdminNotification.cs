namespace MiniERP.Models
{
    public class AdminNotification
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? Message { get; set; }
        public string? Type { get; set; } // info, warning, danger...
        public DateTime? CreatedAt { get; set; }
        public string? From { get; set; }
    }
}

