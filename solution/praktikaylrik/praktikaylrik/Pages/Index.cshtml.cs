using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Reflection.Metadata;
using System.Web;

namespace praktikaylrik.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public List<Event> eventsFuture = new List<Event>();
        public List<Event> eventsPast = new List<Event>();



        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ergas\Documents\GitHub\praktikaylesannerik\database\registration_system.mdf;Integrated Security=True;Connect Timeout=30";
            Event eventObj;
            SqlConnection cnn;
            SqlCommand command;
            String sql;
            SqlDataReader dataReader;

            cnn = new SqlConnection(connectionString);
            cnn.Open();

            sql = "SELECT * FROM event";

            command = new SqlCommand(sql, cnn);
            
            dataReader = command.ExecuteReader();

            while (dataReader.Read())
            {
                int id = (int)dataReader["event_id"];
                string name = (string)dataReader["event_name"];
                DateTime dateTime = (DateTime)dataReader["event_date"];
                string location = (string)dataReader["location"];
                string addInfo = (string)dataReader["add_info"];

                eventObj = new Event();

                eventObj.EventId = id;
                eventObj.Name = name;
                eventObj.EventDate = dateTime;
                eventObj.Location = location;
                eventObj.AddInfo = addInfo;

                if (dateTime >  DateTime.Now)
                {
                    eventsFuture.Add(eventObj);
                } else
                {
                    eventsPast.Add(eventObj);
                }
            }

            cnn.Close();

            eventsFuture.Sort((x, y) => x.EventDate.CompareTo(y.EventDate));
        }
    }
}