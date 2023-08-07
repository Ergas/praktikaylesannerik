using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Numerics;

namespace praktikaylrik.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class Participants : PageModel
    {

        private readonly ILogger<Participants> _logger;
        public Event EventToShow { get; set; }
        public List<Guest> guests = new List<Guest>();

        public Participants(ILogger<Participants> logger)
        {
            _logger = logger;
        }

        public void OnGet(int id)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ergas\Documents\GitHub\praktikaylesannerik\database\registration_system.mdf;Integrated Security=True;Connect Timeout=30";
            SqlConnection cnn;
            SqlCommand command;
            String sql;
            SqlDataReader dataReader;
            
            cnn = new SqlConnection(connectionString);
            cnn.Open();

            sql = "SELECT * FROM event WHERE event_id = " + id;

            command = new SqlCommand(sql, cnn);

            dataReader = command.ExecuteReader();

            if (dataReader.Read())
            {
                string name = (string)dataReader["event_name"];
                DateTime dateTime = (DateTime)dataReader["event_date"];
                string location = (string)dataReader["location"];
                string addInfo = (string)dataReader["add_info"];

                EventToShow = new Event
                {
                    EventId = id,
                    Name = name,
                    EventDate = dateTime,
                    Location = location,
                    AddInfo = addInfo
                };
            }

            sql = "SELECT * FROM guest_in_event WHERE event_id = " + id;

            command = new SqlCommand(sql, cnn);

            dataReader.Close();

            dataReader = command.ExecuteReader();

            while (dataReader.Read())
            {
                int guestId = (int)dataReader["guest_id"];

                sql = "SELECT * FROM guest WHERE guest_id = " + guestId;

                command = new SqlCommand(sql, cnn);
                dataReader.Close();
                dataReader = command.ExecuteReader();
                if (dataReader.Read())
                {
                    guests.Add(new Guest
                    {
                        GuestId = guestId,
                        FirstName = (string)dataReader["first_name"],
                        LastName = (string)dataReader["last_name"],
                        IsCompany = (bool)dataReader["is_company"],
                        IdNumber = (BigInteger)dataReader["id_number"]
                    });;
                }
            }
        }

        public void OnPost(string firstName,  string lastName, BigInteger idNumber, int paymentTypeId, string addInfo)
        {

        }
    }
}