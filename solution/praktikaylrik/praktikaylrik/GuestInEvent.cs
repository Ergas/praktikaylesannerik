namespace praktikaylrik
{
    public class GuestInEvent
    {
        public int GuestInEventId { get; set; }
        public int EventId { get; set; }
        public int GuestId { get; set; }
        public int PaymentTypeId { get; set; }
        public string? AddInfo { get; set; }
    }
}
