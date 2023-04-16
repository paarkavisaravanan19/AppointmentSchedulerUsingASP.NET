using AppointmentApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace AppointmentApplication.Controllers
{
	public class ListAllController : Controller
	{
		private readonly IConfiguration _configuration;
		public ListAllController(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		private SqlConnection _connection;
		private List<SchedulerModel> _meetingList = new List<SchedulerModel>();

		public void Connection()
		{
			string conn = _configuration.GetConnectionString("ScheduleDB");
			_connection = new SqlConnection(conn);
			_connection.Open();
		}
		[HttpGet]
		public IActionResult ListAllIndex()
		{
			Connection();
			string selectQuery = "SELECT top 2 * FROM MeetingInformationTable order by(FromDateTime)";
			using (SqlCommand cmd = new SqlCommand(selectQuery, _connection))
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
				reader.Close();
			}

			ViewBag.MeetingList = _meetingList;
			


			return View(ViewBag);
		}
		public IActionResult Index()
		{
			return View();
		}
	}
}
