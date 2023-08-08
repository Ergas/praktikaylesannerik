using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Numerics;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace praktikaylrik.Pages
{
    public class AddParticipant : PageModel
    {
        private readonly ILogger<AddParticipant> _logger;
        public Guest GuestToShow { get; set; } = new Guest()
        {
            FirstName = "",
            LastName = "",
            IdNumber = "",
            AddInfo = "",
        };

        public Event EventToShow { get; set; }

        public List<string> Errors { get; set; } = new List<string>();

        public bool IsCompany = false;
        public bool IsPrivate = false;
        public int Change = 0;

        public AddParticipant(ILogger<AddParticipant> logger)
        {
            _logger = logger;
        }

        public void OnGet(int id, string clientType, int changeDetails, int clientId)
        {
            GetEvent(id);
            GuestToShow.GuestId = -1;
            if (changeDetails == 1)
            {
                Change = changeDetails;
                GetGuest(clientId);
            }
            
            if (!string.IsNullOrEmpty(clientType))
            {
                if (clientType.Equals("firma"))
                {
                    IsCompany = true;
                    IsPrivate = false;
                    GuestToShow.IsCompany = true;
                } else if (clientType.Equals("eraisik"))
                {
                    IsCompany = false;
                    IsPrivate = true;
                    GuestToShow.IsCompany = false;
                } else
                {
                    Response.Redirect("../AddParticipant?id=" + id);
                }
            }
            
        }

        public void OnPost(bool isCompany, string firstName, string lastName, string idNumber, string payment, string addInfo, int eventId, int guestId, int change)
        {
            GetEvent(eventId);
            if (isCompany) {
                System.Diagnostics.Debug.WriteLine("firma: p2ringust " + isCompany.ToString() + " ja get-ist: " + IsCompany);
                if (firstName == null || firstName.Length < 3)
                {
                    Errors.Add("Kontrolli ettevõtte nime!");
                }
                if (lastName == null)
                {
                    lastName = "0";
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
                    LastName = lastName!,
                    IdNumber = idNumber!,
                    IsCompany = (bool)IsCompany,
                    EventId = (int)eventId,
                    PaymentType = payment,
                    AddInfo = addInfo
                };

                string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ergas\Documents\GitHub\praktikaylesannerik\database\registration_system.mdf;Integrated Security=True;Connect Timeout=30";
                SqlConnection cnn;
                SqlCommand command;

                cnn = new SqlConnection(connectionString);

                cnn.Open();

                command = cnn.CreateCommand();

                if (change == 1)
                {
                    command.CommandText = "UPDATE guest SET first_name = @fname, last_name = @lname, id_number = @idNumber, payment_type = @payment, add_info = @addInfo WHERE guest_id = @guestId;SELECT CAST(scope_identity() AS int)";
                    command.Parameters.AddWithValue("@fname", GuestToShow.FirstName);
                    command.Parameters.AddWithValue("@lname", GuestToShow.LastName);
                    command.Parameters.AddWithValue("@idNumber", GuestToShow.IdNumber);
                    command.Parameters.AddWithValue("@payment", GuestToShow.PaymentType);
                    if (addInfo != null)
                    {
                        command.Parameters.AddWithValue("@addInfo", GuestToShow.AddInfo);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@addInfo", "");
                    }
                    command.Parameters.AddWithValue("@guestId", guestId);
                }
                else
                {
                    command.CommandText = "INSERT INTO[dbo].[guest] ([first_name], [last_name], [is_company], [id_number], [payment_type], [add_info], [event_id]) VALUES( N'@fname', N'@lname', N'"+ GuestToShow.IsCompany + "', N'@idNumber', N'@payment', N'@addInfo', N'@eventId');";
                    command.Parameters.AddWithValue("@fname", GuestToShow.FirstName);
                    command.Parameters.AddWithValue("@lname", GuestToShow.LastName);
                    command.Parameters.AddWithValue("@idNumber", GuestToShow.IdNumber);
                    command.Parameters.AddWithValue("@payment", GuestToShow.PaymentType);
                    command.Parameters.AddWithValue("@eventId", GuestToShow.EventId);
                    //command.Parameters.AddWithValue("@isCompany", GuestToShow.IsCompany);
                    if (addInfo != null)
                    {
                        command.Parameters.AddWithValue("@addInfo", GuestToShow.AddInfo);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@addInfo", "");
                    }
                }

                command.ExecuteScalar();

                cnn.Close();

                Response.Redirect("../Participants?id=" + eventId);
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

            dataReader.Close();
            cnn.Close();   
        }

        private void GetGuest(int id)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ergas\Documents\GitHub\praktikaylesannerik\database\registration_system.mdf;Integrated Security=True;Connect Timeout=30";
            SqlConnection cnn;
            SqlCommand command;
            string sql;
            SqlDataReader dataReader;

            cnn = new SqlConnection(connectionString);
            cnn.Open();

            sql = "SELECT * FROM guest WHERE guest_id=" + id;

            command = new SqlCommand(sql, cnn);

            dataReader = command.ExecuteReader();

            if (dataReader.Read())
            {
                GuestToShow = new Guest()
                {
                    GuestId = id,
                    FirstName = (string)dataReader["first_name"],
                    LastName = (string)dataReader["last_name"],
                    EventId = (int)dataReader["event_id"],
                    IsCompany = (bool)dataReader["is_company"],
                    IdNumber = (string)dataReader["id_number"],
                    PaymentType = (string)dataReader["payment_type"]
                };

                if (!dataReader["add_info"].Equals(System.DBNull.Value))
                {
                    GuestToShow.AddInfo = (string)dataReader["add_info"];
                }
                else
                {
                    GuestToShow.AddInfo = "";
                }
            }

            cnn.Close();
        }
    }
}