namespace Job_Recruiting_System.Models
{
    public class ClientProfile
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string FullName { get; set; }
        public int? Age { get; set; }
        public string Gender { get; set; }
    }

}
