using Microsoft.VisualStudio.TestTools.UnitTesting;
using praktikaylrik.Pages;
using praktikaylrikTests;
using System.Data.SqlClient;

namespace praktikaylrik.PagesTests
{
    [TestClass()]
    public class IndexTests
    {

        [TestMethod()]
        public void OnPostCanDeleteEventTest()
        {
            Event insertedEvent = TestHelper.AddEventToDatabase("With guests", new DateTime(2023, 12, 12, 12, 00, 00), "Secret location", "No info provided");

            Guest guest1 = TestHelper.AddGuestToDatabase("Test", "Private", 1, "44712282729", 1, "No info provided", insertedEvent.EventId);
            Guest guest2 = TestHelper.AddGuestToDatabase("Company", "7", 2, "17384752", 1, "No info provided", insertedEvent.EventId);

            IndexModel index = new IndexModel();

            index.OnPost(insertedEvent.EventId);

            SqlConnection cnn = new SqlConnection(DatabaseConnection.ConnectionString);
            cnn.Open();
            
            SqlCommand command = cnn.CreateCommand();

            command.CommandText = "SELECT * FROM event WHERE event_id=@eventId";
            command.Parameters.AddWithValue("@eventId", insertedEvent.EventId);

            SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                reader.Close();
                command.Dispose();
                cnn.Close();
                throw new Exception("Sündmuse andmebaasist kustutamine ebaõnnestus!");
            }
            reader.Close();
            command.Dispose();

            command = cnn.CreateCommand();
            command.CommandText = "SELECT * FROM guest WHERE event_id=@eventId";
            command.Parameters.AddWithValue("@eventId", insertedEvent.EventId);
            reader = command.ExecuteReader();

            if (reader.Read()) 
            {
                reader.Close();
                command.Dispose();
                cnn.Close();
                throw new Exception("Sündmuse " + insertedEvent.Name + " kõikide või osade külaliste andmebaasist kustutamine ebaõnnestus.");
            }

            reader.Close();
            command.Dispose();
            cnn.Close();

        }
    }
}