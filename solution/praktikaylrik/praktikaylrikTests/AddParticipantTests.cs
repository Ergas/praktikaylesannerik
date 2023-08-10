using Microsoft.VisualStudio.TestTools.UnitTesting;
using praktikaylrikTests;

namespace praktikaylrik.PagesTests
{
    [TestClass()]
    public class AddParticipantTests
    {

        // Tests when trying to add private person as guest

        [TestMethod()]
        public void OnPostSuccessTest()
        {
            Event insertedEvent = TestHelper.AddEventToDatabase("With guests", new DateTime(2023, 12, 12, 12, 00, 00), "Secret location", "No info provided");

            TestHelper.AddGuestToDatabase("Test", "Private", 1, "44712282729", 1, "No info provided", insertedEvent.EventId);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void OnPostWrongIdCodeTest()
        {
            Event insertedEvent = TestHelper.AddEventToDatabase("With guests", new DateTime(2023, 12, 12, 12, 00, 00), "Secret location", "No info provided");

            TestHelper.AddGuestToDatabase("Test", "Private", 1, "33333333333", 1, "No info provided", insertedEvent.EventId);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void OnPostTooLongAddInfoTest()
        {
            Event insertedEvent = TestHelper.AddEventToDatabase("With guests", new DateTime(2023, 12, 12, 12, 00, 00), "Secret location", "No info provided");

            TestHelper.AddGuestToDatabase("Test", "Private", 1, "44712282729", 1, String.Concat(Enumerable.Repeat("T", 1501)), insertedEvent.EventId);
        }



        // Tests when trying to add company as guest

        [TestMethod()]
        public void OnPostSuccessCompanyTest()
        {
            Event insertedEvent = TestHelper.AddEventToDatabase("With guests", new DateTime(2023, 12, 12, 12, 00, 00), "Secret location", "No info provided");

            TestHelper.AddGuestToDatabase("Company", "7", 2, "17384752", 1, "No info provided", insertedEvent.EventId);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void OnPostTooLongAddInfoCompanyTest()
        {
            Event insertedEvent = TestHelper.AddEventToDatabase("With guests", new DateTime(2023, 12, 12, 12, 00, 00), "Secret location", "No info provided");

            TestHelper.AddGuestToDatabase("Company", "7", 1, "17384752", 1, String.Concat(Enumerable.Repeat("T", 5001)), insertedEvent.EventId);
        }
    }
}