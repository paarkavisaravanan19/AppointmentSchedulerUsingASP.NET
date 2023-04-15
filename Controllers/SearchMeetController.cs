using AppointmentApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace AppointmentApplication.Controllers
{
	public class SearchMeetController : Controller
	{
        private readonly IConfiguration _configuration;
        public SearchMeetController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private SqlConnection _connection;
        private List<SchedulerModel> _meetingList = new List<SchedulerModel>();

        public bool ShowDiv = false;

        public void Connection()
        {
            string conn = _configuration.GetConnectionString("ScheduleDB");
            _connection = new SqlConnection(conn);
            _connection.Open();
        }
        public IActionResult SearchIndex()
        {
            ViewData["ShowDiv"] = ShowDiv;
            return View();
        }
        [HttpPost]
        public IActionResult SearchIndex(string search)
        {
            Connection();
            ShowDiv = true;
            ViewData["ShowDiv"] = ShowDiv;

            string searchQuery = $"SELECT * FROM MeetingInformationTable WHERE MeetingInfoDetails LIKE '{search}%' OR MeetingInfoBrief LIKE '{search}%' OR emailID LIKE '{search}%';";
            Console.WriteLine(searchQuery);
            try
            {
                using (SqlCommand cmd = new SqlCommand(searchQuery, _connection))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        
                            SchedulerModel meetModel = new SchedulerModel();
                            meetModel.MeetingID = (int)reader[0];
                            meetModel.emailID = (string)reader[1];
                            meetModel.MeetingInfoDetails = (string)reader[2];
                            meetModel.MeetingInfoBrief = (string)reader[3];
                            meetModel.FromDateTime = (DateTime)reader[4];
                            meetModel.ToDateTime = (DateTime)reader[5];
                            _meetingList.Add(meetModel);
                            
                        
                    }

                    ViewBag.MeetingList = _meetingList;
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View(ViewBag);
        }

    
    public IActionResult Index()
		{
			return View();
		}
	}
}
