namespace Job_Recruiting_System.Models
{
    public class Job
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
