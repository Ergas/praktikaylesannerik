using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Numerics;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace praktikaylrik.Pages
{
    public class Participants : PageModel
    {
        public List<Guest> guests = new();

        public Event? EventToShow { get; set; }

        /// <summary>
        /// Method to fetch information about an event from the database or to delete guests from an event.
        /// </summary>
        /// <param name="id">Id of the event.</param>
        /// <param name="guestId">Id of the guest.</param>
        /// <param name="delete"Will have information when user wants to delete a guest from the database.</param>
        public void OnGet(int id, int guestId, string delete)
        {
            if (!string.IsNullOrEmpty(delete))
            {
                SqlConnection cnn;
                SqlCommand command;
                string sql;

                cnn = new SqlConnection(DatabaseConnection.ConnectionString);
                cnn.Open();

                sql = "DELETE FROM [dbo].[guest] WHERE guest_id=" + guestId;

                command = new SqlCommand(sql, cnn);

                command.ExecuteNonQuery();

                Response.Redirect("../Participants?id=" + id);
            }
            GetEvent(id);
        }

        /// <summary>
        /// Helper method to get information about an event from the database.
        /// </summary>
        /// <param name="eventId">Id of the event.</param>
        private void GetEvent(int eventId)
        {
            SqlConnection cnn;
            SqlCommand command;
            string sql;
            SqlDataReader dataReader;

            cnn = new SqlConnection(DatabaseConnection.ConnectionString);
            cnn.Open();

            sql = "SELECT * FROM event WHERE event_id = " + eventId;

            command = new SqlCommand(sql, cnn);

            dataReader = command.ExecuteReader();

            if (dataReader.Read())
            {
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
                

                EventToShow = new Event
                {
                    EventId = eventId,
                    Name = name,
                    EventDate = dateTime,
                    Location = location,
                    AddInfo = addInfo
                };
            }

            sql = "SELECT * FROM guest WHERE event_id = " + eventId;

            command = new SqlCommand(sql, cnn);

            dataReader.Close();

            dataReader = command.ExecuteReader();

            while (dataReader.Read())
            {
                guests.Add(new Guest
                {
                    GuestId = (int)dataReader["guest_id"],
                    FirstName = (string)dataReader["first_name"],
                    LastName = (string)dataReader["last_name"],
                    ClientTypeId = (int)dataReader["client_type"],
                    IdNumber = (string)dataReader["id_number"],
                    PaymentTypeId = (int)dataReader["payment_type"],
                    AddInfo = (string)dataReader["add_info"],
                    EventId = eventId
                });
            }

            dataReader.Close();
        }
    }
}