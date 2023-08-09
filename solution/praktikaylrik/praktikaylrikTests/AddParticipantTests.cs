using Microsoft.VisualStudio.TestTools.UnitTesting;
using praktikaylrik.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace praktikaylrik.PagesTests
{
    [TestClass()]
    public class AddParticipantTests
    {

        // Tests when trying to add company as guest

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void OnPostTest()
        {
            AddParticipant addParticipant = new AddParticipant();
            DateTime time = new(2023, 08, 12, 15, 00, 00);
        }

        // Tests when trying to add participant as guest
    }
}