using AppointmentApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System;

namespace AppointmentApplication.Controllers
{
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;
        public UserController(IConfiguration configuration)
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
        public IActionResult UserIndex()
        {
			Connection();
			string opnMode = "Normal";
			string meetIdStr = "";


			if (!Request.Query.IsNullOrEmpty())
			{
				opnMode = Request.Query["opnMode"];
				int meetId = int.Parse(Request.Query["MeetingId"].ToString());
				if (opnMode == "Delete")
				{
					string deleteMeetQuery = $"DELETE FROM MeetingInformationTable WHERE MeetingId = {meetId}";
					try
					{
						using (SqlCommand cmd = new SqlCommand(deleteMeetQuery, _connection))
						{
							cmd.ExecuteNonQuery();
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
				else if (opnMode == "Update")
				{
					opnMode = "Update";
					meetIdStr = Request.Query["MeetingId"].ToString();

				}
			}
			
            string selectQuery = "SELECT * FROM MeetingInformationTable";
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
                    meetModel.FromDateTime = reader.GetDateTime(4);
                    meetModel.ToDateTime = reader.GetDateTime(4);
                    _meetingList.Add(meetModel);
                }
                reader.Close();
            }

            ViewBag.MeetingList = _meetingList;
			ViewBag.status = opnMode;
			ViewBag.meetId = meetIdStr;


			return View(ViewBag);
        }
		[HttpPost]
		public IActionResult UserIndex(SchedulerModel schedulerModel)
		{
			Connection(); 
			string updateQuery = $"UPDATE MeetingInformationTable SET emailID = '{schedulerModel.emailID}'," +
					$"MeetingInfoDetails = '{schedulerModel.MeetingInfoDetails}', " +
					$"MeetingInfoBrief = '{schedulerModel.MeetingInfoBrief}', " +
					$"FromDateTime  = '2023-04-23 09:30:00'," +
					$"ToDateTime ='2023-04-23 10:30:00' WHERE MeetingID = {schedulerModel.MeetingID};";

			try
			{
				using (SqlCommand cmd = new SqlCommand(updateQuery, _connection))
				{
					var result = cmd.ExecuteNonQuery();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			return RedirectToAction("UserIndex", "User");

		}

		public IActionResult CreateMeet()
		{
			return View();
		}

		[HttpPost]
		public IActionResult CreateMeet(SchedulerModel schedulerModel)
		{
			Connection();
			
			schedulerModel.emailID = Request.Form["emailID"];
			schedulerModel.MeetingInfoDetails = Request.Form["MeetingInfoDetails"];
			schedulerModel.MeetingInfoBrief = Request.Form["MeetingInfoBrief"];
			schedulerModel.FromDateTime= Convert.ToDateTime(Request.Form["FromDateTime"]);
			schedulerModel.ToDateTime = Convert.ToDateTime(Request.Form["ToDateTime"]); 

			string addScheduleQuery = $"INSERT INTO MeetingInformationTable VALUES('{schedulerModel.emailID}','{schedulerModel.MeetingInfoDetails}','{schedulerModel.MeetingInfoBrief}','2023-05-23 01:30:00','2023-05-23 01:30:00')";

			try
			{
				using (SqlCommand cmd = new SqlCommand(addScheduleQuery, _connection))
				{
					cmd.ExecuteNonQuery();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			return RedirectToAction("UserIndex");
		}

	



	public IActionResult Index()
        {
            return View();
        }
    }
}
