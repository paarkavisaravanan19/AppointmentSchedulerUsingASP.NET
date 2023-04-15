namespace AppointmentApplication.Models
{
    public class SchedulerModel
    {
        public int MeetingID { get; set; }
        public string emailID { get; set; }
        public string MeetingInfoDetails { get; set; }
        public string MeetingInfoBrief { get; set; }
        public DateTime FromDateTime { get; set; } 
        public DateTime ToDateTime { get; set; }
    }
}
