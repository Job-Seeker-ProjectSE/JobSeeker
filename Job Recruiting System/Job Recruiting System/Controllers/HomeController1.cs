using Microsoft.AspNetCore.Mvc;
using Job_Recruiting_System.Models;
using System;
using System.Data.SQLite;
using System.Data;
using System.Diagnostics;

namespace Job_Recruiting_System.Controllers
{
    public class JobsController : Controller
    {
        // Declaration of the List which tested the form, not usable anymore due to implementation of jobs_DB.db.
        // If database is unable to connect then the jobs inside this List will appear instead.
        private static List<Job> Jobs = new List<Job>
        {
            new Job { Id = 1, Title = "Receptionist", Company = "New W Hotel", Location = "Durres, Albania", Description = "Seeking a receptionist with good communication skills and flexible hours.", UpdatedAt = DateTime.Now.AddDays(-6) },
            new Job { Id = 2, Title = "Bank Teller", Company = "MonCheri", Location = "Fier, Albania", Description = "Looking for a bank teller with several years of experience.", UpdatedAt = DateTime.Now.AddDays(-7) },
            new Job { Id = 3, Title = "Civil Engineer", Company = "Vibtis SHPK", Location = "Tirane, Albania", Description = "Hiring an experienced civil engineer.", UpdatedAt = DateTime.Now.AddDays(-4) }
        };

        //The function for the filter to work , accept a Title string and a Location string
        public IActionResult Index(string title, string location)
        {
            var role = HttpContext.Session.GetString("UserRole");
            Debug.WriteLine("UserRole = " + role);
            Debug.WriteLine("SomeoneApplied = " + ApplicationState.SomeoneApplied);

            List<Job> jobs = new List<Job>();  

            // Database link
            string connectionString = "Data Source=jobs_DB.db;Version=3;"; 

            //If the database is connected then... Continue Code
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                //Get Jobs from database , Works with filtering too.
                string query = "SELECT * FROM Jobs WHERE 1=1";
                if (!string.IsNullOrEmpty(title))
                {
                    query += " AND Title LIKE @Title";
                }
                if (!string.IsNullOrEmpty(location))
                {
                    query += " AND Location LIKE @Location";
                }

                //Add an Empty String if filter is Null.
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(title))
                        cmd.Parameters.AddWithValue("@Title", $"%{title}%");
                    if (!string.IsNullOrEmpty(location))
                        cmd.Parameters.AddWithValue("@Location", $"%{location}%");

                    //Get data from SqLite
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //Put values into our Job class. \\Models\\Class.cs
                            jobs.Add(new Job
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Company = reader.GetString(2),
                                Location = reader.GetString(3),
                                Description = reader.GetString(4),
                                UpdatedAt = reader.GetDateTime(5)
                            });
                        }
                    }
                }
            }
            if (HttpContext.Session.GetString("UserRole") == "Company"
    && ApplicationState.SomeoneApplied)
            {
                ViewBag.ShowApplyNotification = true;
                ViewBag.ApplicantName = ApplicationState.ApplicantName;
                ViewBag.ApplicantField = ApplicationState.ApplicantField;
                ViewBag.ApplicantCV = ApplicationState.ApplicantCV;

                // One-time display
                ApplicationState.SomeoneApplied = false;
            }
            else
            {
                ViewBag.ShowApplyNotification = false;
            }



            return View(jobs); //Simply reload page 
        }

        //The function to open Details.cshtml view.
        public IActionResult Details(int id)
        {
            //Clear our job
            Job job = null;
            
            string connectionString = "Data Source=jobs_DB.db;Version=3;";

            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Jobs WHERE Id = @Id";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            //Insert the detailed job
                            job = new Job
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Company = reader.GetString(2),
                                Location = reader.GetString(3),
                                Description = reader.GetString(4),
                                UpdatedAt = reader.GetDateTime(5)
                            };
                        }
                    }
                }
            }


            if (job == null) return NotFound();  //Gives error
            return View(job);
        }

        [HttpGet]
        public IActionResult PostJob()
        {
            return View(); // Render the form
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PostJob(Job job)
        {
            if (ModelState.IsValid) //Checks if the details we have put are the same as the class we constructed before. Class.cs
            {
                string connectionString = "Data Source=jobs_DB.db;Version=3;";

                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    //Prepare the query we put inside SQLite file, in this case we are creating a new Job , inserting a new record (row)
                    string query = "INSERT INTO Jobs (Title, Company, Location, Description, UpdatedTime) VALUES (@Title, @Company, @Location, @Description, @UpdatedAt)";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Title", job.Title);
                        cmd.Parameters.AddWithValue("@Company", job.Company);
                        cmd.Parameters.AddWithValue("@Location", job.Location);
                        cmd.Parameters.AddWithValue("@Description", job.Description);
                        cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                        cmd.ExecuteNonQuery(); //Execute the string query. SQLite package function
                    }
                }

                return RedirectToAction("Index"); //Go back to Home page
            }

            return View(job);  // Stay at the same Page
        }


        //This function currently only opens the view but does not save anything!
        [HttpGet]
        public IActionResult ApplyJob()
        {
            return View(new ApplyJobViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApplyJob(ApplyJobViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                ApplicationState.ApplicantName = model.Name;
                ApplicationState.ApplicantField = model.Field;
                ApplicationState.ApplicantCV = model.CV;
                ApplicationState.SomeoneApplied = true;

                return RedirectToAction("Index", "Jobs");
            }

            return View(model); // If validation fails, return to form
        }



        [HttpGet]
        public IActionResult Policy()
        {
            return View(); // Go to Policy View
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Policy(Job policy)
        {
            if (ModelState.IsValid)
            {
                
                return RedirectToAction("Index"); // Go back to Home page
            }

            return View(policy); //Stay at same page
        }

        [HttpPost]

        //Delete function
        public IActionResult Delete(int id)
        {
            string connectionString = "Data Source=jobs_DB.db;Version=3;";

            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM Jobs WHERE Id = @Id";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    //Every Job when it's create contains an id as a invisible string.
                    //<input type="hidden" name="id" value="@job.Id" />
                    //It gets the value from the database which is set AutoIncrement and Primary Key.
                    //Then Delete the row which have this id.
                    //Refresh page
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");
        }
    }
}




