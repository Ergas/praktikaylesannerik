using Microsoft.VisualStudio.TestTools.UnitTesting;
using praktikaylrik.Pages;

namespace praktikaylrik.PagesTests
{
    [TestClass()]
    public class AddEventTests
    {

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void OnPostWithMissingNameTest()
        {
            AddEvent addEvent = new();

            addEvent.OnPost("", new DateTime(2023, 09, 12, 15, 00, 00), "Location", "Add info");
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void OnPostWithWrongDateTestAsync()
        {
            AddEvent addEvent = new();

            addEvent.OnPost("Testttt", new DateTime(2023, 01, 12, 15, 00, 00), "Location", "Add info");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void OnPostWithMissingLocationTestAsync()
        {
            AddEvent addEvent = new();

            addEvent.OnPost("Testttt", new DateTime(2023, 09, 12, 15, 00, 00), "", "Add info");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void OnPostWithTooLongAdditionalInfoTestAsync()
        {
            AddEvent addEvent = new();

            addEvent.OnPost("Testttt", new DateTime(2023, 09, 12, 15, 00, 00), "Location", String.Concat(Enumerable.Repeat("T", 1001)));
        }

        [TestMethod()]
        public void OnPostSuccessTestAsync()
        {
            AddEvent addEvent = new();

            addEvent.OnPost("Testttt", new DateTime(2023, 09, 12, 15, 00, 00), "Location", "Add Info");
        }
    }
}