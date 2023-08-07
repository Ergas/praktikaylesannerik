using System.Numerics;

namespace praktikaylrik
{
    public class Guest
    {
        public int GuestId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsCompany { get; set; }
        public BigInteger IdNumber { get; set; }
    }
}
