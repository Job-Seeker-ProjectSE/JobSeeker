using Microsoft.AspNetCore.Mvc;
using System.Data.SQLite;

public class ProfileController : Controller
{
    public IActionResult Index()
    {
        string role = HttpContext.Session.GetString("UserRole");
        int? userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
            return RedirectToAction("Login", "Account");

        using var conn = Db.GetConnection();
        conn.Open();

        if (role == "Client")
        {
            var cmd = new SQLiteCommand(
                "SELECT FullName, Age FROM ClientProfiles WHERE UserId = @id", conn);
            cmd.Parameters.AddWithValue("@id", userId);

            using var r = cmd.ExecuteReader();
            if (r.Read())
            {
                ViewBag.FullName = r["FullName"];
                ViewBag.Age = r["Age"];
            }

            return View("ClientProfile");
        }
        else // Company
        {
            // Company profile
            var cmd = new SQLiteCommand(
                "SELECT CompanyName FROM CompanyProfiles WHERE UserId = @id", conn);
            cmd.Parameters.AddWithValue("@id", userId);

            // Use ExecuteScalar since we only need one value
            var companyName = cmd.ExecuteScalar();
            ViewBag.CompanyName = companyName != null ? companyName.ToString() : "";

            // Return the CompanyProfile view
            return View("CompanyProfile");


        }
    }
    // POST: /Profile/UpdateClient
    [HttpPost]
    public IActionResult UpdateClient(string FullName, string Age, string Skills, string About, string Education, string Experience, string Phone, string Availability)
    {
        // Store temporarily in TempData to display after redirect
        TempData["FullName"] = FullName;
        TempData["Age"] = Age;
        TempData["Skills"] = Skills;
        TempData["About"] = About;
        TempData["Education"] = Education;
        TempData["Experience"] = Experience;
        TempData["Phone"] = Phone;
        TempData["Availability"] = Availability;

        // Redirect back to profile
        return RedirectToAction("Index", "Jobs");
    }

    // POST: /Profile/UpdateCompany
    [HttpPost]
    public IActionResult UpdateCompany(
        string CompanyName, string Address, string Field, string Description,
        string Email, string Phone, string Website, string LinkedIn)
    {
        // Store temporarily in TempData to display after redirect
        TempData["CompanyName"] = CompanyName;
        TempData["Address"] = Address;
        TempData["Field"] = Field;
        TempData["Description"] = Description;
        TempData["Email"] = Email;
        TempData["Phone"] = Phone;
        TempData["Website"] = Website;
        TempData["LinkedIn"] = LinkedIn;

        // Redirect back to profile
        return RedirectToAction("Index", "Jobs");
    }
}


