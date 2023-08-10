using System.Numerics;
using System.Text.Json;

namespace praktikaylrik
{
    public class Event
    {

        public int EventId { get; set; }
        public string Name { get; set; } = "";

        public DateTime EventDate { get; set; }
        public string Location { get; set; } = "";
        public string AddInfo { get; set; } = "";

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
