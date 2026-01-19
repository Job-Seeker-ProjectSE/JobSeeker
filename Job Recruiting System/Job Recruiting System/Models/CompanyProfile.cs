namespace Job_Recruiting_System.Models
{
    public class CompanyProfile
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string CompanyName { get; set; }
        public string Industry { get; set; }
        public string Website { get; set; }
    }

}
