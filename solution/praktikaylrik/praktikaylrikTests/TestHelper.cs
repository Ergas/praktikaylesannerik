using Microsoft.VisualStudio.TestTools.UnitTesting;
using praktikaylrik;
using praktikaylrik.Pages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace praktikaylrikTests
{
    internal static class TestHelper
    {
        public static Event AddEventToDatabase(string name, DateTime date, string location, string addInfo)
        {
            AddEvent addEvent = new();

            Event insertEvent = new Event()
            {
                Name = name,
                EventDate = date,
                Location = location,
                AddInfo = addInfo
            };

            int eventId = addEvent.OnPost(insertEvent.Name, insertEvent.EventDate, insertEvent.Location, insertEvent.AddInfo);

            Event eventObj;
            SqlConnection cnn;
            SqlCommand command;
            SqlDataReader dataReader;

            cnn = new SqlConnection(DatabaseConnection.ConnectionString);
            cnn.Open();

            command = cnn.CreateCommand();

            command.CommandText = "SELECT * FROM event WHERE event_id=@eventId";
            command.Parameters.AddWithValue("@eventId", eventId);

            dataReader = command.ExecuteReader();

            if (dataReader.Read())
            {
                eventObj = new Event()
                {
                    EventId = eventId,
                    Name = (string)dataReader["event_name"],
                    EventDate = (DateTime)dataReader["event_date"],
                    Location = (string)dataReader["location"],
                    AddInfo = (string)dataReader["add_info"]
                };

                insertEvent.EventId = eventId;

                Assert.AreEqual(insertEvent.ToString(), eventObj.ToString());

                return eventObj;

            }
            else
            {
                throw new ArgumentException("Üritust id-ga \" " + eventId + "\" andmebaasis ei leidu.");
            }
        }

        public static Guest AddGuestToDatabase(string firstName,  string lastName, int clientTypeId, string idNumber, int paymentTypeId, string addInfo, int eventId)
        {
            Guest insertedGuest = new Guest()
            {
                FirstName = firstName,
                LastName = lastName,
                ClientTypeId = clientTypeId,
                IdNumber = idNumber,
                PaymentTypeId = paymentTypeId,
                AddInfo = addInfo,
                EventId = eventId
            };

            AddParticipant addParticipant = new AddParticipant();

            int guestId = addParticipant.OnPost(insertedGuest.EventId, -1, insertedGuest.FirstName, insertedGuest.LastName, insertedGuest.IdNumber, insertedGuest.PaymentTypeId, insertedGuest.AddInfo, insertedGuest.ClientTypeId, 0);

            Guest guestObj;
            SqlConnection cnn;
            SqlCommand command;
            SqlDataReader dataReader;

            cnn = new SqlConnection(DatabaseConnection.ConnectionString);
            cnn.Open();

            command = cnn.CreateCommand();

            command.CommandText = "SELECT * FROM guest WHERE guest_id=@guestId";
            command.Parameters.AddWithValue("@guestId", guestId);

            dataReader = command.ExecuteReader();

            if (dataReader.Read())
            {
                guestObj = new Guest()
                {
                    GuestId = (int)dataReader["guest_id"],
                    FirstName = (string)dataReader["first_name"],
                    LastName = (string)dataReader["last_name"],
                    ClientTypeId = (int)dataReader["client_type"],
                    IdNumber = (string)dataReader["id_number"],
                    PaymentTypeId = (int)dataReader["payment_type"],
                    AddInfo = (string)dataReader["add_info"],
                    EventId = (int)dataReader["event_id"],
                };

                insertedGuest.GuestId = guestId;

                Assert.AreEqual(insertedGuest.ToString(), guestObj.ToString());

                return guestObj;

            }
            else
            {
                throw new ArgumentException("Üritust id-ga \" " + eventId + "\" andmebaasis ei leidu.");
            }
        }
    }
}
