using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.Design.Serialization;
using System.Data.SqlClient;

namespace praktikaylrik.Pages
{
    public class ParticipantInfoModel : PageModel
    {
        public Guest GuestToShow { get; set; } = new Guest();
        public List<string> Errors { get; set; } = new List<string>();
        public void OnGet(int id)
        {
            GetGuest(id);
        }

        public void OnPost(string firstName,  string lastName, string idCode, string payment, string addInfo, int guestId, int eventId, bool isCompany) {
            GuestToShow.IsCompany = isCompany;
            GetGuest(guestId);

            if (isCompany)
            {
                if (firstName.Length < 2)
                {
                    Errors.Add("Kontrolli ettevõtte nime!");
                }
                if (idCode.Length != 8)
                {
                    Errors.Add("Kontrolli ettevõtte registrikoodi!");
                }
            } else
            {
                if (firstName.Length < 2)
                {
                    Errors.Add("Kontrolli eesnime!");
                }
                if (lastName.Length < 2)
                {
                    Errors.Add("Kontrolli perenime!");
                }
                if (idCode.Length != 11)
                {
                    Errors.Add("Kontrolli isikukoodi!");
                }
            }

            GuestToShow = new Guest()
            {
                FirstName = firstName,
                LastName = lastName,
                IdNumber = idCode,
                PaymentType = payment,
                AddInfo = addInfo,
                GuestId = guestId,
                EventId = eventId,
                IsCompany = isCompany
            };

            if (Errors.Count == 0) {
                string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ergas\Documents\GitHub\praktikaylesannerik\database\registration_system.mdf;Integrated Security=True;Connect Timeout=30";
                SqlConnection cnn;
                SqlCommand command;
                string sql;

                cnn = new SqlConnection(connectionString);

                cnn.Open();

                command = cnn.CreateCommand();

                command.CommandText = "UPDATE guest SET first_name = @fname, last_name = @lname, id_number = @idNumber, payment_type = @payment, add_info = @addInfo WHERE guest_id = @guestId;SELECT CAST(scope_identity() AS int)";
                command.Parameters.AddWithValue("@fname", GuestToShow.FirstName);
                command.Parameters.AddWithValue("@lname", GuestToShow.LastName);
                command.Parameters.AddWithValue("@idNumber", GuestToShow.IdNumber);
                command.Parameters.AddWithValue("@payment", GuestToShow.PaymentType);
                if (addInfo != null) {
                    command.Parameters.AddWithValue("@addInfo", GuestToShow.AddInfo);
                } else
                {
                    command.Parameters.AddWithValue("@addInfo", "");
                }
                command.Parameters.AddWithValue("@guestId", GuestToShow.GuestId);



                command.ExecuteScalar();

                cnn.Close();

                Response.Redirect("../Participants?id=" + GuestToShow.EventId);
            }
        }

        private void GetGuest(int id)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ergas\Documents\GitHub\praktikaylesannerik\database\registration_system.mdf;Integrated Security=True;Connect Timeout=30";
            Event eventObj;
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
                    EventId = (int)dataReader["event_id"],
                    IsCompany = (bool)dataReader["is_company"],
                    IdNumber = (string)dataReader["id_number"],
                    PaymentType = (string)dataReader["payment_type"]
                };



                if (!dataReader["last_name"].Equals(System.DBNull.Value))
                {
                    GuestToShow.LastName = (string)dataReader["last_name"];
                }
                else
                {
                    GuestToShow.LastName = "";
                }

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
