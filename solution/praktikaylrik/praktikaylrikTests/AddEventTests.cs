using Microsoft.AspNetCore.Builder;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using praktikaylrik.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace praktikaylrik.PagesTests
{
    [TestClass()]
    public class AddEventTests
    {
        private readonly HttpClient _httpClient;

        public AddEventTests()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new System.Uri("http://localhost:5242/");
           
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public async Task OnPostWithMissingNameTestAsync()
        {
            AddEvent addEvent = new AddEvent();
            DateTime time = new(2023, 08, 12, 15, 00, 00);
            //addEvent.OnPost("", time, "Tallinn", "Info");


            using StringContent jsonContent = new(
                JsonSerializer.Serialize(new
                {
                    name = "",
                    date = time,
                    location = "Test Location",
                    addInfo = "Add info exists"
                }),
                Encoding.UTF8,
                "application/json");

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync("/AddEvent", jsonContent);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                throw new ArgumentException(e.Message);
            }

            
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void OnPostWithWrongDateTest()
        {
            AddEvent addEvent = new AddEvent();
            DateTime time = new(2023, 02, 12, 15, 00, 00);
            addEvent.OnPost("Name", time, "Tallinn", "Info");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void OnPostWithMissingLocationTest()
        {
            AddEvent addEvent = new AddEvent();
            DateTime time = new(2023, 08, 12, 15, 00, 00);
            addEvent.OnPost("Name", time, "", "Info");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void OnPostWithTooLongAdditionalInfoTest()
        {
            AddEvent addEvent = new AddEvent();
            DateTime time = new(2023, 08, 12, 15, 00, 00);
            string info = String.Concat(Enumerable.Repeat("T", 1001));
            addEvent.OnPost("", time, "Tallinn", "Info");
        }

        [TestMethod()]
        public void OnPostSuccessTest()
        {
            DateTime time = new(2023, 08, 12, 15, 00, 00);
            using StringContent jsonContent = new(
                JsonSerializer.Serialize(new
                {
                    name = "Name",
                    date = time,
                    location = "Test Location",
                    addInfo = "Add info exists"
                }),
                Encoding.UTF8,
                "application/json");
            _httpClient.PostAsync("/AddEvent", jsonContent);
        }
    }
}