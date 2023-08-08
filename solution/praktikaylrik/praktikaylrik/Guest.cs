using System.Numerics;

namespace praktikaylrik
{
    public class Guest
    {
        public int GuestId { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public int ClientTypeId { get; set; } = 0;
        public string IdNumber { get; set; } = "";
        public int PaymentTypeId { get; set; }
        public string AddInfo { get; set; } = "";
        public int EventId { get; set; }
    }
}
