using System.Numerics;

namespace praktikaylrik
{
    public class Guest
    {
        public int GuestId { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public bool IsCompany { get; set; }
        public string IdNumber { get; set; } = "";
        public string PaymentType { get; set; } = "";
        public string AddInfo { get; set; } = "";
        public int EventId { get; set; }
    }
}
