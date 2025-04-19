using System.Data.SQLite;

namespace URLShortenerCore;

public class Database
{
    private static Database? db = null;
    private readonly string strCon = $"Data Source={"URLShorts.sqlite"};Version=3;";

    private Database()
    {
        CreateDatabase();
    }

    public static Database GetInstance()
    {
        if (db is null)
            db = new Database();
        return db;
    }
    
    public void CreateDatabase()
    {
        if (!File.Exists("URLShorts.sqlite"))
            SQLiteConnection.CreateFile("URLShorts.sqlite");
        
        var connection = new SQLiteConnection(strCon);
        connection.Open();

        var sql = @"CREATE TABLE IF NOT EXISTS url(
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    shortURL TEXT NOT NULL UNIQUE,
                    longURL TEXT NOT NULL
);";
        var cmd = new SQLiteCommand(sql, connection);
        cmd.ExecuteNonQuery();
    }
    
    public string? Search(string shortQuery)
    {
        try
        {
            var connection = new SQLiteConnection(strCon);
            connection.Open();
            
            var sql = "SELECT * FROM url WHERE shortURL = @shortQuery";
            var cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@shortQuery", shortQuery);
            var reader = cmd.ExecuteReader();

            if (reader.HasRows && reader.Read())
            {
                var id = reader.GetInt32(0);
                var shortUrl = reader.GetString(1);
                var fullUrl = reader.GetString(2);
                return $"{id} - {shortUrl} - {fullUrl}";
            }
            else
            {
                return null;
            }
        }
        catch (SQLiteException e)
        {
            return e.Message;
        }
    }
    
    public string? SearchFullString(string shortQuery)
    {
        Console.WriteLine("Looking for DB at: " + Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "URLShorts.sqlite"));
        try
        {
            var connection = new SQLiteConnection(strCon);
            connection.Open();
            
            var sql = "SELECT * FROM url WHERE shortURL = @shortQuery";
            var cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@shortQuery", shortQuery);
            var reader = cmd.ExecuteReader();

            if (reader.HasRows && reader.Read())
            {
                return reader.GetString(2);
            }
            else
            {
                return null;
            }
        }
        catch (SQLiteException e)
        {
            return e.Message;
        }
    }

    public void Insert(string shortUrl, string longUrl)
    {
        try
        {
            var connection = new SQLiteConnection(strCon);
            connection.Open();

            var sql = @"INSERT INTO url (shortURL, longURL) VALUES (@shortUrl, @longUrl);";
            var cmd = new SQLiteCommand(sql,connection);
            cmd.Parameters.AddWithValue("@shortUrl", shortUrl);
            cmd.Parameters.AddWithValue("@longUrl", longUrl);
            cmd.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine("Error inserting URL. Shortened URL is possibly invalid.");
        }
    }

    public string GetAllUrls()
    {
        var urls = "";
        try
        {
            var connection = new SQLiteConnection(strCon);
            connection.Open();
            
            var sql = "SELECT * FROM url";
            var cmd = new SQLiteCommand(sql, connection);
            var reader = cmd.ExecuteReader();
            
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var shortUrl = reader.GetString(1);
                    var fullUrl = reader.GetString(2);
                    urls += $"{id} - {shortUrl} - {fullUrl}\n";
                }
            }
            else
            {
                return null;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Database:" + e);
            throw;
        }

        return urls;
    }
}