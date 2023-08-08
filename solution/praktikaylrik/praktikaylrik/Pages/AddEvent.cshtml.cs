﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR.Protocol;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace praktikaylrik.Pages
{
    public class AddEvent : PageModel
    {
        public List<string> errors = new List<string>();

        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public string AddInfo { get; set; }


        public void OnGet()
        {
        }

        public void OnPost(string name, DateTime date, string location, string addInfo)
        {
            if (string.IsNullOrEmpty(name))
            {
                errors.Add("Ürituse nime lahter ei tohi olla tühi!");
            }
            if (string.IsNullOrEmpty(location))
            {
                errors.Add("Ürituse asukoha lahter ei tohi olla tühi!");
            }
            if (DateTime.Compare(DateTime.Now, date) > 0)
            {
                errors.Add("Lisatav üritus peab toimuma tulevikus!");
            }
            if (addInfo != null && addInfo.Length > 1000)
            {
                errors.Add("Lisainfo pikkus tohib olla maksimaalselt 1000 tähemärki. Praegu on pikkus " + addInfo.Length + " tähemärki.");
            }

            if (errors.Count == 0)
            {

                Event event1 = new()
                {
                    Name = name,
                    EventDate = date,
                    Location = location,
                    AddInfo = addInfo
                };

                SqlConnection cnn;
                SqlCommand command;
                string sql;

                sql = "INSERT INTO[dbo].[event] ([event_name], [event_date], [location], [add_info]) VALUES( N'" + name + "', N'" + date.ToString("yyyy-MM-dd HH:mm:ss") + "', N'" + location + "', N'" + addInfo + "')";

                cnn = new SqlConnection(DatabaseConnection.ConnectionString);

                cnn.Open();

                command = new SqlCommand(sql, cnn);

                command.ExecuteReader();

                cnn.Close();

                Response.Redirect("../Index");
            }

            Name = name;
            Date = date;
            Location = location;
            AddInfo = addInfo;
        }
    }
}