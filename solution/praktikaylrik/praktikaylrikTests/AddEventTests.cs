using Microsoft.VisualStudio.TestTools.UnitTesting;
using praktikaylrikTests;

namespace praktikaylrik.PagesTests
{
    [TestClass()]
    public class AddEventTests
    {

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void OnPostWithMissingNameTest()
        {
            TestHelper.AddEventToDatabase("", new DateTime(2023, 09, 12, 15, 00, 00), "Location", "Add info");
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void OnPostWithWrongDateTest()
        {
            TestHelper.AddEventToDatabase("Testttt", new DateTime(2023, 01, 12, 15, 00, 00), "Location", "Add info");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void OnPostWithMissingLocationTest()
        {
            TestHelper.AddEventToDatabase("Testttt", new DateTime(2023, 09, 12, 15, 00, 00), "", "Add info");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void OnPostWithTooLongAdditionalInfoTest()
        {
            TestHelper.AddEventToDatabase("Testttt", new DateTime(2023, 09, 12, 15, 00, 00), "Location", String.Concat(Enumerable.Repeat("T", 1001)));
        }

        [TestMethod()]
        public void OnPostSuccessTest()
        {
            TestHelper.AddEventToDatabase("SuccessEvent", new DateTime(2023, 09, 12, 15, 00, 00), "Location", "Add info");
        }
    }
}