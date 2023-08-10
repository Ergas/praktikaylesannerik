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

        public void OnGet()
        {

        }

        /// <summary>
        /// Function which creates a new event and saves it to the database.
        /// </summary>
        /// <param name="name">Name of the event that is being created.</param>
        /// <param name="date">Date of when the event should happen.</param>
        /// <param name="location">Location where the event would be held.</param>
        /// <param name="addInfo">Additional information about the event.</param>
        public int OnPost(string name, DateTime date, string location, string addInfo)
        {
            addInfo ??= "";
            CheckForErrors(name, date, location, addInfo);
            int eventId = -1;

            if (errors.Count == 0)
            {
                SqlConnection cnn;
                SqlCommand command;

                cnn = new SqlConnection(DatabaseConnection.ConnectionString);
                cnn.Open();

                command = cnn.CreateCommand();


                command.CommandText = "INSERT INTO[dbo].[event] ([event_name], [event_date], [location], [add_info]) VALUES( @name, @date, @location, @addInfo);SELECT SCOPE_IDENTITY();";

                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@location", location);
                command.Parameters.AddWithValue("@addInfo", addInfo);

                try
                {
                    eventId = int.Parse(command.ExecuteScalar().ToString()!);

                    command.Dispose();
                    cnn.Close();
                } catch
                {
                    command.Dispose();
                    cnn.Close();

                    throw new ArgumentException("Andmebaasi sisestamine ebaõnnestus!");
                }
                


                // Adding try-catch for tests since tests don't like redirecting
                // and I couldn't find a way for tests to ignore redirecting.
                try
                {
                    Response.Redirect("../Index");
                } catch (NullReferenceException)
                {
                }

                
            }

            Name = name;
            Date = date;
            Location = location;
            AddInfo = addInfo;

            return eventId;
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