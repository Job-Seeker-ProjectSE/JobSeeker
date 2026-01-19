using Microsoft.AspNetCore.Mvc;
using System.Data.SQLite;

public class AccountController : Controller
{
    
    public IActionResult Register()
    {
        return View();
    }

    
    [HttpPost]
    public IActionResult Register(string email, string password, string confirmPassword, string role)
    {
        if (password != confirmPassword)
        {
            ViewBag.Error = "Passwords do not match!";
            return View();
        }

        using var conn = Db.GetConnection();
        conn.Open();

        // Check if email already exists
        var checkCmd = new SQLiteCommand("SELECT COUNT(*) FROM Users WHERE Email = @e", conn);
        checkCmd.Parameters.AddWithValue("@e", email);
        long count = (long)checkCmd.ExecuteScalar();
        if (count > 0)
        {
            ViewBag.Error = "Email already exists!";
            return View();
        }

        // Insert user
        var cmd = new SQLiteCommand(
            "INSERT INTO Users (Email, PasswordHash, Role) VALUES (@e,@p,@r)", conn);
        cmd.Parameters.AddWithValue("@e", email);
        cmd.Parameters.AddWithValue("@p", password);  // plain text
        cmd.Parameters.AddWithValue("@r", role);
        cmd.ExecuteNonQuery();

        long userId = conn.LastInsertRowId;

        // Insert profile
        if (role == "Client")
        {
            var c = new SQLiteCommand(
                "INSERT INTO ClientProfiles (UserId) VALUES (@id)", conn);
            c.Parameters.AddWithValue("@id", userId);
            c.ExecuteNonQuery();
        }
        else
        {
            var c = new SQLiteCommand(
                "INSERT INTO CompanyProfiles (UserId) VALUES (@id)", conn);
            c.Parameters.AddWithValue("@id", userId);
            c.ExecuteNonQuery();
        }

        return RedirectToAction("Login");
    }

    // GET: /Account/Login
    public IActionResult Login()
    {
        return View();
    }

    // POST: /Account/Login
    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        using var conn = Db.GetConnection();
        conn.Open();

        var cmd = new SQLiteCommand(
            "SELECT Id, PasswordHash, Role FROM Users WHERE Email = @e", conn);
        cmd.Parameters.AddWithValue("@e", email);

        using var reader = cmd.ExecuteReader();
        if (!reader.Read())
        {
            ViewBag.Error = "Invalid email or password!";
            return View();
        }

        int userId = reader.GetInt32(0);
        string storedPassword = reader.GetString(1);
        string role = reader.GetString(2);

        if (storedPassword != password)
        {
            ViewBag.Error = "Invalid email or password!";
            return View();
        }

        // ✅ Store user info in session
        HttpContext.Session.SetInt32("UserId", userId);
        HttpContext.Session.SetString("UserRole", role);
        HttpContext.Session.SetString("UserEmail", email);



        // Get display name
        string displayName = email; // default
        using var profileCmd = new SQLiteCommand("", conn);

        if (role == "Client")
        {
            profileCmd.CommandText = "SELECT FullName FROM ClientProfiles WHERE UserId = @id";
            profileCmd.Parameters.AddWithValue("@id", userId);
            var nameObj = profileCmd.ExecuteScalar();
            if (nameObj != null && nameObj != DBNull.Value)
                displayName = nameObj.ToString();
        }
        else
        {
            profileCmd.CommandText = "SELECT CompanyName FROM CompanyProfiles WHERE UserId = @id";
            profileCmd.Parameters.AddWithValue("@id", userId);
            var nameObj = profileCmd.ExecuteScalar();
            if (nameObj != null && nameObj != DBNull.Value)
                displayName = nameObj.ToString();
        }

        HttpContext.Session.SetString("UserName", displayName);

        // Redirect based on role
         return RedirectToAction("Index", "Jobs");
   
           
    }
    public IActionResult Logout()
    {
        HttpContext.Session.Clear(); // clear all session values
        return RedirectToAction("Login");
    }

}
