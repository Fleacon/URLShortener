using URLShortenerCore;

var server = new WebServer();
Database.GetInstance();
Console.WriteLine($"Creating database... {Database.GetInstance()}");
server.Start();
Console.WriteLine("Press any key to stop the server...");
Console.ReadKey();