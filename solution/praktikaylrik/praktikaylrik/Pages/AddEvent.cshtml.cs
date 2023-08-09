using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace praktikaylrik.Pages
{
    public class AddEvent : PageModel
    {
        public List<string> errors = new();

        public string? Name { get; set; }
        public DateTime Date { get; set; }
        public string? Location { get; set; }
        public string? AddInfo { get; set; }

        /// <summary>
        /// Function which creates a new event and saves it to the database.
        /// </summary>
        /// <param name="name">Name of the event that is being created.</param>
        /// <param name="date">Date of when the event should happen.</param>
        /// <param name="location">Location where the event would be held.</param>
        /// <param name="addInfo">Additional information about the event.</param>
        public void OnPost(string name, DateTime date, string location, string addInfo)
        {

            CheckForErrors(name, date, location, addInfo);

            if (errors.Count == 0)
            {
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

        /// <summary>
        /// Method to check for faulty inputs.
        /// </summary>
        /// <param name="name">Name of the event that is being created.</param>
        /// <param name="date">Date of when the event should happen.</param>
        /// <param name="location">Location where the event would be held.</param>
        /// <param name="addInfo">Additional information about the event.</param>
        private void CheckForErrors(string name, DateTime date, string location, string addInfo)
        {
            if (string.IsNullOrEmpty(name))
            {
                errors.Add("Ürituse nime lahter ei tohi olla tühi!");
                throw new ArgumentException("Name of the event must not be empty!");
            }
            if (string.IsNullOrEmpty(location))
            {
                errors.Add("Ürituse asukoha lahter ei tohi olla tühi!");
                throw new ArgumentException("Location of the event must not be empty!");
            }
            if (DateTime.Compare(DateTime.Now, date) > 0)
            {
                errors.Add("Lisatav üritus peab toimuma tulevikus!");
                throw new ArgumentException("Date of the event must be in the future!");
            }
            if (addInfo != null && addInfo.Length > 1000)
            {
                errors.Add("Lisainfo pikkus tohib olla maksimaalselt 1000 tähemärki. Praegu on pikkus " + addInfo.Length + " tähemärki.");
                throw new ArgumentException("Additional information can maximum be up to 1000 characters!");
            }
        }
    }
}