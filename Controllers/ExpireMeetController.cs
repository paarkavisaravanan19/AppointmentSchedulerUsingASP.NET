using AppointmentApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace AppointmentApplication.Controllers
{
    public class ExpireMeetController : Controller
    {
        private readonly IConfiguration _configuration;
        public ExpireMeetController(IConfiguration configuration)
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
        public IActionResult ExpireMeetIndex()
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
                
            }
            string selectQuery = "select * from dbo.MeetingInformationTable where ToDateTime < GETDATE()";
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
