using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace praktikaylrik.Pages
{
    public class IndexModel : PageModel
    {
        public List<Event> eventsFuture = new();
        public List<Event> eventsPast = new();
        public IDictionary<int, int> GuestCount { get; set; } = new Dictionary<int, int>();

        /// <summary>
        /// Method to fetch all events from future and past from the database and to prepare them to show to user on web page.
        /// </summary>
        public void OnGet()
        {
            Event eventObj;
            SqlConnection cnn;
            SqlCommand command;
            string sql;
            string sqlForGuests;
            SqlDataReader dataReader;

            cnn = new SqlConnection(DatabaseConnection.ConnectionString);
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


                eventObj = new Event
                {
                    EventId = id,
                    Name = name,
                    EventDate = dateTime,
                    Location = location,
                    AddInfo = addInfo
                };

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
                int clientType = (int)dataReader["client_type"];
                int guestCount = 1;
                if (clientType == 2)
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

        /// <summary>
        /// Method to delete event from the database.
        /// </summary>
        /// <param name="deleteId">Id of the event that should be deleted.</param>
        public void OnPost(int deleteId)
        {
            SqlConnection cnn;
            SqlCommand command;

            cnn = new SqlConnection(DatabaseConnection.ConnectionString);
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