using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

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

        public int ChangeDetails = 0;

        public int ClientType = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventId">Id of the event to which user wants to add participants or change guest's info.</param>
        /// <param name="guestId">Id of the participant whose info is being edited.</param>
        /// <param name="clientType">Type of guest (whether private person or company).</param>
        /// <param name="changeDetails">Number 0 means creating new participant for the event, 1 means editing someone's info.</param>
        public void OnGet(int eventId, int guestId, int clientType, int changeDetails)
        {
            GetEvent(eventId);
            GetPaymentTypes();
            ChangeDetails = changeDetails;
            if (ChangeDetails == 1)
            {
                GetGuest(guestId);
            }
            if (clientType != 0)
            {
                ClientType = clientType;
            }
        }

        /// <summary>
        /// Creatinga new customer or editing existing one.
        /// </summary>
        /// <param name="eventId">Id of the event to which user wants to add participants or change guest's info.</param>
        /// <param name="guestId">Id of the participant whose info is being edited.</param>
        /// <param name="firstName">First name of a private person or name of the company.</param>
        /// <param name="lastName">Last name of a private person or amount of guests from a company.</param>
        /// <param name="idNumber">Personal identification code for private person, registry number for company.</param>
        /// <param name="paymentTypeId">Payment type, whether it would be cash or bank transaction etc.</param>
        /// <param name="addInfo">Additional information about the guest in this event.</param>
        /// <param name="clientTypeId">Type of guest (whether private person or company).</param>
        /// <param name="isChanging">Number 0 means creating new participant for the event, 1 means editing someone's info.</param>
        public void OnPost(int eventId, int guestId, string firstName, string lastName, string idNumber, int paymentTypeId, string addInfo, int clientTypeId, int isChanging)
        {
            GetEvent(eventId);
            // Check if all the fields are filled as required
            CheckForErrors(eventId, guestId, firstName, lastName, idNumber, paymentTypeId, addInfo, clientTypeId, isChanging);

            if (Errors.Count == 0)
            {
                // Create client as object
                CreateGuest(eventId, guestId, firstName!, lastName!, clientTypeId, idNumber!, paymentTypeId, addInfo!, isChanging);

                Response.Redirect("../Index");
            }
        }

        /// <summary>
        /// Method to fetch an event from the database to show it's information on the webpage.
        /// </summary>
        /// <param name="eventId">Id of the event.</param>
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
        /// Method to fetch information about a guest from the database.
        /// </summary>
        /// <param name="id">Id of the guest.</param>
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
                    ClientTypeId = (int)dataReader["client_type"],
                    IdNumber = (string)dataReader["id_number"],
                    PaymentTypeId = (int)dataReader["payment_type"]
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
        /// Method to create guest which will be saved into the database.
        /// </summary>
        /// <param name="eventId">Id of the event to which user wants to add participants or change guest's info.</param>
        /// <param name="guestId">Id of the participant whose info is being edited.</param>
        /// <param name="firstName">First name of a private person or name of the company.</param>
        /// <param name="lastName">Last name of a private person or amount of guests from a company.</param>
        /// <param name="idNumber">Personal identification code for private person, registry number for company.</param>
        /// <param name="paymentTypeId">Payment type, whether it would be cash or bank transaction etc.</param>
        /// <param name="addInfo">Additional information about the guest in this event.</param>
        /// <param name="clientTypeId">Type of guest (whether private person or company).</param>
        /// <param name="isChanging">Number 0 means creating new participant for the event, 1 means editing someone's info.</param>
        private void CreateGuest(int eventId, int guestId, string firstName, string lastName, int clientTypeId, string idNumber, int paymentTypeId, string addInfo, int isChanging)
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
            if (isChanging == 1)
            {
                command.CommandText = "UPDATE guest SET first_name=@fname, last_name=@lname, id_number=@idNumber, payment_type=@paymentTypeId, add_info=@addInfo WHERE guest_id=@guestId;";
                command.Parameters.AddWithValue("@guestId", guestId);
            }
            else
            {
                command.CommandText = "INSERT INTO guest (first_name, last_name, client_type, id_number, payment_type, add_info, event_id) VALUES(@fname, @lname, @clientType, @idNumber, @paymentTypeId, @addInfo, @eventId);";
                command.Parameters.AddWithValue("@eventId", eventId);
                command.Parameters.AddWithValue("@clientType", clientTypeId);
            }
            command.Parameters.AddWithValue("@fname", Client.FirstName);
            command.Parameters.AddWithValue("@lname", Client.LastName);
            command.Parameters.AddWithValue("@idNumber", Client.IdNumber);
            command.Parameters.AddWithValue("@paymentTypeId", paymentTypeId);
            command.Parameters.AddWithValue("@addInfo", Client.AddInfo);


            command.ExecuteScalar();
            cnn.Close();
        }
   
        /// <summary>
        /// Method to fetch all payment types from database.
        /// </summary>
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

        /// <summary>
        /// Method to find faults in inputs.
        /// </summary>
        /// <param name="eventId">Id of the event to which user wants to add participants or change guest's info.</param>
        /// <param name="guestId">Id of the participant whose info is being edited.</param>
        /// <param name="firstName">First name of a private person or name of the company.</param>
        /// <param name="lastName">Last name of a private person or amount of guests from a company.</param>
        /// <param name="idNumber">Personal identification code for private person, registry number for company.</param>
        /// <param name="paymentTypeId">Payment type, whether it would be cash or bank transaction etc.</param>
        /// <param name="addInfo">Additional information about the guest in this event.</param>
        /// <param name="clientTypeId">Type of guest (whether private person or company).</param>
        /// <param name="isChanging">Number 0 means creating new participant for the event, 1 means editing someone's info.</param>
        private void CheckForErrors(int eventId, int guestId, string firstName, string lastName, string idNumber, int paymentTypeId, string addInfo, int clientTypeId, int isChanging)
        {
            if (clientTypeId == 1)
            {
                // first check for private person
                if (firstName != null && firstName.Length < 2)
                {
                    Errors.Add("Kontrolli eesnime, pikkus peaks olema v�hemalt 2 m�rki.");
                    throw new ArgumentException("Eesnimi nimi on liiga l�hike!");
                }
                if (lastName != null && lastName.Length < 2)
                {
                    Errors.Add("Kontrolli perenime, pikkus peaks olema v�hemalt 2 m�rki.");
                    throw new ArgumentException("Perekonnanimi nimi on liiga l�hike!");
                }
                if (idNumber != null && !idNumber.Length.Equals(11))
                {
                    Errors.Add("Kontrolli isikukoodi, pikkus peaks olema 11 numbrit.");
                    throw new ArgumentException("Isikukood ei ole korrektne!");
                }
                if (addInfo != null && addInfo.Length > 1500)
                {
                    Errors.Add("Lisainfo lahtris tohib olla maksimaalselt 1500 m�rki! Praegu on sisestatud " + addInfo.Length + " m�rki.");
                    throw new ArgumentException("Lisainfo lahtris tohib olla maksimaalselt 1500 m�rki!");
                }
            }
            else
            {
                // then check for business
                if (firstName != null && firstName.Length < 2)
                {
                    Errors.Add("Kontrolli firma nime pikkust, pikkus peaks olema v�hemalt 2 m�rki.");
                    throw new ArgumentException("Kontrolli firma nime pikkust, pikkus peaks olema v�hemalt 2 m�rki.");
                }

                lastName ??= "0";

                if (idNumber != null && !idNumber.Length.Equals(8))
                {
                    Errors.Add("Kontrolli registrikoodi, pikkus peaks olema 8 numbrit.");
                    throw new ArgumentException("Kontrolli registrikoodi, pikkus peaks olema 8 numbrit.");
                }
                if (addInfo != null && addInfo.Length > 5000)
                {
                    Errors.Add("Lisainfo lahtris tohib olla maksimaalselt 5000 m�rki! Praegu on sisestatud " + addInfo.Length + " m�rki.");
                    throw new ArgumentException("Lisainfo lahtris tohib olla maksimaalselt 5000 m�rki!");
                }
            }
        }
    }
}