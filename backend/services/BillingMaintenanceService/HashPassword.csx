// Chạy: dotnet script HashPassword.csx
#r "nuget: BCrypt.Net-Next, 4.0.3"

using BCrypt.Net;

string password = "123";
string hash = BCrypt.Net.BCrypt.HashPassword(password);

Console.WriteLine($"Password: {password}");
Console.WriteLine($"BCrypt Hash:");
Console.WriteLine(hash);
Console.WriteLine();
Console.WriteLine("SQL Command:");
Console.WriteLine($"UPDATE Users SET PasswordHash = '{hash}' WHERE Username IN ('admin', 'student01');");
