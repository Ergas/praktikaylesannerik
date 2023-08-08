using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Reflection.Metadata;
using System.Web;

namespace praktikaylrik.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public List<Event> eventsFuture = new List<Event>();
        public List<Event> eventsPast = new List<Event>();
        public IDictionary<int, int> GuestCount { get; set; } = new Dictionary<int, int>();

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ergas\Documents\GitHub\praktikaylesannerik\database\registration_system.mdf;Integrated Security=True;Connect Timeout=30";
            Event eventObj;
            SqlConnection cnn;
            SqlCommand command;
            string sql;
            string sqlForGuests;
            SqlDataReader dataReader;

            cnn = new SqlConnection(connectionString);
            cnn.Open();

            sql = "SELECT * FROM event";
            sqlForGuests = "SELECT * FROM guest";

            command = new SqlCommand(sql, cnn);
            
            dataReader = command.ExecuteReader();

            while (dataReader.Read())
            {
                int id = (int)dataReader["event_id"];
                string name = (string)dataReader["event_name"];
                DateTime dateTime = (DateTime)dataReader["event_date"];
                string location = (string)dataReader["location"];
                string addInfo;
                if (!dataReader["add_info"].Equals(System.DBNull.Value))
                {
                    addInfo = (string)dataReader["add_info"];
                } else
                {
                    addInfo = "";
                }
                

                eventObj = new Event();

                eventObj.EventId = id;
                eventObj.Name = name;
                eventObj.EventDate = dateTime;
                eventObj.Location = location;
                eventObj.AddInfo = addInfo;

                GuestCount.Add(eventObj.EventId, 0);

                if (dateTime >  DateTime.Now)
                {
                    eventsFuture.Add(eventObj);
                } else
                {
                    eventsPast.Add(eventObj);
                }

            }
            dataReader.Close();
            command.Dispose();
            command = new SqlCommand(sqlForGuests, cnn);
            dataReader = command.ExecuteReader();

            while (dataReader.Read())
            {
                int eventId = (int)dataReader["event_id"];
                int guestId = (int)dataReader["guest_id"];
                bool isCompany = (bool)dataReader["is_company"];
                int guestCount = 1;
                if (isCompany)
                {
                    guestCount = int.Parse((string)dataReader["last_name"]);
                }

                if (GuestCount.ContainsKey(eventId))
                {
                    GuestCount[eventId] += guestCount;
                }
            }

            command.Dispose();
            cnn.Close();

            eventsFuture.Sort((x, y) => x.EventDate.CompareTo(y.EventDate));
            eventsPast.Sort((x, y) => y.EventDate.CompareTo(x.EventDate));
        }

        public void OnPost(int deleteId)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ergas\Documents\GitHub\praktikaylesannerik\database\registration_system.mdf;Integrated Security=True;Connect Timeout=30";
            SqlConnection cnn;
            SqlCommand command;

            cnn = new SqlConnection(connectionString);
            cnn.Open();

            command = cnn.CreateCommand();

            command.CommandText = "SELECT * FROM event WHERE event_id=@eventId";
            command.Parameters.AddWithValue("@eventId", deleteId);

            SqlDataReader reader = command.ExecuteReader();

            DateTime dateTime;

            if (reader.Read())
            {
                dateTime = (DateTime)reader["event_date"];
                reader.Close();

                if (DateTime.Compare(dateTime, DateTime.Now) > 0)
                {
                    command = cnn.CreateCommand();

                    command.CommandText = "DELETE FROM [dbo].[event] WHERE event_id=@eventId";
                    command.Parameters.AddWithValue("@eventId", deleteId);

                    command.ExecuteNonQuery();
                    command.Dispose();

                    command = cnn.CreateCommand();
                    command.CommandText = "DELETE FROM [dbo].[guest] WHERE event_id=@eventId";
                    command.Parameters.AddWithValue("@eventId", deleteId);
                    command.ExecuteNonQuery();
                }
            }

            cnn.Close();

            Response.Redirect("../Index");
        }
    }
}