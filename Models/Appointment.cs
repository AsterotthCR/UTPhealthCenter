namespace HealthCare.Models
{
    public class Appointment
    {
        public int id { get; set; }
        public int patientId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string description { get; set; }
        public object patients { get; set; }
    }
}
