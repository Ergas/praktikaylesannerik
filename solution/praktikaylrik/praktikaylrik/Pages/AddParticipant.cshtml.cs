using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Numerics;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace praktikaylrik.Pages
{
    public class AddParticipant : PageModel
    {
        public Event EventToShow { get; set; } = new Event();

        public List<string> Errors { get; set; } = new List<string>();

        public List<PaymentType> PaymentTypes { get; set; } = new List<PaymentType>();

        public Guest Client { get; set; } = new Guest()
        {
            FirstName = "",
            LastName = "",
            IdNumber = "",
            AddInfo = "",
            ClientTypeId = 0
        };

        public bool IsChanging = false;

        public int ClientType = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void OnGet(int eventId, int guestId, int clientType, bool change)
        {
            GetEvent(eventId);
            GetPaymentTypes();
            if (change)
            {
                GetGuest(guestId);
            }
            if (change)
            {
                IsChanging = true;
            }
            if (clientType != 0)
            {
                ClientType = clientType;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="guestId"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="clientTypeId"></param>
        /// <param name="idNumber"></param>
        /// <param name="paymentTypeId"></param>
        /// <param name="addInfo"></param>
        /// <param name="isChanging"></param>
        public void OnPost(int eventId, int guestId, string firstName, string lastName, string idNumber, int paymentTypeId, string addInfo, int clientTypeId)
        {
            GetEvent(eventId);
            // Check if all the fields are filled as required
            if (clientTypeId == 1)
            {
                // first check for private person
                if (firstName != null && firstName.Length < 2)
                {
                    Errors.Add("Kontrolli eesnime, pikkus peaks olema vähemalt 2 märki.");
                }
                if (lastName != null && lastName.Length < 2)
                {
                    Errors.Add("Kontrolli perenime, pikkus peaks olema vähemalt 2 märki.");
                }
                if (idNumber != null && !idNumber.Length.Equals(11))
                {
                    Errors.Add("Kontrolli isikukoodi, pikkus peaks olema 11 numbrit.");
                }
            }
            else
            {
                // then check for business
                if (firstName != null && firstName.Length < 2)
                {
                    Errors.Add("Kontrolli firma nime pikkust, pikkus peaks olema vähemalt 2 märki.");
                }

                lastName ??= "0";

                if (idNumber != null && !idNumber.Length.Equals(8))
                {
                    Errors.Add("Kontrolli registrikoodi, pikkus peaks olema 8 numbrit.");
                }
            }

            if (Errors.Count.Equals(0))
            {
                // Create client as object
                CreateGuest(eventId, guestId, firstName!, lastName!, clientTypeId, idNumber!, paymentTypeId, addInfo, IsChanging);

                Response.Redirect("../Participants?Id=" + eventId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventId"></param>
        private void GetEvent(int eventId)
        {
            SqlConnection cnn;
            SqlCommand command;
            SqlDataReader dataReader;

            cnn = new SqlConnection(DatabaseConnection.ConnectionString);
            cnn.Open();

            
            string sql = "SELECT * FROM event WHERE event_id = " + eventId;
            command = new SqlCommand(sql, cnn);
            //command.Parameters.AddWithValue("@eventId", eventId);

            dataReader = command.ExecuteReader();

            if (dataReader.Read())
            {
                string name = (string)dataReader["event_name"];
                DateTime dateTime = (DateTime)dataReader["event_date"];
                string location = (string)dataReader["location"];
                string addInfo;

                if (!dataReader["add_info"].Equals(DBNull.Value))
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
            command.Dispose();
            cnn.Close();   
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        private void GetGuest(int id)
        {
            SqlConnection cnn;
            SqlCommand command;
            SqlDataReader dataReader;

            cnn = new SqlConnection(DatabaseConnection.ConnectionString);
            cnn.Open();

            command = cnn.CreateCommand();
            command.CommandText = "SELECT * FROM guest WHERE guest_id=@id;";
            command.Parameters.AddWithValue("@id", id);

            dataReader = command.ExecuteReader();

            if (dataReader.Read())
            {
                Client = new Guest()
                {
                    GuestId = id,
                    FirstName = (string)dataReader["first_name"],
                    LastName = (string)dataReader["last_name"],
                    EventId = (int)dataReader["event_id"],
                    ClientTypeId = (int)dataReader["client_type_id"],
                    IdNumber = (string)dataReader["id_number"],
                    PaymentTypeId = (int)dataReader["payment_type_id"]
                };

                if (!dataReader["add_info"].Equals(System.DBNull.Value))
                {
                    Client.AddInfo = (string)dataReader["add_info"];
                }
                else
                {
                    Client.AddInfo = "";
                }
            }

            cnn.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="guestId"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="clientTypeId"></param>
        /// <param name="idNumber"></param>
        /// <param name="paymentTypeId"></param>
        /// <param name="addInfo"></param>
        /// <param name="isChanging"></param>
        private void CreateGuest(int eventId, int guestId, string firstName, string lastName, int clientTypeId, string idNumber, int paymentTypeId, string addInfo, bool isChanging)
        {
            Client = new Guest()
            {
                FirstName = firstName!,
                LastName = lastName!,
                ClientTypeId = clientTypeId,
                IdNumber = idNumber!,
                PaymentTypeId = paymentTypeId,
                EventId = eventId,
            };

            if (addInfo != null)
            {
                Client.AddInfo = addInfo;
            }
            else
            {
                Client.AddInfo = "";
            }

            SqlConnection cnn;
            SqlCommand command;

            cnn = new SqlConnection(DatabaseConnection.ConnectionString);
            cnn.Open();
            command = cnn.CreateCommand();

            // Either update or insert a new object (Guest) into database
            if (isChanging)
            {
                command.CommandText = "UPDATE guest SET first_name=@fname, last_name=@lname, id_number=@idNumber, payment_type_id=@paymentTypeId, add_info=@addInfo WHERE guest_id=@guestId;";
                command.Parameters.AddWithValue("@guestId", guestId);
            }
            else
            {
                command.CommandText = "INSERT INTO guest (first_name, last_name, client_type, id_number, payment_type_id, add_info, event_id) VALUES(@fname, @lname, @clientType, @idNumber, @paymentTypeId, @addInfo, @eventId);";
                command.Parameters.AddWithValue("@eventId", eventId);
                command.Parameters.AddWithValue("@clientType", clientTypeId);
            }
            command.Parameters.AddWithValue("@fname", Client.FirstName);
            command.Parameters.AddWithValue("@lname", Client.LastName);
            command.Parameters.AddWithValue("@idNumber", Client.IdNumber);
            command.Parameters.AddWithValue("@paymentTypeId", Client.PaymentTypeId);
            command.Parameters.AddWithValue("@addInfo", Client.AddInfo);


            command.ExecuteScalar();
            cnn.Close();
        }
   
        private void GetPaymentTypes()
        {
            SqlConnection cnn;
            SqlCommand command;
            SqlDataReader dataReader;

            cnn = new SqlConnection(DatabaseConnection.ConnectionString);
            cnn.Open();

            command = cnn.CreateCommand();
            command.CommandText = "SELECT * FROM payments;";

            dataReader = command.ExecuteReader();

            while (dataReader.Read())
            {
                int id = (int)dataReader["payment_type_id"];
                string name = (string)dataReader["name"];

                PaymentTypes.Add(new PaymentType()
                {
                    PaymentTypeId = id,
                    Name = name,
                });
            }

            dataReader.Close();
            command.Dispose();
            cnn.Close();
        }
    }
}