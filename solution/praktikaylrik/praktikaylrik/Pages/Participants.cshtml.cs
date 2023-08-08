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
        private readonly ILogger<Participants> _logger;
        public List<Guest> guests = new List<Guest>();
        private Guest GuestToShow { get; set; }

        public Event EventToShow { get; set; }

        public List<string> Errors { get; set; } = new List<string>();

        public Participants(ILogger<Participants> logger)
        {
            _logger = logger;
        }

        public void OnGet(int id, int guestId, string delete)
        {
            if (!string.IsNullOrEmpty(delete))
            {
                string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ergas\Documents\GitHub\praktikaylesannerik\database\registration_system.mdf;Integrated Security=True;Connect Timeout=30";
                SqlConnection cnn;
                SqlCommand command;
                string sql;
                SqlDataReader dataReader;

                cnn = new SqlConnection(connectionString);
                cnn.Open();

                sql = "DELETE FROM [dbo].[guest] WHERE guest_id=" + guestId;

                command = new SqlCommand(sql, cnn);

                command.ExecuteNonQuery();

                Response.Redirect("../Participants?id=" + id);
            }
            GetEvent(id);
        }

        public void OnPost(bool clientType, string firstName, string lastName, string idNumber, string payment, string addInfo, int eventId)
        {
            GetEvent(eventId);
            if (clientType) {
                if (firstName == null || firstName.Length < 3)
                {
                    Errors.Add("Kontrolli ettevõtte nime!");
                }
                if (idNumber == null || idNumber.Length != 8)
                {
                    Errors.Add("Kontrolli ettevõtte registrikoodi (vale arv numbreid)!");
                }
            } else
            {
                if (firstName == null || firstName.Length < 2)
                {
                    Errors.Add("Kontrolli eesnime!");
                }
                if (lastName == null || lastName.Length < 2)
                {
                    Errors.Add("Kontrolli perekonnanime!");
                }
                if (idNumber == null || idNumber.Length != 11)
                {
                    Errors.Add("Kontrolli isikukoodi (vale arv numbreid)!");
                }
            }

            if (Errors.Count == 0)
            {
                GuestToShow = new Guest
                {
                    FirstName = firstName!,
                    LastName = lastName,
                    IdNumber = idNumber!,
                    IsCompany = clientType,
                    EventId = eventId,
                    PaymentType = payment,
                    AddInfo = addInfo
                };

                string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ergas\Documents\GitHub\praktikaylesannerik\database\registration_system.mdf;Integrated Security=True;Connect Timeout=30";
                SqlConnection cnn;
                SqlCommand command;
                string sql;

                sql = "INSERT INTO[dbo].[guest] ([first_name], [last_name], [is_company], [id_number], [payment_type], [add_info], [event_id]) VALUES( N'" + GuestToShow.FirstName + "', N'" + GuestToShow.LastName + "', N'" + GuestToShow.IsCompany + "', N'" + GuestToShow.IdNumber + "', N'" + GuestToShow.PaymentType + "', N'" + GuestToShow.AddInfo + "', N'" + GuestToShow.EventId + "');SELECT CAST(scope_identity() AS int)";

                cnn = new SqlConnection(connectionString);

                cnn.Open();

                command = new SqlCommand(sql, cnn);

                command.ExecuteScalar();

                cnn.Close();

                Response.Redirect("../Index");
            }


        }

        private void GetEvent(int eventId)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ergas\Documents\GitHub\praktikaylesannerik\database\registration_system.mdf;Integrated Security=True;Connect Timeout=30";
            SqlConnection cnn;
            SqlCommand command;
            string sql;
            SqlDataReader dataReader;

            cnn = new SqlConnection(connectionString);
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
                    IsCompany = (bool)dataReader["is_company"],
                    IdNumber = (string)dataReader["id_number"],
                    PaymentType = (string)dataReader["payment_type"],
                    AddInfo = (string)dataReader["add_info"],
                    EventId = eventId
                });
            }

            dataReader.Close();
        }
    }
}