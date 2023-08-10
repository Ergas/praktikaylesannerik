using Microsoft.VisualStudio.TestTools.UnitTesting;
using praktikaylrik.Pages;
using praktikaylrikTests;
using System.Data.SqlClient;

namespace praktikaylrik.PagesTests
{
    [TestClass()]
    public class ParticipantsTestsTests
    {

        [TestMethod()]
        public void OnPostCanDeleteEventTest()
        {
            Event insertedEvent = TestHelper.AddEventToDatabase("With guests", new DateTime(2023, 12, 12, 12, 00, 00), "Secret location", "No info provided");

            Guest guest1 = TestHelper.AddGuestToDatabase("Test", "Private", 1, "44712282729", 1, "No info provided", insertedEvent.EventId);
            Guest guest2 = TestHelper.AddGuestToDatabase("Company", "7", 2, "17384752", 1, "No info provided", insertedEvent.EventId);

            Participants participants = new Participants();

            participants.OnGet(insertedEvent.EventId, guest1.GuestId, "y");

            SqlConnection cnn = new SqlConnection(DatabaseConnection.ConnectionString);
            cnn.Open();
            
            SqlCommand command = cnn.CreateCommand();

            command.CommandText = "SELECT * FROM guest WHERE guest_id=@guestId1 OR guest_id=@guestId2";
            command.Parameters.AddWithValue("@guestId1", guest1.GuestId);
            command.Parameters.AddWithValue("@guestId2", guest2.GuestId);

            SqlDataReader dataReader = command.ExecuteReader();

            List<Guest> guests = new List<Guest>();

            while (dataReader.Read()) 
            {
                guests.Add(new Guest()
                {
                    GuestId = (int)dataReader["guest_id"],
                    FirstName = (string)dataReader["first_name"],
                    LastName = (string)dataReader["last_name"],
                    ClientTypeId = (int)dataReader["client_type"],
                    IdNumber = (string)dataReader["id_number"],
                    PaymentTypeId = (int)dataReader["payment_type"],
                    AddInfo = (string)dataReader["add_info"],
                    EventId = (int)dataReader["event_id"],
                });
                
            }

            if (guests.Count == 0) 
            {
                throw new Exception("Andmebaasist oleks pidanud kustuma vaid üks külaline, aga kustusid kõik külalised.");
            } else
            {
                if (guests.Exists(x => x.GuestId == guest1.GuestId))
                {
                    throw new Exception("Külalise " + guest1.FirstName + " kustutamine andmebaasist ebaõnnestus!");
                }
            }

            dataReader.Close();
            command.Dispose();
            cnn.Close();

        }
    }
}