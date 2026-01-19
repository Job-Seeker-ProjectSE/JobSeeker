using System.Data.SQLite;

public static class Db
{
    public static SQLiteConnection GetConnection()
    {
        return new SQLiteConnection("Data Source=Data/app.db");
    }
}

