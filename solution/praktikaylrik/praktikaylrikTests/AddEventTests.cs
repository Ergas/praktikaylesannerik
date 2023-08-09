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
            DateTime time = new(2023, 08, 12, 15, 00, 00);


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
        public async Task OnPostWithWrongDateTestAsync()
        {
            DateTime time = new(2023, 02, 12, 15, 00, 00);

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
        public async Task OnPostWithMissingLocationTestAsync()
        {
            DateTime time = new(2023, 09, 12, 15, 00, 00);

            using StringContent jsonContent = new(
                JsonSerializer.Serialize(new
                {
                    name = "Name",
                    date = time,
                    location = "",
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
        public async Task OnPostWithTooLongAdditionalInfoTestAsync()
        {
            string info = String.Concat(Enumerable.Repeat("T", 1001));
            DateTime time = new(2023, 0, 12, 15, 00, 00);

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
            HttpResponseMessage response = _httpClient.PostAsync("/AddEvent", jsonContent).Result;

            string result = response.Content.ReadAsStringAsync().Result;
            throw new Exception(result);
        }
    }
}